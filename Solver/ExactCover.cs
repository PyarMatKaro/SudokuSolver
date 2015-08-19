using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solver
{
    public enum SolveResult { NoSolutions, SingleSolution, MultipleSolutions, Ongoing, TooDifficult };

    public abstract class ExactCover
    {
        bool verbose = false;

        protected Candidate[] tr;
        protected int trc;
        protected Tile[] ts;
        protected int tsc;

        protected Requirement requirements = new HeadRequirement();
        protected Requirement optionals = new HeadRequirement();

        protected void CreateCandidates(Candidate[] tr)
        {
            this.tr = tr;
            trc = 0;
        }

        protected void CreateSolution(int nsr)
        {
            ts = new Tile[nsr];
            tsc = 0;
        }

        protected void CoverRow(Tile r)
        {
            ts[tsc++] = r;
            for (Tile j = r.r; j != r; j = j.r)
                j.c.Cover();
        }

        protected void UncoverRow()
        {
            Tile r = ts[--tsc];
            for (Tile j = r.l; j != r; j = j.l)
                j.c.Uncover();
        }

        public bool Solved { get { return requirements.Right == requirements; } }

        public Requirement EasiestRequirement
        {
            get
            {
                Requirement c = requirements.Right;
                Requirement bc = null;
                while (c != requirements)
                {
                    if (bc == null || c.s <= bc.s)
                        bc = c;
                    c = c.Right;
                }
                return bc;
            }
        }

        public Requirement FollowSingleOptions()
        {
            while (true)
            {
                if (Solved)
                    return null;
                Requirement c = EasiestRequirement;
                if (c.s != 1)
                    return c;
                SelectCandidate(c.d.Candidate);
            }
        }

        public void CheckSelectCandidate(Candidate k, out Requirement c, out int o)
        {
            int sc = tsc;
            SelectCandidate(k);
            c = EasiestRequirement;
            o = 1;
            if (c != null)
                o = c.s;
            while (tsc > sc)
                UnselectCandidate();
        }

        public void CheckSelectCandidateFollowingSingleOptions(Candidate k, out Requirement c, out int o, out int steps)
        {
            int sc = tsc;
            if (k != null)
                SelectCandidate(k);
            c = FollowSingleOptions();
            steps = tsc - sc;
            o = 1;
            if (c != null)
                o = c.s;
            while (tsc > sc)
                UnselectCandidate();
        }

        public void CheckDiscardCandidateFollowingSingleOptions(Candidate k, out Requirement c, out int o)
        {
            if (k != null)
                DiscardCandidate(k);
            int sc = tsc;
            c = FollowSingleOptions();
            o = 1;
            if (c != null)
                o = c.s;
            while (tsc > sc)
                UnselectCandidate();
            UndiscardCandidate(k);
        }

        public abstract bool OnSolution();

        public bool BacktrackingSearch()
        {
            if (Solved)
                return OnSolution();

            Requirement c = EasiestRequirement;
            if (verbose)
                Console.WriteLine(c.ToString());
            int s = c.s;
            int i = 0;
            if (s == 0)
                return false;
            c.Cover();
            if (verbose && s != 1)
                Console.WriteLine("(start of loop)");
            bool brk = false;
            for (Tile r = c.d; r != c; r = r.d)
            {
                if (verbose && s != 1)
                    Console.WriteLine("(loop " + (++i) + "/" + s + ")");
                if (verbose)
                    Console.WriteLine(r.Candidate.ToString());
                CoverRow(r);
                brk = BacktrackingSearch();
                UncoverRow();
                if (brk)
                    break;
            }
            c.Uncover();
            if (verbose && s != 1)
                Console.WriteLine("(end of loop)");
            return brk;
        }

        public Requirement[] UnfulfilledRequirements
        {
            get
            {
                int n = 0;
                for (Requirement c = requirements.Right; c != requirements; c = c.Right)
                    ++n;
                Requirement[] ret = new Requirement[n];
                for (Requirement c = requirements.Right; c != requirements; c = c.Right)
                    ret[--n] = c;
                return ret;
            }
        }

        public Candidate[] UnselectedCandidates
        {
            get
            {
                List<Candidate> ret = new List<Candidate>();
                foreach (Requirement c in UnfulfilledRequirements)
                    foreach (Candidate k in c.UnselectedCandidates)
                        if (!k.IsMarked)
                        {
                            k.MarkCandidate(true);
                            ret.Add(k);
                        }
                foreach (Candidate k in ret)
                    k.MarkCandidate(false);
                return ret.ToArray();
            }
        }

        public Candidate[] SelectedCandidates
        {
            get
            {
                Candidate[] ret = new Candidate[tsc];
                for (int n = 0; n < tsc; ++n)
                    ret[n] = ts[n].Candidate;
                return ret;
            }
        }

        public void SelectCandidate(Candidate k)
        {
            Tile r = k.Tile;
            r.c.Cover();
            CoverRow(r);
        }

        public void UnselectCandidate()
        {
            Tile r = ts[tsc - 1];
            UncoverRow();
            r.c.Uncover();
        }

        public bool TrySelectCandidate(Candidate k)
        {
            Tile r = k.Tile;
            // Check that candidate is unselected
            if (!UnfulfilledRequirements.Contains(r.c))
                return false;
            if (!r.c.UnselectedCandidates.Contains(k))
                return false;
            SelectCandidate(k);
            return true;
        }

        public void DiscardCandidate(Candidate k)
        {
            Tile t = k.Tile;
            t.ExcludeV();
            for (Tile j = t.r; j != t; j = j.r)
                j.ExcludeV();
        }

        public void UndiscardCandidate(Candidate k)
        {
            Tile t = k.Tile;
            for (Tile j = t.l; j != t; j = j.l)
                j.IncludeV();
            t.IncludeV();
        }

        #region hints

        public Hint[] HintsToPaint(HintFlags ho, HintSelections hs)
        {
            List<Hint> ret = new List<Hint>();
            Requirement c = EasiestRequirement;
            if (c != null)
            {
                if (c.s == 0)
                    return new Hint[] { new ImpossibleHint(c) };
                if (c.s == 1 && hs.ForcedMoves)
                    return new Hint[] { new ForcedMoveHint(c, c.d.Candidate) };
                //ret.Add( new ForcedMoveHint(c, c.d) );
            }
            Candidate[] uc = UnselectedCandidates;
            DiscardableHint[] eventualDiscardableHints;
            EventualSolutionHint[] eventualSolutionHints;
            SelectableHint[] selectableHints;
            BuildEventualHints(out eventualDiscardableHints, out eventualSolutionHints,
                out selectableHints);
            if (hs.EventualDiscardables)
                ret.AddRange(eventualDiscardableHints);
            else if (hs.ImmediateDiscardables)
                ret.AddRange(ImmediateDiscardableHints);
            if (hs.EventualSolutions)
                ret.AddRange(eventualSolutionHints);
            if (hs.Selectables)
                ret.AddRange(selectableHints);
            return ret.ToArray();
        }

        public ImpossibleHint SingleImpossibleHint
        {
            get
            {
                Requirement c = EasiestRequirement;
                if (c != null)
                    if (c.s == 0)
                        return new ImpossibleHint(c);
                return null;
            }
        }

        public ForcedMoveHint SingleForcedHint
        {
            get
            {
                Requirement c = EasiestRequirement;
                if (c != null)
                    if (c.s == 1)
                        return new ForcedMoveHint(c, c.d.Candidate);
                return null;
            }
        }

        public Requirement[] RestrictedRequirements(Tile t, int s)
        {
            int n = 0;
            for (Tile i = t.r; i != t; i = i.r)
                if (i.c.s == s)
                    ++n;
            if (t.c.s == s)
                ++n;
            Requirement[] ret = new Requirement[n];
            for (Tile i = t.r; i != t; i = i.r)
                if (i.c.s == s)
                    ret[--n] = i.c;
            if (t.c.s == s)
                ret[--n] = t.c;
            return ret;
        }

        public Hint SingleHint(HintSelections hs)
        {
            Requirement c = EasiestRequirement;
            if (c != null)
            {
                if (c.s == 0)
                    return new ImpossibleHint(c);
                if (c.s == 1 && hs.ForcedMoves)
                    return new ForcedMoveHint(c, c.d.Candidate);
            }

            Candidate[] uc = UnselectedCandidates;
            if (hs.ImmediateDiscardables)
                foreach (Candidate k in uc)
                {
                    int o;
                    CheckSelectCandidate(k, out c, out o);
                    if (o == 0)
                        return new DiscardableHint(k, c, false);
                }
            foreach (Candidate k in uc)
            {
                int o, steps;
                CheckSelectCandidateFollowingSingleOptions(k, out c, out o, out steps);
                if (hs.EventualDiscardables && o == 0)
                    return new DiscardableHint(k, c, steps > 1);
                if (hs.EventualSolutions && o == 1)
                    return new EventualSolutionHint(k);
                if (hs.Selectables)
                {
                    CheckDiscardCandidateFollowingSingleOptions(k, out c, out o);
                    if (o == 0)
                        return new SelectableHint(k, c);
                }
            }
            return null;
        }

        public DiscardableHint[] ImmediateDiscardableHints
        {
            get
            {
                List<DiscardableHint> ret = new List<DiscardableHint>();
                foreach (Candidate k in UnselectedCandidates)
                {
                    Requirement c;
                    int o;
                    CheckSelectCandidate(k, out c, out o);
                    if (o == 0)
                        ret.Add(new DiscardableHint(k, c, false));
                }
                return ret.ToArray();
            }
        }

        public void BuildEventualHints(
            out DiscardableHint[] eventualDiscardableHints,
            out EventualSolutionHint[] eventualSolutionHints,
            out SelectableHint[] selectableHints)
        {
            List<DiscardableHint> ret0 = new List<DiscardableHint>();
            List<EventualSolutionHint> ret1 = new List<EventualSolutionHint>();
            List<SelectableHint> ret2 = new List<SelectableHint>();
            foreach (Candidate k in UnselectedCandidates)
            {
                Requirement c;
                int o, steps;
                CheckSelectCandidateFollowingSingleOptions(k, out c, out o, out steps);
                if (o == 0)
                    ret0.Add(new DiscardableHint(k, c, steps > 1));
                if (o == 1)
                    ret1.Add(new EventualSolutionHint(k));
                CheckDiscardCandidateFollowingSingleOptions(k, out c, out o);
                if (o == 0)
                    ret2.Add(new SelectableHint(k, c));
            }
            eventualDiscardableHints = ret0.ToArray();
            eventualSolutionHints = ret1.ToArray();
            selectableHints = ret2.ToArray();
        }

        public ImpossibleHint[] ImpossibleHints
        {
            get
            {
                int n = 0;
                for (Requirement c = requirements.Right; c != requirements; c = c.Right)
                    if (c.s < 1)
                        ++n;
                ImpossibleHint[] ret = new ImpossibleHint[n];
                for (Requirement c = requirements.Right; c != requirements; c = c.Right)
                    if (c.s < 1)
                        ret[--n] = new ImpossibleHint(c);
                return ret;
            }
        }

        public ForcedMoveHint[] ForcedHints
        {
            get
            {
                int n = 0;
                for (Requirement c = requirements.Right; c != requirements; c = c.Right)
                    if (c.s == 1)
                        ++n;
                ForcedMoveHint[] ret = new ForcedMoveHint[n];
                for (Requirement c = requirements.Right; c != requirements; c = c.Right)
                    if (c.s == 1)
                        ret[--n] = new ForcedMoveHint(c, c.d.Candidate);
                return ret;
            }
        }

        #endregion

    }
}
