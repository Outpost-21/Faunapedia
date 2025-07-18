﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Faunapedia
{
    public class FaunapediaMod : Mod
    {
        public static FaunapediaMod mod;
        public static FaunapediaSettings settings;

        public Vector2 optionsScrollPosition;
        public float optionsViewRectHeight;

        internal static string VersionDir => Path.Combine(mod.Content.ModMetaData.RootDir.FullName, "Version.txt");
        public static string CurrentVersion { get; private set; }

        public FaunapediaMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<FaunapediaSettings>();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            Log.Message($":: Faunapedia :: ".Colorize(Color.cyan) + $"{CurrentVersion} ::");

            File.WriteAllText(VersionDir, CurrentVersion);

            Harmony harmony = new Harmony("Neronix17.Faunapedia.RimWorld");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => "Faunapedia";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            bool flag = optionsViewRectHeight > inRect.height;
            Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width - (flag ? 26f : 0f), optionsViewRectHeight);
            Widgets.BeginScrollView(inRect, ref optionsScrollPosition, viewRect);
            Listing_Standard listing = new Listing_Standard();
            Rect rect = new Rect(viewRect.x, viewRect.y, viewRect.width, 999999f);
            listing.Begin(rect);
            // ============================ CONTENTS ================================
            DoOptionsCategoryContents(listing);
            // ======================================================================
            optionsViewRectHeight = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();
        }

        public void DoOptionsCategoryContents(Listing_Standard listing)
        {
            listing.CheckboxEnhanced("Faunapedia.UnlockedByDefault".Translate(), "Faunapedia.UnlockedByDefaultDesc".Translate(), ref settings.unlockedByDefault);
            listing.CheckboxEnhanced("Faunapedia.UnlockedBySighting".Translate(), "Faunapedia.UnlockedBySightingDesc".Translate(), ref settings.unlockedBySighting);
            listing.CheckboxEnhanced("Faunapedia.UnlockedByTaming".Translate(), "Faunapedia.UnlockedByTamingDesc".Translate(), ref settings.unlockedByTaming);
            listing.CheckboxEnhanced("Faunapedia.UnknownShown".Translate(), "Faunapedia.UnknownShownDesc".Translate(), ref settings.unknownShown);
            //listing.CheckboxEnhanced("Faunapedia.HideLabels".Translate(), "Faunapedia.HideLabelsDesc".Translate(), ref settings.hideLabels);
            listing.CheckboxEnhanced("Faunapedia.UnknownSilhouettes".Translate(), "Faunapedia.UnknownSilhouettesDesc".Translate(), ref settings.unknownSilhouettes);
        }
    }
}
