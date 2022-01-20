
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
            this.MidiFileNameLabel.Location = new System.Drawing.Point(116, 17);
            this.MidiFileNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.MidiFileNameLabel.Name = "MidiFileNameLabel";
            this.MidiFileNameLabel.Size = new System.Drawing.Size(219, 20);
            this.MidiFileNameLabel.TabIndex = 0;
            this.MidiFileNameLabel.Text = "Select a midi file to harmonify.";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(15, 216);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(499, 296);
            this.textBox1.TabIndex = 1;
            // 
            // ImportFileButton
            // 
            this.ImportFileButton.Location = new System.Drawing.Point(420, 12);
            this.ImportFileButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ImportFileButton.Name = "ImportFileButton";
            this.ImportFileButton.Size = new System.Drawing.Size(96, 31);
            this.ImportFileButton.TabIndex = 2;
            this.ImportFileButton.Text = "Import..";
            this.ImportFileButton.UseVisualStyleBackColor = true;
            this.ImportFileButton.Click += new System.EventHandler(this.ImportFileButton_Click);
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Default;
            this.button1.Location = new System.Drawing.Point(562, 483);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(96, 31);
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
            this.KeyRootComboBox.Location = new System.Drawing.Point(175, 80);
            this.KeyRootComboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.KeyRootComboBox.Name = "KeyRootComboBox";
            this.KeyRootComboBox.Size = new System.Drawing.Size(70, 28);
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
            this.KeyMajorityComboBox.Location = new System.Drawing.Point(253, 80);
            this.KeyMajorityComboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.KeyMajorityComboBox.Name = "KeyMajorityComboBox";
            this.KeyMajorityComboBox.Size = new System.Drawing.Size(134, 28);
            this.KeyMajorityComboBox.TabIndex = 5;
            this.KeyMajorityComboBox.SelectedIndexChanged += new System.EventHandler(this.MajorityComboBox_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(420, 148);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(96, 31);
            this.button2.TabIndex = 6;
            this.button2.Text = "Analyze";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // KeyLabel
            // 
            this.KeyLabel.AutoSize = true;
            this.KeyLabel.Location = new System.Drawing.Point(116, 84);
            this.KeyLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.KeyLabel.Name = "KeyLabel";
            this.KeyLabel.Size = new System.Drawing.Size(33, 20);
            this.KeyLabel.TabIndex = 7;
            this.KeyLabel.Text = "Key";
            this.KeyLabel.Click += new System.EventHandler(this.KeyLabel_Click);
            // 
            // SpiceTrackBar
            // 
            this.SpiceTrackBar.Location = new System.Drawing.Point(165, 148);
            this.SpiceTrackBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SpiceTrackBar.Name = "SpiceTrackBar";
            this.SpiceTrackBar.Size = new System.Drawing.Size(134, 56);
            this.SpiceTrackBar.TabIndex = 9;
            this.SpiceTrackBar.Scroll += new System.EventHandler(this.SpiceTrackBar_Scroll);
            // 
            // AssumeKeysButton
            // 
            this.AssumeKeysButton.Location = new System.Drawing.Point(420, 80);
            this.AssumeKeysButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AssumeKeysButton.Name = "AssumeKeysButton";
            this.AssumeKeysButton.Size = new System.Drawing.Size(96, 31);
            this.AssumeKeysButton.TabIndex = 10;
            this.AssumeKeysButton.Text = "Guess";
            this.AssumeKeysButton.UseVisualStyleBackColor = true;
            this.AssumeKeysButton.Click += new System.EventHandler(this.AssumeKeysButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 536);
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
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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