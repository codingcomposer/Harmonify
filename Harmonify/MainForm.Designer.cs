
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.assumedKeysLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // MidiFileNameLabel
            // 
            this.MidiFileNameLabel.AutoSize = true;
            this.MidiFileNameLabel.Location = new System.Drawing.Point(201, 38);
            this.MidiFileNameLabel.Name = "MidiFileNameLabel";
            this.MidiFileNameLabel.Size = new System.Drawing.Size(174, 15);
            this.MidiFileNameLabel.TabIndex = 0;
            this.MidiFileNameLabel.Text = "Select a midi file to harmonify.";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(86, 136);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(389, 223);
            this.textBox1.TabIndex = 1;
            // 
            // ImportFileButton
            // 
            this.ImportFileButton.Location = new System.Drawing.Point(400, 34);
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
            this.button1.Location = new System.Drawing.Point(400, 390);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Play";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
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
            this.comboBox1.Location = new System.Drawing.Point(117, 89);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(55, 23);
            this.comboBox1.TabIndex = 4;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "모름",
            "Major",
            "Minor"});
            this.comboBox2.Location = new System.Drawing.Point(178, 89);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(105, 23);
            this.comboBox2.TabIndex = 5;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(400, 88);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Analyze";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(85, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Key";
            // 
            // assumedKeysLabel
            // 
            this.assumedKeysLabel.AutoSize = true;
            this.assumedKeysLabel.Location = new System.Drawing.Point(298, 92);
            this.assumedKeysLabel.Name = "assumedKeysLabel";
            this.assumedKeysLabel.Size = new System.Drawing.Size(0, 15);
            this.assumedKeysLabel.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.assumedKeysLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ImportFileButton);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.MidiFileNameLabel);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MidiFileNameLabel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button ImportFileButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label assumedKeysLabel;
    }
}