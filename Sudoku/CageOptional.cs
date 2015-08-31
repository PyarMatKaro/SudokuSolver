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
        SudokuCandidate[] removed;

        public CageOptional(SudokuGrid.CageInfo info)
        {
            this.info = info;
        }

        static SudokuCandidate[] Filter(SudokuSolver ss, int cage, Func<int, bool> included)
        {
            List<SudokuCandidate> toRemove = new List<SudokuCandidate>();
            for (int i = 0; i < 9; ++i) if (!included(i + 1))
                {
                    CageOptional co = ss.GetCageOptional(cage, i);
                    if (co.Included)
                    {
                        foreach(SudokuCandidate sc in co.UnselectedCandidates)
                            if (!sc.IsMarked)
                            {
                                toRemove.Add(sc);
                                sc.MarkCandidate(true);
                            }
                    }
                }

            SudokuCandidate[] removed = toRemove.ToArray();
            for (int i = 0; i < removed.Length; ++i)
            {
                SudokuCandidate sc = removed[i];
                ss.DiscardCandidate(sc);
                sc.MarkCandidate(false);
            }
            return removed.ToArray();
        }

        public static SudokuCandidate[] CheckRemains(SudokuSolver ss, SudokuGrid.CageInfo info, int cage)
        {
            int s = info.remaining_sizes[cage];
            if (s == 1)
            {
                // One empty cell remains in the cage, so make the total
                int total = info.remaining_totals[cage];
                return Filter(ss, cage, (int n) => (n == total));
            }
            /*
            else if(s > 1)
            {
                // 's' numbers left to choose from
                int total = info.remaining_totals[cage];
                int min = s * (s + 1) / 2;
                int max = 45 - s * (s - 1) / 2;
                Filter(ss, cage, (int n) => total + n >= min && total + n <= max);
            }
            */
            return null;
        }

        public void OnCover(ExactCover ec, SudokuCandidate sc)
        {
            int cage = sc.c;
            int num = sc.n + 1;
            System.Diagnostics.Debug.Assert(info.sizes[cage] > 0);
            SudokuSolver ss = (SudokuSolver)ec;
            info.remaining_totals[cage] -= num;
            info.remaining_sizes[cage]--;
            removed = CheckRemains(ss, info, cage);
        }

        public void OnUncover(ExactCover ec, SudokuCandidate sc)
        {
            if (removed != null)
                for (int i = removed.Length - 1; i >= 0; --i)
                    ec.UndiscardCandidate(removed[i]);

            int num = sc.n + 1;
            info.remaining_totals[sc.c] += num;
            info.remaining_sizes[sc.c]++;
        }
    }
}
