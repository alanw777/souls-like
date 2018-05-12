using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class BreathCollider : MonoBehaviour {

        public void OnTriggerEnter(Collider other)
        {
            EnemyStates e = other.GetComponentInParent<EnemyStates>();
            if (e != null)
            {
                e.DoDamage_();
                SpellEffectsManager.singleton.UseSpellEffect("onfire",null,e);
            }
        }
    }


}
