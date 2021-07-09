using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ImpassableFurniture
{
    // Token: 0x02000004 RID: 4
    [StaticConstructorOnStartup]
    internal static class ImpassableFurniture
    {
        // Token: 0x0600000A RID: 10 RVA: 0x00002678 File Offset: 0x00000878
        static ImpassableFurniture()
        {
            ImpassableFurnitureMod.instance.defaultList = (from td in DefDatabase<ThingDef>.AllDefsListForReading
                where td.building != null && td.passability == Traversability.Impassable
                select td).ToList();

            if (ImpassableFurnitureMod.instance.Settings.enabledDefList.NullOrEmpty())
            {
                ImpassableFurnitureMod.instance.Settings.enabledDefList =
                    (from td in DefDatabase<ThingDef>.AllDefsListForReading
                        where td.building != null && td.passability == Traversability.Impassable
                        select td).ToList();
                return;
            }

            DefDatabase<ThingDef>.AllDefsListForReading.ForEach(delegate(ThingDef td)
            {
                if (td.building != null && td.passability == Traversability.Impassable)
                {
                    RemovePassability(td);
                }
            });
            ImpassableFurnitureMod.instance.Settings.enabledDefList.ForEach(AddPassability);
        }

        // Token: 0x0600000B RID: 11 RVA: 0x00002704 File Offset: 0x00000904
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

        // Token: 0x0600000C RID: 12 RVA: 0x00002712 File Offset: 0x00000912
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

        // Token: 0x0600000D RID: 13 RVA: 0x0000272C File Offset: 0x0000092C
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
}