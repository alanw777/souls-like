using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class StateManager : MonoBehaviour
    {
        [Header("Init")]
        public GameObject activeModel;

        [Header("Stats")] 
        public Attributes attributes;
        public CharacterStats characterStats;

        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public bool rt, rb, lt, lb;
        public bool rollInput;
        public bool itemInput;

        [Header("Stats")] 
        public float moveSpeed = 2;
        public float runSpeed = 3.5f;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;
        public float rollSpeed = 1;
        public float parriedOffset = 1.4f;

        [Header("States")] 
        public bool onGround;
        public bool run;
        public bool lockOn;
        public bool inAction;
        public bool isSpellCasting;
        public bool canMove;
        public bool isTwoHanded;
        public bool usingItem;
        public bool isBlocking;
        public bool isLeftHand;
        public bool canBeParried;
        public bool parryIsOn;

        [Header("Other")] 
        public EnemyTarget lockOnTarget;
        public Transform LockonTransform;
        public AnimationCurve roll_curve;
        //public EnemyStates parryTarget;

        [HideInInspector] public float delta;

        [HideInInspector] public Animator anim;

        [HideInInspector] public Rigidbody rigid;

        [HideInInspector] public LayerMask ignoreLayers;

        [HideInInspector] public AnimatorHook a_hook;

        [HideInInspector] public ActionManager actionManager;

        [HideInInspector] public InventoryManager inventoryManager;

        [HideInInspector] public Action currentAction;

        private float _actionDelay;

        public void Init()
        {
            SetupAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            inventoryManager = GetComponent<InventoryManager>();
            inventoryManager.Init(this);

            actionManager = GetComponent<ActionManager>();
            actionManager.Init(this);

            a_hook = activeModel.GetComponent<AnimatorHook>();
            if(a_hook==null)
                a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this,null);


            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);

            anim.SetBool(StaticStrings.onGround,true);
        }

        void SetupAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    Debug.Log("No model found");
                }
                else
                {
                    activeModel = anim.gameObject;
                }
            }

            if (anim == null)
                anim = activeModel.GetComponent<Animator>();

            anim.applyRootMotion = false;
        }

        public void FixedTick(float d)
        {
            delta = d;

            isBlocking = false;
            usingItem = anim.GetBool(StaticStrings.interacting);
            anim.SetBool(StaticStrings.spellcasting,isSpellCasting);
            DetectAction();
            DetectItemAction();

            inventoryManager.rightHandWeapen.weapenModel.SetActive(!usingItem);

            anim.SetBool(StaticStrings.blocking, isBlocking);
            anim.SetBool(StaticStrings.isLeft, isLeftHand);

            if (inAction)
            {
                anim.applyRootMotion = true;

                _actionDelay += delta;
                if (_actionDelay > 0.3f) //crossfade time;
                {
                    inAction = false;
                    _actionDelay = 0;
                }
                else
                {
                    return;
                }
            }

            canMove = anim.GetBool(StaticStrings.canMove);
            if(!canMove)
                return;

           
            

            anim.applyRootMotion = false;

            rigid.drag = (moveAmount > 0 || onGround==false) ? 0 : 4;
            float targetSpeed = moveSpeed;
            if (usingItem || isSpellCasting)
            {
                run = false;
                moveAmount = Mathf.Clamp(moveAmount, 0, 0.5f);
            }
               
            if (run)
                targetSpeed = runSpeed;
            if(onGround)
                rigid.velocity = moveDir * (targetSpeed*moveAmount);
            if (run)
                lockOn = false;

            HandleRotation();

            anim.SetBool(StaticStrings.lockon, lockOn);
            if (lockOn)
                HandleLockonAnimations(moveDir);
            else
                HandleMovementAnimations();

            if (isSpellCasting)
            {
                HandleSpellcasting();
                return;
            }
            

            a_hook.CloseRoll();
            HandleRolls();
        }

        private void HandleRotation()
        {
            Vector3 targetDir;
            if (lockOn == false)
                targetDir = moveDir;
            else
            {
                if (LockonTransform != null)
                    targetDir = LockonTransform.transform.position - transform.position;
                else
                    targetDir = moveDir;
            }

            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = transform.forward;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta*moveAmount*rotateSpeed);
            transform.rotation = targetRotation;
        }

        void DetectItemAction()
        {
            if(canMove==false||usingItem||isBlocking)
                return;
            if(itemInput==false)
                return;
            ItemAction slot = actionManager.consumableAction;
            string targetAnim = slot.targetAnim;
            if(string.IsNullOrEmpty(targetAnim))
                return;
            usingItem = true;
            anim.Play(targetAnim);
        }

        void DetectAction()
        {
            if(canMove==false || usingItem || isSpellCasting)
                return;

            if(rt==false && rb==false && lt==false && lb==false)
                return;

            Action slot = actionManager.GetActionSlot(this);
            if (slot == null)
                return;
            switch (slot.type)
            {
                case ActionType.attack:
                    AttackAction(slot);
                    break;
                case ActionType.block:
                    BlockAction(slot);
                    break;
                case ActionType.spells:
                    SpellAction(slot);
                    break;
                case ActionType.parry:
                    ParryAction(slot);
                    break;
                default:
                    break;
            }
            

        }

        private void SpellAction(Action slot)
        {
            
            if (slot.spellClass != inventoryManager.currentSpell.instance.spellClass)
            {
                Debug.Log("Spell Class doesn't match");
                return;
            }

            ActionInput inp = actionManager.GetActionInput(this);
            if(inp == ActionInput.lb)
                inp = ActionInput.rb;
            if(inp == ActionInput.lt)
                inp = ActionInput.rt;
            Spell s_inst = inventoryManager.currentSpell.instance;
            SpellAction s_slots = s_inst.GetAction(s_inst.actions, inp);
            if (s_slots == null)
            {
                Debug.Log("can not find spell slot");
                return;
            }

            SpellEffectsManager.singleton.UseSpellEffect(s_inst.spell_effect,this);

            isSpellCasting = true;
            spellcastTime = 0;
            maxSpellcastTime = s_slots.castTime;
            spellTargetAnim = s_slots.throwAnim;
            spellMirror = slot.mirror;
            curSpellType = s_inst.spellType;

            string targetAnim = s_slots.targetAnim;
            if (spellMirror)
            {
                targetAnim += "_l";
            }
            else
            {
                targetAnim += "_r";
            }
            anim.SetBool(StaticStrings.spellcasting,true);
            anim.SetBool(StaticStrings.mirror, spellMirror);
            projectileCanidate = inventoryManager.currentSpell.instance.projectile;
            inventoryManager.CreateSpellPartcle(inventoryManager.currentSpell, spellMirror,s_inst.spellType==SpellType.looping);
            anim.CrossFade(targetAnim, 0.2f);

            if (spellcast_start != null)
                spellcast_start();
        }

        private float spellcastTime;
        private float maxSpellcastTime;
        private string spellTargetAnim;
        private bool spellMirror;
        private GameObject projectileCanidate;
        private SpellType curSpellType;

        public delegate void SpellCast_Start();
        public delegate void SpellCast_Loop();
        public delegate void SpellCast_Stop();

        public SpellCast_Start spellcast_start;
        public SpellCast_Loop spellcast_loop;
        public SpellCast_Stop spellcast_stop;


        void HandleSpellcasting()
        {
            if (spellcast_loop != null)
                spellcast_loop();
            if (curSpellType == SpellType.looping)
            {
                if (rb == false && lb == false)
                {
                    if (spellcast_stop != null)
                        spellcast_stop();
                    isSpellCasting = false;
                    return;
                }
                return;
            }

            spellcastTime += delta;
            if (spellcastTime > maxSpellcastTime)
            {
                isSpellCasting = false;
                canMove = false;
                inAction = true;

                spellcastTime = 0;
                string targetAnim = spellTargetAnim;
                anim.SetBool(StaticStrings.mirror, spellMirror);
                anim.CrossFade(targetAnim, 0.2f);
            }
        }

        public void ThrowProjectile()
        {
            if(projectileCanidate==null)
                return;

            GameObject go = Instantiate(projectileCanidate) as GameObject;
            Transform p = anim.GetBoneTransform(spellMirror ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            go.transform.position = p.position;
            if (LockonTransform && lockOn)
            {
                go.transform.rotation = Quaternion.LookRotation(LockonTransform.position + new Vector3(0, 1.5f, 0) - go.transform.position);
            }
            else
            {
                go.transform.rotation = transform.rotation;
            }
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.Init();
        }

        void AttackAction(Action slot)
        {
            if(CheckForParry(slot))
                return;

            if(CheckForBackstab(slot))
                return;

            string targetAnim = null;
            
            targetAnim = slot.targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
                return;

            currentAction = slot;
            canMove = false;
            inAction = true;
            canBeParried = slot.canBenParried;
            float targetSpeed = 1;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }
            anim.SetFloat(StaticStrings.animSpeed, targetSpeed);
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(targetAnim, 0.2f);
     
        }

        bool CheckForParry(Action slot)
        {
            if (slot.canParry == false)
                return false;

            EnemyStates parryTarget = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, rayDir, out hit, 3, ignoreLayers))
            {
                parryTarget = hit.transform.GetComponentInParent<EnemyStates>();
            }

            if (parryTarget == null)
                return false;

            if (parryTarget.parriedBy == null)
                return false;

            Vector3 dir = parryTarget.transform.position - transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(transform.forward, dir);
            if (angle < 60)
            {
                Vector3 targetPosition = -dir*parriedOffset;
                targetPosition += parryTarget.transform.position;
                transform.position = targetPosition;
                if (dir == Vector3.zero)
                    dir = -parryTarget.transform.position;
                Quaternion eRotation = Quaternion.LookRotation(-dir);
                Quaternion ourRotation = Quaternion.LookRotation(dir);

                parryTarget.transform.rotation = eRotation;
                transform.rotation = ourRotation;
                parryTarget.IsGettingParried(slot);

                canMove = false;
                inAction = true;
                anim.SetBool(StaticStrings.mirror, slot.mirror);
                anim.CrossFade(StaticStrings.parry_attack, 0.2f);
                return true;
            }
            return false;
        }

        bool CheckForBackstab(Action slot)
        {
            if (slot.canBackstab == false)
                return false;
            EnemyStates backstab = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, rayDir, out hit, 3, ignoreLayers))
            {
                backstab = hit.transform.GetComponentInParent<EnemyStates>();
            }

            if (backstab == null)
                return false;

            Vector3 dir = transform.position- backstab.transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(backstab.transform.forward, dir);
            if (angle > 150)
            {
                Vector3 targetPosition = dir * parriedOffset;
                targetPosition += backstab.transform.position;
                transform.position = targetPosition;
                backstab.transform.rotation = transform.rotation;
                backstab.IsGettingBackstabed(slot);

                canMove = false;
                inAction = true;
                anim.SetBool(StaticStrings.mirror, slot.mirror);
                anim.CrossFade(StaticStrings.parry_attack, 0.2f);
                //lockOnTarget = null;
                return true;
            }
            return false;
        }

        void BlockAction(Action slot)
        {
            isBlocking = true;
            isLeftHand = slot.mirror;
        }

        void ParryAction(Action slot)
        {
            string targetAnim = null;

            targetAnim = slot.targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
                return;

     
            canMove = false;
            inAction = true;
            float targetSpeed = 1;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }
            anim.SetFloat(StaticStrings.animSpeed, targetSpeed);
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(targetAnim, 0.2f);
            //lockOnTarget = null;
        }

        public void IsGettingParried()
        {
            
        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
            anim.SetBool(StaticStrings.onGround, onGround);

        }

        void HandleRolls()
        {
            if(!rollInput || usingItem)
                return;

            float v = vertical;
            float h = horizontal;

            v = moveAmount > 0.3f ? 1 : 0;
            h = 0;

            if (v != 0)
            {
                if (moveDir == Vector3.zero)
                    moveDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = targetRot;
                a_hook.rm_multi = rollSpeed;
                a_hook.InitForRoll();
            }
            else
            {
                a_hook.rm_multi = 1.3f;                
            }


            anim.SetFloat(StaticStrings.vertical, v);
            anim.SetFloat(StaticStrings.horizontal, h);

            canMove = false;
            inAction = true;
            anim.CrossFade(StaticStrings.Rolls, 0.2f);
        }
        void HandleMovementAnimations()
        {
            anim.SetBool(StaticStrings.run, run);
            anim.SetFloat(StaticStrings.vertical, moveAmount, 0.4f, delta);

        }

        void HandleLockonAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;
            anim.SetFloat(StaticStrings.vertical, v, 0.2f, delta);
            anim.SetFloat(StaticStrings.horizontal, h, 0.2f, delta);
        }

        public void HandleTwoHanded()
        {
            bool isRight = true;
            Weapen w = inventoryManager.rightHandWeapen.instance;
            if (w == null)
            {
                w = inventoryManager.leftHandWeapen.instance;
                isRight = false;
            }
            if(w==null)
                return;
            if (isTwoHanded)
            {
                anim.CrossFade(w.th_idle,0.2f);
                actionManager.UpdateActionsTwoHanded();
                if (isRight)
                {
                    if (inventoryManager.leftHandWeapen)
                    {
                        inventoryManager.leftHandWeapen.weapenModel.SetActive(false);
                    }
                        
                }
                else
                {
                    if (inventoryManager.rightHandWeapen)
                        inventoryManager.rightHandWeapen.weapenModel.SetActive(false);
                }
            }
            else
            {
                anim.Play(StaticStrings.changeWeapen);
                anim.Play(StaticStrings.emptyBoth);
                actionManager.UpdateActionsOneHanded();
                if (isRight)
                {
                    if (inventoryManager.leftHandWeapen)
                        inventoryManager.leftHandWeapen.weapenModel.SetActive(true);
                }
                else
                {
                    if (inventoryManager.rightHandWeapen)
                        inventoryManager.rightHandWeapen.weapenModel.SetActive(true);
                }
            }
                

        }

        public bool OnGround()
        {
            bool r = false;
            Vector3 origin = transform.position + Vector3.up*toGround;
            Vector3 dir = -Vector3.up;
            float dis = toGround + 0.3f;

            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, dis, ignoreLayers))
            {
                r = true;
                transform.position = hit.point;
            }

            return r;
        }

        public void AddHealth()
        {
            characterStats.hp++;
        }
    }

}

