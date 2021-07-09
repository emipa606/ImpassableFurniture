using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ImpassableFurniture
{
    // Token: 0x02000003 RID: 3
    [StaticConstructorOnStartup]
    internal class ImpassableFurnitureMod : Mod
    {
        // Token: 0x04000002 RID: 2
        public static ImpassableFurnitureMod instance;

        internal List<ThingDef> defaultList;

        // Token: 0x04000004 RID: 4
        private Vector2 leftScrollPosition;

        // Token: 0x04000007 RID: 7
        private ThingDef leftSelectedDef;

        // Token: 0x04000005 RID: 5
        private Vector2 rightScrollPosition;

        // Token: 0x04000008 RID: 8
        private ThingDef rightSelectedDef;

        // Token: 0x04000006 RID: 6
        private string searchTerm = "";

        // Token: 0x04000003 RID: 3
        private ImpassableSettings settings;

        // Token: 0x06000003 RID: 3 RVA: 0x0000210A File Offset: 0x0000030A
        public ImpassableFurnitureMod(ModContentPack content) : base(content)
        {
            instance = this;
        }

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000004 RID: 4 RVA: 0x00002124 File Offset: 0x00000324
        // (set) Token: 0x06000005 RID: 5 RVA: 0x0000214A File Offset: 0x0000034A
        internal ImpassableSettings Settings
        {
            get
            {
                ImpassableSettings result;
                if ((result = settings) == null)
                {
                    result = settings = GetSettings<ImpassableSettings>();
                }

                return result;
            }
            set => settings = value;
        }

        // Token: 0x06000006 RID: 6 RVA: 0x00002154 File Offset: 0x00000354
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Text.Font = GameFont.Medium;
            var rect = inRect.TopPart(0.05f);
            searchTerm = Widgets.TextField(rect.RightPart(0.95f).LeftPart(0.95f), searchTerm);
            var rect2 = inRect.TopPart(0.1f).BottomHalf();
            var rect3 = inRect.BottomPart(0.9f);
            var rect4 = rect3.LeftHalf().RightPart(0.9f).LeftPart(0.9f);
            GUI.BeginGroup(rect4, new GUIStyle(GUI.skin.box));
            var list = (from td in DefDatabase<ThingDef>.AllDefs
                where td.building != null && !td.label.Contains("(building)") &&
                      td.passability != Traversability.Impassable &&
                      (td.defName.Contains(searchTerm) || td.label.Contains(searchTerm)) &&
                      !Settings.enabledDefList.Contains(td)
                orderby td.LabelCap.RawText ?? td.defName
                select td).ToList();
            var num = 3f;
            Widgets.BeginScrollView(rect4.AtZero(), ref leftScrollPosition,
                new Rect(0f, 0f, rect4.width / 10f * 9f, list.Count * 32f));
            if (!list.NullOrEmpty())
            {
                foreach (var thingDef in list)
                {
                    var rect5 = new Rect(5f, num, rect4.width - 6f, 30f);
                    Widgets.DrawHighlightIfMouseover(rect5);
                    if (thingDef == leftSelectedDef)
                    {
                        Widgets.DrawHighlightSelected(rect5);
                    }

                    Widgets.Label(rect5, thingDef.LabelCap.RawText ?? thingDef.defName);
                    if (Widgets.ButtonInvisible(rect5, false))
                    {
                        leftSelectedDef = thingDef;
                    }

                    num += 32f;
                }
            }

            Widgets.EndScrollView();
            GUI.EndGroup();
            Widgets.Label(rect2.LeftHalf().RightPart(0.9f), "Green: All, Yellow: Default, Red: None");
            Widgets.Label(rect2.RightHalf().RightPart(0.9f), "Enable Impassability for:");
            var rect6 = rect3.RightHalf().RightPart(0.9f).LeftPart(0.9f);
            GUI.BeginGroup(rect6, GUI.skin.box);
            num = 6f;
            Widgets.BeginScrollView(rect6.AtZero(), ref rightScrollPosition,
                new Rect(0f, 0f, rect6.width / 5f * 4f, Settings.enabledDefList.Count * 32f));
            if (!Settings.enabledDefList.NullOrEmpty())
            {
                foreach (var thingDef2 in from def in Settings.enabledDefList
                    where def.defName.Contains(searchTerm) || def.label.Contains(searchTerm)
                    select def)
                {
                    var rect7 = new Rect(5f, num, rect4.width - 6f, 30f);
                    Widgets.DrawHighlightIfMouseover(rect7);
                    if (thingDef2 == rightSelectedDef)
                    {
                        Widgets.DrawHighlightSelected(rect7);
                    }

                    Widgets.Label(rect7, thingDef2.LabelCap.RawText ?? thingDef2.defName);
                    if (Widgets.ButtonInvisible(rect7, false))
                    {
                        rightSelectedDef = thingDef2;
                    }

                    num += 32f;
                }
            }

            Widgets.EndScrollView();
            GUI.EndGroup();
            if (Widgets.ButtonImage(rect3.BottomPart(0.7f).TopPart(0.08f).RightPart(0.525f).LeftPart(0.1f),
                TexUI.ArrowTexRight, Color.green))
            {
                leftSelectedDef = null;
                ImpassableFurniture.AddPassability(list);
                foreach (var def in list)
                {
                    Settings.enabledDefList.Add(def);
                }
            }

            if (Widgets.ButtonImage(rect3.BottomPart(0.6f).TopPart(0.1f).RightPart(0.525f).LeftPart(0.1f),
                TexUI.ArrowTexRight) && leftSelectedDef != null)
            {
                Settings.enabledDefList.Add(leftSelectedDef);
                Settings.enabledDefList = (from td in Settings.enabledDefList
                    orderby td.LabelCap.RawText ?? td.defName
                    select td).ToList();
                rightSelectedDef = leftSelectedDef;
                leftSelectedDef = null;
                ImpassableFurniture.AddPassability(rightSelectedDef);
            }

            if (Widgets.ButtonImage(rect3.BottomPart(0.5f).TopPart(0.12f).RightPart(0.525f).LeftPart(0.1f),
                TexUI.ArrowTex, Color.yellow))
            {
                leftSelectedDef = null;
                rightSelectedDef = null;
                ImpassableFurniture.RemovePassability(Settings.enabledDefList);
                ImpassableFurniture.AddPassability(defaultList);
                Settings.enabledDefList = new List<ThingDef>();
                foreach (var def in defaultList)
                {
                    Settings.enabledDefList.Add(def);
                }
            }

            if (Widgets.ButtonImage(rect3.BottomPart(0.4f).TopPart(0.15f).RightPart(0.525f).LeftPart(0.1f),
                TexUI.ArrowTexLeft) && rightSelectedDef != null)
            {
                Settings.enabledDefList.Remove(rightSelectedDef);
                leftSelectedDef = rightSelectedDef;
                rightSelectedDef = null;
                ImpassableFurniture.RemovePassability(leftSelectedDef);
            }

            if (Widgets.ButtonImage(rect3.BottomPart(0.3f).TopPart(0.17f).RightPart(0.525f).LeftPart(0.1f),
                TexUI.ArrowTexLeft, Color.red))
            {
                rightSelectedDef = null;
                ImpassableFurniture.RemovePassability(Settings.enabledDefList);
                Settings.enabledDefList = new List<ThingDef>();
            }

            Settings.Write();
        }

        // Token: 0x06000007 RID: 7 RVA: 0x000025DC File Offset: 0x000007DC
        public override string SettingsCategory()
        {
            return "Impassable Furniture";
        }
    }
}