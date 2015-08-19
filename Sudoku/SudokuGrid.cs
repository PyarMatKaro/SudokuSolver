using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Solver;

namespace Sudoku
{
    public class SudokuGrid : Problem
    {
        public enum CellFlags { Free, Fixed, Play, Solved };
        CellFlags[,] flags;
        //public const int FREE = 0;
        //public const int FIXED = 1;
        //public const int PLAY = 2;

        public enum PlayModes { Edit, Play, Pencil };
        public PlayModes PlayMode;

        bool requestedHint;
        List<UpdateListener> listeners = new List<UpdateListener>();
        int[,] values, boxes, cages;
        Point[] cage_indicators;
        int[] cage_totals;
        SudokuSolver solver;
        bool diagonal;
        HintOptions hintOptions = new HintOptions();
        Hint[] paintedHints = new Hint[0];

        public SudokuGrid(bool diagonal)
        {
            ClearGrid(diagonal);
        }

        public void RequestHint()
        {
            requestedHint = true;
            UpdatePaintedHints();
        }

        public Hint[] PaintedHints { get { return paintedHints; } }
        public HintOptions HintOptions { get { return hintOptions; } set { hintOptions = value; } }
        public HintFlags HintFlags { get { return hintOptions.Flags; } set { hintOptions.Flags = value; } }
        public HintSelections HintShow { get { return hintOptions.Show; } set { hintOptions.Show = value; } }
        public HintSelections HintAutoSolve { get { return hintOptions.AutoSolve; } set { hintOptions.AutoSolve = value; } }

        public bool HasDiagonals { get { return diagonal; } }

        public static void ReplaceListener(UpdateListener listener, SudokuGrid value, ref SudokuGrid grid)
        {
            if (grid == value)
                return;
            if (grid != null)
                grid.RemoveListener(listener);
            grid = value;
            if (grid != null)
                grid.AddListener(listener);
        }

        public void ClearGrid()
        {
            ClearGrid(diagonal);
        }

        public static int DefaultBoxAt(int x, int y)
        {
            return (x / 3) + (y / 3) * 3;
        }

        public int BoxAt(int x, int y)
        {
            return boxes[x, y];
        }

        public int CageAt(int x, int y)
        {
            return cages[x, y];
        }

        public CellFlags FlagAt(int x, int y)
        {
            return flags[x, y];
        }

        public int ValueAt(int x, int y)
        {
            return values[x, y];
        }

        public void ClearGrid(bool diagonal)
        {
            this.diagonal = diagonal;
            values = new int[9, 9];
            flags = new CellFlags[9, 9];
            boxes = new int[9, 9];
            for (int x = 0; x < 9; ++x)
                for (int y = 0; y < 9; ++y)
                {
                    values[x, y] = -1;
                    flags[x, y] = CellFlags.Free;
                    boxes[x, y] = DefaultBoxAt(x, y);
                }
            solver = new SudokuSolver(this);
        }

        public bool IsJigsaw
        {
            get
            {
                for (int x = 0; x < 9; ++x)
                    for (int y = 0; y < 9; ++y)
                        if (boxes[x, y] != DefaultBoxAt(x, y))
                            return true;
                return false;
            }
        }

        public void AddListener(UpdateListener listener)
        {
            listeners.Add(listener);
        }

        public void RemoveListener(UpdateListener listener)
        {
            listeners.Remove(listener);
        }

        public void NotifyListeners(Action<UpdateListener> f)
        {
            foreach (UpdateListener listener in listeners)
                f(listener);
        }

        void PaintCages(PaintContext context, Pen thick)
        {
            for (int x = 0; x < 8; ++x)
                for (int y = 0; y < 9; ++y)
                    if (CageAt(x, y) != CageAt(x + 1, y))
                        context.DrawVerticalLine(x, y, thick);
            for (int x = 0; x < 9; ++x)
                for (int y = 0; y < 8; ++y)
                    if (CageAt(x, y) != CageAt(x, y + 1))
                        context.DrawHorizontalLine(x, y, thick);
        }

        void PaintCageTotals(PaintContext context)
        {
            for (int i = 0; i < cage_totals.Length; ++i)
                context.DrawTotal(Brushes.Black, cage_indicators[i].X, cage_indicators[i].Y, cage_totals[i].ToString());
        }

        public void Paint(PaintContext context)
        {
            Graphics graphics = context.graphics;

            // Background
            Brush back = (solver.UnfulfilledRequirements.Length == 0)
                ? Brushes.LightGreen : Brushes.White;
            context.FillBackground(back);

            // Diagonals
            if (diagonal)
            {
                for (int x = 0; x < 9; ++x)
                {
                    context.FillCell(x, x, Brushes.LightGray);
                    if (x != 8 - x)
                        context.FillCell(x, 8 - x, Brushes.LightGray);
                }
            }

            // Cages
            if (cages != null)
            {
                using (Pen outer = new Pen(Color.LightBlue, 11.0f))
                    PaintCages(context, outer);
                using (Pen inner = new Pen(Color.White, 5.0f))
                    PaintCages(context, inner);
                PaintCageTotals(context);
            }

            foreach (Hint hint in paintedHints)
                hint.PaintBackground(context);

            context.DrawSelection();

            // Grid
            using (Pen thick = new Pen(Color.Black, 3.0f))
            {
                for (int x = 0; x < 8; ++x)
                    for (int y = 0; y < 9; ++y)
                    {
                        Pen p = BoxAt(x, y) == BoxAt(x + 1, y) ? Pens.Black : thick;
                        context.DrawVerticalLine(x, y, p);
                    }
                for (int x = 0; x < 9; ++x)
                    for (int y = 0; y < 8; ++y)
                    {
                        Pen p = BoxAt(x, y) == BoxAt(x, y + 1) ? Pens.Black : thick;
                        context.DrawHorizontalLine(x, y, p);
                    }
            }

            // Givens
            {
                for (int x = 0; x < 9; ++x)
                    for (int y = 0; y < 9; ++y)
                        if (flags[x, y] == CellFlags.Fixed)
                            context.DrawCell(Brushes.Black, x, y, values[x, y]);
                        else if (flags[x, y] == CellFlags.Play)
                            context.DrawCell(Brushes.Purple, x, y, values[x, y]);
                        else if (flags[x, y] == CellFlags.Solved)
                            context.DrawCell(Brushes.Green, x, y, values[x, y]);
            }

            // Knowns of solver
            {
                foreach (Candidate k in solver.SelectedCandidates)
                {
                    SudokuCandidate sc = (SudokuCandidate)k;
                    if (flags[sc.x, sc.y] == CellFlags.Free)
                        context.DrawCell(Brushes.Green, sc.x, sc.y, sc.n);
                }
            }

            // Unknowns as pencil marks
            if (HintFlags.PaintPencilMarks)
            {
                foreach (Candidate k in solver.UnselectedCandidates)
                {
                    SudokuCandidate sc = (SudokuCandidate)k;
                    context.DrawSubcell(Brushes.Black, sc.x, sc.y, sc.n);
                }
            }

            foreach (Hint hint in paintedHints)
                hint.PaintForeground(context);
        }

        void UpdatePaintedHints()
        {
            if (PlayMode == PlayModes.Edit)
            {
                Hint hint = solver.SingleImpossibleHint;
                if (hint != null)
                    paintedHints = new Hint[] { hint };
                else
                    paintedHints = new Hint[0];
            }
            else if (requestedHint || HintFlags.PaintHints)
            {
                //if (hintAutoSolve.ForcedHints)
                //    solver.FollowSingleOptions();
                solver.DoLogicalSolve(this, HintAutoSolve);
                paintedHints = solver.HintsToPaint(HintFlags, HintShow);
            }
            else
            {
                paintedHints = new Hint[0];
            }
            requestedHint = false;
        }

        public ForcedMoveHint SingleForcedHint { get { return solver.SingleForcedHint; } }

        public bool ClearCell(int x, int y)
        {
            if (flags[x, y] == CellFlags.Fixed && PlayMode != PlayModes.Edit)
                return false;
            if (flags[x, y] != CellFlags.Free)
            {
                flags[x, y] = CellFlags.Free;
                ResetSolver();
            }
            return true;
        }

        public bool SetCell(int x, int y, int n)
        {
            return SetCell(x, y, n, PlayMode);
        }

        public bool SetCell(int x, int y, int n, PlayModes playMode)
        {
            if (playMode == PlayModes.Pencil)
                return PencilCell(x, y, n);

            CellFlags fx = playMode == PlayModes.Edit ? CellFlags.Fixed : CellFlags.Play;
            if (flags[x, y] == CellFlags.Free)
                return SetFreeCell(x, y, n, fx);
            else
                return SetFilledCell(x, y, n, fx);
        }

        bool PencilCell(int x, int y, int n)
        {
            foreach (Candidate k in solver.UnselectedCandidates)
            {
                SudokuCandidate sc = (SudokuCandidate)k;
                if (x == sc.x && y == sc.y && n == sc.n)
                {
                    solver.DiscardCandidate(k);
                    Updated();
                    return true;
                }
            }
            return false;
        }

        bool SetFreeCell(int x, int y, int n, CellFlags fx)
        {
            // Agree with any existing known for that cell
            foreach (Candidate k in solver.SelectedCandidates)
            {
                SudokuCandidate sc = (SudokuCandidate)k;
                if (x == sc.x && y == sc.y)
                {
                    if (n == sc.n)
                    {
                        flags[x, y] = fx;
                        values[x, y] = n;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return AddClue(x, y, n, fx);
        }

        bool AddClue(int x, int y, int n, CellFlags fx)
        {
            SudokuCandidate sc = (SudokuCandidate)solver.GetCandidate(x, y, n);
            if (solver.TrySelectCandidate(sc))
            {
                // Add clue
                flags[x, y] = fx;
                values[x, y] = n;
                Updated();
                return true;
            }
            else
            {
                // Clue cannot be added
                return false;
            }
        }

        bool SetFilledCell(int x, int y, int n, CellFlags fx)
        {
            if (flags[x, y] == CellFlags.Fixed && fx != CellFlags.Fixed)
            {
                // Don't play into fixed cell
                return values[x, y] == n;
            }
            flags[x, y] = fx;
            if (values[x, y] != n)
            {
                // Change clue, reset solver
                values[x, y] = n;
                ResetSolver();
            }
            return true;
        }

        public override void SelectCandidate(Candidate k)
        {
            SudokuCandidate sc = (SudokuCandidate)k;
            values[sc.x, sc.y] = sc.n;
            flags[sc.x, sc.y] = CellFlags.Solved;
            solver.SelectCandidate(k);
        }

        public override void DiscardCandidate(Candidate k)
        {
            solver.DiscardCandidate(k);
        }

        internal void UndiscardCandidate(Candidate k)
        {
            solver.UndiscardCandidate(k);
        }

        public void ResetSolver()
        {
            for(int x=0;x<9;x++)
                for(int y=0;y<9;y++)
                    if (FlagAt(x, y) == CellFlags.Solved)
                    {
                        flags[x, y] = CellFlags.Free;
                        values[x, y] = -1;
                    }
            solver = new SudokuSolver(this);
            Updated();
        }

        public void Updated()
        {
            FireChanged();
            UpdatePaintedHints();
        }

        public void SolveLogically()
        {
            HintSelections.Level[] levels = new HintSelections.Level[]{
                HintSelections.Level.Easy,
                HintSelections.Level.Medium,
                HintSelections.Level.Hard,
                HintSelections.Level.Diabolical};
            foreach (HintSelections.Level level in levels)
            {
                HintSelections hs = new HintSelections(level);
                SolveResult solns = solver.DoLogicalSolve(this, hs);
                if (solns == SolveResult.SingleSolution)
                {
                    SolveLevel(level);
                    return;
                }
                else if (solns != SolveResult.TooDifficult)
                {
                    ShowSolveResult(solns);
                    return;
                }
            }
            ShowSolveResult(SolveResult.TooDifficult);
        }

        public void SolveBacktracking()
        {
            SolveResult solns = solver.DoBacktrackingSolve();
            ShowSolveResult(solns);
        }

        public void SolveLevel(HintSelections.Level level)
        {
            Updated(); // Made some progress
            MessageBox.Show("Solution found, " + level, "Solve", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowSolveResult(SolveResult solns)
        {
            Updated(); // May have made some progress
            switch (solns)
            {
                case SolveResult.MultipleSolutions:
                    MessageBox.Show("Multiple solutions", "Solve", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case SolveResult.NoSolutions:
                    MessageBox.Show("No solutions", "Solve", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case SolveResult.Ongoing:
                    break;
                case SolveResult.SingleSolution:
                    MessageBox.Show("Single solution found", "Solve", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case SolveResult.TooDifficult:
                    MessageBox.Show("Too difficult", "Solve", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        public void FireChanged()
        {
            NotifyListeners((UpdateListener listener) => listener.OnChangeGrid());
        }

        public string[] GridStringsCompact()
        {
            string[] s = new string[9];
            for (int y = 0; y < 9; ++y)
            {
                for (int x = 0; x < 9; ++x)
                    if (flags[x, y] == CellFlags.Fixed)
                        s[y] += (char)('1' + values[x, y]);
                    else
                        s[y] += ".";
            }
            return s;
        }

        public string[] GridStrings()
        {
            if (IsJigsaw)
                return GridStringsJigsaw;
            else
                return GridStringsNonJigsaw;
        }

        string[] GridStringsJigsaw
        {
            get
            {
                List<string> l = new List<string>();
                if (diagonal)
                    l.Add("DIAGONALS");
                for (int y = 0; y < 9; ++y)
                {
                    string s = "";
                    for (int x = 0; x < 9; ++x)
                    {
                        s += (char)('a' + boxes[x, y]);
                        if (flags[x, y] == CellFlags.Fixed)
                            s += (char)('1' + values[x, y]);
                        else
                            s += ".";
                    }
                    l.Add(s);
                }
                return l.ToArray();
            }
        }

        string[] GridStringsNonJigsaw
        {
            get
            {
                List<string> l = new List<string>();
                if (diagonal)
                    l.Add("DIAGONALS");
                for (int y = 0; y < 9; ++y)
                {
                    string s = " ";
                    for (int x = 0; x < 9; ++x)
                    {
                        if (flags[x, y] == CellFlags.Fixed)
                            s += (char)('1' + values[x, y]);
                        else
                            s += ".";
                        s += " ";
                        if (x == 2 || x == 5)
                            s += "| ";
                    }
                    l.Add(s);
                    if (y == 2 || y == 5)
                        l.Add("-------+-------+-------");
                }
                return l.ToArray();
            }
        }

        public string[] SolutionStrings
        {
            get
            {
                return solver.SolutionStrings();
            }
        }

        public void SetGridStrings(string[] a)
        {
            cages = new int[9, 9];
            if (SetGridStrings9x9Killer(a))
                return;
            cages = null;
            cage_totals = null;
            cage_indicators = null;
            if (SetGridStrings9x9Jigsaw(a))
                return;
            if (SetGridStrings9x9(a))
                return;
            SetGridStrings81(a);
        }

        public bool SetGridStrings81(string[] a)
        {
            for (int i = 0; i < a.Length; ++i)
            {
                string s = a[i];
                if (s.Length == 81)
                {
                    ClearGrid(false);
                    for (int y = 0; y < 9; ++y)
                        for (int x = 0; x < 9; ++x)
                        {
                            char c = s[y * 9 + x];
                            if (c >= '1' && c <= '9')
                            {
                                values[x, y] = c - '1';
                                flags[x, y] = CellFlags.Fixed;
                            }
                        }
                    ResetSolver();
                    return true;
                }
            }
            return false;
        }

        public bool SetGridStrings9x9(string[] a)
        {
            bool diags = false;
            List<string> l = new List<string>();
            for (int y = 0; y < a.Length; ++y)
            {
                if (a[y].ToUpper().StartsWith("D"))
                    diags = true;
                else
                {
                    string s = "";
                    for (int x = 0; x < a[y].Length; ++x)
                    {
                        char c = a[y][x];
                        if (c == '.' || c >= '1' && c <= '9')
                            s += c;
                    }
                    if (s.Length == 9)
                        l.Add(s);
                }
            }
            a = l.ToArray();
            if (a.Length != 9)
                return false;
            ClearGrid(diags);
            for (int y = 0; y < 9; ++y)
                for (int x = 0; x < 9; ++x)
                {
                    char c = a[y][x];
                    if (c >= '1' && c <= '9')
                    {
                        values[x, y] = c - '1';
                        flags[x, y] = CellFlags.Fixed;
                    }
                }
            ResetSolver();
            return true;
        }

        public bool SetGridStrings9x9Killer(string[] a)
        {
            bool diags = false;
            List<string> l = new List<string>();
            int y = 0, i=0;
            if (a[0].ToUpper().StartsWith("DIAG"))
            {
                diags = true;
                i++;
            }
            Dictionary<char, int> names = new Dictionary<char, int>();
            for (int j = 0; j < 9; ++j)
            {
                if (i >= a.Length)
                    return false;
                string s = a[i].Replace(" ", "");
                if (s.Length != 9)
                    return false;
                for (int x = 0; x < 9; ++x)
                {
                    char c = s[x];
                    int this_cage;
                    if (!names.TryGetValue(c, out this_cage))
                    {
                        names[c] = this_cage = names.Count;
                    }
                    values[x, y] = -1;
                    flags[x, y] = CellFlags.Free;
                    cages[x, y] = this_cage;
                    boxes[x, y] = DefaultBoxAt(x, y);
                }
                y++;
                i++;
            }
            cage_totals = new int[names.Count];
            while (true)
            {
                if (i >= a.Length)
                    return false;
                string[] b = a[i].Split(' ');
                foreach (string e in b)
                    if (!e.Contains("="))
                        return false;
                foreach (string e in b)
                {
                    string[] es = e.Split('=');
                    if (es.Length != 2 || es[0].Length != 1)
                        return false;
                    int tot;
                    if(!int.TryParse(es[1], out tot))
                        return false;
                    char c = es[0][0];
                    int this_cage;
                    if (!names.TryGetValue(c, out this_cage))
                        return false;
                    cage_totals[this_cage] = tot;
                    if (this_cage == names.Count - 1)
                    {
                        cage_indicators = new Point[names.Count];
                        for (int y1 = 8; y1 >= 0; --y1)
                            for (int x = 8; x >= 0; --x)
                                cage_indicators[cages[x, y1]] = new Point(x, y1);
                        ResetSolver();
                        return true;
                    }
                }
                i++;
            }
        }

        public bool SetGridStrings9x9Jigsaw(string[] a)
        {
            bool diags = false;
            List<string> l = new List<string>();
            for (int y = 0; y < a.Length; ++y)
            {
                if (a[y].ToUpper().StartsWith("DIAG"))
                    diags = true;
                else
                {
                    string s = "";
                    for (int x = 0; x < a[y].Length - 1; ++x)
                    {
                        char b = a[y][x];
                        if (b >= 'a' && b <= 'i')
                        {
                            ++x;
                            char c = a[y][x];
                            if (c == '0')
                                c = '.';
                            if (c == '.' || c >= '1' && c <= '9')
                            {
                                s += b;
                                s += c;
                            }
                        }
                    }
                    if (s.Length == 18)
                        l.Add(s);
                }
            }
            a = l.ToArray();
            if (a.Length != 9)
                return false;
            diagonal = diags;
            for (int y = 0; y < 9; ++y)
                for (int x = 0; x < 9; ++x)
                {
                    char b = a[y][x * 2];
                    char c = a[y][x * 2 + 1];
                    if (c >= '1' && c <= '9')
                    {
                        values[x, y] = c - '1';
                        flags[x, y] = CellFlags.Fixed;
                    }
                    else
                    {
                        values[x, y] = -1;
                        flags[x, y] = CellFlags.Free;
                    }
                    boxes[x, y] = b - 'a';
                }
            ResetSolver();
            return true;
        }

        public void ClearEntries()
        {
            for(int x=0;x<9;++x)
                for (int y = 0; y < 9; ++y)
                {
                    CellFlags cf = FlagAt(x, y);
                    if (cf == CellFlags.Play || cf == CellFlags.Solved)
                    {
                        flags[x, y] = CellFlags.Free;
                        values[x, y] = -1;
                    }
                }
            ResetSolver();
        }

        public void Generate(HintSelections hints)
        {
            // Take any solution for a clear grid
            ClearGrid();
            Candidate[] soln = solver.RandomSolution();
            if (soln == null)
                return;
            int[,] ans = new int[9, 9];
            foreach (Candidate k in soln)
            {
                SudokuCandidate sc = (SudokuCandidate)k;
                ans[sc.x, sc.y] = sc.n;
            }
            // Add clues until soluable
            PlayMode = PlayModes.Edit;
            while (true)
            {
                SolveResult sr = solver.DoLogicalSolve(this, hints);
                if (sr == SolveResult.SingleSolution)
                    break;
                int x = Utils.Utils.Rnd(9);
                int y = Utils.Utils.Rnd(9);
                if (sr == SolveResult.NoSolutions)
                {
                    ClearCell(x, y);
                    ClearCell(8 - x, 8 - y);
                }
                else
                {
                    SetCell(x, y, ans[x, y]);
                    SetCell(8 - x, 8 - y, ans[8 - x, 8 - y]);
                }
            }
            // Remove any clues where it remains soluable
            for (int x = 0; x < 9; ++x)
                for (int y = 0; y < 9; ++y)
                {
                    ClearCell(x, y);
                    if (solver.DoLogicalSolve(this, hints) != SolveResult.SingleSolution)
                        SetCell(x, y, ans[x, y]);
                }
            ResetSolver();
        }

    }
}
