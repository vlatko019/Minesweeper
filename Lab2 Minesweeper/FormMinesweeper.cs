using NewGame;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class FormMinesweeper : Form
    {
        public FormMinesweeper(int column = 9, int row = 9, int bombs = 10)
        {
            InitializeComponent();
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
            Minefiled[,] mat = new Minefiled[row, column];

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

                    mat[i, j] = new Minefiled
                    {
                        Size = new Size(50, 50),
                        ForeColor = Color.Red,
                        BackColor = Color.LightGray,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left,
                        Location = new Point(X, Y)
                    };
                    mat[i, j].MouseDown += FormMinesweeper_MouseDown;
                    this.panelGame.Controls.Add(mat[i, j]);
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
                        if (!mat[i, j].isBomb && plantBomb)
                        {
                            mat[i, j].isBomb = true;
                            if (--bombs == 0) return;
                            break;
                        }
                    }
                }
            }
        }

        private partial class Minefiled : Button
        {
            public bool isBomb;
        }

        private void FormMinesweeper_MouseDown(object sender, MouseEventArgs e)
        {
            Minefiled minefiled = sender as Minefiled;
            if (e.Button == MouseButtons.Left)
            {
                if (minefiled.isBomb)
                {
                    minefiled.BackgroundImage = Image.FromFile("C:\\Users\\vlatk\\Desktop\\mine-clipart-50x50.png");
                    minefiled.BackColor = Color.White;
                }
                if (!minefiled.isBomb)
                {
                    minefiled.BackColor = Color.White;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                minefiled.ForeColor = Color.Blue;
                if (minefiled.Text == "Flag") { minefiled.Text = ""; }
                else if (minefiled.BackColor != Color.White) minefiled.Text = "Flag";
            }

            ActiveControl = null;
        }

    }
}
