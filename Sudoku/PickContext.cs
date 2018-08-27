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

        public int Cells { get { return cellA * cellB; } }
        public int CellA { get { return cellA; } }
        public int CellB { get { return cellB; } }

        public PickContext(Size size, int cellA, int cellB)
        {
            this.cellA = cellA;
            this.cellB = cellB;
            FullSize = size;
            scw = size.Width / (Cells * cellA);
            sch = size.Height / (Cells * cellB);
            ox = (size.Width - scw * Cells * cellA) / 2;
            oy = (size.Height - sch * Cells * cellB) / 2;
        }

        public Size SubcellSize { get { return new Size(scw, sch); } }
        public Size CellSize { get { return new Size(scw * cellA, sch * cellB); } }
        public Point CellLocation(int x, int y) { return new Point(ox + scw * cellA * x, oy + sch * cellB * y); }
        public Point SubcellLocation(int x, int y) { return new Point(ox + scw * x, oy + sch * y); }

        public void PositionOf(int mouseX, int mouseY, out int cellX, out int cellY)
        {
            cellX = (mouseX-ox) / (scw*cellA);
            cellY = (mouseY-oy) / (sch*cellB);
        }
    }
}
