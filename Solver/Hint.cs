using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Solver
{
    public abstract class Hint
    {
        public enum Kind {
            Never, // candidate cannot be selected
            Maybe, // candidate may or may not be selected
            Must, // candidate must be selected
            AmongMust // another candidate must be selected for the requirement
        };

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
            c.PaintBackground(context, 0);
        }

        public override void PaintForeground(HintPainter context)
        {
            c.PaintForeground(context, 0);
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
            Requirement.PaintBackground(context, Hint.Kind.AmongMust);
            Candidate.PaintBackground(context, Hint.Kind.Must);
        }

        public override void PaintForeground(HintPainter context)
        {
            Candidate.PaintForeground(context, Hint.Kind.Must);
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
            Candidate.PaintForeground(context, 0);
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
            Candidate.PaintForeground(context, Hint.Kind.AmongMust);
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
            Candidate.PaintForeground(context, Hint.Kind.AmongMust);
        }

        public override SolveResult Apply(Problem grid)
        {
            grid.SelectCandidate(r);
            return SolveResult.Ongoing;
        }

    }
}
