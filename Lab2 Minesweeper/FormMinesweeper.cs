using NewGame;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Minesweeper
{
    public partial class FormMinesweeper : Form
    {
        public int _tableR, _tableC, _tableBombs, _tableSafe;

        private Field _field;

        public Timer timer = new Timer();
        private DateTime _startTime;
        public FormMinesweeper(int column = 9, int row = 9, int bombs = 10)
        {
            InitializeComponent();
            _tableC = column;
            _tableR = row;
            _tableBombs = bombs;
            _tableSafe = _tableC * _tableR - _tableBombs;
            InitButtons(column, row, bombs);
            timer.Tick += Timer_Tick;
            timer.Interval = 500;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Text = (DateTime.Now - _startTime).ToString(@"hh\:mm\:ss");
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormNewGame formNewGame = new FormNewGame(this);
            formNewGame.Show();
            this.Enabled = false;
        }
        public void InitButtons(int row, int column, int bombs)
        {
            _field = new Field(row, column, bombs, column * row - bombs, this);
        }
        private void endGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var v in _field._mat/*_mat*/)
            {
                _field.winCondition = -1;
                v.OpenField(_field, ((ToolStripMenuItem)sender).Text);
                v.Enabled = false;
            }
            timer.Stop();
        }
        private void FormMinesweeper_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
        private void saveStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _field.Serialize("lastSave.xml", ((ToolStripMenuItem)sender).Text);
        }
        private void saveStateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _field.Serialize("lastState.xml", ((ToolStripMenuItem)sender).Text);
        }
        public void FormMinesweeper_MouseDown(object sender, MouseEventArgs e)
        {
            if (!timer.Enabled)
            {
                timer.Start();
                _startTime = DateTime.Now;
            }

            Minefiled minefiled = sender as Minefiled;

            if (e.Button == MouseButtons.Left)
            {
                minefiled.OpenField(_field, sender.ToString());
                if (_field.winCondition != 0) timer.Stop();
                if (_field.winCondition == 1)
                {
                    MessageBox.Show("VI VON ZULUL");
                    return;
                }
                if (_field.winCondition == -1)
                {
                    MessageBox.Show("You lost!");
                    return;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (minefiled.IsOpen) return;
                if (minefiled.IsFlag)
                {
                    minefiled.IsFlag = false;
                    minefiled.Image = Image.FromFile("Resources\\closedField-50x50.png");
                }
                else
                {
                    minefiled.IsFlag = true;
                    minefiled.Image = Image.FromFile("Resources\\flag-50x50.png");
                }

            }

            ActiveControl = null;
        }
    }
}
