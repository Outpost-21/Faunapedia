using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Faunapedia
{
    public class FaunaFilter_Comp : FaunaFilter
    {
        public override bool FitsInFilter(ThingDef pawnDef)
        {
            if (pawnDef.comps.Any(c => c.compClass == def.compClass))
            {
                return true;
            }
            return false;
        }
    }
}
