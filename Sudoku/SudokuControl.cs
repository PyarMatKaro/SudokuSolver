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
        public int Cells { get { return grid.Cells; } }
        public int CellA { get { return grid.CellA; } }
        public int CellB { get { return grid.CellB; } }

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
                    grid = new SudokuGrid(new SudokuGrid.GridOptions(3, 3, '1'));

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
            PickContext context = new PickContext(ClientSize, CellA, CellB);
            int x, y;
            context.PositionOf(e.X, e.Y, out x, out y);
            if (x < 0 || x >= Cells || y < 0 || y >= Cells)
                return;
            if (grid.PlayMode == SudokuGrid.PlayModes.EditBox)
            {
                if (grid.IsJigsaw)
                    grid.ChangeBox(x, y);
            }
            else if (x == selx && y == sely)
                ClearSelection();
            else
                MakeSelection(x, y);
            Invalidate();
        }

        void AdvanceSelection()
        {
            if (HasSelection)
            {
                if (selx + 1 == Cells)
                    if (sely + 1 == Cells)
                        MakeSelection(0, 0);
                    else
                        MakeSelection(0, sely + 1);
                else
                    MakeSelection(selx + 1, sely);
                Invalidate();
            }
        }

        void ClearSelection()
        {
            selx = sely = -1;
            Grid.UpdatePaintedHints(false, null);
        }

        void MakeSelection(int x, int y)
        {
            selx = x;
            sely = y;
            Grid.UpdatePaintedHints(false, null);
        }

        void MoveSelection(int dx, int dy)
        {
            if (!HasSelection)
                return;
            int nx = selx + dx, ny = sely + dy;
            if (nx < 0 || nx >= Cells || ny < 0 || ny >= Cells)
                return;
            MakeSelection(selx + dx, sely + dy);
            Invalidate();
        }

        bool SelectForcedHint()
        {
            ForcedMoveHint hint = null;
            if (Grid.HintFlags.SelectForcedMoveHints &&
                Grid.PaintedHints.Length > 0 &&
                Grid.PaintedHints[0] is ForcedMoveHint)
                hint = Grid.PaintedHints[0] as ForcedMoveHint;
            if (hint != null)
            {
                SudokuCandidate sc = (SudokuCandidate)hint.Candidate;
                // MakeSelection(sc.x, sc.y); -- loses the hint if it was requested with "show hints" off
                selx = sc.x;
                sely = sc.y;
                return true;
            }
            return false;
        }

        void HandleNumber(int n)
        {
            int x = selx, y = sely;
            if (HasSelection && Grid.SetCell(x, y, n))
            {
                if (!SelectForcedHint() && grid.PlayMode != SudokuGrid.PlayModes.Pencil)
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
            char kc = char.ToUpperInvariant((char)e.KeyValue);
            if (kc >= grid.Start && kc <= grid.Start + grid.Cells)
            {
                HandleNumber(kc - grid.Start);
                e.Handled = true;
                return;
            }

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

                case Keys.F1:
                    if (HasSelection)
                        Grid.RequestHintAt(selx, sely);
                    else
                        Grid.RequestHint();
                    SelectForcedHint();
                    Invalidate();
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

        static bool AppliesAt(Hint hint, int x, int y, SudokuGrid grid)
        {
            if (!(hint is ForcedMoveHint))
            {
                SudokuCandidate k = (SudokuCandidate)hint.Candidate;
                if (k != null)
                    return k.x == x && k.y == y;
            }

            SudokuRequirement c = (SudokuRequirement)hint.Requirement;
            if (c != null)
                return c.AppliesAt(x, y, grid);

            return false;
        }

        void UpdateTooltip(int x, int y, SudokuGrid grid)
        {
            StringBuilder hints = new StringBuilder();
            foreach (Hint hint in Grid.PaintedHints)
                if (AppliesAt(hint, x, y, grid))
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
            PickContext context = new PickContext(ClientSize, CellA, CellB);
            int x, y;
            context.PositionOf(e.X, e.Y, out x, out y);
            if (x < 0 || x >= Cells || y < 0 || y >= Cells)
            {
                toolTipText = null;
                toolTip1.Active = false;
                return;
            }
            UpdateTooltip(x, y, Grid);
        }
    }
}
