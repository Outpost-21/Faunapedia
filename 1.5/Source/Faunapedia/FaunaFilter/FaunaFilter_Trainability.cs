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
    public class FaunaFilter_TrainabilityNone : FaunaFilter
    {
        public override bool FitsInFilter(ThingDef pawnDef)
        {
            return pawnDef.race.trainability == TrainabilityDefOf.None;
        }
    }

    public class FaunaFilter_TrainabilityIntermediate : FaunaFilter
    {
        public override bool FitsInFilter(ThingDef pawnDef)
        {
            return pawnDef.race.trainability == TrainabilityDefOf.Intermediate;
        }
    }

    public class FaunaFilter_TrainabilityAdvanced : FaunaFilter
    {
        public override bool FitsInFilter(ThingDef pawnDef)
        {
            return pawnDef.race.trainability == TrainabilityDefOf.Advanced;
        }
    }
}
