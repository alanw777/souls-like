using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class SpellEffectsManager : MonoBehaviour
    {
        Dictionary<string,int> s_effects = new Dictionary<string, int>();

        public void UseSpellEffect(string id, StateManager c, EnemyStates e = null)
        {
            int index = GetEffect(id);
            if (index == -1)
            {
                Debug.Log("sepll effect does't exist!");
                return;
            }
            switch (index)
            {
                case 0:
                    FireBreath(c);
                    break;
                case 1:
                    DarkShield(c);
                    break;
                case 2:
                    HealingSmall(c);
                    break;
                case 3:
                    FireBall(c);
                    break;
                case 4:
                    OnFire(c,e);
                    break;
            }

        }

        int GetEffect(string id)
        {
            int index = -1;
            if (s_effects.TryGetValue(id, out index))
            {
                index = s_effects[id];
            }
            return index;
        }

        void FireBreath(StateManager c)
        {
            c.spellcast_start = c.inventoryManager.OpenBreathCollider;
            c.spellcast_loop = c.inventoryManager.EmitSpellPartcle;
            c.spellcast_stop = c.inventoryManager.CloseBreathCollider;
        }

        void DarkShield(StateManager c)
        {
            c.spellcast_start = c.inventoryManager.OpenBlockCollider;
            c.spellcast_loop = c.inventoryManager.EmitSpellPartcle;
            c.spellcast_stop = c.inventoryManager.CloseBlockCollider;
        }

        void HealingSmall(StateManager c)
        {
            c.spellcast_loop = c.AddHealth;
        }

        void FireBall(StateManager c)
        {
            c.spellcast_start = null;
            c.spellcast_loop = c.inventoryManager.EmitSpellPartcle;
            c.spellcast_stop = null;
        }

        void OnFire(StateManager c, EnemyStates e)
        {
            if (e != null)
            {
                e.spellEffect_loop = e.OnFire;
            }
        }

        public static SpellEffectsManager singleton;

        void Awake()
        {
            singleton = this;

            s_effects.Add("firebreath",0);
            s_effects.Add("darkshield", 1);
            s_effects.Add("healingsmall",2);
            s_effects.Add("fireball", 3);
            s_effects.Add("onfire",4);
        }
    }

}


