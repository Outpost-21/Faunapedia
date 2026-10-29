using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Faunapedia
{
    public class FaunaFilterDef : Def
    {
        public Type filterClass = typeof(FaunaFilter);
        public FaunaFilter filterClassInt;
        public FaunaFilter Worker
        {
            get
            {
                if (filterClassInt == null)
                {
                    filterClassInt = (FaunaFilter)Activator.CreateInstance(filterClass);
                    filterClassInt.def = this;
                }
                return filterClassInt;
            }
        }

        public Type compClass = typeof(ThingComp);
    }
}
