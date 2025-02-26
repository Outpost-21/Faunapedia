using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace Faunapedia
{
    [StaticConstructorOnStartup]
    public class MainTabWindow_Faunapedia : MainTabWindow
    {
        public QuickSearchWidget quickSearchWidget = new QuickSearchWidget();
        public string searchFilter = "";

        public float scrollHeight;
        public Vector2 scrollPos;

        public float xPos = 0f;
        public float yPos = 0f;

        public GameComp_AnimalTracking cachedAnimalTracking;
        public GameComp_AnimalTracking AnimalTracking
        {
            get
            {
                if(cachedAnimalTracking == null)
                {
                    cachedAnimalTracking = FindUtil.AnimalTracking;
                }
                return cachedAnimalTracking;
            }
        }

        public override Vector2 RequestedTabSize
        {
            get
            {
                return new Vector2(1010f, 640f);
            }
        }

        public override void PreOpen()
        {
            base.PreOpen();
        }

        public override void DoWindowContents(Rect inRect)
        {
            bool flag = scrollHeight > inRect.height;
            Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width - (flag ? 26f : 0f), scrollHeight);
            Widgets.BeginScrollView(inRect, ref scrollPos, viewRect);
            Listing_Standard listing = new Listing_Standard();
            Rect rect = new Rect(viewRect.x, viewRect.y, viewRect.width, 999999f);
            listing.Begin(rect);
            // ============================ CONTENTS ================================
            Rect searchRect = listing.GetRect(30f);
            quickSearchWidget.OnGUI(searchRect);
            searchFilter = quickSearchWidget.filter.Text;
            listing.GapLine();
            DoContents(listing);
            // ======================================================================
            scrollHeight = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();
        }

        public void DoContents(Listing_Standard listing)
        {
            float abCurY = 38f;
            float abCurX = 0f;
            string curSource = "";
            int curCount = 0;
            List<ThingDef> animalList = AnimalUtil.GetListableAnimals().Where(a => a.label.Contains(searchFilter)).ToList();
            for (int i = 0; i < animalList.Count(); i++)
            {
                Def def = animalList[i] as Def;
                if (def.modContentPack.Name != curSource)
                {
                    curSource = def.modContentPack.Name;
                    if (abCurX != 0f)
                    {
                        abCurY += 80f;
                        abCurX = 0f;
                        curCount = 0;
                    }
                    Text.Font = GameFont.Tiny;
                    GUI.color = Color.white;
                    float textGap = Text.CalcHeight(curSource, listing.ColumnWidth);
                    Rect textRect = new Rect(abCurX, abCurY, listing.ColumnWidth, textGap);
                    Widgets.Label(textRect, curSource);
                    Text.Font = GameFont.Small;
                    abCurY += textGap;
                    float y = abCurY + 6f;
                    Color color = GUI.color;
                    GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
                    Widgets.DrawLineHorizontal(0, y, listing.ColumnWidth - 18f);
                    GUI.color = color;
                    abCurY += 12f;
                }
                //DrawSelector(new Rect(abCurX, abCurY, 80f, 75f), listing, def);
                if (FaunapediaMod.settings.unlockedByDefault)
                {
                    DrawAnimalCard(new Rect(abCurX, abCurY, 80f, 75f), listing, animalList[i]);
                }
                else if (FaunapediaMod.settings.unlockedBySighting && AnimalTracking.animalsSeen[animalList[i]])
                {
                    DrawAnimalCard(new Rect(abCurX, abCurY, 80f, 75f), listing, animalList[i]);
                }
                else if (FaunapediaMod.settings.unlockedByTaming && AnimalTracking.animalsTamed[animalList[i]])
                {
                    DrawAnimalCard(new Rect(abCurX, abCurY, 80f, 75f), listing, animalList[i]);
                }
                else if (FaunapediaMod.settings.unknownShown)
                {
                    DrawAnimalCard(new Rect(abCurX, abCurY, 80f, 75f), listing, animalList[i], true);
                }
                // Handle Row/Column Position.
                curCount++;
                if (curCount < animalList.Count)
                {
                    if (curCount % Mathf.FloorToInt(listing.ColumnWidth / 80f) == 0)
                    {
                        abCurY += 80f;
                        abCurX = 0f;
                        curCount = 0;
                    }
                    else
                    {
                        abCurX += 80f;
                    }
                }
            }
            listing.curY = abCurY + 80f;
        }

        public void DrawAnimalCard(Rect rect, Listing_Standard listing, ThingDef animalDef, bool obfuscate = false)
        {
            if (animalDef != null)
            {
                Color color = Color.white;
                Rect inRect = new Rect(rect.x + ((rect.width / 2f) - (75f / 2f)), rect.y, 75f, rect.height);
                if (Mouse.IsOver(inRect))
                {
                    color = GenUI.MouseoverColor;
                }
                MouseoverSounds.DoRegion(inRect, SoundDefOf.Mouseover_Command);
                Material material = (false ? TexUI.GrayscaleGUI : null);
                GenUI.DrawTextureWithMaterial(inRect, Command.BGTex, material);
                Rect animalRect = inRect.ContractedBy(8f);
                if (!obfuscate) 
                { 
                    Widgets.DefIcon(animalRect, animalDef);
                    if (Mouse.IsOver(inRect))
                    {
                        TooltipHandler.TipRegion(inRect, animalDef.LabelCap);
                    }
                    if (Widgets.ButtonInvisible(inRect))
                    {
                        Find.WindowStack.Add(new Dialog_InfoCard(animalDef));
                    }
                }
                else 
                {
                    Widgets.DrawTextureFitted(animalRect, MaterialPool.MatFrom("UI/Overlays/QuestionMark").mainTexture, 0.8f);
                }
                string labelCap = animalDef.LabelCap;
                float labelHeight = Text.CalcHeight(labelCap, inRect.width + 0.1f);
                Rect animalLabel = new Rect(inRect.x, inRect.yMax - labelHeight + 12f, inRect.width, labelHeight);
                GUI.DrawTexture(animalLabel, TexUI.GrayTextBG);
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(animalLabel, labelCap);
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
            }
        }
    }
}
