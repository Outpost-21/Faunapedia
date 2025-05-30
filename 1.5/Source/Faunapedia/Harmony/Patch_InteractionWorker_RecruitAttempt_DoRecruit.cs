﻿using HarmonyLib;
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
    [HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), "DoRecruit", new Type[] { typeof(Pawn), typeof(Pawn), typeof(string), typeof(string), typeof(bool), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal })]
    public static class Patch_InteractionWorker_RecruitAttempt_DoRecruit
    {
        public static void Postfix(Pawn recruitee)
        {
            if (recruitee != null)
            {
                if (recruitee.def.IsFaunapediaAnimal())
                {
                    FindUtil.AnimalTracking.MarkAnimalAsTamed(recruitee.def);
                }
            }
        }
    }
}
