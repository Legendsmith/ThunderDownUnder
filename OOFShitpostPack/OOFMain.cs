using System;
using System.IO;
using BepInEx;
using UnityEngine;
using R2API;
using R2API.Utils;
using RoR2;
using EntityStates;
using System.Collections;

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
        public SoundLoopStarter soundloopstarter;
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
            // loop id 2693398676
            //test
            On.EntityStates.Commando.CommandoWeapon.FireFMJ.OnEnter += (orig, self) =>
            {
                AkSoundEngine.PostEvent(1047166050, self.outer.gameObject);
                orig(self);
                soundloopstarter.Init(self.outer.gameObject);
            };
            //
            On.EntityStates.BrotherMonster.ThroneSpawnState.OnEnter += (orig, self) =>
            {
                AkSoundEngine.PostEvent(1047166050, self.outer.gameObject);
                orig(self);
                soundloopstarter.Init(self.outer.gameObject);
            };
            On.EntityStates.BrotherMonster.TrueDeathState.OnEnter += (orig, self) =>
            {
                AkSoundEngine.StopPlayingID(2693398676);
                orig(self);
                
            };
        }

    }
    public class SoundLoopStarter : MonoBehaviour
    {
        GameObject Target;
        private IEnumerator coroutine;
        public void Init(GameObject obj)
        {
            Target = obj;
            coroutine = WaitAndPlay();
            StartCoroutine(coroutine);
            
        }
        private IEnumerator WaitAndPlay()
        {
            yield return new WaitForSeconds(5);
            AkSoundEngine.PostEvent(2693398676, Target);
        }
    }
}
