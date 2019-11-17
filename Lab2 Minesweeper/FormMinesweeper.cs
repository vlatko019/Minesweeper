using NewGame;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class FormMinesweeper : Form
    {
        private Minefiled[,] _mat;
        private int _tableR, _tableC, _tableBombs, _tableSafe;
        public FormMinesweeper(int column = 9, int row = 9, int bombs = 10)
        {
            InitializeComponent();
            _tableC = column;
            _tableR = row;
            _tableBombs = bombs;
            _tableSafe = _tableC * _tableR - _tableBombs;
            InitButtons(column, row, bombs);

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormNewGame formNewGame = new FormNewGame(this);
            formNewGame.Show();
            this.Hide();
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
                        //Name = $"{i}:{j}"
                        X = i,
                        Y = j,
                        IsOpen = false
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
                        plantBomb = random.NextDouble() >= 0.8 ? true : false;
                        if (!_mat[i, j].IsBomb && plantBomb)
                        {
                            _mat[i, j].IsBomb = true;
                            if (--bombs == 0) return;
                            break;
                        }
                    }
                }
            }
        }

        private partial class Minefiled : Button
        {
            public bool IsBomb { get; set; }
            public bool IsOpen { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        private void FormMinesweeper_MouseDown(object sender, MouseEventArgs e)
        {
            Minefiled minefiled = sender as Minefiled;
            if (e.Button == MouseButtons.Left)
            {
                if (minefiled.Text == "Flag") { minefiled.Text = ""; }
                if (minefiled.IsBomb)
                {
                    minefiled.BackColor = Color.White;
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
                    minefiled.BackColor = Color.White;
                    minefiled.IsOpen = true;
                    minefiled.Text = numberOfBombsAround(minefiled);
                    if(--_tableSafe == 0)
                    {
                        MessageBox.Show("VI VON ZULUL");
                    }
                    //otvori polja koja su prazna ako je polje prazno

                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                minefiled.ForeColor = Color.Blue;
                if (minefiled.Text == "Flag") { 
                    minefiled.Text = "";
                    minefiled.ForeColor = Color.White;
                }
                else if (minefiled.BackColor != Color.White) minefiled.Text = "Flag";
            }



            ActiveControl = null;
        }

        private string numberOfBombsAround(Minefiled minefiled)
        {
            int LowXbounds = minefiled.X - 1 < 0 ? 0 : minefiled.X - 1;
            int HighXbounds = minefiled.X + 1 >= _tableR ? _tableR - 1 : minefiled.X + 1;
            int LowYbounds = minefiled.Y - 1 < 0 ? 0 : minefiled.Y - 1;
            int HighYbounds = minefiled.Y + 1 >= _tableC ? _tableC - 1 : minefiled.Y + 1;

            int bombsAround = 0;
            for (int i = LowXbounds; i <= HighXbounds; i++)
            {
                for (int j = LowYbounds; j <= HighYbounds; j++)
                {
                    if (_mat[i, j].IsBomb)
                    {
                        bombsAround++;
                    }
                }
            }
            return bombsAround.ToString();
        }

    }
}
