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
        private Minefiled[,] _mat;
        public int _tableR, _tableC, _tableBombs, _tableSafe;
        Timer timer = new Timer();
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

        private void saveGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To do...");
        }

        public void InitButtons(int row, int column, int bombs)
        {
            _mat = new Minefiled[row, column];

            int X = 2;
            int Y = 0;
            var size = new Size(X + 50 * column + 20, Y + 50 * row + 67);
            this.Size = size;

            var random = new Random();
            bool plantBomb;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {

                    _mat[i, j] = new Minefiled
                    {
                        Size = new Size(50, 50),
                        ForeColor = Color.Red,
                        BackColor = Color.LightGray,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left,
                        Location = new Point(X, Y),
                        BackgroundImage = Image.FromFile("Resources\\closedField-50x50.png"),
                        X = i,
                        Y = j,
                        IsOpen = false,
                        BombsAround = 0
                    };
                    _mat[i, j].MouseDown += FormMinesweeper_MouseDown;
                    this.panelGame.Controls.Add(_mat[i, j]);
                    X += 50;
                }
                Y += 50;
                X = 2;
            }
            if (bombs > row * column) bombs = row * column;
            while (true)
            {
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        plantBomb = random.NextDouble() >= 0.85 ? true : false;
                        if (!_mat[i, j].IsBomb && plantBomb)
                        {
                            _mat[i, j].IsBomb = true;

                            int LowXbounds = i - 1 < 0 ? 0 : i - 1;
                            int HighXbounds = i + 1 >= _tableR ? _tableR - 1 : i + 1;
                            int LowYbounds = j - 1 < 0 ? 0 : j - 1;
                            int HighYbounds = j + 1 >= _tableC ? _tableC - 1 : j + 1;

                            for (int ia = LowXbounds; ia <= HighXbounds; ia++)
                            {
                                for (int ja = LowYbounds; ja <= HighYbounds; ja++)
                                {
                                    _mat[ia, ja].BombsAround++;
                                }
                            }

                            if (--bombs == 0) return;
                            break;
                        }
                    }
                }
            }
        }
        public partial class Minefiled : Button
        {
            public bool IsBomb { get; set; }
            public bool IsOpen { get; set; }
            public bool IsFlag { get; set; }
            public int BombsAround { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        public class MinefieldOnly
        {
            public int X { get; set; }
            public int Y { get; set; }
            public bool IsFlag { get; set; }
            public bool IsOpen { get; set; }
            public bool IsBomb { get; set; }
            public int BombsAround { get; set; }
        }

        private void endGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var v in _mat)
            {
                OpenField(v);
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
            Serialize("lastSave.xml", ((ToolStripMenuItem)sender).Text);
        }
        private void saveStateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Serialize("lastState.xml", ((ToolStripMenuItem)sender).Text);
        }
        private void Serialize(string filename, string sender)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MinefieldOnly[][]));
            TextWriter writer = new StreamWriter(filename);

            MinefieldOnly[][] minefileds = new MinefieldOnly[_tableR][];

            for (int i = 0; i < _tableC; i++)
            {
                minefileds[i] = new MinefieldOnly[_tableC];
            }

            for (int i = 0; i < _tableR; i++)
            {
                for (int j = 0; j < _tableC; j++)
                {
                    minefileds[i][j] = new MinefieldOnly();
                    minefileds[i][j].BombsAround = _mat[i, j].BombsAround;
                    minefileds[i][j].IsBomb = _mat[i, j].IsBomb;
                    minefileds[i][j].IsFlag = _mat[i, j].IsFlag;
                    minefileds[i][j].IsOpen = sender == "&Save" ? _mat[i, j].IsOpen : false;
                    minefileds[i][j].X = _mat[i, j].X;
                    minefileds[i][j].Y = _mat[i, j].Y;
                }
            }
            serializer.Serialize(writer, minefileds);
            writer.Close();

        }


        private void FormMinesweeper_MouseDown(object sender, MouseEventArgs e)
        {
            if (!timer.Enabled)
            {
                timer.Start();
                _startTime = DateTime.Now;
            }
            Minefiled minefiled = sender as Minefiled;
            if (e.Button == MouseButtons.Left)
            {
                if (minefiled.IsFlag)
                {
                    return;
                }
                if (minefiled.IsBomb)
                {
                    timer.Stop();
                    foreach (var x in _mat)
                    {
                        if (x.IsBomb)
                        {
                            x.BackgroundImage = Image.FromFile("Resources\\mine-clipart-50x50-inactive.png");
                        }
                        x.Enabled = false;
                    }
                    minefiled.BackgroundImage = Image.FromFile("Resources\\mine-clipart-50x50.png");
                }
                if (minefiled.IsOpen) return;
                if (!minefiled.IsBomb)
                {
                    minefiled.IsOpen = true;
                    minefiled.Text = minefiled.BombsAround > 0 ? minefiled.BombsAround.ToString() : "";
                    minefiled.BackColor = Color.White;
                    minefiled.BackgroundImage = null;
                    OpenEmptySurrounding(minefiled);
                    if (--_tableSafe == 0)
                    {
                        timer.Stop();
                        MessageBox.Show("VI VON ZULUL");
                        foreach (var v in _mat)
                        {
                            v.Enabled = false;
                        }
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (minefiled.IsOpen) return;
                if (minefiled.IsFlag)
                {
                    minefiled.ForeColor = Color.White;
                    minefiled.IsFlag = false;
                    minefiled.BackgroundImage = Image.FromFile("Resources\\closedField-50x50.png");
                }
                else
                {
                    minefiled.ForeColor = Color.Blue;
                    minefiled.IsFlag = true;
                    minefiled.BackgroundImage = Image.FromFile("Resources\\flag-50x50.png");
                }

            }

            ActiveControl = null;
        }

        private void OpenEmptySurrounding(Minefiled minefiled)
        {
            int LowXbounds = minefiled.X - 1 < 0 ? 0 : minefiled.X - 1;
            int HighXbounds = minefiled.X + 1 >= _tableR ? _tableR - 1 : minefiled.X + 1;
            int LowYbounds = minefiled.Y - 1 < 0 ? 0 : minefiled.Y - 1;
            int HighYbounds = minefiled.Y + 1 >= _tableC ? _tableC - 1 : minefiled.Y + 1;

            for (int i = LowXbounds; i <= HighXbounds; i++)
            {
                for (int j = LowYbounds; j <= HighYbounds; j++)
                {
                    if (i == minefiled.X && j == minefiled.Y) continue;
                    if (_mat[i, j].IsOpen) continue;
                    if (minefiled.Text != "") continue;
                    if (minefiled.Text == "") OpenField(_mat[i, j]);
                    OpenEmptySurrounding(_mat[i, j]);
                }
            }
        }

        private void OpenField(Minefiled minefiled)
        {
            if (minefiled.IsFlag) return;
            if (minefiled.IsOpen) return;
            if (!minefiled.IsBomb)
            {
                minefiled.IsOpen = true;
                minefiled.Text = minefiled.BombsAround > 0 ? minefiled.BombsAround.ToString() : "";
                minefiled.BackColor = Color.White;
                minefiled.BackgroundImage = null;
                --_tableSafe;
            }
        }

    }
}
