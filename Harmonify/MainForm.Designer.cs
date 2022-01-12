
namespace Harmonify
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MidiFileNameLabel = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.ImportFileButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.KeyRootComboBox = new System.Windows.Forms.ComboBox();
            this.KeyMajorityComboBox = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.KeyLabel = new System.Windows.Forms.Label();
            this.SpiceTrackBar = new System.Windows.Forms.TrackBar();
            this.AssumeKeysButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SpiceTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // MidiFileNameLabel
            // 
            this.MidiFileNameLabel.AutoSize = true;
            this.MidiFileNameLabel.Location = new System.Drawing.Point(90, 13);
            this.MidiFileNameLabel.Name = "MidiFileNameLabel";
            this.MidiFileNameLabel.Size = new System.Drawing.Size(174, 15);
            this.MidiFileNameLabel.TabIndex = 0;
            this.MidiFileNameLabel.Text = "Select a midi file to harmonify.";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 162);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(389, 223);
            this.textBox1.TabIndex = 1;
            // 
            // ImportFileButton
            // 
            this.ImportFileButton.Location = new System.Drawing.Point(327, 9);
            this.ImportFileButton.Name = "ImportFileButton";
            this.ImportFileButton.Size = new System.Drawing.Size(75, 23);
            this.ImportFileButton.TabIndex = 2;
            this.ImportFileButton.Text = "Import..";
            this.ImportFileButton.UseVisualStyleBackColor = true;
            this.ImportFileButton.Click += new System.EventHandler(this.ImportFileButton_Click);
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Default;
            this.button1.Location = new System.Drawing.Point(437, 362);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Play";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // KeyRootComboBox
            // 
            this.KeyRootComboBox.FormattingEnabled = true;
            this.KeyRootComboBox.Items.AddRange(new object[] {
            "모름",
            "C",
            "C#/Db",
            "D",
            "D#/Eb",
            "E",
            "F",
            "F#/Gb",
            "G",
            "G#/Ab",
            "A",
            "A#/Bb",
            "B"});
            this.KeyRootComboBox.Location = new System.Drawing.Point(136, 60);
            this.KeyRootComboBox.Name = "KeyRootComboBox";
            this.KeyRootComboBox.Size = new System.Drawing.Size(55, 23);
            this.KeyRootComboBox.TabIndex = 4;
            this.KeyRootComboBox.SelectedIndexChanged += new System.EventHandler(this.KeyRootComboBox_SelectedIndexChanged);
            // 
            // KeyMajorityComboBox
            // 
            this.KeyMajorityComboBox.FormattingEnabled = true;
            this.KeyMajorityComboBox.Items.AddRange(new object[] {
            "모름",
            "Major",
            "Minor"});
            this.KeyMajorityComboBox.Location = new System.Drawing.Point(197, 60);
            this.KeyMajorityComboBox.Name = "KeyMajorityComboBox";
            this.KeyMajorityComboBox.Size = new System.Drawing.Size(105, 23);
            this.KeyMajorityComboBox.TabIndex = 5;
            this.KeyMajorityComboBox.SelectedIndexChanged += new System.EventHandler(this.MajorityComboBox_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(327, 111);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Analyze";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // KeyLabel
            // 
            this.KeyLabel.AutoSize = true;
            this.KeyLabel.Location = new System.Drawing.Point(90, 63);
            this.KeyLabel.Name = "KeyLabel";
            this.KeyLabel.Size = new System.Drawing.Size(26, 15);
            this.KeyLabel.TabIndex = 7;
            this.KeyLabel.Text = "Key";
            this.KeyLabel.Click += new System.EventHandler(this.KeyLabel_Click);
            // 
            // SpiceTrackBar
            // 
            this.SpiceTrackBar.Location = new System.Drawing.Point(128, 111);
            this.SpiceTrackBar.Name = "SpiceTrackBar";
            this.SpiceTrackBar.Size = new System.Drawing.Size(104, 45);
            this.SpiceTrackBar.TabIndex = 9;
            this.SpiceTrackBar.Scroll += new System.EventHandler(this.SpiceTrackBar_Scroll);
            // 
            // AssumeKeysButton
            // 
            this.AssumeKeysButton.Location = new System.Drawing.Point(327, 60);
            this.AssumeKeysButton.Name = "AssumeKeysButton";
            this.AssumeKeysButton.Size = new System.Drawing.Size(75, 23);
            this.AssumeKeysButton.TabIndex = 10;
            this.AssumeKeysButton.Text = "Guess";
            this.AssumeKeysButton.UseVisualStyleBackColor = true;
            this.AssumeKeysButton.Click += new System.EventHandler(this.AssumeKeysButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 402);
            this.Controls.Add(this.AssumeKeysButton);
            this.Controls.Add(this.SpiceTrackBar);
            this.Controls.Add(this.KeyLabel);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.KeyMajorityComboBox);
            this.Controls.Add(this.KeyRootComboBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ImportFileButton);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.MidiFileNameLabel);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SpiceTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MidiFileNameLabel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button ImportFileButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox KeyRootComboBox;
        private System.Windows.Forms.ComboBox KeyMajorityComboBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label KeyLabel;
        private System.Windows.Forms.TrackBar SpiceTrackBar;
        private System.Windows.Forms.Button AssumeKeysButton;
    }
}