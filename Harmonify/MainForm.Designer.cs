
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
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
    }
}