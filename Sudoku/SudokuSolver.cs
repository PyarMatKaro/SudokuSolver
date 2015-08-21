using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solver;
using System.Drawing;

namespace Sudoku
{
    public class SudokuSolver : ExactCover
    {
        private int solnsFound;
        private Candidate[] sampleSoln;
        protected SudokuRequirement[] ca;

        public SudokuSolver(SudokuGrid grid)
        {
            CreateMatrix(grid);
            for (int x = 0; x < 9; ++x)
                for (int y = 0; y < 9; ++y)
                    if (grid.FlagAt(x, y) != SudokuGrid.CellFlags.Free)
                        TrySelectCandidate(GetCandidate(x, y, grid.ValueAt(x, y)));
        }

        protected void CreateRequirements(int nc)
        {
            ca = new SudokuRequirement[nc];
            for (int i = 0; i < ca.Length; ++i)
                ca[i] = new SudokuRequirement();
        }

        public SolveResult DoLogicalSolve(SudokuGrid grid, HintSelections hs)
        {
            while (true)
            {
                if (Solved)
                    return SolveResult.SingleSolution;
                Hint hint = SingleHint(hs);
                if (hint == null)
                    return SolveResult.TooDifficult;
                SolveResult result = hint.Apply(grid);
                if(result != SolveResult.Ongoing)
                    return result;
            }
        }

        public SolveResult DoBacktrackingSolve()
        {
            //verbose = false;
            solnsFound = 0;
            int osc = tsc;
            BacktrackingSearch();
            if (solnsFound == 0)
                return SolveResult.NoSolutions;
            if (solnsFound == 1)
            {
                for (int i = osc; i < sampleSoln.Length; ++i)
                    SelectCandidate(sampleSoln[i]);
                return SolveResult.SingleSolution;
            }
            return SolveResult.MultipleSolutions;
        }

        public override bool OnSolution()
        {
            //Console.WriteLine("Solution");
            //Console.WriteLine(GridString());
            ++solnsFound;
            if (solnsFound > 1)
                return true;
            sampleSoln = SelectedCandidates;
            return false;
        }

        public SudokuRequirement GetRequirement(int x, int y, Houses house)
        {
            int type = (int) house;
            return ca[x + y * 9 + type * 9 * 9];
        }

        SudokuRequirement GetMajorDiagonalRequirement(int n)
        {
            return ca[ 9 * 9 * 4 + n];
        }

        SudokuRequirement GetMinorDiagonalRequirement(int n)
        {
            return ca[ 9 * 9 * 4 + 9 + n];
        }

        /*
        public SudokuSolver(string[] lines)
        {
            CreateMatrix();
            Apply(lines);
        }
        */

        void CreateMatrix(SudokuGrid grid)
        {
            // Create requirements - columns of 0/1 matrix
            CreateRequirements(9 * 9 * 4 + grid.NumDiagonals * 9);
            Houses[] houses = new Houses[]{
                Houses.Cell,
                Houses.Column,
                Houses.Row,
                Houses.Box};
            for (int i0 = 0; i0 < 9; ++i0)
            {
                for (int i1 = 0; i1 < 9; ++i1)
                {
                    for (int type = 0; type < 4; ++type)
                    {
                        Houses house = houses[type];
                        SudokuRequirement c = GetRequirement(i0, i1, house);
                        c.i0 = i0;
                        c.i1 = i1;
                        c.house = house;
                        requirements.AddRequirement(c);
                    }
                }
            }
            if (grid.NumDiagonals > 0)
            {
                for (int i0 = 0; i0 < 9; ++i0)
                {
                    SudokuRequirement c = GetMajorDiagonalRequirement(i0);
                    c.i0 = i0;
                    c.i1 = -1;
                    c.house = Houses.MajorDiagonal;
                    requirements.AddRequirement(c);
                }
            }
            if (grid.NumDiagonals > 1)
            {
                for (int i0 = 0; i0 < 9; ++i0)
                {
                    SudokuRequirement c = GetMinorDiagonalRequirement(i0);
                    c.i0 = i0;
                    c.i1 = -1;
                    c.house = Houses.MinorDiagonal;
                    requirements.AddRequirement(c);
                }
            }
            
            // Create candidates - rows of 0/1 matrix
            CreateCandidates(new SudokuCandidate[9 * 9 * 9]);
            CreateSolution(9 * 9);
            for (int x = 0; x < 9; ++x)
            {
                for (int y = 0; y < 9; ++y)
                {
                    int b = grid.BoxAt(x, y);
                    for (int n = 0; n < 9; ++n)
                    {
                        SudokuCandidate k = new SudokuCandidate(x, y, n, b);
                        k.AddCandidate(GetRequirement(x, y, Houses.Cell));
                        k.AddCandidate(GetRequirement(x, n, Houses.Column));
                        k.AddCandidate(GetRequirement(y, n, Houses.Row));
                        k.AddCandidate(GetRequirement(b, n, Houses.Box));
                        if (grid.NumDiagonals > 0 && x == y)
                            k.AddCandidate(GetMajorDiagonalRequirement(n));
                        if (grid.NumDiagonals > 1 && x == 8 - y)
                            k.AddCandidate(GetMinorDiagonalRequirement(n));
                        tr[trc++] = k;
                    }
                }
            }

            /*
            if (grid.IsKiller)
            {
                SudokuGrid.CageInfo info = grid.cageInfo;
                for (int ca = 0; ca < info.cages.Length; ++ca)
                {
                    List<Point> pts = new List<Point>();
                    for (int x = 0; x < 9; ++x)
                        for (int y = 0; y < 8; ++y)
                            if (info.cages[x, y] == ca)
                                pts.Add(new Point(x, y));
                    int[] ns = new int[9];
                    for (int i = 0; i < 9; ++i) ns[i] = i;
                    Candidate k = new Candidate();
                    KillerRequire(k, info.totals[ca], pts.ToArray(), ns);
                }
            }
            */
        }

        /*
        void KillerRequire(Candidate k, int tot, Point[] pts, int[] ns)
        {
            Candidate k1 = new Candidate();
        }
        */

        public Candidate GetCandidate(int x, int y, int n)
        {
            return tr[n + y * 9 + x * 9 * 9];
        }

        public string[] SolutionStrings()
        {
            string[] ret = new string[9];
            char[,] g = new char[9, 9];
            for (int x = 0; x < 9; ++x)
                for (int y = 0; y < 9; ++y)
                    g[x, y] = '.';
            for (int i = 0; i < tsc; ++i)
            {
                SudokuCandidate sc = (SudokuCandidate)ts[i].Candidate;
                g[sc.x, sc.y] = (char)('1' + sc.n);
            }
            for (int y = 0; y < 9; ++y)
            {
                ret[y] = "";
                for (int x = 0; x < 9; ++x)
                    if (g[x, y] == '.')
                    {
                        ret[y]+="[";
                        SudokuRequirement c = GetRequirement(x, y, Houses.Cell);
                        foreach (SudokuCandidate k in c.UnselectedCandidates)
                            ret[y] += (char)('1' + k.n);
                        ret[y] += "]";
                    }
                    else
                        ret[y] += g[x, y];
            }
            return ret;
        }

        public Candidate[] RandomSolution()
        {
            if (SolveRandomly() == SolveResult.SingleSolution)
                return sampleSoln;
            return null;
        }

        SolveResult SolveRandomly()
        {
            if (Solved)
            {
                sampleSoln = SelectedCandidates;
                return SolveResult.SingleSolution;
            }
            Requirement c = EasiestRequirement;
            if (c.s == 0)
                return SolveResult.NoSolutions;

            List<Candidate> list = new List<Candidate>();
            list.AddRange(c.UnselectedCandidates);
            while (list.Count > 0)
            {
                int i = Utils.Utils.Rnd(list.Count);
                Candidate t = list[i];
                list.RemoveAt(i);
                SelectCandidate(t);
                SolveResult sr = SolveRandomly();
                UnselectCandidate();
                if (sr == SolveResult.SingleSolution)
                    return SolveResult.SingleSolution;
            }
            return SolveResult.NoSolutions;
        }

    }
}
