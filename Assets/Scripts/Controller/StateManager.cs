using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class StateManager : MonoBehaviour
    {
        [Header("Init")]
        public GameObject activeModel;

        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public bool rt, rb, lt, lb;
        public bool rollInput;

        [Header("Stats")] 
        public float moveSpeed = 2;
        public float runSpeed = 3.5f;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;
        public float rollSpeed = 1;

        [Header("States")] 
        public bool onGround;
        public bool run;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool isTwoHanded;

        [Header("Other")] 
        public EnemyTarget lockOnTarget;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public LayerMask ignoreLayers;
        [HideInInspector]
        public AnimatorHook a_hook;

        private float _actionDelay;

        public void Init()
        {
            SetupAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this);

            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);

            anim.SetBool("onGround",true);
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

            DetectAction();
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

            canMove = anim.GetBool("canMove");
            if(!canMove)
                return;

            a_hook.rm_multi = 1;
            HandleRolls();
            

            anim.applyRootMotion = false;

            rigid.drag = (moveAmount > 0 || onGround==false) ? 0 : 4;
            float targetSpeed = moveSpeed;
            if (run)
                targetSpeed = runSpeed;
            if(onGround)
                rigid.velocity = moveDir * (targetSpeed*moveAmount);
            if (run)
                lockOn = false;
            
            Vector3 targetDir = lockOn ? lockOnTarget.transform.position - transform.position : moveDir;
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = transform.forward;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta*moveAmount*rotateSpeed);
            transform.rotation = targetRotation;

            anim.SetBool("lockon",lockOn);

            if (lockOn)
                HandleLockonAnimations(moveDir);
            else
                HandleMovementAnimations();


        }

        void DetectAction()
        {
            if(canMove==false)
                return;

            if(rt==false && rb==false && lt==false && lb==false)
                return;

            string targetAnim = null;
            if (rt)
                targetAnim = "oh_attack_1";
            if (rb)
                targetAnim = "oh_attack_2";
            if (lt)
                targetAnim = "oh_attack_3";
            if (lb)
                targetAnim = "th_attack_1";

            if(string.IsNullOrEmpty(targetAnim))
                return;

            canMove = false;
            inAction = true;
            anim.CrossFade(targetAnim,0.2f);

        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
            anim.SetBool("onGround", onGround);

        }

        void HandleRolls()
        {
            if(!rollInput)
                return;

            float v = vertical;
            float h = horizontal;

            v = moveAmount > 0.3f ? 1 : 0;
            h = 0;

//            if (lockOn)
//            {
//                if (Mathf.Abs(v) < 0.3f)
//                    v = 0;
//                if (Mathf.Abs(h) < 0.3f)
//                    h = 0;
//            }
//            else
//            {
//                v = moveAmount > 0.3f ? 1 : 0;
//                h = 0;
//            }

            if (v != 0)
            {
                if (moveDir == Vector3.zero)
                    moveDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = targetRot;
            }

            a_hook.rm_multi = rollSpeed;

            anim.SetFloat("vertical", v);
            anim.SetFloat("horizontal", h);

            canMove = false;
            inAction = true;
            anim.CrossFade("Rolls", 0.2f);

        }
        void HandleMovementAnimations()
        {
            anim.SetBool("run",run);
            anim.SetFloat("vertical", moveAmount, 0.4f, delta);

        }

        void HandleLockonAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;

            anim.SetFloat("vertical", v, 0.2f, delta);
            anim.SetFloat("horizontal", h, 0.2f, delta);
        }

        public void HandleTwoHanded()
        {
            anim.SetBool("two_handed",isTwoHanded);
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
    }

}

