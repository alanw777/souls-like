using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class EnemyStates : MonoBehaviour
    {
        public float health;
        public bool canBenParried = true;
        public bool parryIsOn = true;
        //public bool doParry = false;
        public bool isInviciable;
        public bool canMove;
        public bool isDead;
        public bool dontDoAnything;

        public Animator anim;
        private EnemyTarget enTarget;
        private AnimatorHook a_hook;
        public Rigidbody rigid;
        public float delta;
        public StateManager parriedBy;

        private List<Rigidbody> ragdollRigids=new List<Rigidbody>();
        private List<Collider> ragdollColliders=new List<Collider>();
        private float _actionDelay;
        private float timer;
        // Use this for initialization
        void Start()
        {
            health = 100;
            anim = GetComponentInChildren<Animator>();
            enTarget = GetComponent<EnemyTarget>();
            enTarget.Init(this);
            rigid = GetComponent<Rigidbody>();
            a_hook = anim.GetComponent<AnimatorHook>();
            if (a_hook == null)
                a_hook = anim.gameObject.AddComponent<AnimatorHook>();
            a_hook.Init(null, this);

            InitRagdoll();

            parryIsOn = false;
        }

        void InitRagdoll()
        {
            Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rigs.Length; i++)
            {
                if(rigs[i]==rigid)
                    continue;

                ragdollRigids.Add(rigs[i]);
                rigs[i].isKinematic = true;

                Collider col = rigs[i].gameObject.GetComponent<Collider>();
                col.isTrigger = true;
                ragdollColliders.Add(col);
            }
        }

        public void EnableRagdolls()
        {
            for (int i = 0; i < ragdollRigids.Count; i++)
            {
                ragdollRigids[i].isKinematic = false;
                ragdollColliders[i].isTrigger = false;
            }

            Collider col = rigid.gameObject.GetComponent<Collider>();
            col.enabled = false;
            rigid.isKinematic = true;
            StartCoroutine(CloseAnimator());
        }

        IEnumerator CloseAnimator()
        {
            yield return new WaitForEndOfFrame();
            anim.enabled = false;
            this.enabled = false;
        }

        void Update()
        {
            delta = Time.deltaTime;
            canMove = anim.GetBool("canMove");

            if (dontDoAnything)
            {
                dontDoAnything = !canMove;
                return;
            }

            if (health <= 0)
            {
                if (isDead == false)
                {
                    isDead = true;
                    EnableRagdolls();
                }
            }

            if (isInviciable)
            {
                _actionDelay += delta;
                if (_actionDelay > 0.1f)
                {
                    isInviciable = !canMove;
                    _actionDelay = 0;
                }
            }

            if (parriedBy != null && parryIsOn == false)
            {
//                parriedBy.parryTarget = null;
                parriedBy = null;
            }

            if (canMove)
            {
                parryIsOn = false;
                anim.applyRootMotion = false;

                //debug
                timer += Time.deltaTime;
                if (timer > 3)
                {
                    DoAction();
                    timer = 0;
                }
            }
           

            
        }

        void DoAction()
        {
            anim.Play("oh_attack_1");
            anim.applyRootMotion = true;
            anim.SetBool("canMove", false);
        }

        public void DoDamage(float v)
        {
            if(isInviciable)
                return;
            health -= v;
            isInviciable = true;
            anim.Play("damage_1");
            anim.applyRootMotion = true;
            anim.SetBool("canMove",false);
        }

        public void CheckForParry(Transform target,StateManager states)
        {
            if (canBenParried == false || parryIsOn == false || isInviciable)
                return;

            //面对敌人时才允许盾反
            Vector3 dir = transform.position - target.position;
            dir.Normalize();
            float dot = Vector3.Dot(target.forward, dir);
            if(dot<0)
                return;

            isInviciable = true;
            anim.Play("attack_interrupt");
            anim.applyRootMotion = true;
            anim.SetBool("canMove", false);
            //states.parryTarget = this;
            parriedBy = states;
        }

        public void IsGettingParried()
        {
            health -= 500;
            dontDoAnything = true;
            anim.SetBool("canMove", false);
            anim.Play("parry_recieved");
        }

        public void IsGettingBackstabed()
        {
            health -= 500;
            dontDoAnything = true;
            anim.SetBool("canMove", false);
            anim.Play("getting_backstabbed");
        }
    }

}

