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
        CageOptional cageOptional;
        public int x, y, n, b, c;

        public SudokuCandidate(int x, int y, int n, int b, int c)
        {
            this.x = x;
            this.y = y;
            this.n = n;
            this.b = b;
            this.c = c;
        }

        public void AddCageOptional(CageOptional c)
        {
            this.cageOptional = c;
            AddCandidate(c);
        }

        public override void OnCover(ExactCover ec)
        {
            if (cageOptional != null)
                cageOptional.OnCover(ec, this);
        }

        public override void OnUncover(ExactCover ec)
        {
            if (cageOptional != null)
                cageOptional.OnUncover(ec, this);
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
