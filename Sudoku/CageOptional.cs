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

        public CageOptional(SudokuGrid.CageInfo info)
        {
            this.info = info;
        }

        public void OnCover(ExactCover ec, SudokuCandidate sc)
        {
            int cage = sc.c;
            System.Diagnostics.Debug.Assert(info.sizes[cage] > 0);
            int num = sc.n + 1;
            info.running[sc.c] -= num;
            info.sizes[sc.c]--;
        }

        public void OnUncover(ExactCover ec, SudokuCandidate sc)
        {
            int num = sc.n + 1;
            info.running[sc.c] += num;
            info.sizes[sc.c]++;
        }
    }
}
