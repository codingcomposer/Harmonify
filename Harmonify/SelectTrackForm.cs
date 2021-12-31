using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Harmonify
{
    public partial class SelectTrackForm : Form
    {
        Harmonify harmonify;
        public SelectTrackForm()
        {
            InitializeComponent();
        }

        public void SetComboBox(string[] trackNames, Harmonify _harmonify)
        {
            harmonify = _harmonify;
            comboBox1.Items.AddRange(trackNames);
            comboBox1.SelectedIndex = 0;
            MessageBox.Show("here");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            harmonify.SetTrackIndex(comboBox1.SelectedIndex);
        }
    }
}
