using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class TextForm : Form
    {
        public TextForm()
        {
            InitializeComponent();
        }

        private void TextForm_Resize(object sender, EventArgs e)
        {
            UpdateLayout();
        }

        private void TextForm_Load(object sender, EventArgs e)
        {
            UpdateLayout();
        }

        void UpdateLayout()
        {
            var r = ClientRectangle;
            int bh = btnCancel.Height;
            int sp = 5;
            tbText.Location = new Point(sp, sp);
            tbText.Size = new Size(r.Width - 2 * sp, r.Height - 3 * sp - bh);
            if (btnOK.Visible)
            {
                btnOK.Location = new Point(r.Width / 2 - sp / 2 - btnOK.Width, r.Height - sp - bh);
                btnCancel.Location = new Point(btnOK.Right + sp, r.Height - sp - bh);
            }
            else
                btnOK.Location = new Point((r.Width - btnOK.Width) / 2);
        }
    }
}
