using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Solver;

namespace Sudoku
{
    public class SudokuRequirement : Requirement
    {
        public Houses house;
        public int i0, i1;

        public SudokuRequirement()
        {
            ClearRequirement(1);
        }

        public bool AppliesAt(int x, int y, int b)
        {
            if (house == Houses.Cell)
                return x == i0 && y == i1;
            if (house == Houses.Column)
                return x == i0;
            if (house == Houses.Row)
                return y == i0;
            if (house == Houses.Box)
                return b == i0;
            if (house == Houses.MajorDiagonal)
                return x == y;
            if (house == Houses.MinorDiagonal)
                return x == 8 - y;
            return false;
        }

        public override string RequirementString()
        {
            if (house == Houses.Cell)
                return "fill cell at " + (1 + i0) + "," + (1 + i1);
            else if (house == Houses.Column)
                return "place " + (1 + i1) + " in column " + (1 + i0);
            else if (house == Houses.Row)
                return "place " + (1 + i1) + " in row " + (1 + i0);
            else if (house == Houses.Box)
                return "place " + (1 + i1) + " in box " + (1 + i0);
            else if (house == Houses.MajorDiagonal)
                return "place " + (1 + i0) + " in major diagonal";
            else if (house == Houses.MinorDiagonal)
                return "place " + (1 + i0) + " in minor diagonal";
            return "?";
        }

        public override void PaintForeground(HintPainter hp, Brush br)
        {
            PaintContext context = (PaintContext)hp;
            if (house == Houses.Column)
            {
                for (int y = 0; y < 9; ++y)
                    if (context.grid.FlagAt(i0, y) == SudokuGrid.CellFlags.Free)
                        context.DrawSubcell(br, i0, y, i1);
            }
            else if (house == Houses.Row)
            {
                for (int x = 0; x < 9; ++x)
                    if (context.grid.FlagAt(x, i0) == SudokuGrid.CellFlags.Free)
                        context.DrawSubcell(br, x, i0, i1);
            }
            else if (house == Houses.Box)
            {
                for (int x = 0; x < 9; ++x)
                    for (int y = 0; y < 9; ++y)
                        if (context.grid.FlagAt(x, y) == SudokuGrid.CellFlags.Free &&
                            context.grid.BoxAt(x, y) == i0)
                            context.DrawSubcell(br, x, y, i1);
            }
            else if (house == Houses.MajorDiagonal)
            {
                for (int x = 0; x < 9; ++x)
                    if (context.grid.FlagAt(x, x) == SudokuGrid.CellFlags.Free)
                        context.DrawSubcell(br, x, x, i0);
            }
            else if (house == Houses.MinorDiagonal)
                for (int x = 0; x < 9; ++x)
                    if (context.grid.FlagAt(x, 8 - x) == SudokuGrid.CellFlags.Free)
                        context.DrawSubcell(br, x, 8 - x, i0);
        }

        public override void PaintBackground(HintPainter hp, Brush back)
        {
            PaintContext context = (PaintContext)hp;
            if (house == Houses.Cell)
                context.FillCell(i0, i1, back);
            else if (house == Houses.Column)
                context.FillColumn(i0, back);
            else if (house == Houses.Row)
                context.FillRow(i0, back);
            else if (house == Houses.Box)
                context.FillBox(i0, back);
            else if (house == Houses.MajorDiagonal)
                for (int x = 0; x < 9; ++x)
                    context.FillCell(x, x, back);
            else if (house == Houses.MinorDiagonal)
                for (int x = 0; x < 9; ++x)
                    context.FillCell(x, 8 - x, back);
        }

    }
}
