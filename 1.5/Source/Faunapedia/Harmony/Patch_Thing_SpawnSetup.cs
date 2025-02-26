using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Faunapedia
{
    [HarmonyPatch(typeof(Thing), "SpawnSetup")]
    public static class Patch_Thing_SpawnSetup
    {
        public static void Postfix (Thing __instance)
        {
            if(__instance as Pawn != null)
            {
                Pawn pawn = __instance as Pawn;
                if(pawn.RaceProps.Animal && pawn.RaceProps.IsFlesh)
                {
                    FindUtil.AnimalTracking.MarkAnimalAsSeen(pawn.def);
                }
            }
        }
    }
}
