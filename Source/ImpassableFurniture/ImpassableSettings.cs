using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ImpassableFurniture
{
    // Token: 0x02000002 RID: 2
    internal class ImpassableSettings : ModSettings
    {
        // Token: 0x04000001 RID: 1
        public List<ThingDef> enabledDefList = new List<ThingDef>();

        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public override void ExposeData()
        {
            base.ExposeData();
            var list = enabledDefList;
            List<string> list2;
            if (list == null)
            {
                list2 = null;
            }
            else
            {
                list2 = (from td in list
                    select td.defName).ToList();
            }

            var source = list2 ?? new List<string>();
            Scribe_Collections.Look(ref source, "enabledDefList");
            enabledDefList = (from td in source.Select(DefDatabase<ThingDef>.GetNamedSilentFail)
                where td != null && td.modContentPack?.PackageId != "kentington.saveourship2"
                select td).ToList();
        }
    }
}