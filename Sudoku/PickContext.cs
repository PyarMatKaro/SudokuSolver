using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sudoku
{
    public class PickContext
    {
        public readonly int scw, sch, ox, oy, cellA, cellB;
        public readonly Size FullSize;

        public PickContext(Size size, int cellA, int cellB)
        {
            this.cellA = cellA;
            this.cellB = cellB;
            int cells = cellA * cellB;
            FullSize = size;
            scw = size.Width / (cells * cellA);
            sch = size.Height / (cells * cellB);
            ox = (size.Width - scw * cells * cellA) / 2;
            oy = (size.Height - sch * cells * cellB) / 2;
        }

        public Size SubcellSize { get { return new Size(scw, sch); } }
        public Size CellSize { get { return new Size(scw * cellA, sch * cellB); } }
        public Point CellLocation(int x, int y) { return new Point(ox + scw * cellA * x, oy + sch * cellB * y); }
        public Point SubcellLocation(int x, int y) { return new Point(ox + scw * x, oy + sch * y); }

    }
}
