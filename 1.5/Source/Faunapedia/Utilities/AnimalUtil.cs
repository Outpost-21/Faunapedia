using RimWorld;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Faunapedia
{
    public static class AnimalUtil
    {
        public static bool IsFaunapediaAnimal(this ThingDef def)
        {
            if(def.race != null && def.race.Animal && def.race.IsFlesh && !def.IsCorpse)
            {
                return true;
            }
            return false;
        }

        public static List<ThingDef> GetListableAnimals()
        {
            List<ThingDef> results = new List<ThingDef>();
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.IsFaunapediaAnimal())
                {
                    if(FaunapediaMod.settings.unknownShown || def.IsKnownAnimal())
                    {
                        results.Add(def);
                    }
                }
            }
            return results;
        }

        public static bool IsKnownAnimal(this ThingDef thingDef)
        {
            if (FaunapediaMod.settings.unlockedByDefault 
                || (FaunapediaMod.settings.unlockedBySighting && FindUtil.AnimalTracking.animalsSeen[thingDef]) 
                || (FaunapediaMod.settings.unlockedByTaming && FindUtil.AnimalTracking.animalsTamed[thingDef]))
            {
                return true;
            }
            return false;
        }
    }
}
