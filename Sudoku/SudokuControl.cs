using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Solver;

namespace Sudoku
{
    public partial class SudokuControl : UserControl, UpdateListener
    {

        SudokuGrid grid;
        int selx = -1, sely = -1;

        public SudokuControl()
        {
            DoubleBuffered = true;
            InitializeComponent();
        }

        public SudokuGrid Grid
        {
            get { return grid; }
            set { SudokuGrid.ReplaceListener(this, value, ref grid); }
        }

        private void SudokuControl_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                SudokuGrid grid = Grid;

                // Keep form designer happy
                if (grid == null)
                    grid = new SudokuGrid(false);

                using (PaintContext context = new PaintContext(e.Graphics, Size, grid, selx, sely))
                {
                    grid.Paint(context);
                }
            }
            catch (Exception ex)
            {
                e.Graphics.DrawString("! " + ex.ToString(), Font, Brushes.Red, 0, 0);
            }
        }

        private void SudokuControl_MouseClick(object sender, MouseEventArgs e)
        {
            PickContext context = new PickContext(ClientSize);
            Size cs = context.CellSize;
            int x = e.X / cs.Width;
            int y = e.Y / cs.Height;
            if (x < 0 || x >= 9 || y < 0 || y >= 9)
                return;
            selx = x;
            sely = y;
            Invalidate();
        }

        void AdvanceSelection()
        {
            if (selx == -1 || sely == -1)
                return;
            Invalidate();
            selx++;
            if (selx != 9)
                return;
            selx = 0;
            sely++;
            if (sely != 9)
                return;
            sely = 0;
        }

        private void SudokuControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
                AdvanceSelection();

            if (e.KeyChar == '0' && selx != -1 && sely != -1)
            {
                if (Grid.ClearCell(selx, sely))
                    AdvanceSelection();
                else
                    Utils.Utils.ErrorSound();
            }

            if (e.KeyChar == 'h' || e.KeyChar == 'H')
            {
                Grid.RequestHint();
                Invalidate();
            }

            if (e.KeyChar >= '1' && e.KeyChar <= '9' && selx != -1 && sely != -1)
            {
                int n = (e.KeyChar - '1');
                int x = selx, y = sely;
                if (Grid.SetCell(x, y, n))
                {
                    ForcedMoveHint hint = null;
                    if (Grid.HintFlags.SelectForcedMoveHints &&
                        Grid.PaintedHints.Length > 0 &&
                        Grid.PaintedHints[0] is ForcedMoveHint)
                        hint = Grid.PaintedHints[0] as ForcedMoveHint;
                    if (hint != null)
                    {
                        SudokuCandidate sc = (SudokuCandidate) hint.Candidate;
                        selx = sc.x;
                        sely = sc.y;
                    }
                    else if (grid.PlayMode != SudokuGrid.PlayModes.Pencil)
                        AdvanceSelection();
                }
                else
                {
                    Utils.Utils.ErrorSound();
                    Invalidate();
                }
            }
        }

        public void OnChangeGrid()
        {
            Invalidate();
        }

        private void SudokuControl_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        string toolTipText;

        static bool AppliesAt(Hint hint, int x, int y, int b)
        {
            if (!(hint is ForcedMoveHint))
            {
                SudokuCandidate k = (SudokuCandidate)hint.Candidate;
                if (k != null)
                    return k.x == x && k.y == y;
            }

            SudokuRequirement c = (SudokuRequirement) hint.Requirement;
            if(c != null)
                return c.AppliesAt(x,y,b);

            return false;
        }

        void UpdateTooltip(int x, int y, int b)
        {
            StringBuilder hints = new StringBuilder();
            foreach (Hint hint in Grid.PaintedHints)
                if (AppliesAt(hint, x, y, b))
                    hints.AppendLine(hint.ToString());
            if (hints.Length == 0)
            {
                if (toolTipText != null)
                {
                    toolTipText = null;
                    toolTip1.Active = false;
                }
            }
            else
            {
                if (toolTipText == null ||
                    !toolTipText.Equals(hints.ToString()))
                {
                    toolTipText = hints.ToString();
                    toolTip1.Active = true;
                    toolTip1.SetToolTip(this, toolTipText);
                }
            }
        }

        private void SudokuControl_MouseMove(object sender, MouseEventArgs e)
        {
            PickContext context = new PickContext(ClientSize);
            Size cs = context.CellSize;
            int x = e.X / cs.Width;
            int y = e.Y / cs.Height;
            if (x < 0 || x >= 9 || y < 0 || y >= 9)
            {
                toolTipText = null;
                toolTip1.Active = false;
                return;
            }
            int b = Grid.BoxAt(x, y);
            UpdateTooltip(x, y, b);
        }

    }
}
