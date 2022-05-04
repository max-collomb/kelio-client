
using System.Windows.Forms;

namespace kelio_client
{
  public class ExRichTextBox : System.Windows.Forms.RichTextBox
  {
    const int WM_SETCURSOR = 0x0020;
    protected override void WndProc(ref Message m)
    {
      if (m.Msg == WM_SETCURSOR)
        Cursor.Current = this.Cursor;
      else
        base.WndProc(ref m);
    }
  }
  partial class MainForm
  {
    /// <summary>
    /// Variable nécessaire au concepteur.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Nettoyage des ressources utilisées.
    /// </summary>
    /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Code généré par le Concepteur Windows Form

    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.panel2 = new System.Windows.Forms.Panel();
      this.reminderDropDownBtn = new System.Windows.Forms.Button();
      this.panel4 = new System.Windows.Forms.Panel();
      this.refreshButton = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.paramButton = new System.Windows.Forms.Button();
      this.kelioUrlLabel = new System.Windows.Forms.LinkLabel();
      this.panel3 = new System.Windows.Forms.Panel();
      this.reminderPanel = new System.Windows.Forms.Panel();
      this.reminderFeedbackLabel = new System.Windows.Forms.Label();
      this.panel9 = new System.Windows.Forms.Panel();
      this.reminderTextBox = new System.Windows.Forms.TextBox();
      this.panel8 = new System.Windows.Forms.Panel();
      this.validateReminderBtn = new System.Windows.Forms.Button();
      this.panel5 = new System.Windows.Forms.Panel();
      this.cancelReminderBtn = new System.Windows.Forms.Button();
      this.feedbackLabel = new System.Windows.Forms.Label();
      this.progressBar = new System.Windows.Forms.ProgressBar();
      this.clockInOutButton = new System.Windows.Forms.Button();
      this.panel6 = new System.Windows.Forms.Panel();
      this.inOutBox = new kelio_client.ExRichTextBox();
      this.panel7 = new System.Windows.Forms.Panel();
      this.label10 = new System.Windows.Forms.Label();
      this.totalDiffLabel = new System.Windows.Forms.Label();
      this.weekDiffLabel = new System.Windows.Forms.Label();
      this.weekDiffTitleLabel = new System.Windows.Forms.Label();
      this.timer = new System.Windows.Forms.Timer(this.components);
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.panel2.SuspendLayout();
      this.panel3.SuspendLayout();
      this.reminderPanel.SuspendLayout();
      this.panel9.SuspendLayout();
      this.panel6.SuspendLayout();
      this.panel7.SuspendLayout();
      this.SuspendLayout();
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.reminderDropDownBtn);
      this.panel2.Controls.Add(this.panel4);
      this.panel2.Controls.Add(this.refreshButton);
      this.panel2.Controls.Add(this.panel1);
      this.panel2.Controls.Add(this.paramButton);
      this.panel2.Controls.Add(this.kelioUrlLabel);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel2.Location = new System.Drawing.Point(0, 0);
      this.panel2.Margin = new System.Windows.Forms.Padding(0);
      this.panel2.Name = "panel2";
      this.panel2.Padding = new System.Windows.Forms.Padding(8, 4, 8, 0);
      this.panel2.Size = new System.Drawing.Size(260, 26);
      this.panel2.TabIndex = 9;
      // 
      // reminderDropDownBtn
      // 
      this.reminderDropDownBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
      this.reminderDropDownBtn.BackgroundImage = global::kelio_client.Properties.Resources.notif_disabled;
      this.reminderDropDownBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.reminderDropDownBtn.Dock = System.Windows.Forms.DockStyle.Right;
      this.reminderDropDownBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.reminderDropDownBtn.Location = new System.Drawing.Point(176, 4);
      this.reminderDropDownBtn.Margin = new System.Windows.Forms.Padding(0);
      this.reminderDropDownBtn.Name = "reminderDropDownBtn";
      this.reminderDropDownBtn.Size = new System.Drawing.Size(22, 22);
      this.reminderDropDownBtn.TabIndex = 8;
      this.toolTip.SetToolTip(this.reminderDropDownBtn, "Définir un rappel");
      this.reminderDropDownBtn.UseVisualStyleBackColor = false;
      this.reminderDropDownBtn.Click += new System.EventHandler(this.reminderDropDownBtn_Click);
      // 
      // panel4
      // 
      this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
      this.panel4.Location = new System.Drawing.Point(198, 4);
      this.panel4.Margin = new System.Windows.Forms.Padding(0);
      this.panel4.Name = "panel4";
      this.panel4.Size = new System.Drawing.Size(5, 22);
      this.panel4.TabIndex = 5;
      // 
      // refreshButton
      // 
      this.refreshButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
      this.refreshButton.BackgroundImage = global::kelio_client.Properties.Resources.refresh;
      this.refreshButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.refreshButton.Dock = System.Windows.Forms.DockStyle.Right;
      this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.refreshButton.Location = new System.Drawing.Point(203, 4);
      this.refreshButton.Margin = new System.Windows.Forms.Padding(0);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(22, 22);
      this.refreshButton.TabIndex = 7;
      this.toolTip.SetToolTip(this.refreshButton, "Rafraîchir");
      this.refreshButton.UseVisualStyleBackColor = false;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // panel1
      // 
      this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
      this.panel1.Location = new System.Drawing.Point(225, 4);
      this.panel1.Margin = new System.Windows.Forms.Padding(0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(5, 22);
      this.panel1.TabIndex = 4;
      // 
      // paramButton
      // 
      this.paramButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
      this.paramButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("paramButton.BackgroundImage")));
      this.paramButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.paramButton.Dock = System.Windows.Forms.DockStyle.Right;
      this.paramButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.paramButton.Location = new System.Drawing.Point(230, 4);
      this.paramButton.Margin = new System.Windows.Forms.Padding(0);
      this.paramButton.Name = "paramButton";
      this.paramButton.Size = new System.Drawing.Size(22, 22);
      this.paramButton.TabIndex = 3;
      this.toolTip.SetToolTip(this.paramButton, "Options");
      this.paramButton.UseVisualStyleBackColor = false;
      this.paramButton.Click += new System.EventHandler(this.paramButton_Click);
      // 
      // kelioUrlLabel
      // 
      this.kelioUrlLabel.ActiveLinkColor = System.Drawing.Color.GhostWhite;
      this.kelioUrlLabel.AutoSize = true;
      this.kelioUrlLabel.Dock = System.Windows.Forms.DockStyle.Left;
      this.kelioUrlLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.kelioUrlLabel.LinkColor = System.Drawing.Color.CornflowerBlue;
      this.kelioUrlLabel.Location = new System.Drawing.Point(8, 4);
      this.kelioUrlLabel.Margin = new System.Windows.Forms.Padding(0);
      this.kelioUrlLabel.Name = "kelioUrlLabel";
      this.kelioUrlLabel.Size = new System.Drawing.Size(0, 18);
      this.kelioUrlLabel.TabIndex = 2;
      this.kelioUrlLabel.UseCompatibleTextRendering = true;
      this.kelioUrlLabel.VisitedLinkColor = System.Drawing.Color.CornflowerBlue;
      this.kelioUrlLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.kelioUrlLabel_LinkClicked);
      // 
      // panel3
      // 
      this.panel3.Controls.Add(this.reminderPanel);
      this.panel3.Controls.Add(this.feedbackLabel);
      this.panel3.Controls.Add(this.progressBar);
      this.panel3.Controls.Add(this.clockInOutButton);
      this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel3.Location = new System.Drawing.Point(0, 26);
      this.panel3.Name = "panel3";
      this.panel3.Padding = new System.Windows.Forms.Padding(8);
      this.panel3.Size = new System.Drawing.Size(260, 56);
      this.panel3.TabIndex = 10;
      // 
      // reminderPanel
      // 
      this.reminderPanel.Controls.Add(this.reminderFeedbackLabel);
      this.reminderPanel.Controls.Add(this.panel9);
      this.reminderPanel.Controls.Add(this.panel8);
      this.reminderPanel.Controls.Add(this.validateReminderBtn);
      this.reminderPanel.Controls.Add(this.panel5);
      this.reminderPanel.Controls.Add(this.cancelReminderBtn);
      this.reminderPanel.Location = new System.Drawing.Point(0, 0);
      this.reminderPanel.Name = "reminderPanel";
      this.reminderPanel.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
      this.reminderPanel.Size = new System.Drawing.Size(250, 30);
      this.reminderPanel.TabIndex = 6;
      this.reminderPanel.Visible = false;
      // 
      // reminderFeedbackLabel
      // 
      this.reminderFeedbackLabel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.reminderFeedbackLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.reminderFeedbackLabel.ForeColor = System.Drawing.Color.Red;
      this.reminderFeedbackLabel.Location = new System.Drawing.Point(0, 4);
      this.reminderFeedbackLabel.Name = "reminderFeedbackLabel";
      this.reminderFeedbackLabel.Size = new System.Drawing.Size(156, 26);
      this.reminderFeedbackLabel.TabIndex = 6;
      this.reminderFeedbackLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // panel9
      // 
      this.panel9.AutoSize = true;
      this.panel9.Controls.Add(this.reminderTextBox);
      this.panel9.Dock = System.Windows.Forms.DockStyle.Right;
      this.panel9.Location = new System.Drawing.Point(156, 4);
      this.panel9.Margin = new System.Windows.Forms.Padding(0);
      this.panel9.Name = "panel9";
      this.panel9.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
      this.panel9.Size = new System.Drawing.Size(40, 26);
      this.panel9.TabIndex = 9;
      // 
      // reminderTextBox
      // 
      this.reminderTextBox.BackColor = System.Drawing.Color.Black;
      this.reminderTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.reminderTextBox.Dock = System.Windows.Forms.DockStyle.Right;
      this.reminderTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.reminderTextBox.ForeColor = System.Drawing.Color.White;
      this.reminderTextBox.Location = new System.Drawing.Point(0, 2);
      this.reminderTextBox.MaxLength = 5;
      this.reminderTextBox.Name = "reminderTextBox";
      this.reminderTextBox.Size = new System.Drawing.Size(40, 23);
      this.reminderTextBox.TabIndex = 1;
      this.reminderTextBox.Text = "17:36";
      this.reminderTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.reminderTextBox_KeyDown);
      // 
      // panel8
      // 
      this.panel8.Dock = System.Windows.Forms.DockStyle.Right;
      this.panel8.Location = new System.Drawing.Point(196, 4);
      this.panel8.Margin = new System.Windows.Forms.Padding(0);
      this.panel8.Name = "panel8";
      this.panel8.Size = new System.Drawing.Size(5, 26);
      this.panel8.TabIndex = 8;
      // 
      // validateReminderBtn
      // 
      this.validateReminderBtn.BackColor = System.Drawing.Color.Black;
      this.validateReminderBtn.BackgroundImage = global::kelio_client.Properties.Resources.check;
      this.validateReminderBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.validateReminderBtn.Dock = System.Windows.Forms.DockStyle.Right;
      this.validateReminderBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.validateReminderBtn.Location = new System.Drawing.Point(201, 4);
      this.validateReminderBtn.Margin = new System.Windows.Forms.Padding(0);
      this.validateReminderBtn.Name = "validateReminderBtn";
      this.validateReminderBtn.Size = new System.Drawing.Size(22, 26);
      this.validateReminderBtn.TabIndex = 4;
      this.toolTip.SetToolTip(this.validateReminderBtn, "Valider rappel");
      this.validateReminderBtn.UseVisualStyleBackColor = false;
      this.validateReminderBtn.Click += new System.EventHandler(this.validateReminderBtn_Click);
      // 
      // panel5
      // 
      this.panel5.Dock = System.Windows.Forms.DockStyle.Right;
      this.panel5.Location = new System.Drawing.Point(223, 4);
      this.panel5.Margin = new System.Windows.Forms.Padding(0);
      this.panel5.Name = "panel5";
      this.panel5.Size = new System.Drawing.Size(5, 26);
      this.panel5.TabIndex = 7;
      // 
      // cancelReminderBtn
      // 
      this.cancelReminderBtn.BackColor = System.Drawing.Color.Black;
      this.cancelReminderBtn.BackgroundImage = global::kelio_client.Properties.Resources.close;
      this.cancelReminderBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.cancelReminderBtn.Dock = System.Windows.Forms.DockStyle.Right;
      this.cancelReminderBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.cancelReminderBtn.Location = new System.Drawing.Point(228, 4);
      this.cancelReminderBtn.Margin = new System.Windows.Forms.Padding(0);
      this.cancelReminderBtn.Name = "cancelReminderBtn";
      this.cancelReminderBtn.Size = new System.Drawing.Size(22, 26);
      this.cancelReminderBtn.TabIndex = 5;
      this.toolTip.SetToolTip(this.cancelReminderBtn, "Annuler rappel");
      this.cancelReminderBtn.UseVisualStyleBackColor = false;
      this.cancelReminderBtn.Click += new System.EventHandler(this.cancelReminderBtn_Click);
      // 
      // feedbackLabel
      // 
      this.feedbackLabel.Dock = System.Windows.Forms.DockStyle.Top;
      this.feedbackLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
      this.feedbackLabel.ForeColor = System.Drawing.Color.Gray;
      this.feedbackLabel.Location = new System.Drawing.Point(8, 13);
      this.feedbackLabel.Name = "feedbackLabel";
      this.feedbackLabel.Size = new System.Drawing.Size(244, 2);
      this.feedbackLabel.TabIndex = 5;
      this.feedbackLabel.Text = "POST /open/j_spring_security_check";
      this.feedbackLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.feedbackLabel.Visible = false;
      // 
      // progressBar
      // 
      this.progressBar.BackColor = System.Drawing.Color.DarkRed;
      this.progressBar.Dock = System.Windows.Forms.DockStyle.Top;
      this.progressBar.ForeColor = System.Drawing.Color.DodgerBlue;
      this.progressBar.Location = new System.Drawing.Point(8, 8);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(244, 5);
      this.progressBar.TabIndex = 4;
      this.progressBar.Value = 50;
      this.progressBar.Visible = false;
      // 
      // clockInOutButton
      // 
      this.clockInOutButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
      this.clockInOutButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.clockInOutButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
      this.clockInOutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.clockInOutButton.Font = new System.Drawing.Font("Segoe UI", 11F);
      this.clockInOutButton.ForeColor = System.Drawing.Color.White;
      this.clockInOutButton.Location = new System.Drawing.Point(8, 8);
      this.clockInOutButton.Margin = new System.Windows.Forms.Padding(8);
      this.clockInOutButton.Name = "clockInOutButton";
      this.clockInOutButton.Size = new System.Drawing.Size(244, 40);
      this.clockInOutButton.TabIndex = 1;
      this.clockInOutButton.Text = "Badger";
      this.toolTip.SetToolTip(this.clockInOutButton, "Badger (Win+Shift+B puis Entrée)");
      this.clockInOutButton.UseVisualStyleBackColor = false;
      this.clockInOutButton.Visible = false;
      this.clockInOutButton.Click += new System.EventHandler(this.clockInOutButton_ClickAsync);
      // 
      // panel6
      // 
      this.panel6.Controls.Add(this.inOutBox);
      this.panel6.Controls.Add(this.panel7);
      this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel6.Location = new System.Drawing.Point(0, 82);
      this.panel6.Name = "panel6";
      this.panel6.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
      this.panel6.Size = new System.Drawing.Size(260, 107);
      this.panel6.TabIndex = 12;
      // 
      // inOutBox
      // 
      this.inOutBox.BackColor = System.Drawing.Color.Black;
      this.inOutBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.inOutBox.Cursor = System.Windows.Forms.Cursors.Arrow;
      this.inOutBox.DetectUrls = false;
      this.inOutBox.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.inOutBox.ForeColor = System.Drawing.Color.White;
      this.inOutBox.Location = new System.Drawing.Point(8, 0);
      this.inOutBox.Name = "inOutBox";
      this.inOutBox.ReadOnly = true;
      this.inOutBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.inOutBox.ShortcutsEnabled = false;
      this.inOutBox.Size = new System.Drawing.Size(110, 107);
      this.inOutBox.TabIndex = 2;
      this.inOutBox.TabStop = false;
      this.inOutBox.Text = "";
      this.inOutBox.WordWrap = false;
      // 
      // panel7
      // 
      this.panel7.Controls.Add(this.label10);
      this.panel7.Controls.Add(this.totalDiffLabel);
      this.panel7.Controls.Add(this.weekDiffLabel);
      this.panel7.Controls.Add(this.weekDiffTitleLabel);
      this.panel7.Dock = System.Windows.Forms.DockStyle.Right;
      this.panel7.Location = new System.Drawing.Point(185, 0);
      this.panel7.Name = "panel7";
      this.panel7.Padding = new System.Windows.Forms.Padding(0, 0, 8, 10);
      this.panel7.Size = new System.Drawing.Size(75, 107);
      this.panel7.TabIndex = 1;
      // 
      // label10
      // 
      this.label10.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.label10.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.label10.ForeColor = System.Drawing.Color.Silver;
      this.label10.Location = new System.Drawing.Point(0, 57);
      this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(67, 20);
      this.label10.TabIndex = 14;
      this.label10.Text = "Total";
      this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.toolTip.SetToolTip(this.label10, "Crédit / débit total arrêté à la veille");
      // 
      // totalDiffLabel
      // 
      this.totalDiffLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.totalDiffLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.totalDiffLabel.ForeColor = System.Drawing.Color.Orange;
      this.totalDiffLabel.Location = new System.Drawing.Point(0, 77);
      this.totalDiffLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.totalDiffLabel.Name = "totalDiffLabel";
      this.totalDiffLabel.Size = new System.Drawing.Size(67, 20);
      this.totalDiffLabel.TabIndex = 13;
      this.totalDiffLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.toolTip.SetToolTip(this.totalDiffLabel, "Crédit / débit total arrêté à la veille");
      // 
      // weekDiffLabel
      // 
      this.weekDiffLabel.Dock = System.Windows.Forms.DockStyle.Top;
      this.weekDiffLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.weekDiffLabel.ForeColor = System.Drawing.Color.MediumSeaGreen;
      this.weekDiffLabel.Location = new System.Drawing.Point(0, 20);
      this.weekDiffLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.weekDiffLabel.Name = "weekDiffLabel";
      this.weekDiffLabel.Size = new System.Drawing.Size(67, 20);
      this.weekDiffLabel.TabIndex = 12;
      this.weekDiffLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.toolTip.SetToolTip(this.weekDiffLabel, "Crédit / débit hebdomadaire arrêté à la veille");
      // 
      // weekDiffTitleLabel
      // 
      this.weekDiffTitleLabel.Dock = System.Windows.Forms.DockStyle.Top;
      this.weekDiffTitleLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.weekDiffTitleLabel.ForeColor = System.Drawing.Color.Silver;
      this.weekDiffTitleLabel.Location = new System.Drawing.Point(0, 0);
      this.weekDiffTitleLabel.Margin = new System.Windows.Forms.Padding(2);
      this.weekDiffTitleLabel.Name = "weekDiffTitleLabel";
      this.weekDiffTitleLabel.Size = new System.Drawing.Size(67, 20);
      this.weekDiffTitleLabel.TabIndex = 10;
      this.weekDiffTitleLabel.Text = "Semaine";
      this.weekDiffTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.toolTip.SetToolTip(this.weekDiffTitleLabel, "Crédit / débit hebdomadaire arrêté à la veille");
      // 
      // timer
      // 
      this.timer.Enabled = true;
      this.timer.Interval = 1000;
      this.timer.Tick += new System.EventHandler(this.timer_Tick);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.BackColor = System.Drawing.Color.Black;
      this.ClientSize = new System.Drawing.Size(260, 189);
      this.Controls.Add(this.panel6);
      this.Controls.Add(this.panel3);
      this.Controls.Add(this.panel2);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(2);
      this.MaximizeBox = false;
      this.Name = "MainForm";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.Text = "Kelio";
      this.Activated += new System.EventHandler(this.MainForm_Activated);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.Shown += new System.EventHandler(this.MainForm_Shown);
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.panel3.ResumeLayout(false);
      this.reminderPanel.ResumeLayout(false);
      this.reminderPanel.PerformLayout();
      this.panel9.ResumeLayout(false);
      this.panel9.PerformLayout();
      this.panel6.ResumeLayout(false);
      this.panel7.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.LinkLabel kelioUrlLabel;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.Button clockInOutButton;
    private System.Windows.Forms.Panel panel6;
    private System.Windows.Forms.Panel panel7;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label totalDiffLabel;
    private System.Windows.Forms.Label weekDiffLabel;
    private System.Windows.Forms.Label weekDiffTitleLabel;
    private ExRichTextBox inOutBox;
    private Label feedbackLabel;
    private ProgressBar progressBar;
    private Button paramButton;
    private Timer timer;
    private Panel panel1;
    private ToolTip toolTip;
    private Panel panel4;
    private Button refreshButton;
    private Panel reminderPanel;
    private Button cancelReminderBtn;
    private Button validateReminderBtn;
    private Label reminderFeedbackLabel;
    private Panel panel8;
    private Panel panel5;
    private Button reminderDropDownBtn;
    private Panel panel9;
    private TextBox reminderTextBox;
  }
}

