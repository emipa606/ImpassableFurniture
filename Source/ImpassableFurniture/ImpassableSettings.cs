using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ImpassableFurniture;

internal class ImpassableSettings : ModSettings
{
    public List<ThingDef> EnabledDefList = [];

    public override void ExposeData()
    {
        base.ExposeData();
        var list = EnabledDefList;
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

        var source = list2 ?? [];
        Scribe_Collections.Look(ref source, "enabledDefList");
        EnabledDefList = (from td in source.Select(DefDatabase<ThingDef>.GetNamedSilentFail)
            where td != null && td.modContentPack?.PackageId != "kentington.saveourship2"
            select td).ToList();
    }
}