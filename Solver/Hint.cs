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

        public enum Actions { Discard, Select, Impossible, Solved };
        public abstract override string ToString();
        public abstract Actions Recommendation { get; }
        public abstract Actions Illustration { get; }
        public abstract bool IsComplex { get; }
        public abstract bool IsIn(HintSelections hs);

        public SolveResult Apply(Problem grid)
        {
            switch (Recommendation)
            {
                case Actions.Discard:
                    // DiscardableHint
                    grid.DiscardCandidate(Candidate);
                    break;
                case Actions.Select:
                    // ForcedMoveHint SelectableHint
                    grid.SelectCandidate(Candidate);
                    break;
                case Actions.Impossible:
                    // ImpossibleHint
                    return SolveResult.NoSolutions;
                case Actions.Solved:
                    break;
            }

            return SolveResult.Ongoing;
        }

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
            return "There is no solution, since " + c.RequirementString(true, 0);
        }

        public override bool IsComplex { get { return false; } }
        public override Actions Recommendation { get { return Actions.Impossible; } }
        public override Actions Illustration { get { return Actions.Select; } }
        public override bool IsIn(HintSelections hs) { return true; }

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
            return "We must " + Candidate + ", since " + c.RequirementString(true, 1);
        }

        public override bool IsComplex { get { return false; } }
        public override Actions Recommendation { get { return Actions.Select; } }
        public override Actions Illustration { get { return Actions.Discard; } }
        public override bool IsIn(HintSelections hs) { return hs.ForcedMoves; }

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
            return "We cannot " + Candidate + ", since this " +
                (eventual ? "eventually " : "") + "leads to " + c.RequirementString(false, 0);
        }

        public override bool IsComplex { get { return eventual; } }
        public override Actions Recommendation { get { return Actions.Discard; } }
        public override Actions Illustration { get { return Actions.Select; } }
        public override bool IsIn(HintSelections hs) { return eventual ? hs.EventualDiscardables : hs.ImmediateDiscardables; }

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

        public override bool IsComplex { get { return true; } }
        public override Requirement Requirement { get { return c; } }
        public override Candidate Candidate { get { return r; } }

        public override string ToString()
        {
            return "We must " + Candidate + ", not doing so eventually leads to " + c.RequirementString(false, 0);
        }

        public override Actions Recommendation { get { return Actions.Select; } }
        public override Actions Illustration { get { return Actions.Discard; } }
        public override bool IsIn(HintSelections hs) { return hs.Selectables; }

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
            return "We should " + Candidate + ", assuming there is a single solution";
        }

        public override void PaintForeground(HintPainter context)
        {
            Candidate.PaintForeground(context, Hint.Kind.AmongMust);
        }

        public override bool IsComplex { get { return true; } }
        public override Actions Recommendation { get { return Actions.Select; } }
        public override Actions Illustration { get { return Actions.Select; } }
        public override bool IsIn(HintSelections hs) { return hs.EventualSolutions; }

    }
}
