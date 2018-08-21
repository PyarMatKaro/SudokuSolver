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
        public int CellA { get { return gridOptions.cellA; } }
        public int CellB { get { return gridOptions.cellB; } }
        public int Cells { get { return gridOptions.Cells; } }
        public char Start { get { return gridOptions.start; } }

        public enum PlayModes { EditCell, EditBox, Play, Pencil };
        public PlayModes PlayMode;

        List<UpdateListener> listeners = new List<UpdateListener>();
        int[,] values;
        GridOptions gridOptions;
        SudokuSolver solver;
        HintOptions hintOptions = new HintOptions();
        Hint[] paintedHints = new Hint[0];

        public class GridOptions
        {
            public bool majorDiagonal, minorDiagonal, isKiller, isJigsaw;
            public int[,] boxes, colours;
            public int cellA, cellB;
            public char start;

            public int Cells { get { return cellA * cellB; } }

            public bool FromChar(char c, out int i)
            {
                i = -1;
                if (c != '.' && c >= start && c < start + Cells)
                    i = c - start;
                else
                    return false;
                return true;
            }

            public char ToChar(int i)
            {
                if (i == -1)
                    return '.';
                return (char)(start + i);
            }

            public GridOptions(int cellA, int cellB, char start)
            {
                this.cellA = cellA;
                this.cellB = cellB;
                this.start = start;
                DefaultBoxes();
            }

            GridOptions DefaultBoxes()
            {
                boxes = new int[Cells, Cells];
                for (int x = 0; x < Cells; ++x)
                    for (int y = 0; y < Cells; ++y)
                        boxes[x, y] = DefaultBoxAt(x, y, cellA, cellB);
                return this;
            }

            public GridOptions DefaultColours()
            {
                colours = new int[Cells, Cells];
                for (int x = 0; x < Cells; ++x)
                    for (int y = 0; y < Cells; ++y)
                        colours[x, y] = (x / 3 + y / 3) % 4;
                return this;
            }

            public void AddHeaderLines(List<string> lines)
            {
                if (cellA != 3 || cellB != 3 || start != '0')
                    lines.Add(cellA + "," + cellB + "," + start);
                if (majorDiagonal)
                    if (minorDiagonal)
                        lines.Add("DIAGONAL X");
                    else
                        lines.Add("DIAGONAL \\");
                else if (minorDiagonal)
                    lines.Add("DIAGONAL /");
            }

            public bool CheckDiagonalsLine(string check)
            {
                if (check == "DIAGONALS" || check == "DIAGONAL X")
                    majorDiagonal = minorDiagonal = true;
                else if (check == "DIAGONAL" || check == "DIAGONAL \\")
                    majorDiagonal = true;
                else if (check == "DIAGONAL /")
                    minorDiagonal = true;
                else
                    return false;
                return true;
            }
        }

        public class CageInfo
        {
            public int[,] cages;
            public int[] totals, sizes;
            public int[] remaining_totals, remaining_sizes;
            public int Cells;

            public CageInfo(int cells)
            {
                this.Cells = cells;
                cages = new int[Cells, Cells];
            }

            public int NumCages { get { return totals.Length; } }

            public void Reset()
            {
                sizes = new int[NumCages];
                for (int x = 0; x < Cells; ++x)
                    for (int y = 0; y < Cells; ++y)
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

        public SudokuGrid(GridOptions options)
        {
            ClearGrid(options);
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

        public int NumCages { get { return cageInfo == null ? 0 : cageInfo.NumCages; } }
        public bool MajorDiagonal { get { return gridOptions.majorDiagonal; } }
        public bool MinorDiagonal { get { return gridOptions.minorDiagonal; } }

        public bool InEditMode { get { return PlayMode == PlayModes.EditBox || PlayMode == PlayModes.EditCell; } }
        public bool InPlayMode { get { return PlayMode == PlayModes.Play || PlayMode == PlayModes.Pencil; } }

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
            ClearGrid(gridOptions);
        }

        public int DefaultBoxAt(int x, int y)
        {
            return DefaultBoxAt(x, y, CellA, CellB);
        }

        public static int DefaultBoxAt(int x, int y, int cellA, int cellB)
        {
            return (x / cellA) + (y / cellB) * cellB;
        }

        public int BoxAt(int x, int y)
        {
            return gridOptions.boxes[x, y];
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

        public void ClearGrid(GridOptions options)
        {
            this.cageInfo = null;
            this.gridOptions = options;
            values = new int[Cells, Cells];
            flags = new CellFlags[Cells, Cells];
            for (int x = 0; x < Cells; ++x)
                for (int y = 0; y < Cells; ++y)
                {
                    values[x, y] = -1;
                    flags[x, y] = CellFlags.Free;
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
                return gridOptions.isJigsaw;
                //for (int x = 0; x < Cells; ++x)
                //    for (int y = 0; y < Cells; ++y)
                //        if (boxes[x, y] != DefaultBoxAt(x, y))
                //            return true;
                //return false;
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
            for (int x = 0; x < Cells - 1; ++x)
                for (int y = 0; y < Cells; ++y)
                    if (CageAt(x, y) != CageAt(x + 1, y))
                        context.DrawVerticalLine(x, y, thick);
            for (int x = 0; x < Cells; ++x)
                for (int y = 0; y < Cells - 1; ++y)
                    if (CageAt(x, y) != CageAt(x, y + 1))
                        context.DrawHorizontalLine(x, y, thick);
        }

        public void Paint(PaintContext context)
        {
            Graphics graphics = context.graphics;

            // Background
            Brush back =
                InEditMode ? Brushes.LightSalmon :
                (solver.UnfulfilledRequirements.Length == 0) ? Brushes.LightGreen :
                Brushes.White;
            context.FillBackground(back);

            // Coloured cages or boxes
            if (gridOptions.colours != null)
            {
                Color[] cage_colours = new Color[]{
                    Color.FromArgb(255,253,152),
                    Color.FromArgb(207,231,153),
                    Color.FromArgb(203,232,250),
                    Color.FromArgb(248,207,223)};
                Brush[] cage_brushes = new Brush[4];
                for (int i = 0; i < 4; ++i)
                    cage_brushes[i] = new SolidBrush(cage_colours[i]);
                for (int x = 0; x < Cells; ++x)
                    for (int y = 0; y < Cells; ++y)
                        context.FillCell(x, y, cage_brushes[gridOptions.colours[x, y]]);
                for (int i = 0; i < 4; ++i)
                    cage_brushes[i].Dispose();
            }

            // Diagonals - brush  previously Brushes.LightGray
            if (MajorDiagonal)
                using (Brush br = new HatchBrush(HatchStyle.ForwardDiagonal, Color.DarkGray, Color.Transparent))
                    for (int x = 0; x < Cells; ++x)
                        context.FillCell(x, x, br);
            if (MinorDiagonal)
                using (Brush br = new HatchBrush(HatchStyle.BackwardDiagonal, Color.DarkGray, Color.Transparent))
                    for (int x = 0; x < Cells; ++x)
                        context.FillCell(x, Cells - x - 1, br);

            // Cage totals
            if (IsKiller)
            {
                Point?[] cage_indicators = new Point?[NumCages];
                for (int y = 8; y >= 0; --y)
                    for (int x = 8; x >= 0; --x)
                        if (flags[x, y] == CellFlags.Free)
                            cage_indicators[cageInfo.cages[x, y]] = new Point(x, y);

                using (Pen inner = new Pen(Color.White, 5.0f))
                    PaintCages(context, inner);

                for (int i = 0; i < NumCages; ++i)
                    if (cage_indicators[i] != null)
                        context.DrawTotal(Brushes.Black,
                            cage_indicators[i].Value.X, cage_indicators[i].Value.Y, cageInfo.remaining_totals[i].ToString());
            }

            foreach (Hint hint in paintedHints)
                hint.PaintBackground(context);

            context.DrawSelection();

            // Grid
            Dictionary<int, int> bs = new Dictionary<int, int>();
            for (int x = 0; x < Cells; ++x)
                for (int y = 0; y < Cells; ++y)
                {
                    int b = BoxAt(x, y);
                    if (bs.ContainsKey(b))
                        ++bs[b];
                    else
                        bs[b] = 1;
                }
            Func<int, int, int, int, int> border = (int x0, int y0, int x1, int y1) =>
            {
                int b0 = BoxAt(x0, y0), b1 = BoxAt(x1, y1);
                if (b0 == b1)
                    return 0;
                if (bs[b0] < Cells || bs[b1] < Cells)
                    return 2;
                return 1;
            };
            using (Pen thick = new Pen(Color.Black, 3.0f))
            using (Pen wrong = new Pen(Color.Red, 3.0f))
            {
                Pen[] pens = { Pens.Black, thick, wrong };
                for (int x = 0; x < Cells - 1; ++x)
                    for (int y = 0; y < Cells; ++y)
                        context.DrawVerticalLine(x, y, pens[border(x, y, x + 1, y)]);
                for (int x = 0; x < Cells; ++x)
                    for (int y = 0; y < Cells - 1; ++y)
                        context.DrawHorizontalLine(x, y, pens[border(x, y, x, y + 1)]);
            }

            // Givens
            {
                for (int x = 0; x < Cells; ++x)
                    for (int y = 0; y < Cells; ++y)
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
            if (InEditMode)
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
            if (flags[x, y] == CellFlags.Fixed && PlayMode != PlayModes.EditCell)
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
            if (playMode == PlayModes.EditBox)
                return false;
            if (playMode == PlayModes.Pencil)
                return PencilCell(x, y, n);

            CellFlags fx = playMode == PlayModes.EditCell ? CellFlags.Fixed : CellFlags.Play;
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
            for (int x = 0; x < Cells; x++)
                for (int y = 0; y < Cells; y++)
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

        public void SolveProof()
        {
            var old = solver.log;
            using (MemoryStream ms = new MemoryStream())
            {
                solver.log = new StreamWriter(ms);
                SolveResult solns = solver.DoBacktrackingSolve(this);
                ms.Position = 0;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(DescribeSolveResult(solns));
                string line;
                using (StreamReader sr = new StreamReader(ms))
                    while ((line = sr.ReadLine()) != null)
                        sb.AppendLine(line);
                solver.log = old;

                TextForm form = new TextForm();
                form.btnCancel.Visible = false;
                form.tbText.ReadOnly = true;
                form.tbText.Text = sb.ToString();
                form.ShowDialog();
            }

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

        public string DescribeSolveResult(SolveResult solns, out MessageBoxIcon? kind)
        {
            switch (solns)
            {
                case SolveResult.MultipleSolutions:
                    kind = MessageBoxIcon.Error;
                    return "Multiple solutions";
                case SolveResult.NoSolutions:
                    kind = MessageBoxIcon.Error;
                    return "No solutions";
                default:
                case SolveResult.Ongoing:
                    kind = null;
                    return null;
                case SolveResult.SingleSolution:
                    kind = MessageBoxIcon.Information;
                    return "Single solution found";
                case SolveResult.TooDifficult:
                    kind = MessageBoxIcon.Information;
                    return "Too difficult";
            }
        }

        public string DescribeSolveResult(SolveResult solns)
        {
            MessageBoxIcon? icon;
            return DescribeSolveResult(solns, out icon);
        }

        public void ShowSolveResult(SolveResult solns)
        {
            Updated(); // May have made some progress
            MessageBoxIcon? icon;
            string message = DescribeSolveResult(solns, out icon);
            if (icon != null)
                MessageBox.Show(message, "Solve", MessageBoxButtons.OK, icon.Value);
        }

        public void FireChanged()
        {
            NotifyListeners((UpdateListener listener) => listener.OnChangeGrid());
        }

        public string[] GridStringsCompact()
        {
            string[] s = new string[Cells];
            for (int y = 0; y < Cells; ++y)
            {
                for (int x = 0; x < Cells; ++x)
                    if (flags[x, y] == CellFlags.Fixed)
                        s[y] += gridOptions.ToChar(values[x, y]);
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
                gridOptions.AddHeaderLines(ret);
                for (int y = 0; y < Cells; ++y)
                {
                    string lcl = "";
                    string gs = "";
                    for (int x = 0; x < Cells; ++x)
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
                gridOptions.AddHeaderLines(l);
                for (int y = 0; y < Cells; ++y)
                {
                    string s = "";
                    for (int x = 0; x < Cells; ++x)
                    {
                        s += (char)('a' + gridOptions.boxes[x, y]);
                        if (flags[x, y] == CellFlags.Fixed)
                            s += gridOptions.ToChar(values[x, y]);
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
                gridOptions.AddHeaderLines(l);
                StringBuilder line = new StringBuilder();
                for (int x1 = 0; x1 < Cells; ++x1)
                {
                    if (x1 != 0 && x1 % CellA == 0)
                        line.Append("+");
                    line.Append("-");
                }
                string sepLine = line.ToString();
                for (int y = 0; y < Cells; ++y)
                {
                    if (y != 0 && y % CellB == 0)
                        l.Add(sepLine);
                    line.Clear();
                    for (int x = 0; x < Cells; ++x)
                    {
                        if (x != 0 && x % CellA == 0)
                            line.Append("|");
                        if (flags[x, y] == CellFlags.Fixed)
                            line.Append(gridOptions.ToChar(values[x, y]));
                        else
                            line.Append(".");
                    }
                    l.Add(line.ToString());
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

        void CheckHeader(string[] a, out int i, out GridOptions options)
        {
            int cellA = 3, cellB = 3;
            char start = '1';
            i = 0;
            if (a.Length > 0)
            {
                string[] os = a[i].Split(',');
                if (os.Length >= 2)
                {
                    cellA = int.Parse(os[0]);
                    cellB = int.Parse(os[1]);
                    ++i;
                    if (os.Length >= 3 && os[2].Length == 1)
                        start = os[2][0];
                }
            }
            options = new GridOptions(cellA, cellB, start);
            if (options.CheckDiagonalsLine(a[i].ToUpperInvariant()))
                i++;
        }

        public bool SetGridStrings81(string[] a)
        {
            GridOptions options;
            int i;
            CheckHeader(a, out i, out options);
            if (i >= a.Length)
                return false;
            string s = a[i];
            if (s.Length == options.Cells * options.Cells)
            {
                ClearGrid(options);
                for (int y = 0; y < Cells; ++y)
                    for (int x = 0; x < Cells; ++x)
                    {
                        char c = s[y * Cells + x];
                        int v;
                        if (gridOptions.FromChar(c, out v) && v != -1)
                        {
                            values[x, y] = v;
                            flags[x, y] = CellFlags.Fixed;
                        }
                    }
                ResetSolver();
                return true;
            }

            return false;
        }

        public bool SetGridStrings9x9(string[] a)
        {
            GridOptions options;
            int i;
            CheckHeader(a, out i, out options);
            List<string> l = new List<string>();
            for (int y = i; y < a.Length; ++y)
            {
                string s = "";
                for (int x = 0; x < a[y].Length; ++x)
                {
                    char c = a[y][x];
                    int v;
                    if (options.FromChar(c, out v))
                        s += c;
                }
                if (s.Length == Cells)
                    l.Add(s);
            }
            a = l.ToArray();
            if (a.Length != Cells)
                return false;
            ClearGrid(options);
            for (int y = 0; y < Cells; ++y)
                for (int x = 0; x < Cells; ++x)
                {
                    char c = a[y][x];
                    int v;
                    if(options.FromChar(c, out v) && v != -1)
                    {
                        values[x, y] = v;
                        flags[x, y] = CellFlags.Fixed;
                    }
                }
            ResetSolver();
            return true;
        }

        public bool SetGridStrings9x9Killer(string[] a0)
        {
            GridOptions options;
            int i0;
            CheckHeader(a0, out i0, out options);
            options.isKiller = true;
            List<string> l = new List<string>();
            List<string> aLine = new List<string>();
            List<string> aSum = new List<string>();
            for (int i=i0; i < a0.Length; ++i)
            {
                string[] a = a0[i].Split(' ');
                foreach (string s in a)
                    if (s.Length == Cells)
                        aLine.Add(s);
                    else if (s.Length > 2 && s[1] == '=')
                        aSum.Add(s);
                    else if (s.Length > 0)
                        return false;
            }
            if (aLine.Count != Cells)
                return false;
            ClearGrid(options);
            cageInfo = new CageInfo(Cells);
            Dictionary<char, int> names = new Dictionary<char, int>();
            Dictionary<int, int> totals = new Dictionary<int, int>();
            int num_cages = 0;
            for (int y = 0; y < Cells; ++y)
            {
                string s = aLine[y];
                for (int x = 0; x < Cells; ++x)
                {
                    int this_cage;
                    char c = s[x];
                    int v;
                    if (options.FromChar(c, out v) && v != -1)
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
            if (grand != Cells * Cells * 5)
                return false;

            ColourSolver cs = new ColourSolver();
            gridOptions.colours = cs.Solve(this, cageInfo.cages, NumCages);

            cageInfo.Reset();
            /*
            for (int y = 0; y < Cells; ++y)
                for (int x = 0; x < Cells; ++x)
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
            GridOptions options;
            int i;
            CheckHeader(a, out i, out options);
            List<string> l = new List<string>();
            options.isJigsaw = true;
            options.boxes = new int[Cells, Cells];
            for (int y = i; y < a.Length; ++y)
            {
                string s = "";
                for (int x = 0; x < a[y].Length - 1; ++x)
                {
                    char b = a[y][x];
                    if (b >= 'a' && b < 'a' + Cells)
                    {
                        ++x;
                        char c = a[y][x];
                        if (c == '0')
                            c = '.';
                        int v;
                        if(options.FromChar(c, out v))
                        {
                            s += b;
                            s += c;
                        }
                    }
                }
                if (s.Length == 18)
                    l.Add(s);
            }
            a = l.ToArray();
            if (a.Length != Cells)
                return false;
            ClearGrid(options);
            for (int y = 0; y < Cells; ++y)
                for (int x = 0; x < Cells; ++x)
                {
                    char b = a[y][x * 2];
                    char c = a[y][x * 2 + 1];
                    int v;
                    options.FromChar(c, out v);
                    if (v != -1)
                    {
                        values[x, y] = v;
                        flags[x, y] = CellFlags.Fixed;
                    }
                    else
                    {
                        values[x, y] = -1;
                        flags[x, y] = CellFlags.Free;
                    }
                    gridOptions.boxes[x, y] = b - 'a';
                }

            ColourSolver cs = new ColourSolver();
            gridOptions.colours = cs.Solve(this, gridOptions.boxes, Cells);
            ResetSolver();
            return true;
        }

        public bool SetGridStrings81Jigsaw(string[] a)
        {
            GridOptions options;
            int i1;
            CheckHeader(a, out i1, out options);
            // format used at www.icosahedral.net
            string[] a2 = (from string line in a.Skip(i1) where line.Length == Cells * Cells select line).ToArray();
            if (a2.Length != 2)
                return false;
            options.isJigsaw = true;
            ClearGrid(options);
            gridOptions.boxes = new int[Cells, Cells];
            for (int i = 0; i < Cells * Cells; ++i)
            {
                int x = i % Cells;
                int y = i / Cells;
                char c = a2[i1][i];
                char b = a2[i1+1][i];
                int v;
                if (!options.FromChar(c, out v))
                    return false;
                if (v != -1)
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
                    gridOptions.boxes[x, y] = Cells - 1;
                else
                    gridOptions.boxes[x, y] = b - '0';
            }

            ColourSolver cs = new ColourSolver();
            gridOptions.colours = cs.Solve(this, gridOptions.boxes, Cells);
            ResetSolver();
            return true;
        }

        public void ClearEntries()
        {
            for (int x = 0; x < Cells; ++x)
                for (int y = 0; y < Cells; ++y)
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
            int[,] ans = new int[Cells, Cells];
            foreach (Candidate k in soln)
            {
                SudokuCandidate sc = (SudokuCandidate)k;
                ans[sc.x, sc.y] = sc.n;
            }
            // Add clues until soluable
            PlayMode = PlayModes.EditCell;
            while (true)
            {
                SolveResult sr = solver.DoLogicalSolve(this, hints);
                if (sr == SolveResult.SingleSolution)
                    break;
                int x = Utils.Utils.Rnd(Cells);
                int y = Utils.Utils.Rnd(Cells);
                if (sr == SolveResult.NoSolutions)
                {
                    ClearCell(x, y);
                    ClearCell(8 - x, 8 - y);
                }
                else
                {
                    SetCell(x, y, ans[x, y]);
                    SetCell(Cells - x - 1, Cells - y - 1, ans[Cells - x - 1, Cells - y - 1]);
                }
            }
            // Remove any clues where it remains soluable
            for (int x = 0; x < Cells; ++x)
                for (int y = 0; y < Cells; ++y)
                {
                    ClearCell(x, y);
                    if (solver.DoLogicalSolve(this, hints) != SolveResult.SingleSolution)
                        SetCell(x, y, ans[x, y]);
                }
            ResetSolver();
        }

        public IEnumerable<Tuple<int,int>> Neighbours(int x, int y)
        {
            for (int dx = -1; dx <= 1; ++dx)
                for (int dy = -1; dy <= 1; ++dy)
                    if ((dx != 0 || dy != 0) && (dx == 0 || dy == 0) && x + dx >= 0 && y + dy >= 0 && x + dx < Cells && y + dy < Cells)
                        yield return new Tuple<int, int>(x + dx, y + dy);
        }

        public void ChangeBox(int x, int y)
        {
            Func<Tuple<int, int>, int> getBox = (Tuple<int, int> p) => gridOptions.boxes[p.Item1, p.Item2];
            List<int> bs = new List<int>();
            foreach (int b in from p in Neighbours(x, y) select getBox(p))
                if (!bs.Contains(b))
                    bs.Add(b);
            int i = bs.IndexOf(gridOptions.boxes[x, y]);
            gridOptions.boxes[x, y] = bs[i == bs.Count - 1 ? 0 : i + 1];
            ColourSolver cs = new ColourSolver();
            gridOptions.colours = cs.Solve(this, gridOptions.boxes, Cells);
        }
    }
}
