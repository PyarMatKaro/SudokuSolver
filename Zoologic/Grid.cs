using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solver;

namespace Zoologic
{
    public enum K { A, G, B, M, H, C, F, I, X, O };

    public class Grid : ExactCover
    {
        public Dictionary<Pt, K> pt2kind = new Dictionary<Pt, K>();
        public Dictionary<Pt, ReqP> pt2req = new Dictionary<Pt, ReqP>();
        public Dictionary<K, ReqK> kind2req = new Dictionary<K, ReqK>();
        public Dictionary<PtKind, Can> ptKind2can = new Dictionary<PtKind, Can>();
        public Dictionary<PtKind, ReqPK> ptKind2req = new Dictionary<PtKind, ReqPK>();

        public override bool OnSolution()
        {
            return true;
        }

        public Grid()
        {
            // GGACCMBBBF

            pt2kind[new Pt(2, 0)] = K.O;

            pt2kind[new Pt(0, 1)] = K.O;
            pt2kind[new Pt(1, 1)] = K.I;
            pt2kind[new Pt(2, 1)] = K.I;
            pt2kind[new Pt(3, 1)] = K.O;
            pt2kind[new Pt(4, 1)] = K.C;

            pt2kind[new Pt(1, 2)] = K.O;
            pt2kind[new Pt(2, 2)] = K.O;
            pt2kind[new Pt(3, 2)] = K.O;

            pt2kind[new Pt(1, 3)] = K.O;
            pt2kind[new Pt(2, 3)] = K.O;
            pt2kind[new Pt(3, 3)] = K.H;

            /*
            pt2kind[new Pt(1, 0)] = K.O;
            pt2kind[new Pt(2, 0)] = K.B;

            pt2kind[new Pt(1, 1)] = K.G;
            pt2kind[new Pt(2, 1)] = K.O;
            pt2kind[new Pt(3, 1)] = K.H;

            pt2kind[new Pt(1, 2)] = K.G;
            pt2kind[new Pt(2, 2)] = K.O;

            pt2kind[new Pt(0, 3)] = K.H;
            pt2kind[new Pt(1, 3)] = K.H;
            pt2kind[new Pt(2, 3)] = K.G;

            pt2kind[new Pt(1, 4)] = K.O;
            pt2kind[new Pt(2, 4)] = K.M;
            */

            // Requirements, place tile on each cell
            foreach (Pt p in pt2kind.Keys)
            {
                ReqP r = new ReqP(p);
                pt2req[p] = r;
                requirements.AddRequirement(r);
            }

            // Requirements, use the available kinds GGACCMBBBF
            kind2req[K.G] = new ReqK(K.G, 2);
            kind2req[K.A] = new ReqK(K.A, 1);
            kind2req[K.C] = new ReqK(K.C, 2);
            kind2req[K.M] = new ReqK(K.M, 1);
            kind2req[K.B] = new ReqK(K.B, 3);
            kind2req[K.F] = new ReqK(K.F, 1);

            // Optionals, either an angry dog in a cell or dogs in its neighbours
            foreach (Pt p in pt2kind.Keys)
                foreach (K k in new K[]{
                    K.A,//angry dog, not next to dog
                    K.G,//dog, not next to bone
                    K.C,//cat, not next to dog or fish
                    K.I,//ants, not under food
                    K.X//bull, not under animal
                })
                {
                    PtKind p2 = new PtKind(p, k);
                    ReqPK r = new ReqPK(p, k);
                    ptKind2req[p2] = r;
                    optionals.AddRequirement(r);
                }

            foreach (Pt p in pt2kind.Keys)
                foreach (K k in new K[] { K.G, K.B, K.M, K.H })
                    if (k != K.O && k != K.I && k != K.X)
                    {
                        PtKind p2 = new PtKind(p, k);
                        Can ca = new Can(p2);
                        ptKind2can[p2] = ca;
                        ca.AddCandidate(pt2req[p]);
                        ca.AddCandidate(kind2req[k]);
                        for(int x=-1;x<=1;++x)
                            for (int y = -1; y <= 1; ++y)
                                if((x!=0||y!=0)&&(x==0||y==0))
                            {
                                    Pt pt2 = new Pt(p.X+x,p.Y+y);
                                    if (pt2kind.ContainsKey(pt2) && pt2kind[pt2] == K.O)
                                    {
                                    }
                            }
                    }

        }
    }
}
