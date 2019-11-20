using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Minesweeper
{
    public class Field
    {
        public int winCondition { get; set; }
        public int _tableR { get; private set; }
        public int _tableC { get; private set; }
        public int _tableBombs { get; private set; }
        public int _tableSafe { get; private set; }
        public Minefiled[,] _mat { get; private set; }
        public Field(int tableR, int tableC, int tableBombs, int tableSafe, FormMinesweeper parent)
        {
            this._tableR = tableR;
            this._tableC = tableC;
            this._tableBombs = tableBombs;
            this._tableSafe = tableSafe;
            _mat = new Minefiled[_tableR, _tableC];

            int X = 2;
            int Y = 0;
            var size = new Size(X + 50 * _tableC + 20, Y + 50 * _tableR + 67);
            parent.Size = size;

            var random = new Random();

            for (int i = 0; i < _tableR; i++)
            {
                for (int j = 0; j < _tableC; j++)
                {

                    _mat[i, j] = new Minefiled
                    {
                        Size = new Size(50, 50),
                        BackColor = Color.LightGray,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left,
                        Location = new Point(X, Y),
                        Image = Image.FromFile("Resources\\closedField-50x50.png"),
                        X = j,
                        Y = i,
                        IsOpen = false,
                        BombsAround = 0
                    };
                    _mat[i, j].MouseDown += parent.FormMinesweeper_MouseDown;
                    parent.panelGame.Controls.Add(_mat[i, j]);
                    X += 50;
                }
                Y += 50;
                X = 2;
            }
            if (_tableBombs > _tableR * _tableC) _tableBombs = _tableR * _tableC;
            while (true)
            {
                int i = random.Next(0, _tableR);
                int j = random.Next(0, _tableC);

                if (!_mat[i, j].IsBomb)
                {
                    _mat[i, j].IsBomb = true;
                    _mat[i, j].BackgroundImage = Image.FromFile("Resources\\mine-clipart-50x50-inactive.png");

                    int LowYbounds = i - 1 < 0 ? 0 : i - 1;
                    int HighYbounds = i + 1 >= _tableR ? _tableR - 1 : i + 1;
                    int LowXbounds = j - 1 < 0 ? 0 : j - 1;
                    int HighXbounds = j + 1 >= _tableC ? _tableC - 1 : j + 1;

                    for (int ia = LowYbounds; ia <= HighYbounds; ia++)
                    {
                        for (int ja = LowXbounds; ja <= HighXbounds; ja++)
                        {
                            _mat[ia, ja].BombsAround++;
                        }
                    }

                    if (--_tableBombs == 0) return;
                }
            }
        }
        public void DecreaseSafeFields()
        {
            if (--_tableSafe == 0) winCondition = 1;
        }

        public void Serialize(string filename, string sender)
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
                    minefileds[i][j].BombsAround = this._mat[i, j].BombsAround;
                    minefileds[i][j].IsBomb = this._mat[i, j].IsBomb;
                    minefileds[i][j].IsFlag = this._mat[i, j].IsFlag;
                    minefileds[i][j].IsOpen = sender == "&Save" ? this._mat[i, j].IsOpen : false;
                    minefileds[i][j].X = this._mat[i, j].X;
                    minefileds[i][j].Y = this._mat[i, j].Y;
                }
            }
            serializer.Serialize(writer, minefileds);
            writer.Close();

        }

    }
}
