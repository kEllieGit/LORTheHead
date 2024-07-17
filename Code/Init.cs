using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using LOR_DiceSystem;
using BigDLL4221.Utils;
using BigDLL4221.Models;
using BigDLL4221.Enum;
using HarmonyLib;

namespace TheHead
{
    /// <summary>
    /// Mod Parameters
    /// </summary>
    public static class Parameters
    {
        public const string PackageId = "TheHead";
        public static string Path = "";
    }

    public class Initializer : ModInitializer
    {
        /// <summary>
        /// Called when the mod is loaded.
        /// </summary>
        public override void OnInitializeMod()
        {
            OnInitParameters();
            ArtUtil.GetArtWorks(new DirectoryInfo(Parameters.Path + "/ArtWork"));
            ArtUtil.MakeCustomBook(Parameters.PackageId);
            ArtUtil.PreLoadBufIcons();
            CardUtil.ChangeCardItem(ItemXmlDataList.instance, Parameters.PackageId);
            CardUtil.LoadEmotionAndEgoCards(Parameters.PackageId, Parameters.Path + "/EmotionCards", new List<Assembly> { Assembly.GetExecutingAssembly() });
            CardUtil.InitKeywordsList(new List<Assembly> { Assembly.GetExecutingAssembly() });
            KeypageUtil.ChangeKeypageItem(BookXmlList.Instance, Parameters.PackageId);
            PassiveUtil.ChangePassiveItem(Parameters.PackageId);
            LocalizeUtil.AddGlobalLocalize(Parameters.PackageId);
            LocalizeUtil.RemoveError();
            ArtUtil.InitCustomEffects(new List<Assembly> { Assembly.GetExecutingAssembly() });
        }

        /// <summary>
        /// Initialize Mod parameters and all other mod items.
        /// </summary>
        private static void OnInitParameters()
        {
            // Parameters
            ModParameters.PackageIds.Add(Parameters.PackageId);
            Parameters.Path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            ModParameters.Path.Add(Parameters.PackageId, Parameters.Path);
            ModParameters.Assemblies.Add(Assembly.GetExecutingAssembly());

            // Modded Items
            OnInitStages();
            OnInitCards();
            OnInitCategories();
            OnInitPassives();

            // Harmony
            Harmony.CreateAndPatchAll(typeof(Patches));
        }

        private static void OnInitStages()
        {
            ModParameters.StageOptions.Add(Parameters.PackageId, new List<StageOptions>
            {
                new StageOptions(379390001, preBattleOptions: new PreBattleOptions(battleType: PreBattleType.HybridUnits, sephirahUnits:
                new Dictionary<SephirahType, List<SephirahType>>
                {
                    { SephirahType.Keter, new List<SephirahType> { SephirahType.Keter, SephirahType.Gebura, SephirahType.Binah } }
                }))
            });
        }

        /// <summary>
        /// Initialize custom card options.
        /// </summary>
        private static void OnInitCards()
        {
            ModParameters.CardOptions.Add(Parameters.PackageId, new List<CardOptions>
            {
				// An Arbiter Combat Pages
				new CardOptions(379320027, CardOption.OnlyPage, new List<string> { "ArbiterPage_Re21341" },
                new List<LorId> { new LorId(Parameters.PackageId, 10000551) }),
                new CardOptions(379320028, CardOption.OnlyPage, new List<string> { "ArbiterPage_Re21341" },
                new List<LorId> { new LorId(Parameters.PackageId, 10000551) }),
                new CardOptions(379320030, CardOption.OnlyPage, new List<string> { "ArbiterPage_Re21341" },
                new List<LorId> { new LorId(Parameters.PackageId, 10000551) }),
                new CardOptions(379320031, CardOption.OnlyPage, new List<string> { "ArbiterPage_Re21341" },
                new List<LorId> { new LorId(Parameters.PackageId, 10000551) }),
                new CardOptions(379320033, CardOption.OnlyPage, new List<string> { "ArbiterPage_Re21341" },
                new List<LorId> { new LorId(Parameters.PackageId, 10000551) }),
                new CardOptions(379320034, CardOption.OnlyPage, new List<string> { "ArbiterPage_Re21341" },
                new List<LorId> { new LorId(Parameters.PackageId, 10000551) }),
                new CardOptions(379320032, CardOption.OnlyPage, new List<string> { "ArbiterPage_Re21341" },
                new List<LorId> { new LorId(Parameters.PackageId, 10000551) }),

				// A Paladin Combat Pages
				new CardOptions(379320021, CardOption.OnlyPage, new List<string> { "PaladinPage_Re21341" },
                new List<LorId> { new LorId(Parameters.PackageId, 10000552) }),
                new CardOptions(379320022, CardOption.OnlyPage, new List<string> { "PaladinPage_Re21341" },
                new List<LorId> { new LorId(Parameters.PackageId, 10000552) }),
                new CardOptions(379320023, CardOption.OnlyPage, new List<string> { "PaladinPage_Re21341" },
                new List<LorId> { new LorId(Parameters.PackageId, 10000552) }),
                new CardOptions(379320024, CardOption.OnlyPage, new List<string> { "PaladinPage_Re21341" },
                new List<LorId> { new LorId(Parameters.PackageId, 10000552) }),
                new CardOptions(379320025, CardOption.OnlyPage, new List<string> { "PaladinPage_Re21341" },
                new List<LorId> { new LorId(Parameters.PackageId, 10000552) }),
                new CardOptions(379320026, CardOption.OnlyPage, new List<string> { "PaladinPage_Re21341" },
                new List<LorId> { new LorId(Parameters.PackageId, 10000552) }),
            });
        }

        private static void OnInitCategories()
        {
            ModParameters.CategoryOptions.Add(Parameters.PackageId, new List<CategoryOptions>
            {
                new CategoryOptions(Parameters.PackageId, "_1",
                categoryBooksId: new List<int> {10000552, 10000551, 10000553},
                categoryName: "The Head", credenzaBooksId: new List<int> {10000552, 10000551, 10000553})
            });
        }

        private static void OnInitPassives()
        {
            ModParameters.PassiveOptions.Add(Parameters.PackageId, new List<PassiveOptions>
            {
                new PassiveOptions(379312008, false),
                new PassiveOptions(379312010, false),
            });
        }
    }
}