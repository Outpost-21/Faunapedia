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
    public class GameComp_AnimalTracking : GameComponent
    {
        public Dictionary<ThingDef, bool> animalsSeen = new Dictionary<ThingDef, bool>();
        public Dictionary<ThingDef, bool> animalsTamed = new Dictionary<ThingDef, bool>();

        public GameComp_AnimalTracking(Game game)
        {
            animalsSeen = new Dictionary<ThingDef, bool>();
            animalsTamed = new Dictionary<ThingDef, bool>();
            foreach(ThingDef def in DefDatabase<ThingDef>.AllDefs.Where(d => d.race != null && d.race.Animal && d.race.IsFlesh).ToList())
            {
                animalsSeen.Add(def, false);
                animalsTamed.Add(def, false);
            }
        }

        public void MarkAnimalAsSeen(ThingDef thingDef)
        {
            animalsSeen[thingDef] = true;
        }

        public void MarkAnimalAsTamed(ThingDef thingDef) 
        {
            animalsTamed[thingDef] = true;
        }
    }
}
