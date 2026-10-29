using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Faunapedia
{
    public abstract class FaunaFilter
    {
        public FaunaFilterDef def;

        public virtual bool FitsInFilter(ThingDef pawnDef)
        {
            return false;
        }
    }
}
