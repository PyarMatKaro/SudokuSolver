using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Solver;

namespace Sudoku
{
    public partial class SudokuForm : Form, HintSupport
    {

        public SudokuForm()
        {
            InitializeComponent();
            CreateChildMenus();
            LoadOptions();
        }

        public SudokuForm(SudokuGrid grid)
        {
            InitializeComponent();
            sudokuControl.Grid = grid;
            CreateChildMenus();
            UpdateMode();
            LoadOptions();
        }

        void CreateChildMenus()
        {
            showToolStripMenuItem.Create(this, ()=>HintShow, true);
            autoSolveToolStripMenuItem.Create(this, ()=>HintAutoSolve, true);
        }

        public SudokuGrid Grid { get { return sudokuControl.Grid; } }
        public HintFlags HintFlags { get { return Grid.HintFlags; } set { Grid.HintFlags = value; } }
        public HintSelections HintShow { get { return Grid.HintShow; } set { Grid.HintShow = value; } }
        public HintSelections HintAutoSolve { get { return Grid.HintAutoSolve; } set { Grid.HintAutoSolve = value; } }

        private void SudokuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            sudokuControl.Grid = null;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextForm tf = new TextForm();
            tf.tbText.Lines = Grid.GridStrings();
            if (tf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Grid.SetGridStrings(tf.tbText.Lines);
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grid.ResetSolver();
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Grid.PlayMode = SudokuGrid.PlayModes.Edit;
            UpdateMode();
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grid.PlayMode = SudokuGrid.PlayModes.Play;
            UpdateMode();
        }

        bool LoadOptions()
        {
            try
            {
                HintOptions ho = Utils.Utils.ReadObject<HintOptions>("HintOptions.xml");
                Grid.HintOptions = ho;
                UpdateHintChecks();
                return true;
            }
            catch (FileNotFoundException ex)
            {
                return false;
            }
        }

        void SaveOptions()
        {
            Utils.Utils.WriteObject("HintOptions.xml", Grid.HintOptions);
        }

        void UpdateMode()
        {
            editToolStripMenuItem1.Checked = Grid.PlayMode == SudokuGrid.PlayModes.Edit;
            playToolStripMenuItem.Checked = Grid.PlayMode == SudokuGrid.PlayModes.Play;
            pencilToolStripMenuItem.Checked = Grid.PlayMode == SudokuGrid.PlayModes.Pencil;
            Grid.Updated();
        }

        private void newGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grid.ClearGrid(false);
            Grid.ResetSolver();
        }

        private void newDiagonalGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grid.ClearGrid(true);
            Grid.ResetSolver();
        }

        private void solveLogicallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grid.SolveLogically();
        }

        private void solveWithBacktrackingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grid.SolveBacktracking();
        }

        private void solutionAsTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextForm tf = new TextForm();
            tf.tbText.Lines = Grid.SolutionStrings;
            tf.ShowDialog();
        }

        public void UpdateHints()
        {
            Grid.Updated();
            UpdateHintChecks();
            SaveOptions();
        }

        void UpdateHintChecks()
        {
            showPencilMarksToolStripMenuItem.Checked = HintFlags.PaintPencilMarks;
            showHintsToolStripMenuItem.Checked = HintFlags.PaintHints;
            selectForcedMovesToolStripMenuItem.Checked = HintFlags.SelectForcedMoveHints;

            showToolStripMenuItem.UpdateSelectionHints();
            autoSolveToolStripMenuItem.UpdateSelectionHints();
        }

        private void showPencilMarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HintFlags.PaintPencilMarks = !showPencilMarksToolStripMenuItem.Checked;
            UpdateHints();
        }

        private void showHintsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HintFlags.PaintHints = !showHintsToolStripMenuItem.Checked;
            UpdateHints();
        }

        private void selectForcedMovesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HintFlags.SelectForcedMoveHints = !selectForcedMovesToolStripMenuItem.Checked;
            HintFlags.PaintHints |= HintFlags.SelectForcedMoveHints;
            UpdateHints();
        }

        private void restartPlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grid.PlayMode = SudokuGrid.PlayModes.Play;
            Grid.ClearEntries();
            UpdateMode();
        }

        private void pencilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grid.PlayMode = SudokuGrid.PlayModes.Pencil;
            UpdateMode();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.DefaultExt="sud";
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    FileStream fs = new FileStream(fd.FileName, FileMode.Open);
                    StreamReader sr = new StreamReader(fs);
                    List<string> lines = new List<string>();
                    while (!sr.EndOfStream)
                        lines.Add(sr.ReadLine());
                    sr.Close();
                    Grid.SetGridStrings(lines.ToArray());

                    Grid.PlayMode = SudokuGrid.PlayModes.Play;
                    UpdateMode();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.DefaultExt = "sud";
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string[] lines = Grid.GridStrings();
                    FileStream fs = new FileStream(fd.FileName, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    foreach(string s in lines)
                        sw.WriteLine(s);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grid.Generate(new HintSelections(HintSelections.Level.Diabolical));
            Grid.PlayMode = SudokuGrid.PlayModes.Play;
            UpdateMode();
        }
    }
}
