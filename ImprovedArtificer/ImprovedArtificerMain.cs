using System;
using BepInEx;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace ThunderDownUnder.ImprovedArtificer
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(
        "com.Legendsmith.ImprovedArtificer",
        "ImprovedArtificer",
        "1.0.0")]
    [R2APISubmoduleDependency(nameof(LoadoutAPI), nameof(SurvivorAPI), nameof(LanguageAPI))]
    public class ImprovedArtificerMain : BaseUnityPlugin
    {
        public void Awake()
        {
            // myCharacter should either be
            // Resources.Load<GameObject>("prefabs/characterbodies/BanditBody");
            // or BodyCatalog.FindBodyIndex("BanditBody");
            var myCharacter = Resources.Load<GameObject>("prefabs/characterbodies/MageBody");

            // If you're confused about the language tokens, they're the proper way to add any strings used by the game.
            // We use AssetPlus API for that

            var mySurvivorDef = new SurvivorDef
            {
                //We're finding the body prefab here,
                bodyPrefab = myCharacter,
                //Description
                descriptionToken = "MAGE_DESCRIPTION",
                //Display 
                displayPrefab = Resources.Load<GameObject>("Prefabs/Characters/MageDisplay"),
                //Color on select screen
                primaryColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
                //Unlockable name
                unlockableName = "",
            };
            SurvivorAPI.AddSurvivor(mySurvivorDef);

            var mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(ThunderDownUnder.ImprovedArtificer.TEntityStates.ExampleState));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 0f;
            mySkillDef.beginSkillCooldownOnSkillEnd = true;
            mySkillDef.canceledFromSprinting = true;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Any;
            mySkillDef.isBullets = true;
            mySkillDef.isCombatSkill = false;
            mySkillDef.mustKeyPress = false;
            mySkillDef.noSprint = true;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0.5f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Resources.Load<Sprite>("NotAnActualPath");
            mySkillDef.skillDescriptionToken = "CHARACTERNAME_SKILLSLOT_SKILLNAME_DESCRIPTION";
            mySkillDef.skillName = "CHARACTERNAME_SKILLSLOT_SKILLNAME_NAME";
            mySkillDef.skillNameToken = "CHARACTERNAME_SKILLSLOT_SKILLNAME_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);
            //This adds our skilldef. If you don't do this, the skill will not work.

            var skillLocator = myCharacter.GetComponent<SkillLocator>();

            //Note; if your character does not originally have a skill family for this, use the following:
            //skillLocator.special = gameObject.AddComponent<GenericSkill>();
            //var newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            //LoadoutAPI.AddSkillFamily(newFamily);
            //skillLocator.special.SetFieldValue("_skillFamily", newFamily);
            //var specialSkillFamily = skillLocator.special.skillFamily;


            //Note; you can change component.primary to component.secondary , component.utility and component.special
            var skillFamily = skillLocator.primary.skillFamily;

            //If this is an alternate skill, use this code.
            // Here, we add our skill as a variant to the exisiting Skill Family.
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)

            };

            //If this is the default/first skill, copy this code and remove the //,
            //skillFamily.variants[0] = new SkillFamily.Variant
            //{
            //    skillDef = mySkillDef,
            //    unlockableName = "",
            //    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            //};
        }
    }
}