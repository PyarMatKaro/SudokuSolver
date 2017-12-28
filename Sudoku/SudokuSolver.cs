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
        protected CageOptional[,] co;

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

        protected void CreateOptionals(SudokuGrid grid, int nc)
        {
            co = new CageOptional[nc, 9];
            for (int i0 = 0; i0 < nc; ++i0)
                for (int i1 = 0; i1 < 9; ++i1)
                    co[i0, i1] = new CageOptional(grid.cageInfo);
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

        public SolveResult DoBacktrackingSolve(SudokuGrid grid)
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
                    grid.SelectCandidate(sampleSoln[i]);
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

        SudokuRequirement GetDiagonalRequirement(int d, int n)
        {
            return ca[ 9 * 9 * 4 + 9 * d + n];
        }

        public CageOptional GetCageOptional(int cage, int n)
        {
            return co[cage, n];
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
            int num_cages = grid.NumCages;
            int diags = 0;
            if (grid.MajorDiagonal)
                ++diags;
            if (grid.MinorDiagonal)
                ++diags;
            int d = grid.MajorDiagonal ? 1 : 0; // Where minor diagonal is found
            CreateRequirements(9 * 9 * 4 + diags * 9);
            CreateOptionals(grid, num_cages);
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
            if (grid.MajorDiagonal)
            {
                for (int i0 = 0; i0 < 9; ++i0)
                {
                    SudokuRequirement c = GetDiagonalRequirement(0, i0);
                    c.i0 = i0;
                    c.i1 = -1;
                    c.house = Houses.MajorDiagonal;
                    requirements.AddRequirement(c);
                }
            }
            if (grid.MinorDiagonal)
            {
                for (int i0 = 0; i0 < 9; ++i0)
                {
                    SudokuRequirement c = GetDiagonalRequirement(d, i0);
                    c.i0 = i0;
                    c.i1 = -1;
                    c.house = Houses.MinorDiagonal;
                    requirements.AddRequirement(c);
                }
            }
            for (int i0 = 0; i0 < num_cages; ++i0)
                for (int i1 = 0; i1 < 9; ++i1)
                {
                    CageOptional ca = GetCageOptional(i0, i1);
                    ca.i0 = i0;
                    ca.i1 = i1;
                    ca.house = Houses.Cage;
                    //optionals.AddRequirement(ca);
                    ca.SetIncluded();
                }
            
            // Create candidates - rows of 0/1 matrix
            CreateCandidates(new SudokuCandidate[9 * 9 * 9]);
            CreateSolution(9 * 9);
            for (int x = 0; x < 9; ++x)
            {
                for (int y = 0; y < 9; ++y)
                {
                    int cage = grid.CageAt(x, y);
                    int box = grid.BoxAt(x, y);
                    for (int n = 0; n < 9; ++n)
                    {
                        SudokuCandidate k = new SudokuCandidate(x, y, n, box, cage);
                        k.AddCandidate(GetRequirement(x, y, Houses.Cell));
                        k.AddCandidate(GetRequirement(x, n, Houses.Column));
                        k.AddCandidate(GetRequirement(y, n, Houses.Row));
                        k.AddCandidate(GetRequirement(box, n, Houses.Box));
                        if (grid.MajorDiagonal && x == y)
                            k.AddCandidate(GetDiagonalRequirement(0, n));
                        if (grid.MinorDiagonal && x == 8 - y)
                            k.AddCandidate(GetDiagonalRequirement(d, n));
                        if (cage != -1)
                            k.AddCageOptional(GetCageOptional(cage, n));
                        tr[trc++] = k;

                        // Discard candidates where cage size already reached 1
                        //if (cage != -1 && grid.cageInfo.sizes[cage] == 1 && n + 1 != grid.cageInfo.totals[cage])
                        //    DiscardCandidate(k);
                    }
                }
            }

            // Discard candidates that do not meet cage total
            for (int i0 = 0; i0 < num_cages; ++i0)
                CageOptional.CheckRemains(this, grid.cageInfo, i0, -1);

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
