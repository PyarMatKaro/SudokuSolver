using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Solver
{
    public class HintSelectionsMenu : ToolStripMenuItem
    {
        HintSupport form;
        Func<HintSelections> funcHints;
        bool causeShowHints;
        ToolStripMenuItem forcedMovesToolStripMenuItem;
        ToolStripMenuItem immediateDiscardablesToolStripMenuItem;
        ToolStripMenuItem eventualDiscardablesToolStripMenuItem;
        ToolStripMenuItem selectablesToolStripMenuItem;
        ToolStripMenuItem eventualSolutionsToolStripMenuItem;
        ToolStripMenuItem noneToolStripMenuItem;
        ToolStripMenuItem easyToolStripMenuItem;
        ToolStripMenuItem mediumToolStripMenuItem;
        ToolStripMenuItem hardToolStripMenuItem;
        ToolStripMenuItem diabolicalToolStripMenuItem;

        public HintSelections Hints { get { return funcHints(); } }

        public void Create(HintSupport form, Func<HintSelections> hints, bool causeShowHints)
        {
            this.form = form;
            this.funcHints = hints;
            this.causeShowHints = causeShowHints;

            forcedMovesToolStripMenuItem = new ToolStripMenuItem();
            forcedMovesToolStripMenuItem.Text = "Forced Moves";
            forcedMovesToolStripMenuItem.Click += forcedMovesToolStripMenuItem_Click;
            forcedMovesToolStripMenuItem.ToolTipText = "Only way to meet a requirement";

            immediateDiscardablesToolStripMenuItem = new ToolStripMenuItem();
            immediateDiscardablesToolStripMenuItem.Text = "Immediate Discardables";
            immediateDiscardablesToolStripMenuItem.Click += immediateDiscardablesToolStripMenuItem_Click;
            immediateDiscardablesToolStripMenuItem.ToolTipText = "Directly prevents a requirement being met when selected";

            eventualDiscardablesToolStripMenuItem = new ToolStripMenuItem();
            eventualDiscardablesToolStripMenuItem.Text = "Eventual Discardables";
            eventualDiscardablesToolStripMenuItem.Click += eventualDiscardablesToolStripMenuItem_Click;
            eventualDiscardablesToolStripMenuItem.ToolTipText = "Indirectly prevents a requirement being met when selected";

            selectablesToolStripMenuItem = new ToolStripMenuItem();
            selectablesToolStripMenuItem.Text = "Selectables";
            selectablesToolStripMenuItem.Click += selectablesToolStripMenuItem_Click;
            selectablesToolStripMenuItem.ToolTipText = "Prevents a requirement being met when not selected";

            eventualSolutionsToolStripMenuItem = new ToolStripMenuItem();
            eventualSolutionsToolStripMenuItem.Text = "Eventual Solutions";
            eventualSolutionsToolStripMenuItem.Click += eventualSolutionsToolStripMenuItem_Click;
            eventualSolutionsToolStripMenuItem.ToolTipText = "Leads to a solution when selected (so should be selected assuming a single solution exists)";

            ToolStripSeparator divider = new ToolStripSeparator();

            noneToolStripMenuItem = new ToolStripMenuItem();
            noneToolStripMenuItem.Text = "None";
            noneToolStripMenuItem.Click += noneToolStripMenuItem_Click;

            easyToolStripMenuItem = new ToolStripMenuItem();
            easyToolStripMenuItem.Text = "Easy";
            easyToolStripMenuItem.Click += easyToolStripMenuItem_Click;

            mediumToolStripMenuItem = new ToolStripMenuItem();
            mediumToolStripMenuItem.Text = "Medium";
            mediumToolStripMenuItem.Click += mediumToolStripMenuItem_Click;

            hardToolStripMenuItem = new ToolStripMenuItem();
            hardToolStripMenuItem.Text = "Hard";
            hardToolStripMenuItem.Click += hardToolStripMenuItem_Click;

            diabolicalToolStripMenuItem = new ToolStripMenuItem();
            diabolicalToolStripMenuItem.Text = "Diabolical";
            diabolicalToolStripMenuItem.Click += diabolicalToolStripMenuItem_Click;

            DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                forcedMovesToolStripMenuItem,
                immediateDiscardablesToolStripMenuItem,
                eventualDiscardablesToolStripMenuItem,
                selectablesToolStripMenuItem,
                eventualSolutionsToolStripMenuItem,
                divider,
                noneToolStripMenuItem,
                easyToolStripMenuItem,
                mediumToolStripMenuItem,
                hardToolStripMenuItem,
                diabolicalToolStripMenuItem});
        }

        void UpdateAllHints(bool set)
        {
            if (set && causeShowHints)
                form.HintFlags.PaintHints = true;
            form.UpdateHints();
        }

        private void forcedMovesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hints.ForcedMoves = !forcedMovesToolStripMenuItem.Checked;
            UpdateAllHints(Hints.ForcedMoves);
        }

        private void immediateDiscardablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hints.ImmediateDiscardables = !immediateDiscardablesToolStripMenuItem.Checked;
            UpdateAllHints(Hints.ImmediateDiscardables);
        }

        private void eventualDiscardablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hints.EventualDiscardables = !eventualDiscardablesToolStripMenuItem.Checked;
            UpdateAllHints(Hints.EventualDiscardables);
        }

        private void selectablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hints.Selectables = !selectablesToolStripMenuItem.Checked;
            UpdateAllHints(Hints.Selectables);
        }

        private void eventualSolutionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hints.EventualSolutions = !eventualSolutionsToolStripMenuItem.Checked;
            UpdateAllHints(Hints.EventualSolutions);
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hints.SetLevel(HintSelections.Level.None);
            UpdateAllHints(false);
        }

        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hints.SetLevel(HintSelections.Level.Easy);
            UpdateAllHints(false);
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hints.SetLevel(HintSelections.Level.Medium);
            UpdateAllHints(false);
        }

        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hints.SetLevel(HintSelections.Level.Hard);
            UpdateAllHints(false);
        }

        private void diabolicalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hints.SetLevel(HintSelections.Level.Diabolical);
            UpdateAllHints(false);
        }

        public void UpdateSelectionHints()
        {
            forcedMovesToolStripMenuItem.Checked = Hints.ForcedMoves;
            immediateDiscardablesToolStripMenuItem.Checked = Hints.ImmediateDiscardables;
            eventualDiscardablesToolStripMenuItem.Checked = Hints.EventualDiscardables;
            selectablesToolStripMenuItem.Checked = Hints.Selectables;
            eventualSolutionsToolStripMenuItem.Checked = Hints.EventualSolutions;
            noneToolStripMenuItem.Checked = Hints.IsLevel(HintSelections.Level.None);
            easyToolStripMenuItem.Checked = Hints.IsLevel(HintSelections.Level.Easy);
            mediumToolStripMenuItem.Checked = Hints.IsLevel(HintSelections.Level.Medium);
            hardToolStripMenuItem.Checked = Hints.IsLevel(HintSelections.Level.Hard);
            diabolicalToolStripMenuItem.Checked = Hints.IsLevel(HintSelections.Level.Diabolical);
        }
    }
}
