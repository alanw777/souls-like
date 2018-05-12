using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class AnimatorHook : MonoBehaviour
    {
        private Animator anim;
        private StateManager states;
        private EnemyStates eStates;
        private Rigidbody rigid;

        public float rm_multi;
        private bool rolling;
        private float roll_t;
        private AnimationCurve rollCurve;
        private float delta;

        public void Init(StateManager st,EnemyStates eSt)
        {
            states = st;
            eStates = eSt;
            if (st != null)
            {
                anim = st.anim;
                rigid = st.rigid;
                rollCurve = st.roll_curve;
                delta = st.delta;
            }
            if (eSt != null)
            {
                anim = eSt.anim;
                rigid = eSt.rigid;
                delta = eSt.delta;
            }
        }

        public void InitForRoll()
        {
            rolling = true;
            roll_t = 0;
        }

        public void CloseRoll()
        {
            if(rolling==false)
                return;
            rm_multi = 1;
            rolling = false;
            roll_t = 0;
        }

        void OnAnimatorMove()
        {
            if(states == null && eStates == null)
                return;
            if(rigid==null)
                return;
            if (states != null)
            {
                if (states.canMove)
                    return;
                delta = states.delta;
            }
            if (eStates != null)
            {
                if(eStates.canMove)
                    return;
                delta = eStates.delta;
            }
            
            //if(states)
                rigid.drag = 0;

            if (rm_multi == 0)
                rm_multi = 1;

            if (rolling == false)
            {
                Vector3 delta2 = anim.deltaPosition;
                delta2.y = 0;
                Vector3 v = delta2 * rm_multi / delta;
                rigid.velocity = v;
            }
            else
            {
                if(states==null)
                    return;
                roll_t += delta / 0.6f;
                if (roll_t > 1)
                    roll_t = 1;
                float zValue = rollCurve.Evaluate(roll_t);
                Vector3 v1 = Vector3.forward*zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * rm_multi);
                rigid.velocity = v2;
            }
        }

        public void OpenDamageColliders()
        {
            if (states)
            {
                states.inventoryManager.OpenAllDamageColliders();
            }
            OpenParryFlag();
        }

        public void CloseDamageColliders()
        {
            if (states)
            {
                states.inventoryManager.CloseAllDamageColliders();
            }
            CloseParryFlag();
        }

        public void OpenParryColliders()
        {
            if (states == null)
                return;
            states.inventoryManager.OpenParryCollider();
        }

        public void CloseParryColliders()
        {
            if (states == null)
                return;
            states.inventoryManager.CloseParryCollider();

        }

        public void OpenParryFlag()
        {
            if (states)
                states.parryIsOn = true;
            if (eStates)
                eStates.parryIsOn = true;
        }

        public void CloseParryFlag()
        {
            if (states)
                states.parryIsOn = false;
            if (eStates)
                eStates.parryIsOn = false;
        }

        public void CloseParticle()
        {
            if (states)
            {
//                if(states.inventoryManager.currentSpell.currentParticle!=null)
//                    states.inventoryManager.currentSpell.currentParticle.SetActive(false);
            }
        }

        public void InitiateThrowForProjectile()
        {
            if (states)
            {
                states.ThrowProjectile();
            }
        }
    }


}
