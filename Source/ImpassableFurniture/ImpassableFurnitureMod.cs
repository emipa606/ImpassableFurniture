using System;
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
		// Token: 0x06000003 RID: 3 RVA: 0x0000210A File Offset: 0x0000030A
		public ImpassableFurnitureMod(ModContentPack content) : base(content)
		{
			ImpassableFurnitureMod.instance = this;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002124 File Offset: 0x00000324
		// (set) Token: 0x06000005 RID: 5 RVA: 0x0000214A File Offset: 0x0000034A
		internal ImpassableSettings Settings
		{
			get
			{
				ImpassableSettings result;
				if ((result = this.settings) == null)
				{
					result = (this.settings = base.GetSettings<ImpassableSettings>());
				}
				return result;
			}
			set
			{
				this.settings = value;
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002154 File Offset: 0x00000354
		public override void DoSettingsWindowContents(Rect inRect)
		{
			base.DoSettingsWindowContents(inRect);
			Text.Font = GameFont.Medium;
			Rect rect = GenUI.TopPart(inRect, 0.05f);
            this.searchTerm = Widgets.TextField(GenUI.LeftPart(GenUI.RightPart(rect, 0.95f), 0.95f), this.searchTerm);
			Rect rect2 = GenUI.BottomHalf(GenUI.TopPart(inRect, 0.1f));
			Rect rect3 = GenUI.BottomPart(inRect, 0.9f);
			Rect rect4 = GenUI.LeftPart(GenUI.RightPart(GenUI.LeftHalf(rect3), 0.9f), 0.9f);
			GUI.BeginGroup(rect4, new GUIStyle(GUI.skin.box));
			List<ThingDef> list = (from td in DefDatabase<ThingDef>.AllDefs
			where td.building != null && !td.label.Contains("(building)") && td.passability != Traversability.Impassable && (td.defName.Contains(this.searchTerm) || td.label.Contains(this.searchTerm)) && !this.Settings.enabledDefList.Contains(td)
			orderby td.LabelCap.RawText ?? td.defName
			select td).ToList<ThingDef>();
			float num = 3f;
			Widgets.BeginScrollView(GenUI.AtZero(rect4), ref this.leftScrollPosition, new Rect(0f, 0f, rect4.width / 10f * 9f, (float)list.Count * 32f), true);
			if (!list.NullOrEmpty<ThingDef>())
			{
				foreach (ThingDef thingDef in list)
				{
					Rect rect5 = new Rect(5f, num, rect4.width - 6f, 30f);
					Widgets.DrawHighlightIfMouseover(rect5);
					if (thingDef == this.leftSelectedDef)
					{
						Widgets.DrawHighlightSelected(rect5);
					}
					Widgets.Label(rect5, thingDef.LabelCap.RawText ?? thingDef.defName);
					if (Widgets.ButtonInvisible(rect5, false))
					{
						this.leftSelectedDef = thingDef;
					}
					num += 32f;
				}
			}
			Widgets.EndScrollView();
			GUI.EndGroup();
            Widgets.Label(GenUI.RightPart(GenUI.LeftHalf(rect2), 0.9f), "Green: All, Yellow: Default, Red: None");
			Widgets.Label(GenUI.RightPart(GenUI.RightHalf(rect2), 0.9f), "Enable Impassability for:");
			Rect rect6 = GenUI.LeftPart(GenUI.RightPart(GenUI.RightHalf(rect3), 0.9f), 0.9f);
			GUI.BeginGroup(rect6, GUI.skin.box);
			num = 6f;
			Widgets.BeginScrollView(GenUI.AtZero(rect6), ref this.rightScrollPosition, new Rect(0f, 0f, rect6.width / 5f * 4f, (float)this.Settings.enabledDefList.Count * 32f), true);
			if (!this.Settings.enabledDefList.NullOrEmpty<ThingDef>())
			{
				foreach (ThingDef thingDef2 in from def in this.Settings.enabledDefList
				where def.defName.Contains(this.searchTerm) || def.label.Contains(this.searchTerm)
				select def)
				{
					Rect rect7 = new Rect(5f, num, rect4.width - 6f, 30f);
					Widgets.DrawHighlightIfMouseover(rect7);
					if (thingDef2 == this.rightSelectedDef)
					{
						Widgets.DrawHighlightSelected(rect7);
					}
					Widgets.Label(rect7, thingDef2.LabelCap.RawText ?? thingDef2.defName);
					if (Widgets.ButtonInvisible(rect7, false))
					{
						this.rightSelectedDef = thingDef2;
					}
					num += 32f;
				}
			}
			Widgets.EndScrollView();
			GUI.EndGroup();
            if (Widgets.ButtonImage(GenUI.LeftPart(GenUI.RightPart(GenUI.TopPart(GenUI.BottomPart(rect3, 0.7f), 0.08f), 0.525f), 0.1f), TexUI.ArrowTexRight, Color.green))
            {
                this.leftSelectedDef = null;
                ImpassableFurniture.AddPassability(list);
                foreach (ThingDef def in list)
                    this.Settings.enabledDefList.Add(def);
            }
            if (Widgets.ButtonImage(GenUI.LeftPart(GenUI.RightPart(GenUI.TopPart(GenUI.BottomPart(rect3, 0.6f), 0.1f), 0.525f), 0.1f), TexUI.ArrowTexRight) && this.leftSelectedDef != null)
			{
				this.Settings.enabledDefList.Add(this.leftSelectedDef);
				this.Settings.enabledDefList = (from td in this.Settings.enabledDefList
				orderby td.LabelCap.RawText ?? td.defName
				select td).ToList<ThingDef>();
				this.rightSelectedDef = this.leftSelectedDef;
				this.leftSelectedDef = null;
				ImpassableFurniture.AddPassability(this.rightSelectedDef);
            }
            if (Widgets.ButtonImage(GenUI.LeftPart(GenUI.RightPart(GenUI.TopPart(GenUI.BottomPart(rect3, 0.5f), 0.12f), 0.525f), 0.1f), TexUI.ArrowTex, Color.yellow))
            {
                this.leftSelectedDef = null;
                this.rightSelectedDef = null;
                ImpassableFurniture.RemovePassability(this.Settings.enabledDefList);
                ImpassableFurniture.AddPassability(defaultList);
                this.Settings.enabledDefList = new List<ThingDef>();
                foreach (ThingDef def in defaultList)
                    this.Settings.enabledDefList.Add(def);
            }
            if (Widgets.ButtonImage(GenUI.LeftPart(GenUI.RightPart(GenUI.TopPart(GenUI.BottomPart(rect3, 0.4f), 0.15f), 0.525f), 0.1f), TexUI.ArrowTexLeft) && this.rightSelectedDef != null)
			{
				this.Settings.enabledDefList.Remove(this.rightSelectedDef);
				this.leftSelectedDef = this.rightSelectedDef;
				this.rightSelectedDef = null;
				ImpassableFurniture.RemovePassability(this.leftSelectedDef);
            }
            if (Widgets.ButtonImage(GenUI.LeftPart(GenUI.RightPart(GenUI.TopPart(GenUI.BottomPart(rect3, 0.3f), 0.17f), 0.525f), 0.1f), TexUI.ArrowTexLeft, Color.red))
            {
                this.rightSelectedDef = null;
                ImpassableFurniture.RemovePassability(this.Settings.enabledDefList);
                this.Settings.enabledDefList = new List<ThingDef>();
            }
            this.Settings.Write();
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000025DC File Offset: 0x000007DC
		public override string SettingsCategory()
		{
			return "Impassable Furniture";
		}

		// Token: 0x04000002 RID: 2
		public static ImpassableFurnitureMod instance;

		// Token: 0x04000003 RID: 3
		private ImpassableSettings settings;

		// Token: 0x04000004 RID: 4
		private Vector2 leftScrollPosition;

		// Token: 0x04000005 RID: 5
		private Vector2 rightScrollPosition;

		// Token: 0x04000006 RID: 6
		private string searchTerm = "";

		// Token: 0x04000007 RID: 7
		private ThingDef leftSelectedDef;

		// Token: 0x04000008 RID: 8
		private ThingDef rightSelectedDef;

        internal List<ThingDef> defaultList;
        
	}
}
