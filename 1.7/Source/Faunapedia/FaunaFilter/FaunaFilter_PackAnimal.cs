using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Faunapedia
{
    public class FaunaFilter_PackAnimal : FaunaFilter
    {
        public override bool FitsInFilter(ThingDef pawnDef)
        {
            if (pawnDef.race.packAnimal)
            {
                return true;
            }
            return false;
        }
    }
}
