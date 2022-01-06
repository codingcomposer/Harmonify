using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Harmonify
{
    public partial class MainForm : Form, IuiHandler
    {
        public Song song;
        public string midiFilePath;
        public MainForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void ImportFileButton_Click(object sender, EventArgs e)
        {
            midiFilePath = ShowFileOpenDialog();
            if (midiFilePath != null)
            {
                song = new Song(midiFilePath, this);
            }
            else
            {
                MidiFileNameLabel.Text = "아직 선택되지 않음.";
            }
        }
        public string ShowFileOpenDialog()
        {
            //파일오픈창 생성 및 설정
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "분석할 미디파일을 선택하세요";
            ofd.Filter = "미디 파일 (*.mid) | *.mid;";

            //파일 오픈창 로드
            DialogResult dr = ofd.ShowDialog();

            //OK버튼 클릭시
            if (dr == DialogResult.OK)
            {
                //File명과 확장자를 가지고 온다.
                string fileName = ofd.SafeFileName;
                //File경로와 File명을 모두 가지고 온다.
                string fileFullName = ofd.FileName;
                //File경로만 가지고 온다.
                string filePath = fileFullName.Replace(fileName, "");

                //출력 예제용 로직
                MidiFileNameLabel.Text = "파일 이름  : " + fileName;
                //File경로 + 파일명 리턴
                return fileFullName;
            }
            //취소버튼 클릭시 또는 ESC키로 파일창을 종료 했을경우
            else if (dr == DialogResult.Cancel)
            {
                return "";
            }

            return "";
        }

        public int GetTrackIndex(List<string> trackNames)
        {
            SelectTrackForm popup = new SelectTrackForm(trackNames, this);
            DialogResult dr = popup.ShowDialog();
            if(dr == DialogResult.Cancel)
            {
                return popup.selectedIndex;
            }
            return 0;
        }

        public void SetTrackIndex(int index)
        {
            song.SetTrackIndex(index);
        }

        private void MakeChordsButton_Click(object sender, EventArgs e)
        {
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        // Analyze 버튼
        private void button2_Click(object sender, EventArgs e)
        {
            if (song == null)
            {
                return;
            }
            if(comboBox1.SelectedIndex != 0)
            {
                song.KeySignature = new KeySignature();
                song.KeySignature.tonicNote = comboBox1.SelectedIndex - 1;
            }
            if (Song.MidiFile != null)
            {
                song.Analyze();                
            }
            textBox1.Text = null;
            textBox1.Text += "Time Signature : " + Song.TimeSigTop + "/" + Song.TimeSigBottom + "\r\n";
            if (song.KeySignature == null)
            {
                textBox1.Text += "Key Signature : Unknown";
            }
            else
            {
                textBox1.Text += "Key Signature : " + Note.GetNoteName(song.KeySignature.tonicNote);
            }
            textBox1.Text += "\r\n";
            for (int i = 0; i < song.sections.Count; i++)
            {
                textBox1.Text += "\r\n" + i.ToString() + ":";
                for (int j = 0; j < song.sections[i].measures.Count; j++)
                {
                    for (int k = 0; k < song.sections[i].measures[j].chords.Count; k++)
                    {
                        if (song.sections[i].measures[j].chords[k].chordNotes.Count > 0)
                        {
                            textBox1.Text += Chord.GetChordNotation(song.sections[i].measures[j].chords[k].chordNotes);
                        }
                    }
                    textBox1.Text += "|";
                }
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
