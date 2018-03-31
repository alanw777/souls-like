using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace AW
{
    public class ParryCollider : MonoBehaviour
    {
        private StateManager states;
        private EnemyStates eStates;
        public float maxTimer = 0.6f;
        private float timer;

        public void InitPlayer(StateManager st)
        {
            states = st;
        }
        public void InitEnemy(EnemyStates st)
        {
            eStates = st;
        }

        void Update()
        {
            if (states)
            {
                timer += states.delta;
                if (timer > maxTimer)
                {
                    timer = 0;
                    gameObject.SetActive(false);
                }
            }
            if (eStates)
            {
                timer += eStates.delta;
                if (timer > maxTimer)
                {
                    timer = 0;
                    gameObject.SetActive(false);
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {

            if (states)
            {
                EnemyStates e_st = other.GetComponentInParent<EnemyStates>();

                if (e_st != null)
                {
                    e_st.CheckForParry(transform.root,states);
                }
            }

            if (eStates)
            {
                //check for player
            }
        }
    }
}

