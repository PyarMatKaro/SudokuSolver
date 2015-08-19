using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sudoku
{
    public class PickContext
    {
        public readonly int scw, sch;
        public readonly Size FullSize;

        public PickContext(Size size)
        {
            FullSize = size;
            scw = size.Width / 27;
            sch = size.Height / 27;
        }

        public Size SubcellSize { get { return new Size(scw, sch); } }
        public Size CellSize { get { return new Size(scw * 3, sch * 3); } }
        public Point CellLocation(int x, int y) { return new Point(scw * 3 * x, sch * 3 * y); }
        public Point SubcellLocation(int x, int y) { return new Point(scw * x, sch * y); }

    }
}
