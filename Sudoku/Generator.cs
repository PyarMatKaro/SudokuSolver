using Solver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sudoku
{
    public class Generator
    {
        string desc;
        SudokuGrid grid;
        bool stopped;
        ManualResetEvent finished = new ManualResetEvent(false);

        public Generator(SudokuGrid.GridOptions options)
        {
            grid = new SudokuGrid(options);
            grid.Setup();

            desc = options.isJigsaw ? "Jigsaw" : options.isKiller ? "Killer" : "Normal";
            if (grid.MajorDiagonal && grid.MinorDiagonal) desc += " Cross";
            else if (grid.MajorDiagonal) desc += " Major";
            else if (grid.MinorDiagonal) desc += " Minor";
            desc += " " + options.Cells;

            new Thread(new ThreadStart(AutoGenerate)).Start();
        }

        void AutoGenerate()
        {
            string folder = @"C:\Users\savag\OneDrive\Documents\Sudoku\Auto";
            int n = 0;
            while (!stopped)
            {
                //g.Generate(new HintSelections(HintSelections.Level.Diabolical));
                grid.Generate(null);
                HintSelections.Level? level;
                SolveResult solns;
                grid.RateDifficulty(out level, out solns);

                string difficulty = (level == null) ? "Impossible" : level.ToString();
                string[] lines = grid.GridStrings();
                string fileName = desc + " " + difficulty + " " + (n++) + ".sud";
                File.WriteAllLines(Path.Combine(folder, fileName), lines);
            }

            finished.Set();
        }

        public void Stop()
        {
            stopped = true;
            finished.WaitOne();
        }
    }
}
