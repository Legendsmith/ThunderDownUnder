using System;
using BepInEx;
using R2API;
using RoR2;
using System.Reflection;
using UnityEngine;
using RoR2.Projectile;

namespace ThunderDownUnder.ImprovedArtificer
{
    [BepInPlugin("com.ThunderDownUnder.ImprovedArtificer", "ImprovedArtificer", "1.0.1")]
    //[BepInDependency("R2API")]
    public class ImprovedArtificerMain: BaseUnityPlugin
    {
        public void Awake()
        {
            Chat.AddMessage("Loaded ImprovedArtificer!");
            SurvivorAPI.SurvivorCatalogReady += delegate (object s, EventArgs e)
            {
                //get commando body
                GameObject gameObject = BodyCatalog.FindBodyPrefab("MageBody");
                GenericSkill special = gameObject.GetComponent<SkillLocator>().special;
                special.activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Mage.Weapon.MageSpecial));
                object box = special.activationState;
                var field = typeof(EntityStates.SerializableEntityStateType)?.GetField("_typeName", BindingFlags.NonPublic | BindingFlags.Instance);
                field?.SetValue(box, typeof(EntityStates.Mage.Weapon.MageSpecial)?.AssemblyQualifiedName);
                special.activationState = (EntityStates.SerializableEntityStateType)box;
                special.beginSkillCooldownOnSkillEnd = true;
                special.baseRechargeInterval = 10f;
                special.canceledFromSprinting = false;
                special.noSprint = false;
                special.skillNameToken = "Detonation";
                special.skillDescriptionToken = "Leaps up in the air and aims an instant explosion. Press R again to Detonate on your target.";
            };
        }
    }
}


namespace EntityStates.Mage.Weapon
{
    public class MageSpecial : BaseState
    {
        public Vector3 idealHeight;
        public float prepDuration;
        public float baseprepduration=0.5f;
        public float stopwatch;
        public Vector3 flyVector;
        public Vector3 startPosition;
        public Boolean beginFly;
        public Boolean endFly;
        public EffectManager GFX;
        public override void OnEnter()
        {
            base.OnEnter();
            //TODO: insert sound
            this.prepDuration = this.baseprepduration / this.attackSpeedStat;
            base.PlayAnimation("Gesture, Additive", "FireWall");
            startPosition=base.transform.position;
            if (base.characterMotor)
            {
                base.characterMotor.velocity = Vector3.zero;
            }
            flyVector = Vector3.up;
            if (base.cameraTargetParams)
            {
                base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
            }
            //effects
            EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/flamethrowereffect"), new EffectData
            {
                origin = base.transform.position,
                scale = 1,

            }, true);

        }
        public override void OnExit()
        {
            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
            //movement
            if (this.stopwatch >= this.prepDuration && base.characterMotor && !endFly)
            {
                base.characterMotor.rootMotion += this.flyVector * (base.characterBody.jumpPower * 4 * Time.fixedDeltaTime);

            }
            if (stopwatch > 2f && !endFly)
            {
                endFly = true;
                this.outer.SetNextState(new MageDetonate());
            }
        }
    }
    public class MageDetonate : BaseState
    {
        private GameObject areaIndicatorInstance;
        //private GameObject muzzleFlashprefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashmagelightninglarge");
        public bool fireDetonate;
        public float stopwatch;
        public float maxDuration = 2f;
        public GameObject muzzleFlash = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashmagefirelarge");
        public GameObject explosioneffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/explosivepotexplosion");
        public BlastAttack attack;

        public override void OnEnter()
        {
            base.OnEnter();
            fireDetonate = false;
            stopwatch = 0f;
            this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Huntress.ArrowRain.areaIndicatorPrefab);
            this.areaIndicatorInstance.transform.localScale = new Vector3(20, 20, 20);
        }
        public override void OnExit()
        {
            
            if (base.cameraTargetParams)
            {
                base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
            }
            if (this.areaIndicatorInstance)
            {
                if (this.fireDetonate)
                {
                    EffectManager.instance.SimpleMuzzleFlash(muzzleFlash, base.gameObject, "MuzzleLeft", true);
                    EffectManager.instance.SimpleMuzzleFlash(muzzleFlash, base.gameObject, "MuzzleRight", true);

                    EffectManager.instance.SpawnEffect(explosioneffect, new EffectData
                    {
                        origin = this.areaIndicatorInstance.transform.position,
                        scale = 30f
                    }, true);
                    new BlastAttack
                    {
                    attacker = base.gameObject,
                    radius = 30,
                    inflictor = base.gameObject,
                    procCoefficient = 2f,
                    baseDamage = base.characterBody.damage * 10f,
                    falloffModel = BlastAttack.FalloffModel.None,
                    damageType = DamageType.BypassArmor,
                    baseForce = 20f * base.characterBody.damage,
                    teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
                    position = this.areaIndicatorInstance.transform.position,
                    crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
                    }.Fire();

                }
            }
            EntityState.Destroy(this.areaIndicatorInstance.gameObject);
            base.outer.SetNextStateToMain();
            base.OnExit();
            //TODO: SOUND

        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            base.characterMotor.velocity = Vector3.zero;
            if(base.skillLocator&&base.skillLocator.secondary.CanExecute() && base.inputBank.skill2.justPressed && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
            if ((this.stopwatch >= maxDuration || base.inputBank.skill1.justPressed || base.inputBank.skill4.justPressed) && base.isAuthority)
            {
                this.skillLocator.primary.CancelInvoke();
                this.fireDetonate = true;
                this.outer.SetNextStateToMain();
            }
        }
        public override void Update()
        {
            base.Update();
            if (areaIndicatorInstance)
            {
                float maxdist = 1000f;
                RaycastHit raycastHit;
                if(Physics.Raycast(base.GetAimRay(),out raycastHit, maxdist, LayerIndex.CommonMasks.bullet))
                {
                    this.areaIndicatorInstance.transform.position = raycastHit.point;
                    this.areaIndicatorInstance.transform.up = raycastHit.normal;
                }
            }
        }
    }

    //obsolete
    public class MageDash:BaseState
    {
        public float baseDuration = 5f;

        public float duration;
        private Vector3 idealDirection;
        private float damagemulti = 5f;
        internal BlastAttack attack;
        private GameObject hiteffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/claypotprojectileexplosion");
        public float speedMultiplier = 2f;
        private float blasttimer = 0f;
        private float interval = 2f;
        private Transform jetOnEffect;
        public override void OnEnter()
        {
            this.jetOnEffect = base.FindModelChild("JetOn");
            if (this.jetOnEffect)
            {
                this.jetOnEffect.gameObject.SetActive(true);
            }
            base.OnEnter();
            this.duration = baseDuration;
            base.characterMotor.airControl = 1;
            if (base.isAuthority)
            {
                if (base.inputBank)
                {
                    this.idealDirection = base.inputBank.aimDirection;
                }
                this.UpdateDirection();

            };

        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
            if (base.isAuthority)
            {
                //we are sprinting so make sure that's a thing
                if (base.characterBody)
                {
                    base.characterBody.isSprinting = true;
                }
                this.UpdateDirection();
                //movement
                if (base.characterDirection)
                {
                    base.characterDirection.moveVector = this.idealDirection;
                    if (base.inputBank.jump.down)
                    {
                        Vector3 x = this.idealDirection + Vector3.up;
                        x.Normalize();
                        base.characterDirection.moveVector = x.normalized;
                    }

                    if (base.characterMotor)
                    {
                        base.characterMotor.rootMotion += this.GetIdealVelocity() * Time.fixedDeltaTime;
                    }
                }
                //damage
                this.blasttimer -= Time.fixedDeltaTime;
                if (this.blasttimer <= 0f)
                {
                    this.blasttimer = this.interval;
                    Vector3 position = base.transform.position + Vector3.up;
                    EffectManager.instance.SpawnEffect(this.hiteffect, new EffectData
                    {
                        origin = position,
                        scale = 60
                    },true);
                    //make the attack
                    attack.radius = 60;
                    attack.procCoefficient = 2f;
                    attack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                    attack.baseDamage = base.characterBody.damage * damagemulti;
                    attack.falloffModel = BlastAttack.FalloffModel.None;
                    attack.damageType = DamageType.IgniteOnHit;
                    attack.baseForce = 0f;
                    attack.attacker = base.gameObject;
                    attack.teamIndex = TeamComponent.GetObjectTeam(attack.attacker);
                    attack.position = base.transform.position;

                    attack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                    attack.Fire();
                    this.blasttimer = 0;
                }

            }
        }
        public override void OnExit()
        {
            base.OnExit();
            base.characterMotor.airControl = 0.25f;
            if (this.jetOnEffect)
            {
                this.jetOnEffect.gameObject.SetActive(false);
            }
        }
    
        private Vector3 GetIdealVelocity()
        {
            return base.characterDirection.forward * base.characterBody.moveSpeed * this.speedMultiplier;
        }
        private void UpdateDirection()
        {
            if (base.inputBank)
            {
                Vector3 vector = base.GetAimRay().direction;
                if (vector != Vector3.zero)
                {
                    vector.Normalize();
                    this.idealDirection = vector.normalized;
                }
            }
        }
    }
}