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
    private int? reminderH = null;
    private int? reminderM = null;
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

    private void UpdateInfos(string htmlContent)
    {
      MatchCollection matches = new Regex(@"<td class='(?:tabImpair|tabPair)'>([0-9]{2}:[0-9]{2})<\/td>\s*?<td class='(?:tabImpair|tabPair)'>(.*?)<\/td>").Matches(htmlContent);
      inOutBox.Text = "";
      int clockInCount = 0;
      int clockOutCount = 0;
      string lastClockOut = "";
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
          lastClockOut = match.Groups[1].Value;
          AppendText(lastClockOut, Color.Orange, true);
        }
      }

      string weekDiff = new Regex(@"<li>Votre crédit \/ débit hebdomadaire est de (.*)<\/li>").Match(htmlContent).Groups[1].Value;
      string weekDiffYesterday = new Regex(@"<li>Votre crédit \/ débit hebdomadaire arrêté à la veille est de (.*)<\/li>").Match(htmlContent).Groups[1].Value;

      if (clockInCount == 0)
        BeepBeep();

      int? remindH = null;
      int? remindM = null;

      if (clockInCount > clockOutCount)
      {
        if (weekDiff.StartsWith("-"))
        {
          remindH = DateTime.Now.Hour - Int32.Parse(weekDiff.Split(':')[0]);
          remindM = DateTime.Now.Minute + Int32.Parse(weekDiff.Split(':')[1]);
        }
        else
        {
          remindH = DateTime.Now.Hour + Int32.Parse(weekDiff.Split(':')[0]);
          remindM = DateTime.Now.Minute - Int32.Parse(weekDiff.Split(':')[1]);
        }
        while (remindM > 59)
        {
          remindH++;
          remindM -= 60;
        }
        while (remindM < 0)
        {
          remindH--;
          remindM += 60;
        }
        int endOfDayH = remindH.GetValueOrDefault(DateTime.Now.Hour);
        int endOfDayM = remindM.GetValueOrDefault(DateTime.Now.Minute);
        if (DateTime.Now.Hour < 13)
        {
          int pause = GetPause(DateTime.Now.DayOfWeek);
          if (pause > 0)
          {
            endOfDayH = remindH.GetValueOrDefault(0);
            endOfDayM = remindM.GetValueOrDefault(0) + pause;
            if (pause > 105)
            {
              remindH = 12;
              remindM = 0;
              AppendText("12:00", Color.FromArgb(96, 96, 96), true);
              AppendText((12 + (int)Math.Floor((float)pause / 60)) + ":" + (pause % 60).ToString("00"), Color.FromArgb(96, 96, 96), false);
            }
            else
            {
              remindH = 12;
              remindM = 15;
              AppendText("12:15", Color.FromArgb(96, 96, 96), true);
              AppendText((pause == 105 ? "14" : "13") + ":" + ((float)(pause - 45) % 60).ToString("00"), Color.FromArgb(96, 96, 96), false);
            }
            AppendText(" - ", Color.White, false);
          }
        }
        while (endOfDayM > 59)
        {
          endOfDayH++;
          endOfDayM -= 60;
        }
        while (endOfDayM < 0)
        {
          endOfDayH--;
          endOfDayM += 60;
        }
        AppendText(endOfDayH.ToString("00") + ":" + endOfDayM.ToString("00"), Color.FromArgb(96, 96, 96), true);
      }
      else
      {
        if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour <= 13 && lastClockOut != "")
        {
          int nextClockInH = Int32.Parse(lastClockOut.Split(':')[0]);
          int nextClockInM = Int32.Parse(lastClockOut.Split(':')[1]) + GetPause(DateTime.Now.DayOfWeek);
          while (nextClockInM > 59)
          {
            nextClockInH++;
            nextClockInM -= 60;
          }
          if (nextClockInH > 13)
          {
            nextClockInH = 14;
            nextClockInM = 0;
          }
          remindH = nextClockInH - Int32.Parse(weekDiff.Split(':')[0]);
          remindM = nextClockInM + Int32.Parse(weekDiff.Split(':')[1]);
          while (remindM > 59)
          {
            remindH++;
            remindM -= 60;
          }
          AppendText(nextClockInH.ToString("00") + ":" + nextClockInM.ToString("00"), Color.FromArgb(96, 96, 96), false);
          AppendText(" - ", Color.White, false);
          AppendText(remindH?.ToString("00") + ":" + remindM?.ToString("00"), Color.FromArgb(96, 96, 96), true);
          remindH = nextClockInH;
          remindM = nextClockInM;
        }

      }
      if (remindM != null && remindH != null && !reminderEnabled)
      {
        reminderH = remindH;
        reminderM = remindM;
        if (Properties.Settings.Default.AutoReminder)
        {
          reminderEnabled = true;
          reminderDropDownBtn.BackgroundImage = global::kelio_client.Properties.Resources.notif_enabled;
          toolTip.SetToolTip(reminderDropDownBtn, "Modifier / Annuler le rappel à " + reminderH?.ToString("00") + ":" + reminderM?.ToString("00"));
        }
      }

      string totalDiff = new Regex(@"<li>Votre crédit \/ débit total arrêté à la veille est de (.*)<\/li>").Match(htmlContent).Groups[1].Value; ;
      if (clockInCount == clockOutCount && DateTime.Now.Hour >= 16)
      {
        weekDiffLabel.Text = ((weekDiff.StartsWith("-") || weekDiff == "0:00") ? "" : "+") + weekDiff;
        toolTip.SetToolTip(weekDiffTitleLabel, "Crédit / débit hebdomadaire");
        toolTip.SetToolTip(weekDiffLabel, "Crédit / débit hebdomadaire");
      }
      else
      {
        weekDiffLabel.Text = ((weekDiffYesterday.StartsWith("-") || weekDiffYesterday == "0:00") ? "" : "+") + weekDiffYesterday;
        toolTip.SetToolTip(weekDiffTitleLabel, "Crédit / débit hebdomadaire arrêté à la veille");
        toolTip.SetToolTip(weekDiffLabel, "Crédit / débit hebdomadaire arrêté à la veille");
      }

      totalDiffLabel.Text = (totalDiff.StartsWith("-") ? "" : "+") + totalDiff;
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
      if (reminderEnabled && DateTime.Now.Hour == reminderH && DateTime.Now.Minute == reminderM)
      {
        reminderH = null;
        reminderM = null;
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
      if (e.CloseReason == CloseReason.UserClosing && reminderEnabled && reminderH != null && reminderM != null)
      {
        if (MessageBox.Show("Un rappel est activé pour " + reminderH + ":" + reminderM + "\nQuitter ?", "Confirmer la fermeture", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
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
        if (reminderH != null && reminderM != null)
        {
          reminderTextBox.Text = reminderH.GetValueOrDefault(0).ToString("00") + ":" + reminderM.GetValueOrDefault(0).ToString("00");
        }
        else
        {
          reminderTextBox.Text = DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00");
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
        reminderH = Int32.Parse(match.Groups[1].Value);
        reminderM = Int32.Parse(match.Groups[2].Value);
        reminderEnabled = true;
        reminderDropDownBtn.BackgroundImage = global::kelio_client.Properties.Resources.notif_enabled;
        toolTip.SetToolTip(reminderDropDownBtn, "Modifier / Annuler le rappel à " + reminderH?.ToString("00") + ":" + reminderM?.ToString("00"));
        reminderFeedbackLabel.Text = "Rappel à " + reminderH?.ToString("00") + ":" + reminderM?.ToString("00");
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

