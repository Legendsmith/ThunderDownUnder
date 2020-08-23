using BepInEx;
using R2API;
using RoR2;
using System;
using System.Reflection;
using UnityEngine;
using RoR2.Projectile;
namespace ThunderDownUnder.BetterCommando
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Legendsmith.ImprovedCommando","2.0.0")]
    public class BetterCommandoLoader : BaseUnityPlugin
    {
        public void Awake()
        {
            Chat.AddMessage("Loaded ImprovedCommando!");
            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {
                //get commando body
                GameObject gameObject = BodyCatalog.FindBodyPrefab("CommandoBody");
                //replace the utiltiy
                GenericSkill utility = gameObject.GetComponent<SkillLocator>().utility;
                utility.activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Commando.CommandoWeapon.Dodgestateimproved));
                object box2 = utility.activationState;
                var field2 = typeof(EntityStates.SerializableEntityStateType)?.GetField("_typeName", BindingFlags.NonPublic | BindingFlags.Instance);
                field2?.SetValue(box2, typeof(EntityStates.Commando.CommandoWeapon.Dodgestateimproved)?.AssemblyQualifiedName);
                utility.activationState = (EntityStates.SerializableEntityStateType)box2;
                utility.baseRechargeInterval = 5f;
                utility.baseMaxStock = 2;
                //edit utility descriptions
                utility.skillNameToken = "Reposition";
                utility.skillDescriptionToken = "<i>Rolls</i> a short distance, with a short speed boost after. Holds 2 charges.";
                //load the FMJ
                GameObject fmjprefab = Resources.Load<GameObject>("prefabs/projectiles/fmj");
                ProjectileSimple fmjprojectilesimple = fmjprefab.GetComponentInChildren<ProjectileSimple>();
                fmjprojectilesimple.velocity = 200f;
                /*
                //load secondary
                GenericSkill secondary = gameObject.GetComponent<SkillLocator>().secondary;
                secondary.baseMaxStock = 2;
                GenericSkill special = gameObject.GetComponent<SkillLocator>().special;
                special.skillDescriptionToken = "Fire rapidly, stunning enemies for 6x100% damage and reload all Phase Rounds.";
                On.EntityStates.Commando.CommandoWeapon.FireBarrage.OnExit += (orig, self) =>
                {
                    orig(self);
                    GenericSkill secondaryskill = self.outer.commonComponents.characterBody.GetComponent<SkillLocator>().secondary;
                    secondaryskill.Reset();
                };*/

            };
        }
    }
}
namespace EntityStates.Commando.CommandoWeapon
{
    class FireFMJImproved : FireFMJ
    {
        
    }
} 