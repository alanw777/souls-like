using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class ActionManager : MonoBehaviour
    {
        public List<Action> actionSlots = new List<Action>();
        public ItemAction consumableAction;

        public StateManager states;

        public void Init(StateManager st)
        {
            states = st;

            UpdateActionsOneHanded();
        }

        public void UpdateActionsOneHanded()
        {
            EmptyAllSlots();
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapen.instance, ActionInput.rb, ActionInput.rb,actionSlots);
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapen.instance, ActionInput.rt, ActionInput.rt, actionSlots);
            if (states.inventoryManager.hasLeftHandWeapen)
            {
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapen.instance, ActionInput.rb, ActionInput.lb, actionSlots,true);
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapen.instance, ActionInput.rt, ActionInput.lt, actionSlots,true);
            }
            else
            {
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapen.instance, ActionInput.lb, ActionInput.lb, actionSlots);
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapen.instance, ActionInput.lt, ActionInput.lt, actionSlots);
            }
        }

        

        public void UpdateActionsTwoHanded()
        {
            EmptyAllSlots();
            Weapen w = states.inventoryManager.rightHandWeapen.instance;
            for (int i = 0; i < w.two_handenActions.Count; i++)
            {
                Action a = StaticFunctions.GetAction(w.two_handenActions[i].input,actionSlots);
                a.targetAnim = w.two_handenActions[i].targetAnim;
                a.type = w.two_handenActions[i].type;
            }
        }

        public void EmptyAllSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = StaticFunctions.GetAction((ActionInput)i,actionSlots);
                a.targetAnim = null;
                a.mirror = false;
                a.type = ActionType.attack;
            }
        }

        public Action GetActionSlot(StateManager st)
        {
            ActionInput input = GetActionInput(st);
            return StaticFunctions.GetAction(input,actionSlots);
        }

        

        public ActionInput GetActionInput(StateManager st)
        {
            if(st.rb)
                return ActionInput.rb;
            if (st.lb)
                return ActionInput.lb;
            if (st.rt)
                return ActionInput.rt;
            if (st.lt)
                return ActionInput.lt;

            return ActionInput.rb;
        }

        public bool IsLeftHand(Action slot)
        {
            return slot.input == ActionInput.lb || slot.input == ActionInput.lt;
        }

        ActionManager()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = new Action();
                a.input = (ActionInput)i;
                actionSlots.Add(a);
            }
        }
    }

    public enum ActionInput
    {
        rb,lb,rt,lt
    }

    public enum ActionType
    {
        attack,block,spells,parry
    }

    public enum SpellClass
    {
        pyromacy, miracles, sorcery
    }
    public enum SpellType
    {
        projectile, buff, looping
    }

    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public ActionType type;
        public SpellClass spellClass;
        public string targetAnim;
        public bool mirror = false;
        public bool canBenParried = true;
        public bool changeSpeed = false;
        public float animSpeed = 1;
        public bool canBackstab = false;
        public bool canParry = false;

        public bool overrideDamageAnim;
        public string damageAnim;

        public WeapenStats weapenStats;
        [HideInInspector]
        public float parryMultiplier;
        [HideInInspector]
        public float backstabMultiplier;
   
    }

    [System.Serializable]
    public class SpellAction
    {
        public ActionInput input;
        public string targetAnim;
        public string throwAnim;
        public float castTime;
    }

    [System.Serializable]
    public class ItemAction
    {
        public string targetAnim;
        public string item_id;
    }

}

