using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Solver
{
    public abstract class Hint
    {
        public abstract override string ToString();
        public abstract SolveResult Apply(Problem grid);

        public virtual void PaintBackground(HintPainter context) { }
        public virtual void PaintForeground(HintPainter context) { }

        public virtual Candidate Candidate { get { return null; } }
        public virtual Requirement Requirement { get { return null; } }
    }

    public class ImpossibleHint : Hint
    {
        Requirement c;

        public ImpossibleHint(Requirement c)
        {
            this.c = c;
        }

        public override Requirement Requirement { get { return c; } }

        public override string ToString()
        {
            return "No solution, since " + c.RequirementString(0);
        }

        public override SolveResult Apply(Problem grid)
        {
            return SolveResult.NoSolutions;
        }

        public override void PaintBackground(HintPainter context)
        {
            c.PaintBackground(context, Brushes.Pink);
        }

        public override void PaintForeground(HintPainter context)
        {
            c.PaintForeground(context, Brushes.Red);
        }
    }

    public class ForcedMoveHint : Hint
    {
        Requirement c;
        Candidate r;

        public ForcedMoveHint(Requirement c, Candidate r)
        {
            this.c = c;
            this.r = r;
        }

        public override Requirement Requirement { get { return c; } }
        public override Candidate Candidate { get { return r; } }

        public override string ToString()
        {
            return "Must " + Candidate + ", since " + c.RequirementString(1);
        }

        public override SolveResult Apply(Problem grid)
        {
            grid.SelectCandidate(Candidate);
            return SolveResult.Ongoing;
        }

        public override void PaintBackground(HintPainter context)
        {
            Requirement.PaintBackground(context, Brushes.LightGreen);
            Candidate.PaintBackground(context, Brushes.Green);
        }

        public override void PaintForeground(HintPainter context)
        {
            Candidate.PaintForeground(context, Brushes.White);
        }

    }

    public class DiscardableHint : Hint
    {
        Candidate r;
        Requirement c;
        bool eventual;

        public DiscardableHint(Candidate r, Requirement c, bool eventual)
        {
            this.r = r;
            this.c = c;
            this.eventual = eventual;
        }

        public override Candidate Candidate { get { return r; } }

        public override string ToString()
        {
            return "Cannot " + Candidate + ", since this " +
                (eventual ? "eventually " : "") + "leads to " + c.RequirementString(0);
        }

        public override SolveResult Apply(Problem grid)
        {
            grid.DiscardCandidate(Candidate);
            return SolveResult.Ongoing;
        }

        public override void PaintForeground(HintPainter context)
        {
            Candidate.PaintForeground(context, Brushes.Red);
        }

    }

    public class SelectableHint : Hint
    {
        Candidate r;
        Requirement c;

        public SelectableHint(Candidate r, Requirement c)
        {
            this.r = r;
            this.c = c;
        }

        public override Requirement Requirement { get { return c; } }
        public override Candidate Candidate { get { return r; } }

        public override string ToString()
        {
            return "Must " + Candidate + ", discarding eventually leads to " + c.RequirementString(0);
        }

        public override SolveResult Apply(Problem grid)
        {
            grid.SelectCandidate(Candidate);
            return SolveResult.Ongoing;
        }

        public override void PaintForeground(HintPainter context)
        {
            Candidate.PaintForeground(context, Brushes.Green);
        }

    }

    public class EventualSolutionHint : Hint
    {
        Candidate r;

        public EventualSolutionHint(Candidate r)
        {
            this.r = r;
        }

        public override Candidate Candidate { get { return r; } }

        public override string ToString()
        {
            return "Should " + Candidate + ", assuming there is a single solution";
        }

        public override void PaintForeground(HintPainter context)
        {
            Candidate.PaintForeground(context, Brushes.Green);
        }

        public override SolveResult Apply(Problem grid)
        {
            grid.SelectCandidate(r);
            return SolveResult.Ongoing;
        }

    }
}
