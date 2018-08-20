using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solver;

namespace Sudoku
{
    public class ColourSolver : ExactCover
    {
        Dictionary<int, List<int>> nb;
        int[] colourOfCage;
        bool haveSolution;

        int findNeighbour(int ca1, int ca2)
        {
            List<int> ns = nb[ca1];
            int nn = ns.Count;
            for (int n = 0; n < nn; ++n)
                if (ns[n] == ca2)
                    return n;
            return -1;
        }

        public int[,] Solve(SudokuGrid grid, int[,] cages, int numCages)
        {
            int Cells = grid.Cells;
            colourOfCage = new int[numCages];
            nb = new Dictionary<int, List<int>>();
            for (int ca = 0; ca < numCages; ++ca)
                nb[ca] = new List<int>();

            int[] dx = new int[] { -1, 0, 1, 0 };
            int[] dy = new int[] { 0, -1, 0, 1 };
            int numBorders = 0;
            for (int x = 0; x < Cells; x++)
                for (int y = 0; y < Cells; y++)
                    for (int d = 0; d < 4; ++d)
                    {
                        int x1 = x + dx[d];
                        int y1 = y + dy[d];
                        if (x1 >= 0 && x1 < Cells && y1 >= 0 && y1 < Cells)
                        {
                            int c1 = cages[x, y];
                            int c2 = cages[x1, y1];
                            if (c1 != c2)
                            {
                                List<int> ns = nb[c1];
                                if (!ns.Contains(c2))
                                {
                                    ns.Add(c2);
                                    ++numBorders;
                                }
                            }
                        }
                    }

            ColourRequirement[,][] crs = new ColourRequirement[4,numCages][];
            for (int co = 0; co < 4; ++co)
                for (int ca = 0; ca < numCages; ca++)
            {
                int nn = nb[ca].Count;
                ColourRequirement[] crs1 = new ColourRequirement[nn];
                crs[co,ca] = crs1;
                for (int n = 0; n < nn; ++n)
                {
                    ColourRequirement cr = new ColourRequirement(co, ca, n);
                    crs1[n] = cr;
                    requirements.AddRequirement(cr);
                }
            }

            for (int ca1 = 0; ca1 < numCages; ca1++)
                for (int n1 = 0; n1 < nb[ca1].Count; ++n1)
                    for (int co1 = 0; co1 < 4; ++co1)
                        for (int co2 = 0; co2 < 4; ++co2) if (co1 != co2)
                            {
                                int ca2 = nb[ca1][n1];
                                if (ca1 < ca2)
                                {
                                    int n2 = findNeighbour(ca2, ca1);
                                    BorderCandidate bc = new BorderCandidate(co1, ca1, co2, ca2);
                                    bc.AddCandidate(crs[co1, ca1][n1]);
                                    bc.AddCandidate(crs[co2, ca2][n2]);
                                }
                            }

            for (int co1 = 0; co1 < 4; ++co1)
                for (int ca = 0; ca < numCages; ca++)
                {
                    RegionCandidate rc = new RegionCandidate(ca, co1);
                    for (int co2 = 0; co2 < 4; ++co2)
                        if (co2 != co1)
                            for (int n = 0; n < nb[ca].Count; ++n)
                                rc.AddCandidate(crs[co2, ca][n]);
                }

            CreateSolution(numCages + numBorders/2);
            BacktrackingSearch();

            if(!haveSolution)
                return null;
            int[,] ret = new int[Cells, Cells];
            for (int x = 0; x < Cells; x++)
                for (int y = 0; y < Cells; y++)
                    ret[x, y] = colourOfCage[cages[x, y]];
            return ret;
        }

        public override bool OnSolution()
        {
            foreach(Candidate c in SelectedCandidates)
                if (c is BorderCandidate)
                {
                    BorderCandidate bc = c as BorderCandidate;
                    colourOfCage[bc.ca1] = bc.co1;
                    colourOfCage[bc.ca2] = bc.co2;
                }
            haveSolution = true;
            return true;
        }
    }
    
    public class ColourRequirement : Requirement
    {
        // For each side of a border and colour, it's either picked or unpicked
        public int co, ca, b;

        public ColourRequirement(int _co, int _ca, int _b) { co = _co; ca = _ca; b = _b; }

        public override string RequirementString()
        {
            return "Pick colour " + co + " at cage " + ca + " border " + b;
        }
    }

    public class BorderCandidate : Candidate
    {
        // Pick a colour one side of a border
        public int co1, ca1, co2, ca2;

        public BorderCandidate(int _co1, int _ca1, int _co2, int _ca2) { co1 = _co1; ca1 = _ca1; co2 = _co2; ca2 = _ca2; }
    }

    public class RegionCandidate : Candidate
    {
        // Unpick a colour around a region
        public int co, ca;

        public RegionCandidate(int _co, int _ca) { co = _co; ca = _ca; }
    }
}
