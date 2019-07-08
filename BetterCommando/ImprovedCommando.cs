using BepInEx;
using R2API;
using RoR2;
using System;
using System.Reflection;
using UnityEngine;
using RoR2.Projectile;
namespace ThunderDownUnder.BetterCommando
{
    [BepInPlugin("com.ThunderDownUnder.ImprovedCommando", "ImprovedCommando", "1.1.0")]
    public class BetterCommandoLoader :BaseUnityPlugin
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
                utility.skillDescriptionToken = "<i>Rolls</i> a short distance, while using a smoke grenade to turn invisible for 2 seconds. Holds 2 charges.";
                //load the FMJ
                GameObject fmjprefab = Resources.Load<GameObject>("prefabs/projectiles/fmj");
                ProjectileSimple fmjprojectilesimple = fmjprefab.GetComponentInChildren<ProjectileSimple>();
                fmjprojectilesimple.velocity = 180f;
                //load secondary
                GenericSkill secondary = gameObject.GetComponent<SkillLocator>().secondary;
                secondary.baseMaxStock = 2;
                GenericSkill special = gameObject.GetComponent<SkillLocator>().special;
                special.skillDescriptionToken = "Fire rapidly, stunning enemies for 6x100% damage and reload two Phase Round charges.";
                On.EntityStates.Commando.CommandoWeapon.FireBarrage.OnExit += (orig, self) =>
                {
                    orig(self);
                    GenericSkill secondaryskill = self.outer.commonComponents.characterBody.GetComponent<SkillLocator>().secondary;
                    secondaryskill.AddOneStock();
                    secondaryskill.AddOneStock();
                    if (secondaryskill.stock > secondary.maxStock)
                    {
                        secondaryskill.Reset();
                    }
                };

            };
        }
    }
}
namespace EntityStates.Commando.CommandoWeapon
{
    class Dodgestateimproved : DodgeState
    {
        public override void OnEnter()
        {
            EffectManager.instance.SpawnEffect(CastSmokescreenNoDelay.smokescreenEffectPrefab, new EffectData
            {
                origin = base.transform.position
            }, false);
            Util.PlaySound(CastSmokescreen.jumpSoundString, base.gameObject);
            this.outer.commonComponents.characterBody.AddTimedBuff(BuffIndex.CloakSpeed, 2f);
            this.outer.commonComponents.characterBody.AddTimedBuff(BuffIndex.Cloak, 2f);
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        public override void OnExit()
        {
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}