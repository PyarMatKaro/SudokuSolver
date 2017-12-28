using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Solver;
using System.Drawing.Drawing2D;
using System.IO;

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

        List<UpdateListener> listeners = new List<UpdateListener>();
        int[,] values, boxes, colours;
        SudokuSolver solver;
        int diagonals;
        HintOptions hintOptions = new HintOptions();
        Hint[] paintedHints = new Hint[0];

        public class CageInfo
        {
            public int[,] cages;
            public int[] totals, sizes;
            public int[] remaining_totals, remaining_sizes;

            public CageInfo()
            {
                cages = new int[9, 9];
            }

            public int NumCages { get { return totals.Length; } }

            public void Reset()
            {
                sizes = new int[NumCages];
                for (int x = 0; x < 9; ++x)
                    for (int y = 0; y < 9; ++y)
                        ++sizes[cages[x, y]];
                remaining_totals = new int[NumCages];
                remaining_sizes = new int[NumCages];
                for (int i = 0; i < NumCages; ++i)
                {
                    remaining_totals[i] = totals[i];
                    remaining_sizes[i] = sizes[i];
                }
            }
        }

        public CageInfo cageInfo;

        public SudokuGrid(int diagonals)
        {
            ClearGrid(diagonals);
        }

        public void RequestHint()
        {
            UpdatePaintedHints(true);
        }

        public Hint[] PaintedHints { get { return paintedHints; } }
        public HintOptions HintOptions { get { return hintOptions; } set { hintOptions = value; } }
        public HintFlags HintFlags { get { return hintOptions.Flags; } set { hintOptions.Flags = value; } }
        public HintSelections HintShow { get { return hintOptions.Show; } set { hintOptions.Show = value; } }
        public HintSelections HintAutoSolve { get { return hintOptions.AutoSolve; } set { hintOptions.AutoSolve = value; } }

        public int NumDiagonals { get { return diagonals; } }
        public int NumCages { get { return cageInfo == null ? 0 : cageInfo.NumCages; } }

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
            ClearGrid(diagonals);
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
            if (cageInfo == null)
                return -1;
            return cageInfo.cages[x, y];
        }

        public CellFlags FlagAt(int x, int y)
        {
            return flags[x, y];
        }

        public int ValueAt(int x, int y)
        {
            return values[x, y];
        }

        public void ClearGrid(int diagonals)
        {
            this.cageInfo = null;
            this.colours = null;
            this.diagonals = diagonals;
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

        public bool IsKiller
        {
            get
            {
                return cageInfo != null;
            }
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

        public void Paint(PaintContext context)
        {
            Graphics graphics = context.graphics;

            // Background
            Brush back = (solver.UnfulfilledRequirements.Length == 0)
                ? Brushes.LightGreen : Brushes.White;
            context.FillBackground(back);

            // Coloured cages or boxes
            if (colours != null)
            {
                Color[] cage_colours = new Color[]{
                    Color.FromArgb(255,253,152),
                    Color.FromArgb(207,231,153),
                    Color.FromArgb(203,232,250),
                    Color.FromArgb(248,207,223)};
                Brush[] cage_brushes = new Brush[4];
                for (int i = 0; i < 4; ++i)
                    cage_brushes[i] = new SolidBrush(cage_colours[i]);
                for (int x = 0; x < 9; ++x)
                    for (int y = 0; y < 9; ++y)
                        context.FillCell(x, y, cage_brushes[colours[x, y]]);
                for (int i = 0; i < 4; ++i)
                    cage_brushes[i].Dispose();
            }

            // Diagonals - brush  previously Brushes.LightGray
            if (NumDiagonals > 0)
                using (Brush br = new HatchBrush(HatchStyle.ForwardDiagonal, Color.DarkGray, Color.Transparent))
                    for (int x = 0; x < 9; ++x)
                        context.FillCell(x, x, br);
            if (NumDiagonals > 1)
                using (Brush br = new HatchBrush(HatchStyle.BackwardDiagonal, Color.DarkGray, Color.Transparent))
                    for (int x = 0; x < 9; ++x)
                        context.FillCell(x, 8 - x, br);

            // Cage totals
            if (IsKiller)
            {
                Point?[] cage_indicators = new Point?[NumCages];
                for (int y = 8; y >= 0; --y)
                    for (int x = 8; x >= 0; --x)
                        if(flags[x, y] == CellFlags.Free)
                            cage_indicators[cageInfo.cages[x, y]] = new Point(x, y);

                using (Pen inner = new Pen(Color.White, 5.0f))
                    PaintCages(context, inner);

                for (int i = 0; i < NumCages; ++i)
                    if(cage_indicators[i]!=null)
                        context.DrawTotal(Brushes.Black,
                            cage_indicators[i].Value.X, cage_indicators[i].Value.Y, cageInfo.remaining_totals[i].ToString());
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
                foreach (SudokuCandidate k in solver.UnselectedCandidates)
                    context.SetPencil(k, Hint.Kind.Maybe);

            foreach (Hint hint in paintedHints)
                hint.PaintForeground(context);

            context.PaintPencilMarksAndHints();
        }

        void UpdatePaintedHints(bool requestedHint)
        {
            if (PlayMode == PlayModes.Edit)
            {
                Hint hint = solver.SingleImpossibleHint;
                if (hint != null)
                    paintedHints = new Hint[] { hint };
                else
                    paintedHints = new Hint[0];
            }
            else if ((requestedHint && paintedHints.Length == 0)
                || HintFlags.PaintHints)
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
            if (cageInfo != null)
                cageInfo.Reset();
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
            UpdatePaintedHints(false);
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
            SolveResult solns = solver.DoBacktrackingSolve(this);
            ShowSolveResult(solns);
        }

        void SolveBacktracking(string log)
        {
            FileStream fs = new FileStream(log, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            solver.log = sw;
            SolveResult solns = solver.DoBacktrackingSolve(this);
            ShowSolveResult(solns);
            sw.Close();
            solver.log = null;
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
            if (IsKiller)
                return GridStringsKiller;
            if (IsJigsaw)
                return GridStringsJigsaw;
            else
                return GridStringsNonJigsaw;
        }

        string[] GridStringsKiller
        {
            get
            {
                Dictionary<int, char> d = new Dictionary<int, char>();
                char lc = 'a';
                List<string> ret = new List<string>();
                if (diagonals == 1)
                    ret.Add("DIAGONAL");
                else if (diagonals == 2)
                    ret.Add("DIAGONALS");
                for (int y = 0; y < 9; ++y)
                {
                    string lcl = "";
                    string gs = "";
                    for (int x = 0; x < 9; ++x)
                    {
                        int ca = cageInfo.cages[x, y];
                        char c;
                        if (cageInfo.sizes[ca] == 1)
                        {
                            c = (char)('0' + cageInfo.totals[ca]);
                        }
                        else if (!d.TryGetValue(ca, out c))
                        {
                            c = lc;
                            if (lc == 'z')
                                lc = 'A';
                            else if (lc == 'Z')
                                lc = '0';
                            else
                                ++lc;
                            d[ca] = c;
                            string cs = c.ToString() + "=" + cageInfo.totals[ca];
                            lcl += " " + cs;
                        }
                        gs += c;
                    }
                    ret.Add(gs + lcl);
                }
                return ret.ToArray();
            }
        }

        string[] GridStringsJigsaw
        {
            get
            {
                List<string> l = new List<string>();
                if (diagonals == 1)
                    l.Add("DIAGONAL");
                else if (diagonals == 2)
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
                if (diagonals == 1)
                    l.Add("DIAGONAL");
                else if (diagonals == 2)
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
            if (SetGridStrings9x9Killer(a))
                return;
            cageInfo = null;
            if (SetGridStrings9x9Jigsaw(a))
                return;
            if (SetGridStrings81Jigsaw(a))
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
                    ClearGrid(0);
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
            int diags = 0;
            List<string> l = new List<string>();
            for (int y = 0; y < a.Length; ++y)
            {
                if (a[y].ToUpper() == "DIAGONAL")
                    diags = 1;
                else if (a[y].ToUpper() == "DIAGONALS")
                    diags = 2;
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

        public bool SetGridStrings9x9Killer(string[] a0)
        {
            List<string> l = new List<string>();
            int i=0;
            int diags = 0;
            if (a0[i].ToUpper() == "DIAGONAL")
            {
                diags = 1;
                i++;
            }
            else if (a0[i].ToUpper() == "DIAGONALS")
            {
                diags = 2;
                i++;
            }
            ClearGrid(diags);
            List<string> aLine = new List<string>();
            List<string> aSum = new List<string>();
            for (; i < a0.Length; ++i)
            {
                string[] a = a0[i].Split(' ');
                foreach (string s in a)
                    if (s.Length == 9)
                        aLine.Add(s);
                    else if (s.Length > 2 && s[1] == '=')
                        aSum.Add(s);
                    else if (s.Length > 0)
                        return false;
            }
            if (aLine.Count != 9)
                return false;
            cageInfo = new CageInfo();
            Dictionary<char, int> names = new Dictionary<char, int>();
            Dictionary<int, int> totals = new Dictionary<int, int>();
            int num_cages = 0;
            for (int y = 0; y < 9; ++y)
            {
                string s = aLine[y];
                for (int x = 0; x < 9; ++x)
                {
                    int this_cage;
                    char c = s[x];
                    if (c >= '1' && c <= '9')
                    {
                        this_cage = num_cages++;
                        totals[this_cage] = c - '0';
                    }
                    else
                    {
                        if (!names.TryGetValue(c, out this_cage))
                            names[c] = this_cage = num_cages++;
                    }
                    values[x, y] = -1;
                    flags[x, y] = CellFlags.Free;
                    cageInfo.cages[x, y] = this_cage;
                    boxes[x, y] = DefaultBoxAt(x, y);
                }
            }
            cageInfo.totals = new int[num_cages];
            foreach(string s in aSum)
            {
                int tot;
                if(!int.TryParse(s.Substring(2), out tot))
                    return false;
                char c = s[0];
                int this_cage;
                if (!names.TryGetValue(c, out this_cage))
                    return false;
                totals[this_cage] = tot;
            }

            if (totals.Count != num_cages)
                return false;
            cageInfo.totals = new int[num_cages];
            int grand = 0;
            for (int j = 0; j < NumCages; ++j)
            {
                cageInfo.totals[j] = totals[j];
                grand += cageInfo.totals[j];
            }
            if (grand != 81 * 5)
                return false;

            ColourSolver cs = new ColourSolver();
            colours = cs.Solve(this, cageInfo.cages, NumCages);

            cageInfo.Reset();
            /*
            for (int y = 0; y < 9; ++y)
                for (int x = 0; x < 9; ++x)
                {
                    int cage = cageInfo.cages[x, y];
                    if (cageInfo.sizes[cage] == 1)
                    {
                        values[x, y] = cageInfo.totals[cage] - 1;
                        flags[x, y] = CellFlags.Fixed;
                    }
                }
             */

            ResetSolver();
            return true;
        }

        public bool SetGridStrings9x9Jigsaw(string[] a)
        {
            int diags = 0;
            List<string> l = new List<string>();
            for (int y = 0; y < a.Length; ++y)
            {
                if (a[y].ToUpper() == "DIAGONAL")
                    diags = 1;
                else if (a[y].ToUpper() == "DIAGONALS")
                    diags = 2;
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
            diagonals = diags;
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

            ColourSolver cs = new ColourSolver();
            colours = cs.Solve(this, boxes, 9);
            ResetSolver();
            return true;
        }

        public bool SetGridStrings81Jigsaw(string[] a)
        {
            // format used at www.icosahedral.net
            string[] a2 = (from string line in a where line.Length == 81 select line).ToArray();
            if (a2.Length != 2)
                return false;
            for (int i = 0; i < 81; ++i)
            {
                int x = i % 9;
                int y = i / 9;
                char c = a2[0][i];
                char b = a2[1][i];
                if (c >= '0' && c <= '8')
                {
                    values[x, y] = c - '0';
                    flags[x, y] = CellFlags.Fixed;
                }
                else
                {
                    values[x, y] = -1;
                    flags[x, y] = CellFlags.Free;
                }
                if (b == '.')
                    boxes[x, y] = 8;
                else
                    boxes[x, y] = b - '0';
            }

            ColourSolver cs = new ColourSolver();
            colours = cs.Solve(this, boxes, 9);
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
