using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Minefiled : Button
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsFlag { get; set; }
        public bool IsOpen { get; set; }
        public bool IsBomb { get; set; }
        public int BombsAround { get; set; }
        public void OpenField(Field field, string eventSender)
        {
            if (this.IsOpen) return;

            if (eventSender != "&End game" && this.IsFlag) return;
            this.Image = null;
            if (!this.IsBomb)
            {
                this.IsOpen = true;
                this.Text = this.BombsAround > 0 ? this.BombsAround.ToString() : "";
                this.BackColor = Color.White;
                if (this.Text == "") OpenEmptySurrounding(this, field);
                field.DecreaseSafeFields();
            }
            if (this.IsBomb && field.winCondition != -1)
            {
                field.winCondition = -1;
                this.IsOpen = true;
                OpenAllFields(field);
                this.BackgroundImage = Image.FromFile("Resources\\mine-clipart-50x50.png");
            }
            if (field.winCondition == -1)
            {
                this.IsOpen = true;
                OpenAllFields(field);
                this.Image = null;
            }
        }

        private void OpenAllFields(Field field)
        {
            foreach (var v in field._mat/*_mat*/)
            {
                v.OpenField(field, "&End game");
                if (v.IsBomb) v.Image = null;
                v.Enabled = false;
            }
            field.winCondition = -1;

        }
        private void OpenEmptySurrounding(Minefiled parent, Field field)
        {
            int LowXbounds = parent.X - 1 < 0 ? 0 : parent.X - 1;
            int HighXbounds = parent.X + 1 >= field._tableC ? field._tableC - 1 : parent.X + 1;
            int LowYbounds = parent.Y - 1 < 0 ? 0 : parent.Y - 1;
            int HighYbounds = parent.Y + 1 >= field._tableR ? field._tableR - 1 : parent.Y + 1;
        

            for (int i = LowYbounds; i <= HighYbounds; i++)
            {
                for (int j = LowXbounds; j <= HighXbounds; j++)
                {
                    if (i == parent.Y && j == parent.X) continue;
                    if (field._mat[i, j].IsOpen) continue;

                    field._mat[i, j].OpenField(field, "");
                }
            }

        }
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
}
