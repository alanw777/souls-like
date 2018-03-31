using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class InventoryManager : MonoBehaviour
    {
        public Weapen rightHandWeapen;
        public Weapen leftHandWeapen;
        public bool hasLeftHandWeapen = true;

        public GameObject parryCollider;

        private StateManager states;
        public void Init(StateManager st)
        {
            states = st;
            EquipeWeapen(rightHandWeapen, false);
            EquipeWeapen(leftHandWeapen, true);
            CloseAllDamageColliders();

            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(states);
            CloseParryCollider();
        }

        public void EquipeWeapen(Weapen w, bool isLeft)
        {
            string targetIdle = w.oh_idle;
            targetIdle += isLeft ? "_l" : "_r";
            states.anim.SetBool("mirror",isLeft);
            states.anim.Play("changeWeapen");
            states.anim.Play(targetIdle);
        }

        public void CloseAllDamageColliders()
        {
            if(rightHandWeapen.w_hook !=null)
                rightHandWeapen.w_hook.CloseDamageColliders();
            if (leftHandWeapen.w_hook != null)
                leftHandWeapen.w_hook.CloseDamageColliders();
        }

        public void OpenAllDamageColliders()
        {
            if (rightHandWeapen.w_hook != null)
                rightHandWeapen.w_hook.OpenDamageColliders();
            if (leftHandWeapen.w_hook != null)
                leftHandWeapen.w_hook.OpenDamageColliders();
        }

        public void CloseParryCollider()
        {
            parryCollider.SetActive(false);
        }
        public void OpenParryCollider()
        {
            parryCollider.SetActive(true);
        }
    }
    [System.Serializable]
    public class Weapen
    {
        public string oh_idle;
        public string th_idle;

        public List<Action> actions;
        public List<Action> two_handenActions;
        public bool leftHandMirror;
        public GameObject weapenModel;
        public WeapenHook w_hook;

        public Action GetAction(List<Action> l, ActionInput input)
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (input == l[i].input)
                {
                    return l[i];
                }
            }
            return null;
        }
    }

}

