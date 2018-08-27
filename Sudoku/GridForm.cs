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
    public partial class GridForm : Form
    {
        public SudokuForm form;
        int boxWidth, boxHeight;

        public GridForm()
        {
            InitializeComponent();
            UpdateControls();
        }
        
        bool ParseWidthHeight()
        {
            boxWidth = boxHeight = 0;
            return int.TryParse(tbWidth.Text, out boxWidth) &&
                int.TryParse(tbHeight.Text, out boxHeight) &&
                boxWidth * boxHeight <= 26;
        }

        private void WidthHeight_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        public void InitControls(SudokuForm form, SudokuGrid.GridOptions options)
        {
            this.form = form;
            tbWidth.Text = options.cellA.ToString();
            tbHeight.Text = options.cellB.ToString();
            rbJigsaw.Checked = options.isJigsaw;
            rbKiller.Checked = options.isKiller;
            rbNormal.Checked = !options.isJigsaw && !options.isKiller;
            cbMajorDiagonal.Checked = options.majorDiagonal;
            cbMinorDiagonal.Checked = options.minorDiagonal;
        }

        void UpdateControls()
        {
            if (ParseWidthHeight())
            {
                btnOK.Enabled = true;
                lblCells.Text = "This gives " + (boxWidth * boxHeight) + " cells to a row, column or box.";
            }
            else
            {
                btnOK.Enabled = false;
            }
        }

        SudokuGrid.GridOptions CreateOptions()
        {
            if (!ParseWidthHeight())
                return null;
            char start = '1';
            if (boxWidth * boxHeight > 9)
                start = 'A';
            var ret = new SudokuGrid.GridOptions(boxWidth, boxHeight, start);
            ret.isJigsaw = rbJigsaw.Checked;
            ret.isKiller = rbKiller.Checked;
            ret.majorDiagonal = cbMajorDiagonal.Checked;
            ret.minorDiagonal = cbMinorDiagonal.Checked;
            return ret;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var options = CreateOptions();
            form.CreateGrid(options);
        }
    }
}
