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

        public bool HasSelection { get { return (selx != -1 && sely != -1); } }

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
            if (x == selx && y == sely)
                selx = sely = -1;
            else
            {
                selx = x;
                sely = y;
            }
            Invalidate();
        }

        void AdvanceSelection()
        {
            if (!HasSelection)
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

        void MoveSelection(int dx, int dy)
        {
            if (!HasSelection)
                return;
            int nx = selx + dx, ny = sely + dy;
            if (nx < 0 || nx >= 9 || ny < 0 || ny >= 9)
                return;
            selx += dx;
            sely += dy;
            Invalidate();
        }

        void HandleNumber(int n)
        {
            int x = selx, y = sely;
            if (HasSelection && Grid.SetCell(x, y, n))
            {
                ForcedMoveHint hint = null;
                if (Grid.HintFlags.SelectForcedMoveHints &&
                    Grid.PaintedHints.Length > 0 &&
                    Grid.PaintedHints[0] is ForcedMoveHint)
                    hint = Grid.PaintedHints[0] as ForcedMoveHint;
                if (hint != null)
                {
                    SudokuCandidate sc = (SudokuCandidate)hint.Candidate;
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

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (HasSelection)
                switch (e.KeyCode)
                {
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                        e.IsInputKey = true;
                        break;
                }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyData)
            {
                case Keys.Space:
                    AdvanceSelection();
                    break;

                case Keys.Left:
                    MoveSelection(-1, 0);
                    break;

                case Keys.Right:
                    MoveSelection(1, 0);
                    break;

                case Keys.Up:
                    MoveSelection(0, -1);
                    break;

                case Keys.Down:
                    MoveSelection(0, 1);
                    break;

                case Keys.D0:
                    if (HasSelection && Grid.ClearCell(selx, sely))
                        AdvanceSelection();
                    else
                        Utils.Utils.ErrorSound();
                    break;

                case Keys.H:
                    Grid.RequestHint();
                    Invalidate();
                    break;

                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    HandleNumber(e.KeyData - Keys.D1);
                    break;

                default:
                    return;
            }

            e.Handled = true;
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
