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
        public int selectedIndex;
        MainForm mainForm;
        public SelectTrackForm(List<string> trackNames, MainForm _mainForm)
        {
            InitializeComponent();
            mainForm = _mainForm;
            comboBox1.Items.AddRange(trackNames.ToArray());
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = comboBox1.SelectedIndex;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void SelectTrackForm_Load(object sender, EventArgs e)
        {

        }
    }
}
