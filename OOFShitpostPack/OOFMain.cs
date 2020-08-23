using System;
using System.IO;
using BepInEx;
using UnityEngine;
using R2API;
using R2API.Utils;
using RoR2;
using IL.EntityStates;

namespace OOFShitpostPack
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Legendsmith.OOFShitpostPack", "Oof Shitpost Pack", "0.1.5")]
    [R2APISubmoduleDependency(nameof(LanguageAPI))]
    [R2APISubmoduleDependency(nameof(SoundAPI))]
    public class OOFShitpostPack : BaseUnityPlugin
    {
        public static UInt32 LoadSoundBank(Byte[] resourceBytes)
        {
            if (resourceBytes == null) throw new ArgumentNullException(nameof(resourceBytes));

            return R2API.SoundAPI.SoundBanks.Add(resourceBytes);
        }
        public UInt32 takemoonbank;
        public void Awake()
        {
            R2API.LanguageAPI.AddPath("OOFShitpostPack/language/CharacterBodies");
            R2API.LanguageAPI.AddPath("OOFShitpostPack/language/Dialogue");
            R2API.LanguageAPI.AddPath("OOFShitpostPack/language/Equipment");
            R2API.LanguageAPI.AddPath("OOFShitpostPack/language/Items");
            R2API.LanguageAPI.AddPath("OOFShitpostPack/language/Messages");
            R2API.LanguageAPI.AddPath("OOFShitpostPack/language/Interactors");
            //sound
            takemoonbank = LoadSoundBank(Properties.Resources.MoonTake);

            On.EntityStates.BrotherMonster.ThroneSpawnState.OnEnter += (orig, self) =>
            {
                orig(self);
                Util.PlaySound("1047166050", self.outer.gameObject);
            };
            On.EntityStates.Commando.MainState.FixedUpdate += (orig, self) =>
            {
                orig(self);
                Util.PlaySound("2693398676", self.outer.gameObject);
            };
        }
    }
}