using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
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
        public SearchMode searchMode = SearchMode.Name;

        public static SortMethod sortMethod = SortMethod.Alphanumeric_Ascending;

        public Dictionary<FaunaFilterDef, bool> faunaFilters = new Dictionary<FaunaFilterDef, bool>();
        public Dictionary<FaunaFilterDef, int> faunaFilterCounts = new Dictionary<FaunaFilterDef, int>();
        public Dictionary<FaunaFilterDef, int> FaunaFilterCounts
        {
            get
            {
                if (faunaFilterCounts.NullOrEmpty())
                {
                    faunaFilterCounts = new Dictionary<FaunaFilterDef, int>();
                    foreach (ThingDef def in AnimalUtil.GetListableAnimals())
                    {
                        foreach (KeyValuePair<FaunaFilterDef, bool> filter in faunaFilters)
                        {
                            if (filter.Key.Worker.FitsInFilter(def))
                            {
                                if (faunaFilterCounts.ContainsKey(filter.Key))
                                {
                                    faunaFilterCounts[filter.Key] += 1;
                                }
                                else
                                {
                                    faunaFilterCounts.Add(filter.Key, 1);
                                }
                            }
                        }
                    }
                }
                return faunaFilterCounts;
            }
        }
        public List<ThingDef> filteredFauna = new List<ThingDef>();
        public List<ThingDef> FilteredFauna
        {
            get
            {
                filteredFauna.Clear();
                if (!faunaFilters.Values.Any(k => k == true))
                {
                    return AnimalUtil.GetListableAnimals().Where(a => a.label.Contains(searchFilter)).ToList();
                }
                foreach (ThingDef def in AnimalUtil.GetListableAnimals().Where(a => a.label.Contains(searchFilter)).ToList())
                {
                    bool result = true;
                    foreach (KeyValuePair<FaunaFilterDef, bool> filter in faunaFilters)
                    {
                        if (filter.Value && !filter.Key.Worker.FitsInFilter(def))
                        {
                            result = false;
                        }
                    }
                    if (result)
                    {
                        filteredFauna.Add(def);
                    }
                }
                return filteredFauna;
            }
        }

        public float scrollHeightLeft;
        public float scrollHeightRight;
        public Vector2 scrollPos;
        public Vector2 scrollPos2;

        public float xPos = 0f;
        public float yPos = 0f;

        [TweakValue("Faunapedia", 30f, 100f)]
        public static float cardScale = 85.85f;
        [TweakValue("Faunapedia", 6f, 24f)]
        public static float cardTextHeight = 16f;
        public static float cardIconScale => cardScale / 4f;
        public static float cardWidth => cardScale + cardIconScale;
        public static float cardHeight => cardScale + cardTextHeight;
        [TweakValue("Faunapedia", 0f, 24f)]
        public static float cardMarginX = 8f;
        [TweakValue("Faunapedia", 0f, 24f)]
        public static float cardMarginY = 8f;

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

        public override Vector2 RequestedTabSize => new Vector2(UI.screenWidth, (UI.screenHeight / 2) - 35f);

        public override void PreOpen()
        {
            base.PreOpen();
            faunaFilters.Clear();
            foreach (FaunaFilterDef def in DefDatabase<FaunaFilterDef>.AllDefs)
            {
                if (AnimalUtil.GetListableAnimals().Any(a => def.Worker.FitsInFilter(a)))
                {
                    faunaFilters.Add(def, false);
                }
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect inRectLeft = new Rect(inRect.x, inRect.y, (inRect.width / 4) * 3, inRect.height);
            Rect inRectRight = new Rect(inRect.x + inRectLeft.width + 8f, inRect.y, inRect.width - inRectLeft.width - 8f, inRect.height);
            #region Card Listing
            {
                bool scrollFlagLeft = scrollHeightLeft > inRectLeft.height;
                Rect viewRectLeft = new Rect(inRectLeft.x, inRectLeft.y, inRectLeft.width - (scrollFlagLeft ? 26f : 0f), scrollHeightLeft);
                Widgets.BeginScrollView(inRectLeft, ref scrollPos, viewRectLeft);
                Listing_Standard listingLeft = new Listing_Standard();
                Rect rectLeft = new Rect(viewRectLeft.x, viewRectLeft.y, viewRectLeft.width, 999999f);
                listingLeft.Begin(rectLeft);
                // ============================ CONTENTS ================================
                DoCardDisplay(listingLeft);
                // ======================================================================
                scrollHeightLeft = listingLeft.CurHeight;
                listingLeft.End();
                Widgets.EndScrollView();
            }
            #endregion
            #region Filter Window
            {
                bool scrollFlagRight = scrollHeightRight > inRectRight.height;
                Rect viewRectRight = new Rect(inRectRight.x, inRectRight.y, inRectRight.width - (scrollFlagRight ? 26f : 0f), scrollHeightRight);
                Widgets.BeginScrollView(inRectRight, ref scrollPos2, viewRectRight);
                Listing_Standard listingRight = new Listing_Standard();
                Rect rectRight = new Rect(viewRectRight.x, viewRectRight.y, viewRectRight.width, 999999f);
                listingRight.Begin(rectRight);
                // ============================ CONTENTS ================================
                DoFilterDisplay(listingRight);
                // ======================================================================
                scrollHeightRight = listingRight.CurHeight;
                listingRight.End();
                Widgets.EndScrollView();
            }
            #endregion
        }

        public void DoFilterDisplay(Listing_Standard listing)
        {
            Rect searchRect = listing.GetRect(30f);
            quickSearchWidget.OnGUI(searchRect);
            searchFilter = quickSearchWidget.filter.Text;
            listing.GapLine();
            if (listing.ButtonText(sortMethod.ToString().Replace("_", " ")))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                List<SortMethod> enumValues = Enum.GetValues(typeof(SortMethod)).Cast<SortMethod>().ToList();
                foreach (SortMethod enumValue in enumValues)
                {
                    list.Add(new FloatMenuOption(enumValue.ToString().Replace("_", " "), delegate ()
                    {
                        sortMethod = enumValue;
                    }));
                }

                Find.WindowStack.Add(new FloatMenu(list));
            }
            listing.Gap();
            listing.GapLine();
            listing.Label("Filters");
            listing.GapLine();
            List<FaunaFilterDef> filterDefs = DefDatabase<FaunaFilterDef>.AllDefs.ToList();
            filterDefs.SortBy(f => f.defName);
            foreach (FaunaFilterDef def in filterDefs)
            {
                if (faunaFilters.ContainsKey(def))
                {
                    bool bufferBool = faunaFilters[def];
                    listing.CheckboxLabeled(def.LabelCap + " (" + FaunaFilterCounts[def] + ")", ref bufferBool, def.description);
                    faunaFilters[def] = bufferBool;
                }
            }
        }

        public void DoCardDisplay(Listing_Standard listing)
        {
            float abCurY = 0f;
            float abCurX = 0f;
            int curCount = 0;
            List<ThingDef> animalList = FilteredFauna.ToList();
            switch (sortMethod)
            {
                case SortMethod.Alphanumeric_Descending:
                    animalList.SortByDescending(t => t.label);
                    break;
                case SortMethod.Size_Ascending:
                    animalList.SortBy(t => t.race.baseBodySize);
                    break;
                case SortMethod.Size_Descending:
                    animalList.SortByDescending(t => t.race.baseBodySize);
                    break;
                case SortMethod.Value_Ascending:
                    animalList.SortBy(t => t.BaseMarketValue);
                    break;
                case SortMethod.Value_Descending:
                    animalList.SortByDescending(t => t.BaseMarketValue);
                    break;
                default:
                    animalList.SortBy(t => t.label);
                    break;
            }
            for (int i = 0; i < animalList.Count(); i++)
            {
                Def def = animalList[i] as Def;
                if (animalList[i].IsKnownAnimal() || FaunapediaMod.settings.unknownShown)
                {
                    DrawAnimalCard(new Rect(abCurX, abCurY, cardHeight, cardWidth), listing, animalList[i], !animalList[i].IsKnownAnimal());
                }
                curCount++;
                if (curCount < animalList.Count)
                {
                    if (curCount % Mathf.FloorToInt(listing.ColumnWidth / (cardWidth + cardMarginX)) == 0)
                    {
                        abCurY += cardHeight + cardMarginY;
                        abCurX = 0f;
                        curCount = 0;
                    }
                    else
                    {
                        abCurX += cardWidth + cardMarginX;
                    }
                }
            }
            listing.curY = abCurY + (cardHeight + cardMarginY);
        }

        public void DrawAnimalCard(Rect rect, Listing_Standard listing, ThingDef animalDef, bool obfuscate = false)
        {
            if (animalDef != null)
            {
                Color color = Color.white;
                Rect outRect = new Rect(rect.x, rect.y, cardWidth, rect.height);
                Rect imageRect = new Rect(rect.x, rect.y, cardScale, cardScale);
                Rect iconRect = new Rect(rect.x + cardScale, rect.y, cardIconScale, cardScale);
                Rect labelRect = new Rect(rect.x, rect.y + cardScale, cardWidth, cardTextHeight);
                if (Mouse.IsOver(imageRect))
                {
                    color = GenUI.MouseoverColor;
                    TooltipHandler.TipRegion(imageRect, animalDef.LabelCap);
                }
                MouseoverSounds.DoRegion(imageRect, SoundDefOf.Mouseover_Command);
                DoCardIcons(iconRect, animalDef.IsSeenAnimal(), animalDef.IsTamedAnimal(), animalDef.IsHuntedAnimal());
                DoCardImage(imageRect, animalDef, obfuscate);
                DoCardLabel(labelRect, animalDef);
            }
        }

        public void DoCardIcons(Rect rect, bool seen, bool tamed, bool hunted)
        {
            float rectHeight = rect.height / 2f;
            Rect rectSeen = new Rect(rect.x, rect.y, rect.width, rectHeight);
            {
                Widgets.DrawAtlas(rectSeen, Widgets.ButtonSubtleAtlas);
                Rect rectSeenIcon = new Rect(rectSeen.x, rectSeen.y + (rectSeen.height / 2) - (cardIconScale / 2), cardIconScale, cardIconScale);
                if (seen) { Widgets.DrawTextureFitted(rectSeenIcon, MaterialPool.MatFrom("Faunapedia/SeenYes").mainTexture, 0.9f); }
                else { Widgets.DrawTextureFitted(rectSeenIcon, MaterialPool.MatFrom("Faunapedia/SeenNo").mainTexture, 0.9f); }
            }
            Rect rectTamed = new Rect(rect.x, rect.y + rectHeight, rect.width, rectHeight);
            {
                Widgets.DrawAtlas(rectTamed, Widgets.ButtonSubtleAtlas);
                Rect rectTamedIcon = new Rect(rectTamed.x, rectTamed.y + (rectTamed.height / 2) - (cardIconScale / 2), cardIconScale, cardIconScale);
                if (tamed) { Widgets.DrawTextureFitted(rectTamedIcon, MaterialPool.MatFrom("Faunapedia/TameYes").mainTexture, 0.9f); }
                else { Widgets.DrawTextureFitted(rectTamedIcon, MaterialPool.MatFrom("Faunapedia/TameNo").mainTexture, 0.9f); }
            }
            //Rect rectHunted = new Rect(rect.x, rect.y + (rectHeight * 2), rect.width, rectHeight);
            //{
            //    Widgets.DrawAtlas(rectHunted, Widgets.ButtonSubtleAtlas);
            //    Rect rectHuntedIcon = new Rect(rectHunted.x, rectHunted.y + (rectHunted.height / 2) - (cardIconScale / 2), cardIconScale, cardIconScale);
            //    if (seen) { Widgets.DrawTextureFitted(rectHuntedIcon, MaterialPool.MatFrom("UI/Overlays/QuestionMark").mainTexture, 0.9f); }
            //    else { Widgets.DrawTextureFitted(rectHuntedIcon, MaterialPool.MatFrom("UI/Overlays/QuestionMark").mainTexture, 0.9f); }
            //}
        }

        public void DoCardImage(Rect rect, ThingDef animalDef, bool obfuscate)
        {
            Material material = (false ? TexUI.GrayscaleGUI : null);
            GenUI.DrawTextureWithMaterial(rect, Command.BGTex, material);
            Rect animalRect = rect.ContractedBy(8f);
            if (!obfuscate)
            {
                Widgets.DefIcon(animalRect, animalDef);
                if (Widgets.ButtonInvisible(rect))
                {
                    Find.WindowStack.Add(new Dialog_InfoCard(animalDef));
                }
            }
            else
            {
                if (FaunapediaMod.settings.unknownSilhouettes)
                {
                    Widgets.DefIcon(animalRect, animalDef, null, 1, null, false, Color.black, null, null);
                }
                else
                {
                    Widgets.DrawTextureFitted(animalRect, MaterialPool.MatFrom("UI/Overlays/QuestionMark").mainTexture, 0.8f);
                }
            }
        }

        public void DoCardLabel(Rect rect, ThingDef animalDef)
        {
            GUI.DrawTexture(rect, TexUI.GrayTextBG);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(rect, animalDef.LabelCap);
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }
    }
}
