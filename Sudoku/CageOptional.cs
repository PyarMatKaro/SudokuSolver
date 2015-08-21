using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solver;

namespace Sudoku
{
    public class CageOptional : SudokuRequirement
    {
        SudokuGrid.CageInfo info;
        Candidate[] removed;

        public CageOptional(SudokuGrid.CageInfo info)
        {
            this.info = info;
        }

        void Filter(SudokuSolver ss, int cage, Func<int, bool> included)
        {
            List<Candidate> toRemove = new List<Solver.Candidate>();
            for (int i = 0; i < 9; ++i) if (!included(i + 1))
                {
                    CageOptional co = ss.GetCageOptional(i, cage);
                    if (co.Included)
                    {
                        toRemove.AddRange(co.UnselectedCandidates);
                    }
                }
            removed = toRemove.ToArray();
            foreach (Candidate k in removed)
                ss.DiscardCandidate(k);
        }

        public void OnCover(ExactCover ec, SudokuCandidate sc)
        {
            int cage = sc.c;
            int num = sc.n + 1;
            System.Diagnostics.Debug.Assert(info.sizes[cage] > 0);
            SudokuSolver ss = (SudokuSolver)ec;
            info.running[cage] -= num;
            info.sizes[cage]--;
            removed = null;
            int s = info.sizes[cage];
            if(s == 1)
            {
                // One empty cell remains in the cage, so make the total
                int total = info.running[cage];
                Filter(ss, cage, (int n) => (n == total));
            }
            else if(s > 1)
            {
                // 's' numbers left to choose from
                int total = info.running[cage];
                int min = s * (s + 1) / 2;
                int max = 45 - s * (s - 1) / 2;
                Filter(ss, cage, (int n) => total + n >= min && total + n <= max);
            }
        }

        public void OnUncover(ExactCover ec, SudokuCandidate sc)
        {
            if(removed != null)
                foreach (Candidate k in removed)
                    ec.UndiscardCandidate(k);

            int num = sc.n + 1;
            info.running[sc.c] += num;
            info.sizes[sc.c]++;
        }
    }
}
