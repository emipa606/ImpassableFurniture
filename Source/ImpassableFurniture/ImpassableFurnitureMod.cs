using System.Collections.Generic;
using System.Linq;
using Mlie;
using UnityEngine;
using Verse;

namespace ImpassableFurniture;

[StaticConstructorOnStartup]
internal class ImpassableFurnitureMod : Mod
{
    public static ImpassableFurnitureMod instance;

    private static string currentVersion;

    internal List<ThingDef> defaultList;

    private Vector2 leftScrollPosition;

    private ThingDef leftSelectedDef;

    private Vector2 rightScrollPosition;

    private ThingDef rightSelectedDef;

    private string searchTerm = "";

    private ImpassableSettings settings;

    public ImpassableFurnitureMod(ModContentPack content) : base(content)
    {
        instance = this;
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(
                ModLister.GetActiveModWithIdentifier("Mlie.ImpassableFurniture"));
    }

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

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        //Text.Font = GameFont.Medium;
        var rect = inRect.TopPart(0.05f);
        searchTerm = Widgets.TextField(rect.RightPart(0.95f).LeftPart(0.4f), searchTerm);
        if (currentVersion != null)
        {
            GUI.contentColor = Color.gray;
            Widgets.Label(rect.RightPart(0.45f).LeftPart(0.95f), "ImFu.ModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        var rect2 = inRect.TopPart(0.1f).BottomHalf();
        var rect3 = inRect.BottomPart(0.9f);
        var rect4 = rect3.LeftHalf().RightPart(0.9f).LeftPart(0.9f);
        GUI.BeginGroup(rect4, new GUIStyle(GUI.skin.box));
        var list = (from td in DefDatabase<ThingDef>.AllDefs
            where td.building != null && !td.label.Contains("(building)") &&
                  td.passability != Traversability.Impassable &&
                  (td.defName.Contains(searchTerm) || td.label.Contains(searchTerm)) &&
                  !Settings.enabledDefList.Contains(td) && td.modContentPack?.PackageId != "kentington.saveourship2"
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
        Widgets.Label(rect2.LeftHalf().RightPart(0.9f), "ImFu.Coloring".Translate());
        Widgets.Label(rect2.RightHalf().RightPart(0.9f), "ImFu.Impassability".Translate());
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
    }

    public override string SettingsCategory()
    {
        return "Impassable Furniture";
    }
}