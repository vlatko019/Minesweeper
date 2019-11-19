using Minesweeper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewGame
{
    public partial class FormNewGame : Form
    {
        private FormMinesweeper _parent;
        public FormNewGame(FormMinesweeper parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            _parent.panelGame.Controls.Clear();
            _parent.InitButtons((int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value);
            _parent._tableBombs = (int)numericUpDown3.Value;
            _parent._tableC = (int)numericUpDown2.Value;
            _parent._tableR = (int)numericUpDown1.Value;
            _parent._tableSafe = _parent._tableC * _parent._tableR - _parent._tableBombs;
            this.Close();
        }

        private void FormNewGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            _parent.Enabled = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown3.Maximum = numericUpDown1.Value * numericUpDown2.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown3.Maximum = numericUpDown1.Value * numericUpDown2.Value;
        }
    }
}
