using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Faunapedia
{
    public class FaunaFilter_Dryad : FaunaFilter
    {
        public override bool FitsInFilter(ThingDef pawnDef)
        {
            return pawnDef.race.Dryad;
        }
    }
}
