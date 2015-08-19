using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Solver;

namespace Sudoku
{
    public class PaintContext : PickContext, HintPainter, IDisposable
    {
        public readonly Graphics graphics;
        public readonly SudokuGrid grid;
        public readonly Font SmallFont, LargeFont;
        public readonly int selx, sely;

        public PaintContext(Graphics graphics, Size size, SudokuGrid grid, int selx, int sely)
            : base(size)
        {
            this.grid = grid;
            this.graphics = graphics;
            this.selx = selx;
            this.sely = sely;
            SmallFont = new Font(FontFamily.GenericSansSerif, 10.0f);
            LargeFont = new Font(FontFamily.GenericSansSerif, 30.0f);
        }

        public void Dispose()
        {
            SmallFont.Dispose();
            LargeFont.Dispose();
        }

        public void FillBackground(Brush back)
        {
            graphics.FillRectangle(back, new Rectangle(new Point(), FullSize));
        }

        public void FillCell(int x, int y, Brush back)
        {
            graphics.FillRectangle(back, new Rectangle(CellLocation(x, y), CellSize));
        }

        public void FillColumn(int x, Brush back)
        {
            graphics.FillRectangle(back, new Rectangle(CellLocation(x, 0),
                new Size(scw * 3, sch * 27)));
        }

        public void FillRow(int y, Brush back)
        {
            graphics.FillRectangle(back, new Rectangle(CellLocation(0, y),
                new Size(scw * 27, sch * 3)));
        }

        public void FillBox(int b, Brush back)
        {
            for (int x = 0; x < 9; ++x)
                for (int y = 0; y < 9; ++y)
                    if (grid.BoxAt(x, y) == b)
                        graphics.FillRectangle(back, new Rectangle(CellLocation(x, y), CellSize));
        }

        public void DrawCell(Brush br, int x, int y, int n)
        {
            Size csz = CellSize;
            Point p = CellLocation(x, y);
            string s = (n + 1).ToString();
            SizeF sz = graphics.MeasureString(s, LargeFont);
            graphics.DrawString(s, LargeFont, br, p.X + (csz.Width - sz.Width) / 2, p.Y + (csz.Height - sz.Height) / 2);
        }

        public void DrawSubcell(Brush br, int x, int y, int n)
        {
            int x2 = n % 3;
            int y2 = n / 3;
            Size csz = SubcellSize;
            Point p = SubcellLocation(x * 3 + x2, y * 3 + y2);
            string s = (n + 1).ToString();
            SizeF sz = graphics.MeasureString(s, SmallFont);
            graphics.DrawString(s, SmallFont, br, p.X + (csz.Width - sz.Width) / 2, p.Y + (csz.Height - sz.Height) / 2);
        }

        public void DrawTotal(Brush br, int x, int y, string s)
        {
            Size csz = SubcellSize;
            Point p = SubcellLocation(x * 3, y * 3);
            SizeF sz = graphics.MeasureString(s, SmallFont);
            graphics.DrawString(s, SmallFont, br, p.X + (csz.Width - sz.Width) / 2, p.Y + (csz.Height - sz.Height) / 2);
        }

        public void DrawVerticalLine(int x, int y, Pen p)
        {
            Point p0 = CellLocation(x + 1, y);
            Point p1 = CellLocation(x + 1, y + 1);
            graphics.DrawLine(p, p0, p1);
        }

        public void DrawHorizontalLine(int x, int y, Pen p)
        {
            Point p0 = CellLocation(x, y + 1);
            Point p1 = CellLocation(x + 1, y + 1);
            graphics.DrawLine(p, p0, p1);
        }

        public void OutlineCell(Pen selection, int x, int y)
        {
            Rectangle r = new Rectangle(CellLocation(x, y), CellSize);
            graphics.DrawRectangle(selection, r);
        }

        public void DrawSelection()
        {
            if (selx != -1)
                using (Pen selection = new Pen(Color.Purple, 5.0f))
                    OutlineCell(selection, selx, sely);
        }

    }
}
