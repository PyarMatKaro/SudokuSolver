﻿using Solver;
namespace Sudoku
{
    partial class SudokuForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.gridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solutionAsTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.editBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pencilToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.restartPlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solveLogicallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logicalProofToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solveWithBacktrackingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backtrackingProofToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showPencilMarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new Solver.HintSelectionsMenu();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.selectForcedMovesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSolveToolStripMenuItem = new Solver.HintSelectionsMenu();
            this.sudokuControl = new Sudoku.SudokuControl();
            this.autoGenerateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gridToolStripMenuItem,
            this.modeToolStripMenuItem,
            this.solverToolStripMenuItem,
            this.generatorToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(378, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // gridToolStripMenuItem
            // 
            this.gridToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGridToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.textToolStripMenuItem,
            this.solutionAsTextToolStripMenuItem});
            this.gridToolStripMenuItem.Name = "gridToolStripMenuItem";
            this.gridToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.gridToolStripMenuItem.Text = "Grid";
            // 
            // newGridToolStripMenuItem
            // 
            this.newGridToolStripMenuItem.Name = "newGridToolStripMenuItem";
            this.newGridToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.newGridToolStripMenuItem.Text = "New grid...";
            this.newGridToolStripMenuItem.Click += new System.EventHandler(this.newGridToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.loadToolStripMenuItem.Text = "Load...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            this.textToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.textToolStripMenuItem.Text = "Puzzle as text...";
            this.textToolStripMenuItem.Click += new System.EventHandler(this.textToolStripMenuItem_Click);
            // 
            // solutionAsTextToolStripMenuItem
            // 
            this.solutionAsTextToolStripMenuItem.Name = "solutionAsTextToolStripMenuItem";
            this.solutionAsTextToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.solutionAsTextToolStripMenuItem.Text = "Solution as text...";
            this.solutionAsTextToolStripMenuItem.Click += new System.EventHandler(this.solutionAsTextToolStripMenuItem_Click);
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem1,
            this.editBoxToolStripMenuItem,
            this.playToolStripMenuItem,
            this.pencilToolStripMenuItem,
            this.toolStripMenuItem3,
            this.restartPlayToolStripMenuItem});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.modeToolStripMenuItem.Text = "Mode";
            // 
            // editToolStripMenuItem1
            // 
            this.editToolStripMenuItem1.Name = "editToolStripMenuItem1";
            this.editToolStripMenuItem1.Size = new System.Drawing.Size(135, 22);
            this.editToolStripMenuItem1.Text = "Edit cell";
            this.editToolStripMenuItem1.Click += new System.EventHandler(this.editToolStripMenuItem1_Click);
            // 
            // editBoxToolStripMenuItem
            // 
            this.editBoxToolStripMenuItem.Name = "editBoxToolStripMenuItem";
            this.editBoxToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.editBoxToolStripMenuItem.Text = "Edit box";
            this.editBoxToolStripMenuItem.Click += new System.EventHandler(this.editBoxToolStripMenuItem_Click);
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.playToolStripMenuItem.Text = "Play";
            this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
            // 
            // pencilToolStripMenuItem
            // 
            this.pencilToolStripMenuItem.Name = "pencilToolStripMenuItem";
            this.pencilToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.pencilToolStripMenuItem.Text = "Pencil";
            this.pencilToolStripMenuItem.Click += new System.EventHandler(this.pencilToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(132, 6);
            // 
            // restartPlayToolStripMenuItem
            // 
            this.restartPlayToolStripMenuItem.Name = "restartPlayToolStripMenuItem";
            this.restartPlayToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.restartPlayToolStripMenuItem.Text = "Restart play";
            this.restartPlayToolStripMenuItem.Click += new System.EventHandler(this.restartPlayToolStripMenuItem_Click);
            // 
            // solverToolStripMenuItem
            // 
            this.solverToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem,
            this.solveLogicallyToolStripMenuItem,
            this.logicalProofToolStripMenuItem,
            this.solveWithBacktrackingToolStripMenuItem,
            this.backtrackingProofToolStripMenuItem});
            this.solverToolStripMenuItem.Name = "solverToolStripMenuItem";
            this.solverToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.solverToolStripMenuItem.Text = "Solver";
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // solveLogicallyToolStripMenuItem
            // 
            this.solveLogicallyToolStripMenuItem.Name = "solveLogicallyToolStripMenuItem";
            this.solveLogicallyToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.solveLogicallyToolStripMenuItem.Text = "Solve logically";
            this.solveLogicallyToolStripMenuItem.Click += new System.EventHandler(this.solveLogicallyToolStripMenuItem_Click);
            // 
            // logicalProofToolStripMenuItem
            // 
            this.logicalProofToolStripMenuItem.Name = "logicalProofToolStripMenuItem";
            this.logicalProofToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.logicalProofToolStripMenuItem.Text = "Proof logically";
            this.logicalProofToolStripMenuItem.Click += new System.EventHandler(this.logicalProofToolStripMenuItem_Click);
            // 
            // solveWithBacktrackingToolStripMenuItem
            // 
            this.solveWithBacktrackingToolStripMenuItem.Name = "solveWithBacktrackingToolStripMenuItem";
            this.solveWithBacktrackingToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.solveWithBacktrackingToolStripMenuItem.Text = "Solve with backtracking";
            this.solveWithBacktrackingToolStripMenuItem.Click += new System.EventHandler(this.solveWithBacktrackingToolStripMenuItem_Click);
            // 
            // backtrackingProofToolStripMenuItem
            // 
            this.backtrackingProofToolStripMenuItem.Name = "backtrackingProofToolStripMenuItem";
            this.backtrackingProofToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.backtrackingProofToolStripMenuItem.Text = "Proof with backtracking";
            this.backtrackingProofToolStripMenuItem.Click += new System.EventHandler(this.backtrackingProofToolStripMenuItem_Click);
            // 
            // generatorToolStripMenuItem
            // 
            this.generatorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateToolStripMenuItem,
            this.autoGenerateToolStripMenuItem});
            this.generatorToolStripMenuItem.Name = "generatorToolStripMenuItem";
            this.generatorToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.generatorToolStripMenuItem.Text = "Generator";
            // 
            // generateToolStripMenuItem
            // 
            this.generateToolStripMenuItem.Name = "generateToolStripMenuItem";
            this.generateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.generateToolStripMenuItem.Text = "Generate";
            this.generateToolStripMenuItem.Click += new System.EventHandler(this.generateToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showPencilMarksToolStripMenuItem,
            this.showHintsToolStripMenuItem,
            this.showToolStripMenuItem,
            this.toolStripMenuItem4,
            this.selectForcedMovesToolStripMenuItem,
            this.autoSolveToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // showPencilMarksToolStripMenuItem
            // 
            this.showPencilMarksToolStripMenuItem.Name = "showPencilMarksToolStripMenuItem";
            this.showPencilMarksToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.showPencilMarksToolStripMenuItem.Text = "Show pencil marks";
            this.showPencilMarksToolStripMenuItem.Click += new System.EventHandler(this.showPencilMarksToolStripMenuItem_Click);
            // 
            // showHintsToolStripMenuItem
            // 
            this.showHintsToolStripMenuItem.Name = "showHintsToolStripMenuItem";
            this.showHintsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.showHintsToolStripMenuItem.Text = "Show hints";
            this.showHintsToolStripMenuItem.Click += new System.EventHandler(this.showHintsToolStripMenuItem_Click);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.showToolStripMenuItem.Text = "Hints";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(177, 6);
            // 
            // selectForcedMovesToolStripMenuItem
            // 
            this.selectForcedMovesToolStripMenuItem.Name = "selectForcedMovesToolStripMenuItem";
            this.selectForcedMovesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.selectForcedMovesToolStripMenuItem.Text = "Select forced moves";
            this.selectForcedMovesToolStripMenuItem.Click += new System.EventHandler(this.selectForcedMovesToolStripMenuItem_Click);
            // 
            // autoSolveToolStripMenuItem
            // 
            this.autoSolveToolStripMenuItem.Name = "autoSolveToolStripMenuItem";
            this.autoSolveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.autoSolveToolStripMenuItem.Text = "Auto solve";
            // 
            // sudokuControl
            // 
            this.sudokuControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sudokuControl.Grid = null;
            this.sudokuControl.Location = new System.Drawing.Point(0, 24);
            this.sudokuControl.Margin = new System.Windows.Forms.Padding(4);
            this.sudokuControl.Name = "sudokuControl";
            this.sudokuControl.Size = new System.Drawing.Size(378, 408);
            this.sudokuControl.TabIndex = 0;
            // 
            // autoGenerateToolStripMenuItem
            // 
            this.autoGenerateToolStripMenuItem.Name = "autoGenerateToolStripMenuItem";
            this.autoGenerateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.autoGenerateToolStripMenuItem.Text = "Auto Generate";
            this.autoGenerateToolStripMenuItem.Click += new System.EventHandler(this.autoGenerateToolStripMenuItem_Click);
            // 
            // SudokuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 432);
            this.Controls.Add(this.sudokuControl);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SudokuForm";
            this.Text = "Sudoku";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SudokuForm_FormClosing);
            this.Load += new System.EventHandler(this.SudokuForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SudokuControl sudokuControl;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem gridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solveLogicallyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solveWithBacktrackingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solutionAsTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showHintsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectForcedMovesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showPencilMarksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartPlayToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem pencilToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private HintSelectionsMenu autoSolveToolStripMenuItem;
        private HintSelectionsMenu showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logicalProofToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backtrackingProofToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoGenerateToolStripMenuItem;
    }
}