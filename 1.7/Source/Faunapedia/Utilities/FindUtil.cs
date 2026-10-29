using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Faunapedia
{
    public static class FindUtil
    {
        public static GameComp_AnimalTracking AnimalTracking
        {
            get
            {
                return Current.Game.GetComponent<GameComp_AnimalTracking>();
            }
        }
    }
}
