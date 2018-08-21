using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sudoku
{
    public class PickContext
    {
        public readonly int scw, sch, ox, oy;
        public readonly Size FullSize;

        public PickContext(Size size, int cells)
        {
            int sz = cells * 3;
            FullSize = size;
            scw = size.Width / sz;
            sch = size.Height / sz;
            ox = (size.Width - scw * sz) / 2;
            oy = (size.Height - sch * sz) / 2;
        }

        public Size SubcellSize { get { return new Size(scw, sch); } }
        public Size CellSize { get { return new Size(scw * 3, sch * 3); } }
        public Point CellLocation(int x, int y) { return new Point(ox+scw * 3 * x, oy+sch * 3 * y); }
        public Point SubcellLocation(int x, int y) { return new Point(ox+scw * x, oy+sch * y); }

    }
}
