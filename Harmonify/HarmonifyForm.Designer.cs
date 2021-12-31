
namespace Harmonify
{
    partial class Harmonify
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ImportFileButton = new System.Windows.Forms.Button();
            this.MidiFileNameLabel = new System.Windows.Forms.Label();
            this.MakeChordsButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ImportFileButton
            // 
            this.ImportFileButton.Location = new System.Drawing.Point(124, 23);
            this.ImportFileButton.Name = "ImportFileButton";
            this.ImportFileButton.Size = new System.Drawing.Size(101, 23);
            this.ImportFileButton.TabIndex = 0;
            this.ImportFileButton.Text = "파일 가져오기";
            this.ImportFileButton.UseVisualStyleBackColor = true;
            this.ImportFileButton.Click += new System.EventHandler(this.ImportFileButton_Click);
            // 
            // MidiFileNameLabel
            // 
            this.MidiFileNameLabel.AutoSize = true;
            this.MidiFileNameLabel.Location = new System.Drawing.Point(253, 27);
            this.MidiFileNameLabel.Name = "MidiFileNameLabel";
            this.MidiFileNameLabel.Size = new System.Drawing.Size(114, 15);
            this.MidiFileNameLabel.TabIndex = 1;
            this.MidiFileNameLabel.Text = "아직 선택되지 않음.";
            // 
            // MakeChordsButton
            // 
            this.MakeChordsButton.Location = new System.Drawing.Point(124, 74);
            this.MakeChordsButton.Name = "MakeChordsButton";
            this.MakeChordsButton.Size = new System.Drawing.Size(75, 23);
            this.MakeChordsButton.TabIndex = 3;
            this.MakeChordsButton.Text = "만들기";
            this.MakeChordsButton.UseVisualStyleBackColor = true;
            this.MakeChordsButton.Click += new System.EventHandler(this.MakeChordsButton_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(124, 140);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(497, 237);
            this.textBox1.TabIndex = 4;
            // 
            // Harmonify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.MakeChordsButton);
            this.Controls.Add(this.MidiFileNameLabel);
            this.Controls.Add(this.ImportFileButton);
            this.Name = "Harmonify";
            this.Text = "Harmonify";
            this.Load += new System.EventHandler(this.Harmonify_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ImportFileButton;
        private System.Windows.Forms.Label MidiFileNameLabel;
        private System.Windows.Forms.Button MakeChordsButton;
        private System.Windows.Forms.TextBox textBox1;
    }
}

