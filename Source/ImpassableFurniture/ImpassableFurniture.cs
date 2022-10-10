using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ImpassableFurniture;

[StaticConstructorOnStartup]
internal static class ImpassableFurniture
{
    static ImpassableFurniture()
    {
        ImpassableFurnitureMod.instance.defaultList = (from td in DefDatabase<ThingDef>.AllDefsListForReading
            where td.building != null && td.passability == Traversability.Impassable &&
                  td.modContentPack?.PackageId != "kentington.saveourship2"
            select td).ToList();

        if (ImpassableFurnitureMod.instance.Settings.enabledDefList.NullOrEmpty())
        {
            ImpassableFurnitureMod.instance.Settings.enabledDefList =
                (from td in DefDatabase<ThingDef>.AllDefsListForReading
                    where td.building != null && td.passability == Traversability.Impassable &&
                          td.modContentPack?.PackageId != "kentington.saveourship2"
                    select td).ToList();
            return;
        }

        DefDatabase<ThingDef>.AllDefsListForReading.ForEach(delegate(ThingDef td)
        {
            if (td.building != null && td.passability == Traversability.Impassable &&
                td.modContentPack?.PackageId != "kentington.saveourship2")
            {
                RemovePassability(td);
            }
        });
        ImpassableFurnitureMod.instance.Settings.enabledDefList.ForEach(AddPassability);
    }

    public static void AddPassability(ThingDef def)
    {
        def.passability = Traversability.Impassable;
        Regenerate();
    }

    public static void AddPassability(List<ThingDef> defList)
    {
        foreach (var def in defList)
        {
            def.passability = Traversability.Impassable;
        }

        Regenerate();
    }

    public static void RemovePassability(ThingDef def)
    {
        if (def.passability != Traversability.Impassable)
        {
            return;
        }

        def.passability = Traversability.PassThroughOnly;
        Regenerate();
    }

    public static void RemovePassability(List<ThingDef> defList)
    {
        foreach (var def in defList)
        {
            if (def.passability != Traversability.Impassable)
            {
                continue;
            }

            def.passability = Traversability.PassThroughOnly;
        }

        Regenerate();
    }

    private static void Regenerate()
    {
        if (Current.ProgramState != ProgramState.Playing)
        {
            return;
        }

        foreach (var map in Find.Maps)
        {
            map.pathing.RecalculateAllPerceivedPathCosts();
            map.regionAndRoomUpdater.RebuildAllRegionsAndRooms();
        }
    }
}