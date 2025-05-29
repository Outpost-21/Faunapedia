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
        public bool hideLabels = false;
        public bool unknownSilhouettes = true;
        public bool groupBySource = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref unlockedByDefault, "unlockedByDefault", false);
            Scribe_Values.Look(ref unlockedBySighting, "unlockedBySighting", true);
            Scribe_Values.Look(ref unlockedByTaming, "unlockedByTaming", true);

            Scribe_Values.Look(ref unknownShown, "unknownShown", true);
            Scribe_Values.Look(ref hideLabels, "hideLabels", false);
            Scribe_Values.Look(ref unknownSilhouettes, "unknownSilhouettes", true);
            Scribe_Values.Look(ref groupBySource, "groupBySource", false);
        }
    }
}
