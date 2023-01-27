using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kelio_client
{
  public partial class MainForm : Form
  {
    private bool reminderEnabled = false;
    private HourMinute reminder = null;
    private int? date = null;

    private void AppendText(string text, Color color, bool newLine)
    {
      inOutBox.SelectionStart = inOutBox.TextLength;
      inOutBox.SelectionLength = 0;

      inOutBox.SelectionColor = color;
      inOutBox.AppendText(text);
      inOutBox.SelectionColor = inOutBox.ForeColor;
      if (newLine) {
        inOutBox.AppendText(Environment.NewLine);
        Font font = inOutBox.SelectionFont;
        inOutBox.SelectionFont = new Font(inOutBox.Font.Name, 2);
        inOutBox.AppendText(Environment.NewLine);
        inOutBox.SelectionFont = font;
      }
    }

    public MainForm()
    {
      InitializeComponent();
      GlobalHotKey.RegisterHotKey("Win + Shift + B", () => {
        this.WindowState = FormWindowState.Minimized;
        this.Show();
        this.WindowState = FormWindowState.Normal;
        clockInOutButton.Focus();
      });
    }

    private void BeepBeep() {
      System.Media.SoundPlayer player = new System.Media.SoundPlayer();
      player.Stream = Properties.Resources.notif;
      player.Play();
    }

    private struct Tokens
    {
      public string csrf { get; set; }
      public string intranet { get; set; }
    }

    private bool Ping()
    {
      Process cmd = new Process();
      cmd.StartInfo.FileName = "cmd.exe";
      cmd.StartInfo.Arguments = "/c ping " + Properties.Settings.Default.Url.Replace("https://", "").Replace("http://", "") + " -n 1";
      cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      cmd.StartInfo.RedirectStandardInput = true;
      cmd.StartInfo.RedirectStandardOutput = true;
      cmd.StartInfo.CreateNoWindow = true;
      cmd.StartInfo.UseShellExecute = false;
      cmd.Start();
      cmd.WaitForExit();
      return cmd.ExitCode == 0;
    }

    private async Task<Tokens> GetTokens(HttpClient client)
    {
      progressBar.Value = 10;
      int pingCount = 0;
      while (! Ping()) {
        pingCount++;
        feedbackLabel.Text = "PING failed " + (new string('.', pingCount % 20)) + "\nPeut-être un problème de VPN ?";
        await Task.Delay(1000);
      }
      // 1 - get login
      progressBar.Value = 25;
      feedbackLabel.Text = "GET  /open/login";
      HttpResponseMessage response = await client.GetAsync("/open/login");
      string responseBody = await response.Content.ReadAsStringAsync();
      string csrfToken = new Regex(@"<input type=""hidden"" name=""_csrf_bodet"" value=""([a-z0-9\-]+)"" \/>").Match(responseBody).Groups[1].Value;

      // 2 - post login
      progressBar.Value = 50;
      feedbackLabel.Text = "POST /open/j_spring_security_check";
      var postData = new Dictionary<string, string>();
      postData.Add("ACTION", "ACTION_VALIDER_LOGIN");
      postData.Add("username", Properties.Settings.Default.Login);
      try
      {
        postData.Add("password", EncryptionHelper.Decrypt(Properties.Settings.Default.Password));
      }
      catch (Exception)
      {
        postData.Add("password", Properties.Settings.Default.Password);
      }
      postData.Add("_csrf_bodet", csrfToken);
      response = await client.PostAsync("/open/j_spring_security_check", new FormUrlEncodedContent(postData));
      responseBody = await response.Content.ReadAsStringAsync();

      // 3 - get tokens
      progressBar.Value = 75;
      feedbackLabel.Text = "GET  /open/homepage";
      response = await client.GetAsync("/open/homepage?ACTION=intranet&asked=1&header=0");
      responseBody = await response.Content.ReadAsStringAsync();
      Tokens tokens = new Tokens();
      tokens.intranet = new Regex(@"<input type=""hidden"" name=""JETON_INTRANET"" id=""JETON_INTRANET"" value=""([0-9]+)"" >").Match(responseBody).Groups[1].Value;
      tokens.csrf = new Regex(@"<input type=""hidden"" name=""_csrf_bodet"" value=""([a-z0-9\-]+)"" \/>").Match(responseBody).Groups[1].Value;
      return tokens;
    }

    private async Task Consult()
    {
      timer.Enabled = false;
      refreshButton.Visible = false;
      reminderDropDownBtn.Visible = false;
      date = DateTime.Now.DayOfYear;
      kelioUrlLabel.Text = Properties.Settings.Default.Url;

      inOutBox.Text = "";
      AppendText("...", Color.FromArgb(96, 96, 96), false);
      totalDiffLabel.Text = "...";
      totalDiffLabel.ForeColor = Color.FromArgb(96, 96, 96);
      weekDiffLabel.Text = "...";
      weekDiffLabel.ForeColor = Color.FromArgb(96, 96, 96);
      clockInOutButton.Visible = false;
      feedbackLabel.Visible = true;
      progressBar.Visible = true;
      reminderPanel.Visible = false;

      Uri baseAddress = new Uri(Properties.Settings.Default.Url);
      CookieContainer cookieContainer = new CookieContainer();
      using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
      using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
      {
        Tokens tokens = await GetTokens(client);
        if (tokens.csrf != "" && tokens.intranet != "")
        {
          // 4 - get consult
          progressBar.Value = 100;
          feedbackLabel.Text = "POST /open/webgtp/badge";
          var postData = new Dictionary<string, string>();
          postData = new Dictionary<string, string>();
          postData.Add("ACTION", "AFFICHER__CD");
          postData.Add("ACTION_SWITCH", "");
          postData.Add("choixApplication", "");
          postData.Add("application", "1");
          postData.Add("choixOption", "");
          postData.Add("coordonneeLongitude", "");
          postData.Add("coordonneeLatitude", "");
          postData.Add("coordonneePrecision", "");
          postData.Add("coordonneeIndicateur", "-1");
          postData.Add("JETON_INTRANET", tokens.intranet);
          postData.Add("_csrf_bodet", tokens.csrf);
          HttpResponseMessage response = await client.PostAsync("/open/webgtp/badge", new FormUrlEncodedContent(postData));
          string responseBody = await response.Content.ReadAsStringAsync();

          //File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "consult.txt"), responseBody);
          UpdateInfos(responseBody);
        }
        else
        {
          inOutBox.Text = "";
          AppendText("Echec de connexion", Color.Salmon, true);
          AppendText("Mot de passe\nexpiré peut-être ?", Color.FromArgb(96, 96, 96), true);
        }
      }
      feedbackLabel.Visible = false;
      progressBar.Visible = false;
      clockInOutButton.Visible = true;
      refreshButton.Visible = true;
      reminderDropDownBtn.Visible = true;
      reminderPanel.Visible = false;
      clockInOutButton.Focus();
      timer.Enabled = true;
    }

    private int GetPause(DayOfWeek dayOfWeek)
    {
      switch (dayOfWeek)
      {
        case DayOfWeek.Monday:
          return Properties.Settings.Default.PauseMonday;
        case DayOfWeek.Tuesday:
          return Properties.Settings.Default.PauseTuesday;
        case DayOfWeek.Wednesday:
          return Properties.Settings.Default.PauseWednesday;
        case DayOfWeek.Thursday:
          return Properties.Settings.Default.PauseThursday;
        case DayOfWeek.Friday:
          return Properties.Settings.Default.PauseFriday;
      }
      return 0;
    }

    private int GetOffset(DayOfWeek dayOfWeek)
    {
      switch (dayOfWeek)
      {
        case DayOfWeek.Monday:
          return Properties.Settings.Default.OffsetMonday;
        case DayOfWeek.Tuesday:
          return Properties.Settings.Default.OffsetTuesday;
        case DayOfWeek.Wednesday:
          return Properties.Settings.Default.OffsetWednesday;
        case DayOfWeek.Thursday:
          return Properties.Settings.Default.OffsetThursday;
          // case DayOfWeek.Friday:
          //   return Properties.Settings.Default.OffsetFriday;
      }
      return 0;
    }

    private int GetOffsetYesterday(DayOfWeek dayOfWeek)
    {
      switch (dayOfWeek)
      {
        // case DayOfWeek.Monday:
        //  return Properties.Settings.Default.OffsetMonday;
        case DayOfWeek.Tuesday:
          return Properties.Settings.Default.OffsetMonday;
        case DayOfWeek.Wednesday:
          return Properties.Settings.Default.OffsetTuesday;
        case DayOfWeek.Thursday:
          return Properties.Settings.Default.OffsetWednesday;
        case DayOfWeek.Friday:
          return Properties.Settings.Default.OffsetThursday;
      }
      return 0;
    }

    private void UpdateInfos(string htmlContent)
    {
      MatchCollection matches = new Regex(@"<td class='(?:tabImpair|tabPair)'>([0-9]{2}:[0-9]{2})<\/td>\s*?<td class='(?:tabImpair|tabPair)'>(.*?)<\/td>").Matches(htmlContent);
      inOutBox.Text = "";
      int clockInCount = 0;
      int clockOutCount = 0;
      HourMinute lastClockOut = null;
      foreach(Match match in matches)
      {
        if (match.Groups[2].Value == "Entrée")
        {
          clockInCount++;
          AppendText(match.Groups[1].Value, Color.MediumSeaGreen, false);
          AppendText(" - ", Color.White, false);
        }
        else
        {
          clockOutCount++;
          lastClockOut = new HourMinute(match.Groups[1].Value);
          AppendText(lastClockOut.ToString(), Color.Orange, true);
        }
      }

      HourMinuteInterval weekDiff = new HourMinuteInterval(new Regex(@"<li>Votre crédit \/ débit hebdomadaire est de (.*)<\/li>").Match(htmlContent).Groups[1].Value);
      HourMinuteInterval weekDiff2 = new HourMinuteInterval(new Regex(@"<li>Votre crédit \/ débit hebdomadaire est de (.*)<\/li>").Match(htmlContent).Groups[1].Value);
      HourMinuteInterval weekDiffYesterday = new HourMinuteInterval(new Regex(@"<li>Votre crédit \/ débit hebdomadaire arrêté à la veille est de (.*)<\/li>").Match(htmlContent).Groups[1].Value);
      weekDiff.Remove(GetOffset(DateTime.Now.DayOfWeek));
      weekDiff2.Remove(GetOffset(DateTime.Now.DayOfWeek));
      weekDiffYesterday.Remove(GetOffsetYesterday(DateTime.Now.DayOfWeek));

      if (clockInCount == 0)
        BeepBeep();

      HourMinute remind = null;

      if (clockInCount > clockOutCount)
      {
        remind = new HourMinute(DateTime.Now).Remove(weekDiff);
        HourMinute endOfDay = new HourMinute(remind);
        if (DateTime.Now.Hour < 13)
        {
          int pause = GetPause(DateTime.Now.DayOfWeek);
          if (pause > 0)
          {
            HourMinuteInterval pauseInterval = new HourMinuteInterval(pause);
            endOfDay = remind.Add(pauseInterval);
            remind = new HourMinute(12, (pause > 105) ? 0 : 15);
            AppendText(remind.ToString(), Color.FromArgb(96, 96, 96), true);
            AppendText(remind.Add(pauseInterval).ToString(), Color.FromArgb(96, 96, 96), false);
            AppendText(" - ", Color.White, false);
          }
        }
        AppendText(endOfDay.ToString(), Color.FromArgb(96, 96, 96), true);
      }
      else
      {
        if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 13 && lastClockOut != null)
        {
          HourMinute nextClockIn = lastClockOut.Add(new HourMinuteInterval(GetPause(DateTime.Now.DayOfWeek)));
          if (nextClockIn.Hour > 13)
          {
            nextClockIn.Hour = 14;
            nextClockIn.Minute = 0;
          }
          remind = nextClockIn.Remove(weekDiff);
          AppendText(nextClockIn.ToString(), Color.FromArgb(96, 96, 96), false);
          AppendText(" - ", Color.White, false);
          AppendText(remind.ToString(), Color.FromArgb(96, 96, 96), true);
          remind = nextClockIn;
        }

      }
      if (remind != null && !reminderEnabled)
      {
        reminder = remind;
        if (Properties.Settings.Default.AutoReminder)
        {
          reminderEnabled = true;
          reminderDropDownBtn.BackgroundImage = global::kelio_client.Properties.Resources.notif_enabled;
          toolTip.SetToolTip(reminderDropDownBtn, "Modifier / Annuler le rappel à " + reminder.ToString());
        }
      }

      HourMinuteInterval totalDiff = new HourMinuteInterval(new Regex(@"<li>Votre crédit \/ débit total arrêté à la veille est de (.*)<\/li>").Match(htmlContent).Groups[1].Value);
      if (clockInCount == clockOutCount && DateTime.Now.Hour >= 16)
      {
        weekDiffLabel.Text = weekDiff2.ToString();
        weekDiffLabel.ForeColor = weekDiff2.IsNegative ? Color.LightCoral : Color.MediumSeaGreen;
        toolTip.SetToolTip(weekDiffTitleLabel, "Crédit / débit hebdomadaire");
        toolTip.SetToolTip(weekDiffLabel, "Crédit / débit hebdomadaire");
      }
      else
      {
        weekDiffLabel.Text = weekDiffYesterday.ToString();
        weekDiffLabel.ForeColor = weekDiffYesterday.IsNegative ? Color.LightCoral : Color.MediumSeaGreen;
        toolTip.SetToolTip(weekDiffTitleLabel, "Crédit / débit hebdomadaire arrêté à la veille");
        toolTip.SetToolTip(weekDiffLabel, "Crédit / débit hebdomadaire arrêté à la veille");
      }

      totalDiffLabel.Text = totalDiff.ToString();
      totalDiffLabel.ForeColor = totalDiff.IsNegative ? Color.LightCoral : Color.MediumSeaGreen;
    }

    private async Task ClockInOut()
    {
      timer.Enabled = false;
      reminderEnabled = false;
      clockInOutButton.Visible = false;
      feedbackLabel.Visible = true;
      progressBar.Visible = true;
      reminderPanel.Visible = false;

      Uri baseAddress = new Uri(Properties.Settings.Default.Url);
      CookieContainer cookieContainer = new CookieContainer();
      using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
      using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
      {
        Tokens tokens = await GetTokens(client);

        if (tokens.csrf != "" && tokens.intranet != "")
        {
          // 4 - get consult
          progressBar.Value = 100;
          feedbackLabel.Text = "POST /open/webgtp/badge";
          var postData = new Dictionary<string, string>();
          postData = new Dictionary<string, string>();
          postData.Add("ACTION", "BADGER_ES");
          postData.Add("ACTION_SWITCH", "");
          postData.Add("choixApplication", "");
          postData.Add("application", "1");
          postData.Add("choixOption", "");
          postData.Add("coordonneeLongitude", "");
          postData.Add("coordonneeLatitude", "");
          postData.Add("coordonneePrecision", "");
          postData.Add("coordonneeIndicateur", "-1");
          postData.Add("JETON_INTRANET", tokens.intranet);
          postData.Add("_csrf_bodet", tokens.csrf);
          HttpResponseMessage response = await client.PostAsync("/open/webgtp/badge", new FormUrlEncodedContent(postData));
          string htmlContent = await response.Content.ReadAsStringAsync();
          Match success = new Regex(@"Votre badgeage <b>(.*)<\/b> a été pris en compte le <b>(.*?)<\/b> à <b>(.*?)<\/b>").Match(htmlContent);
          inOutBox.Text = "";
          AppendText("Bagde ", Color.White, false);
          AppendText(success.Groups[1].Value, Color.MediumSeaGreen, true);
          AppendText("le ", Color.White, false);
          AppendText(success.Groups[2].Value, Color.MediumSeaGreen, true);
          AppendText("à ", Color.White, false);
          AppendText(success.Groups[3].Value, Color.MediumSeaGreen, false);
          feedbackLabel.Visible = false;
          progressBar.Visible = false;
          await Task.Delay(2000);
          // File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "badge.txt"), responseBody);
          await Consult();
        }
        else
        {
          inOutBox.Text = "";
          AppendText("Echec de connexion", Color.Salmon, true);
          AppendText("Mot de passe\nexpiré peut-être ?", Color.FromArgb(96, 96, 96), true);
          feedbackLabel.Visible = false;
          progressBar.Visible = false;
          clockInOutButton.Visible = true;
          refreshButton.Visible = true;
          reminderDropDownBtn.Visible = true;
          clockInOutButton.Focus();
          timer.Enabled = true;
        }
      }
    }

    private void kelioUrlLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start(kelioUrlLabel.Text);
    }

    private async void clockInOutButton_ClickAsync(object sender, EventArgs e)
    {
      await ClockInOut();
    }

    private void paramButton_Click(object sender, EventArgs e)
    {
      using (ParamsForm paramsForm = new ParamsForm())
      {
        paramsForm.ShowDialog(this);
        _ = Consult();
      }
    }

    private void timer_Tick(object sender, EventArgs e)
    {
      this.Text = DateTime.Now.Hour.ToString("00") + ":"
                + DateTime.Now.Minute.ToString("00") + ":"
                + DateTime.Now.Second.ToString("00");
      if (reminderEnabled && reminder != null && DateTime.Now.Hour == reminder.Hour && DateTime.Now.Minute == reminder.Minute)
      {
        reminder = null;
        reminderEnabled = false;
        reminderDropDownBtn.BackgroundImage = global::kelio_client.Properties.Resources.notif_disabled;
        BeepBeep();
        if (MainForm.ActiveForm == null)
          FlashWindowUtil.FlashWindowEx(this.Handle);
      }
      if (date != DateTime.Now.DayOfYear)
      {
        _ = Consult();
      }
    }

    private void MainForm_Activated(object sender, EventArgs e)
    {
      FlashWindowUtil.StopFlashWindow(this.Handle);
      clockInOutButton.Focus();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing && reminderEnabled && reminder != null)
      {
        if (MessageBox.Show("Un rappel est activé pour " + reminder.ToString() + "\nQuitter ?", "Confirmer la fermeture", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
          e.Cancel = true;
      }
      Properties.Settings.Default.WindowPosition = DesktopBounds;
      Properties.Settings.Default.Save();
    }

    private void refreshButton_Click(object sender, EventArgs e)
    {
      _ = Consult();
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
      if (Properties.Settings.Default.Url == "https://kelio.domain.tld")
      {
        using (ParamsForm paramsForm = new ParamsForm())
        {
          timer.Enabled = false;
          paramsForm.ShowDialog(this);
        }
      }
      _ = Consult();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      if (Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(Properties.Settings.Default.WindowPosition)))
      {
        StartPosition = FormStartPosition.Manual;
        DesktopBounds = Properties.Settings.Default.WindowPosition;
      }
      if (TaskbarManager.IsPlatformSupported)
      {
        ThumbnailToolBarButton button = new ThumbnailToolBarButton(Properties.Resources.chrono, "Badger");
        button.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(ThumbnailButtonClicked);
        TaskbarManager.Instance.ThumbnailToolBars.AddButtons(this.Handle, button);
      }
    }
    private async void ThumbnailButtonClicked(object sender, ThumbnailButtonClickedEventArgs e)
    {
      await ClockInOut();
    }

    private void cancelReminderBtn_Click(object sender, EventArgs e)
    {
      reminderEnabled = false;
      reminderPanel.Visible = false;
      clockInOutButton.Visible = true;
      reminderDropDownBtn.BackgroundImage = global::kelio_client.Properties.Resources.notif_disabled;
      toolTip.SetToolTip(reminderDropDownBtn, "Définir un rappel");
    }

    private void reminderDropDownBtn_Click(object sender, EventArgs e)
    {
      feedbackLabel.Visible = false;
      progressBar.Visible = false;
      if (reminderPanel.Visible)
      {
        clockInOutButton.Visible = true;
        reminderPanel.Visible = false;
      }
      else
      {
        clockInOutButton.Visible = false;
        reminderPanel.Visible = true;
        reminderFeedbackLabel.Text = "";
        if (reminder != null)
        {
          reminderTextBox.Text = reminder.ToString();
        }
        else
        {
          reminderTextBox.Text = new HourMinute(DateTime.Now).ToString();
        }
        
        reminderTextBox.Focus();
      }
    }

    private void validateReminderBtn_Click(object sender, EventArgs e)
    {
      validateReminder();
    }

    private void reminderTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        validateReminder();
        e.Handled = true;
        e.SuppressKeyPress = true;
      }
    }

    private async void validateReminder()
    {
      Match match = new Regex(@"([0-9]{1,2})[:h]([0-9]{1,2})").Match(reminderTextBox.Text);
      if (match.Success)
      {
        reminder = new HourMinute(reminderTextBox.Text);
        reminderEnabled = true;
        reminderDropDownBtn.BackgroundImage = global::kelio_client.Properties.Resources.notif_enabled;
        toolTip.SetToolTip(reminderDropDownBtn, "Modifier / Annuler le rappel à " + reminder.ToString());
        reminderFeedbackLabel.Text = "Rappel à " + reminder.ToString();
        reminderFeedbackLabel.ForeColor = Color.MediumSeaGreen;
        await Task.Delay(2000);
        clockInOutButton.Visible = true;
        reminderPanel.Visible = false;
      }
      else
      {
        reminderFeedbackLabel.Text = "Formats 18:15 / 18h15";
        reminderFeedbackLabel.ForeColor = Color.Red;
      }
    }
  }
}

