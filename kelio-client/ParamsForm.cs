using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kelio_client
{
  public partial class ParamsForm : Form
  {
    public ParamsForm()
    {
      InitializeComponent();
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
      Properties.Settings.Default.Url = urlTextBox.Text;
      Properties.Settings.Default.Login = loginTextBox.Text;
      Properties.Settings.Default.Password = EncryptionHelper.Encrypt(passwordTextBox.Text);
      Properties.Settings.Default.PauseMonday = (int) mondayNumericUpDown.Value;
      Properties.Settings.Default.PauseTuesday = (int) tuesdayNumericUpDown.Value;
      Properties.Settings.Default.PauseWednesday = (int) wednesdayNumericUpDown.Value;
      Properties.Settings.Default.PauseThursday = (int) thursdayNumericUpDown.Value;
      Properties.Settings.Default.PauseFriday = (int) fridayNumericUpDown.Value;
      Properties.Settings.Default.AutoReminder = autoReminderCheckBox.Checked;
      Properties.Settings.Default.Save();
    }

    private void ParamsForm_Load(object sender, EventArgs e)
    {
      string primaryDnsSuffix = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
      if (Properties.Settings.Default.Url == "https://kelio.domain.tld" && primaryDnsSuffix != "")
        urlTextBox.Text = "https://kelio." + primaryDnsSuffix;
      else 
        urlTextBox.Text = Properties.Settings.Default.Url;
      loginTextBox.Text = Properties.Settings.Default.Login;
      try
      {
        passwordTextBox.Text = EncryptionHelper.Decrypt(Properties.Settings.Default.Password);
      }
      catch(Exception)
      {
        passwordTextBox.Text = Properties.Settings.Default.Password;
      }
      mondayNumericUpDown.Value = Properties.Settings.Default.PauseMonday;
      tuesdayNumericUpDown.Value = Properties.Settings.Default.PauseTuesday;
      wednesdayNumericUpDown.Value = Properties.Settings.Default.PauseWednesday;
      thursdayNumericUpDown.Value = Properties.Settings.Default.PauseThursday;
      fridayNumericUpDown.Value = Properties.Settings.Default.PauseFriday;
      autoReminderCheckBox.Checked = Properties.Settings.Default.AutoReminder;
    }
  }
}
