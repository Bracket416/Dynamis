using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamis.Handler
{
    unsafe internal class Jobs
    {

        private static FFXIVClientStructs.FFXIV.Client.Game.JobGaugeManager* Manager = FFXIVClientStructs.FFXIV.Client.Game.JobGaugeManager.Instance();

        public static List<string> Gauges = new List<string> { };
        public static int Get_Gauge(string Gauge)
        {
            switch (Gauge)
            {
                case "PLD Oath Gauge": return Manager->Paladin.OathGauge;
                case "PLD Confiteor": return Manager->Paladin.ConfiteorComboStep;
                case "WAR Beast Gauge": return Manager->Warrior.BeastGauge;
                case "DRK Blood": return Manager->DarkKnight.Blood;
                case "DRK Dark Arts": return Manager->DarkKnight.DarkArtsState;
                case "DRK Delirium": return Manager->DarkKnight.DeliriumStep;
                case "GNB Ammo": return Manager->Gunbreaker.Ammo;
                case "GNB Combo": return Manager->Gunbreaker.AmmoComboStep;
                case "MNK Coeurl": return Manager->Monk.CoeurlStacks;
                case "MNK Raptor": return Manager->Monk.RaptorStacks;
                case "MNK Opo Opo": return Manager->Monk.OpoOpoStacks;
                case "MNK Nadi": return Manager->Monk.Nadi == FFXIVClientStructs.FFXIV.Client.Game.Gauge.NadiFlags.Solar ? 1 : 0; // Can it be both?
                case "MNK Beast Chakra": return Manager->Monk.BeastChakraStacks;
                case "MNK First Beast Chakra": return Manager->Monk.BeastChakra1 == FFXIVClientStructs.FFXIV.Client.Game.Gauge.BeastChakraType.Coeurl ? 1 : (Manager->Monk.BeastChakra1 == FFXIVClientStructs.FFXIV.Client.Game.Gauge.BeastChakraType.Raptor ? 2 : (Manager->Monk.BeastChakra1 == FFXIVClientStructs.FFXIV.Client.Game.Gauge.BeastChakraType.OpoOpo ? 3 : 0));
                case "MNK Second Beast Chakra": return Manager->Monk.BeastChakra2 == FFXIVClientStructs.FFXIV.Client.Game.Gauge.BeastChakraType.Coeurl ? 1 : (Manager->Monk.BeastChakra2 == FFXIVClientStructs.FFXIV.Client.Game.Gauge.BeastChakraType.Raptor ? 2 : (Manager->Monk.BeastChakra2 == FFXIVClientStructs.FFXIV.Client.Game.Gauge.BeastChakraType.OpoOpo ? 3 : 0));
                case "MNK Third Beast Chakra": return Manager->Monk.BeastChakra3 == FFXIVClientStructs.FFXIV.Client.Game.Gauge.BeastChakraType.Coeurl ? 1 : (Manager->Monk.BeastChakra3 == FFXIVClientStructs.FFXIV.Client.Game.Gauge.BeastChakraType.Raptor ? 2 : (Manager->Monk.BeastChakra3 == FFXIVClientStructs.FFXIV.Client.Game.Gauge.BeastChakraType.OpoOpo ? 3 : 0));
                case "DRG Life of the Dragon": return Manager->Dragoon.LotdState;
                case "DRG Eye": return Manager->Dragoon.EyeCount;
                case "DRG Firstmind's Focus": return Manager->Dragoon.FirstmindsFocusCount;
                case "NIN Ninki": return Manager->Ninja.Ninki;
                case "NIN Kazematoi": return Manager->Ninja.Kazematoi;
                case "SAM Kaeshi": return (int)Manager->Samurai.Kaeshi;
                case "SAM Sen Ka": return Manager->Samurai.SenFlags.HasFlag(SenFlags.Ka) ? 1 : 0;
                case "SAM Sen Setsu": return Manager->Samurai.SenFlags.HasFlag(SenFlags.Setsu) ? 1 : 0;
                case "SAM Sen Getsu": return Manager->Samurai.SenFlags.HasFlag(SenFlags.Getsu) ? 1 : 0;
                case "SAM Kenki": return Manager->Samurai.Kenki;
                case "RPR Soul": return Manager->Reaper.Soul;
                case "RPR Void Shroud": return Manager->Reaper.VoidShroud;
                case "RPR Lemure Shroud": return Manager->Reaper.LemureShroud;
                case "VPR Rattling Coil": return Manager->Viper.RattlingCoilStacks;
                case "VPR Anguine Tribute": return Manager->Viper.AnguineTribute;
                //case "VPR Dread": return Manager->Viper.DreadCombo; How does this work on VPR?
                case "VPR Serpent": return Manager->Viper.SerpentComboState;
                case "VPR Offering": return Manager->Viper.SerpentOffering;
                case "WHM Lily": return Manager->WhiteMage.Lily;
                case "WHM Blood Lily": return Manager->WhiteMage.BloodLily;
                case "SCH Aetherflow": return Manager->Scholar.Aetherflow;
                case "SCH Fairy Gauge": return Manager->Scholar.FairyGauge;
                case "AST Cards": return Manager->Astrologian.Cards;
                //case "AST Arcana": return Manager->Astrologian.CurrentArcana;
                //case "AST Cards": return Manager->Astrologian.CurrentDraw; I have to think about how I want to separate these. Probably, I'll just make a package for each card.
                case "SGE Eukrasia": return Manager->Sage.EukrasiaActive ? 1 : 0;
                case "SGE Addersgall": return Manager->Sage.Addersgall;
                case "SGE Addersting": return Manager->Sage.Addersting;
                case "BRD Coda": return Manager->Bard.RadiantFinaleCoda;
                case "BRD Repertoire": return Manager->Bard.Repertoire;
                case "BRD Voice": return Manager->Bard.SoulVoice;
                case "BRD Mage's Ballad": return Manager->Bard.SongFlags.HasFlag(SongFlags.MagesBallad) ? 1 : 0; // There are Coda versions of these... How does that work with BRD? Is it relevant here?
                case "BRD Wanderer's Minuet": return Manager->Bard.SongFlags.HasFlag(SongFlags.WanderersMinuet) ? 1 : 0;
                case "BRD Army's Paeon": return Manager->Bard.SongFlags.HasFlag(SongFlags.ArmysPaeon) ? 1 : 0;
                case "MCH Heat": return Manager->Machinist.Heat;
                case "MCH Battery": return Manager->Machinist.Battery;
                //case "DNC": return Manager->Dancer.CurrentStep; This is another case of something that'll have to be considered later.
                case "DNC Esprit": return Manager->Dancer.Esprit;
                case "DNC Feathers": return Manager->Dancer.Feathers;
                case "DNC Step": return Manager->Dancer.StepIndex;
                case "BLM Astral": return Manager->BlackMage.AstralStacks;
                case "BLM Astral Soul": return Manager->BlackMage.AstralSoulStacks; // I'll have to test the difference between these two.
                case "BLM Element": return Manager->BlackMage.ElementStance;
                case "BLM Polyglot": return Manager->BlackMage.PolyglotStacks;
                case "SMN Attunement": return Manager->Summoner.Attunement;
                case "SMN Ifrit": return Manager->Summoner.AetherFlags.HasFlag(AetherFlags.IfritAttuned) ? 1 : 0;
                case "SMN Ifrit Ready": return Manager->Summoner.AetherFlags.HasFlag(AetherFlags.IfritReady) ? 1 : 0;
                case "SMN Garuda": return Manager->Summoner.AetherFlags.HasFlag(AetherFlags.GarudaAttuned) ? 1 : 0;
                case "SMN Garuda Ready": return Manager->Summoner.AetherFlags.HasFlag(AetherFlags.GarudaReady) ? 1 : 0;
                case "SMN Titan": return Manager->Summoner.AetherFlags.HasFlag(AetherFlags.TitanAttuned) ? 1 : 0;
                case "SMN Titan Ready": return Manager->Summoner.AetherFlags.HasFlag(AetherFlags.TitanReady) ? 1 : 0; // SMN has a few things related to Aetherflow. I don't play the job, so I don't know what those are for or how they're meant to be interpreted.
                case "RDM Black Mana": return Manager->RedMage.BlackMana;
                case "RDM White Mana": return Manager->RedMage.WhiteMana;
                case "RDM Mana Stacks": return Manager->RedMage.ManaStacks;
                case "PCT Paint": return Manager->Pictomancer.Paint;
                case "PCT Pallete": return Manager->Pictomancer.PalleteGauge;
                case "PCT Pom": return Manager->Pictomancer.CanvasFlags.HasFlag(CanvasFlags.Pom) ? 1 : 0;
                case "PCT Maw": return Manager->Pictomancer.CanvasFlags.HasFlag(CanvasFlags.Maw) ? 1 : 0;
                case "PCT Claw": return Manager->Pictomancer.CanvasFlags.HasFlag(CanvasFlags.Claw) ? 1 : 0;
                case "PCT Weapon": return Manager->Pictomancer.CanvasFlags.HasFlag(CanvasFlags.Weapon) ? 1 : 0;
                case "PCT Wing": return Manager->Pictomancer.CanvasFlags.HasFlag(CanvasFlags.Wing) ? 1 : 0;
                case "PCT Landscape": return Manager->Pictomancer.CanvasFlags.HasFlag(CanvasFlags.Landscape) ? 1 : 0;
                case "PCT Landscape Motif": return Manager->Pictomancer.LandscapeMotifDrawn ? 1 : 0;
                case "PCT Creature Motif": return Manager->Pictomancer.CreatureMotifDrawn ? 1 : 0;
                case "PCT Weapon Motif": return Manager->Pictomancer.WeaponMotifDrawn ? 1 : 0;
                case "PCT Mandeen": return Manager->Pictomancer.MadeenPortraitReady ? 1 : 0;
                case "PCT Moogle": return Manager->Pictomancer.MooglePortraitReady ? 1 : 0;
            }
            return 0;
        }
    }
}
