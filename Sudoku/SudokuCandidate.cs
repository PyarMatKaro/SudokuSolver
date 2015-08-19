using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solver;
using System.Drawing;

namespace Sudoku
{
    public class SudokuCandidate : Candidate
    {
        public int x, y, n, b;

        public SudokuCandidate(int x, int y, int n, int b)
        {
            this.x = x;
            this.y = y;
            this.n = n;
            this.b = b;
        }

        public override string ToString()
        {
            return "fill " + (1 + x) + "," + (1 + y) + " with " + (1 + n);
        }

        public override void PaintForeground(HintPainter hp, Brush br)
        {
            PaintContext context = (PaintContext)hp;
            context.DrawSubcell(br, x, y, n);
        }

        public override void PaintBackground(HintPainter hp, Brush br)
        {
            PaintContext context = (PaintContext)hp;
            context.FillCell(x, y, br);
        }

    }
}
