﻿using System;
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

        public bool AppliesAt(int x, int y, SudokuGrid grid)
        {
            if (house == Houses.Cell)
                return x == i0 && y == i1;
            if (house == Houses.Column)
                return x == i0;
            if (house == Houses.Row)
                return y == i0;
            if (house == Houses.Box)
                return grid.BoxAt(x,y) == i0;
            if (house == Houses.MajorDiagonal)
                return x == y;
            if (house == Houses.MinorDiagonal)
                return x == grid.Cells - y - 1;
            if (house == Houses.Cage)
                return grid.CageAt(x, y) == i0;
            return false;
        }

        public override string RequirementString()
        {
            Sudoku.SudokuGrid.GridOptions grid = Sudoku.SudokuGrid.GridOptions.Instance;
            if (house == Houses.Cell)
                return "fill cell at " + (1 + i0) + "," + (1 + i1);
            if (house == Houses.Column)
                return "place " + grid.ToChar(i1) + " in column " + (1 + i0);
            if (house == Houses.Row)
                return "place " + grid.ToChar(i1) + " in row " + (1 + i0);
            if (house == Houses.Box)
                return "place " + grid.ToChar(i1) + " in box " + (char)('a' + i0);
            if (house == Houses.MajorDiagonal)
                return "place " + grid.ToChar(i0) + " in major diagonal";
            if (house == Houses.MinorDiagonal)
                return "place " + grid.ToChar(i0) + " in minor diagonal";
            if (house == Houses.Cage)
                return "place " + grid.ToChar(i1) + " in cage " + (i0 + 1);
            return "?";
        }

        public override void PaintForeground(HintPainter hp, Hint.Kind v)
        {
            PaintContext context = (PaintContext)hp;
            int Cells = context.grid.Cells;
            if (house == Houses.Column)
            {
                for (int y = 0; y < Cells; ++y)
                    if (context.grid.FlagAt(i0, y) == SudokuGrid.CellFlags.Free)
                        context.SetPencil(i0, y, i1, v);
            }
            else if (house == Houses.Row)
            {
                for (int x = 0; x < Cells; ++x)
                    if (context.grid.FlagAt(x, i0) == SudokuGrid.CellFlags.Free)
                        context.SetPencil(x, i0, i1, v);
            }
            else if (house == Houses.Box)
            {
                for (int x = 0; x < Cells; ++x)
                    for (int y = 0; y < Cells; ++y)
                        if (context.grid.FlagAt(x, y) == SudokuGrid.CellFlags.Free &&
                            context.grid.BoxAt(x, y) == i0)
                            context.SetPencil(x, y, i1, v);
            }
            else if (house == Houses.MajorDiagonal)
            {
                for (int x = 0; x < Cells; ++x)
                    if (context.grid.FlagAt(x, x) == SudokuGrid.CellFlags.Free)
                        context.SetPencil(x, x, i0, v);
            }
            else if (house == Houses.MinorDiagonal)
                for (int x = 0; x < Cells; ++x)
                    if (context.grid.FlagAt(x, Cells - x - 1) == SudokuGrid.CellFlags.Free)
                        context.SetPencil(x, Cells - x - 1, i0, v);
        }

        public override void PaintBackground(HintPainter hp, Hint.Kind v)
        {
            PaintContext context = (PaintContext)hp;
            int Cells = context.grid.Cells;
            Brush back = PaintContext.BackgroundBrush(v);
            if (house == Houses.Cell)
                context.FillCell(i0, i1, back);
            else if (house == Houses.Column)
                context.FillColumn(i0, back);
            else if (house == Houses.Row)
                context.FillRow(i0, back);
            else if (house == Houses.Box)
                context.FillBox(i0, back);
            else if (house == Houses.MajorDiagonal)
                for (int x = 0; x < Cells; ++x)
                    context.FillCell(x, x, back);
            else if (house == Houses.MinorDiagonal)
                for (int x = 0; x < Cells; ++x)
                    context.FillCell(x, Cells - x - 1, back);
        }

    }
}
