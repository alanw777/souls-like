﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class InventoryManager : MonoBehaviour
    {
        public List<string> rh_weapens;
        public List<string> lh_weapens;
        public List<string> spell_items;

        public RuntimeSpellItems currentSpell;

        public int r_index;
        public int l_index;
        public int s_index;

        private readonly List<RuntimeWeapen> r_r_weapens = new List<RuntimeWeapen>();
        private readonly List<RuntimeWeapen> r_l_weapens = new List<RuntimeWeapen>();
        private readonly List<RuntimeSpellItems> r_spells = new List<RuntimeSpellItems>(); 

        public RuntimeWeapen leftHandWeapen;
        public RuntimeWeapen rightHandWeapen;
 
        public bool hasLeftHandWeapen = true;

        public GameObject parryCollider;
        public GameObject breathCollider;
        public GameObject blockCollider;

        private StateManager states;
        public void Init(StateManager st)
        {
            states = st;

            LoadInventory(st);

            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(states);
            CloseParryCollider();
            CloseBreathCollider();
            CloseBlockCollider();
        }

        private void LoadInventory(StateManager st)
        {
            for (int i = 0; i < rh_weapens.Count; i++)
            {
                WeapenToRuntimeWeapen(ResourcesManager.singleton.GetWeapen(rh_weapens[i]));
            }
            for (int i = 0; i < lh_weapens.Count; i++)
            {
                WeapenToRuntimeWeapen(ResourcesManager.singleton.GetWeapen(lh_weapens[i]),true);
            }
            if (r_r_weapens.Count > 0)
            {
                if (r_index > r_r_weapens.Count - 1)
                    r_index = 0;

                rightHandWeapen = r_r_weapens[r_index];
            }
            if (r_l_weapens.Count > 0)
            {
                if (l_index > r_l_weapens.Count - 1)
                    l_index = 0;

                leftHandWeapen = r_l_weapens[l_index];
            }
            for (int i = 0; i < spell_items.Count; i++)
            {
                SpellToRuntimeSpell(ResourcesManager.singleton.GetSpell(spell_items[i]));
            }
            if (r_spells.Count > 0)
            {
                EquipSpell(r_spells[0]);
            }

            if (rightHandWeapen != null)
                EquipeWeapen(rightHandWeapen, false);

            if (leftHandWeapen != null)
            {
                hasLeftHandWeapen = true;
                EquipeWeapen(leftHandWeapen, true);
            }
            
            InitAllDamageColliders(st);
            CloseAllDamageColliders();
        }

        public void EquipeWeapen(RuntimeWeapen w, bool isLeft)
        {
            if (isLeft)
            {
                if (leftHandWeapen != null)
                {
                    leftHandWeapen.weapenModel.SetActive(false);
                }
                leftHandWeapen = w;
            }
            else
            {
                if (rightHandWeapen != null)
                {
                    rightHandWeapen.weapenModel.SetActive(false);
                }
                rightHandWeapen = w;
            }

            string targetIdle = w.instance.oh_idle;
            targetIdle += isLeft ? "_l" : "_r";
            states.anim.SetBool(StaticStrings.mirror, isLeft);
            states.anim.Play(StaticStrings.changeWeapen);
            states.anim.Play(targetIdle);

            UI.QuickSlot uiSlot = UI.QuickSlot.singleton;
            uiSlot.UpdateSlot(isLeft?UI.QSlotType.lh:UI.QSlotType.rh,w.instance.icon);

            w.weapenModel.SetActive(true);
        }

        public void EquipSpell(RuntimeSpellItems spell)
        {
            currentSpell = spell;
            UI.QuickSlot uislot = UI.QuickSlot.singleton;
            uislot.UpdateSlot(UI.QSlotType.spell, spell.instance.icon);
        }

        public Weapen GetCurrentWeapen(bool isLeft)
        {
            return isLeft ? leftHandWeapen.instance : rightHandWeapen.instance;
        }

        public void CloseAllDamageColliders()
        {
            if (rightHandWeapen != null)
                rightHandWeapen.w_hook.CloseDamageColliders();
            if (leftHandWeapen != null)
                leftHandWeapen.w_hook.CloseDamageColliders();
        }

        public void InitAllDamageColliders(StateManager st)
        {
            if (rightHandWeapen != null)
                rightHandWeapen.w_hook.InitDamageColliders(st);
            if (leftHandWeapen != null)
                leftHandWeapen.w_hook.InitDamageColliders(st);
        }

        public void OpenAllDamageColliders()
        {
            if (rightHandWeapen != null)
                rightHandWeapen.w_hook.OpenDamageColliders();
            if (leftHandWeapen != null)
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

        public RuntimeWeapen WeapenToRuntimeWeapen(Weapen w,bool isLeft = false)
        {
            GameObject go = new GameObject();
            RuntimeWeapen inst = go.AddComponent<RuntimeWeapen>();
            inst.instance = new Weapen();
            StaticFunctions.DeepCopyWeapen(w,inst.instance);
            go.name = w.itemName;
            inst.weapenModel = Instantiate(inst.instance.modelPrefab) as GameObject;
            Transform p = states.anim.GetBoneTransform(isLeft ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            inst.weapenModel.transform.parent = p;
            inst.weapenModel.transform.localPosition = isLeft?inst.instance.l_model_pos:inst.instance.r_model_pos;
            inst.weapenModel.transform.localEulerAngles = isLeft?inst.instance.l_model_eulers:inst.instance.r_model_eulers;
            inst.weapenModel.transform.localScale = inst.instance.model_scale;
            inst.w_hook = inst.weapenModel.GetComponentInChildren<WeapenHook>();
            inst.w_hook.InitDamageColliders(states);

            if (isLeft)
            {
                r_l_weapens.Add(inst);
            }
            else
            {
                r_r_weapens.Add(inst);
            }
            inst.weapenModel.SetActive(false);
            return inst;
        }

        public void ChangeToNextWeapen(bool isLeft )
        {
            if (isLeft)
            {
                if (l_index < r_l_weapens.Count - 1)
                    l_index++;
                else
                    l_index = 0;

                EquipeWeapen(r_l_weapens[l_index],true);
            }
            else
            {
                if (r_index < r_r_weapens.Count - 1)
                    r_index++;
                else
                    r_index = 0;
                
                EquipeWeapen(r_r_weapens[r_index], false);
            }
            states.actionManager.UpdateActionsOneHanded();
            InitAllDamageColliders(states);
            CloseAllDamageColliders();
        }

        public void ChangeToNextSpell()
        {
            if (s_index < r_spells.Count - 1)
                s_index++;
            else
                s_index = 0;

            EquipSpell(r_spells[s_index]);
        }

        public RuntimeSpellItems SpellToRuntimeSpell(Spell s,bool isLeft = false)
        {
            GameObject go = new GameObject();
            RuntimeSpellItems inst = go.AddComponent<RuntimeSpellItems>();
            inst.instance = new Spell();
            StaticFunctions.DeepCopySpell(s,inst.instance);
            go.name = s.itemName;
            r_spells.Add(inst);
            return inst;
        }

        public void CreateSpellPartcle(RuntimeSpellItems inst, bool isLeft , bool parentUnderRoot = false)
        {
            if (inst.currentParticle == null)
            {
                inst.currentParticle = Instantiate(inst.instance.particlePrefab) as GameObject;
                inst.p_hook = inst.currentParticle.GetComponentInChildren<ParticleHook>();
                inst.p_hook.Init();
            }

            if (!parentUnderRoot)
            {
                Transform p = states.anim.GetBoneTransform(isLeft ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
                inst.currentParticle.transform.parent = p;
                inst.currentParticle.transform.localRotation = Quaternion.identity;
                inst.currentParticle.transform.localPosition = Vector3.zero;
            }
            else
            {
                inst.currentParticle.transform.parent = transform;
                inst.currentParticle.transform.localRotation = Quaternion.identity;
                inst.currentParticle.transform.localPosition = new Vector3(0,1.5f,0.9f);
            }
        }
        #region Delegate Calls
        public void OpenBreathCollider()
        {
            breathCollider.SetActive(true);
        }

        public void CloseBreathCollider()
        {
            breathCollider.SetActive(false);
        }

        public void OpenBlockCollider()
        {
            blockCollider.SetActive(true);
        }

        public void CloseBlockCollider()
        {
            blockCollider.SetActive(false);
        }

        public void EmitSpellPartcle()
        {
            currentSpell.p_hook.Emit(1);
        }
        #endregion
    }

    [System.Serializable]
    public class Item
    {
        public string itemName;
        public string itemDescription;
        public Sprite icon;
    }

    [System.Serializable]
    public class Weapen : Item
    {
        public string oh_idle;
        public string th_idle;

        public List<Action> actions;
        public List<Action> two_handenActions;
        public float parryMultiplier;
        public float backstabMultiplier;
        public bool leftHandMirror;

        public GameObject modelPrefab;

        public Vector3 l_model_pos;
        public Vector3 l_model_eulers;
        public Vector3 r_model_pos;
        public Vector3 r_model_eulers;
        public Vector3 model_scale;
        public Action GetAction(List<Action> l, ActionInput input)
        {
            if (l == null)
                return null;
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
    [System.Serializable]
    public class Spell : Item
    {
        public SpellType spellType;
        public SpellClass spellClass;
        public List<SpellAction> actions = new List<SpellAction>(); 
        public GameObject projectile;
        public GameObject particlePrefab;
        public string spell_effect;

        public SpellAction GetAction(List<SpellAction> l, ActionInput input)
        {
            if (l == null)
                return null;
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

