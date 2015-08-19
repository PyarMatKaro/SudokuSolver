using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Solver
{
    public class Candidate
    {
        Tile tile;
        private bool marked = false;
        
        public void AddCandidate(Requirement c)
        {
            Tile t = new Tile();
            t.k = this;
            if (tile == null)
            {
                tile = t;
            }
            else
            {
                tile.r.l = t;
                t.r = tile.r;
                tile.r = t;
                t.l = tile;
            }
            t.c = c;
            c.s++;
            t.d = c.d;
            c.d.u = t;
            c.d = t;
            t.u = c;
            t.included = true;
        }

        internal void MarkCandidate(bool mark)
        {
            marked = mark;
        }

        public bool IsMarked { get { return marked; } }

        public virtual void PaintForeground(HintPainter context, Brush br) { }
        public virtual void PaintBackground(HintPainter context, Brush br) { }

        public Tile Tile { get { return tile; } }
    }
}
