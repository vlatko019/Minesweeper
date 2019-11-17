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
            _parent = new FormMinesweeper((int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value);
            _parent.Show();
            //_parent.InitButtons((int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value);
        }

        private void FormNewGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            //_parent.Show();
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
