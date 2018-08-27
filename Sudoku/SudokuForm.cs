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
        //int defCellA = 3;
        //int defCellB = 3;

        public static SudokuForm Instance;

        PuzzleTextForm puzzleForm = new PuzzleTextForm() { Text = "Puzzle" };
        internal TextForm proofForm = new TextForm() { Text = "Proof" };

        public SudokuForm()
        {
            System.Diagnostics.Debug.Assert(Instance == null);
            Instance = this;

            InitializeComponent();
            CreateChildMenus();
            LoadOptions();
        }

        public SudokuForm(SudokuGrid grid)
        {
            System.Diagnostics.Debug.Assert(Instance == null);
            Instance = this;

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
            puzzleForm.Form = this;
            puzzleForm.tbText.Lines = Grid.GridStrings();
            puzzleForm.Show();
            puzzleForm.BringToFront();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grid.ResetSolver();
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Grid.PlayMode = SudokuGrid.PlayModes.EditCell;
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
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        void SaveOptions()
        {
            Utils.Utils.WriteObject("HintOptions.xml", Grid.HintOptions);
        }

        public void UpdateMode()
        {
            editBoxToolStripMenuItem.Checked = Grid.PlayMode == SudokuGrid.PlayModes.EditBox;
            editBoxToolStripMenuItem.Enabled = Grid.IsJigsaw;
            editToolStripMenuItem1.Checked = Grid.PlayMode == SudokuGrid.PlayModes.EditCell;
            playToolStripMenuItem.Checked = Grid.PlayMode == SudokuGrid.PlayModes.Play;
            pencilToolStripMenuItem.Checked = Grid.PlayMode == SudokuGrid.PlayModes.Pencil;
            Grid.Updated();
        }
        
        private void solveLogicallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grid.SolveLogically();
        }

        void SolveProof(bool logical)
        {
            string proof = Grid.SolveProof(logical);
            proofForm.btnCancel.Visible = false;
            proofForm.tbText.ReadOnly = true;
            proofForm.tbText.Text = proof;
            Grid.Updated();
            proofForm.Show();
            proofForm.BringToFront();
        }

        private void logicalProofToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SolveProof(true);
        }

        private void solveWithBacktrackingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grid.SolveBacktracking();
        }

        private void backtrackingProofToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SolveProof(false);
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

        string curFile;

        string CurFile
        {
            get
            {
                return curFile;
            }
            set
            {
                curFile = value;
                Text = (curFile == null) ? "New Sudoku" : Path.GetFileName(CurFile);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.DefaultExt = "sud";
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
                    if (Grid.SetGridStrings(lines.ToArray()))
                    {
                        CurFile = fd.FileName;
                        Grid.PlayMode = SudokuGrid.PlayModes.Play;
                        UpdateMode();
                    }
                    else
                    {
                        MessageBox.Show("Grid not loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(null);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(CurFile);
        }

        void Save(string fileName)
        {
            if (fileName == null)
            {
                SaveFileDialog fd = new SaveFileDialog();
                fd.DefaultExt = "sud";
                if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    fileName = fd.FileName;
            }

            if (fileName != null)
            {
                try
                {
                    string[] lines = Grid.GridStrings();
                    FileStream fs = new FileStream(fileName, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    foreach (string s in lines)
                        sw.WriteLine(s);
                    sw.Close();
                    CurFile = fileName;
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

        private void SudokuForm_Load(object sender, EventArgs e)
        {
            ClientSize = new Size(386, 440); // Make border visible
        }
        
        private void editBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grid.PlayMode = SudokuGrid.PlayModes.EditBox;
            UpdateMode();
        }

        private void newGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridForm gf = new GridForm();
            gf.InitControls(this, Grid.gridOptions);
            gf.ShowDialog();
        }

        public void CreateGrid(SudokuGrid.GridOptions options)
        {
            CurFile = null;
            Grid.ClearGrid(options);
            if (options.isKiller)
                Grid.SetDefaultKiller();
            Grid.Setup();
            UpdateMode();
        }
    }
}
