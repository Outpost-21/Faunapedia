using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Faunapedia
{
    public class FaunapediaSettings : ModSettings
    {
        public bool verboseLogging = false;

        public bool unlockedByDefault = false;
        public bool unlockedBySighting = true;
        public bool unlockedByTaming = true;

        public bool unknownShown = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref unlockedByDefault, "unlockedByDefault", false);
            Scribe_Values.Look(ref unlockedBySighting, "unlockedBySighting", true);
            Scribe_Values.Look(ref unlockedByTaming, "unlockedByTaming", true);

            Scribe_Values.Look(ref unknownShown, "unknownShown", true);
        }
    }
}
