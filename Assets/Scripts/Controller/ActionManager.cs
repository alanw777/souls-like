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
            DeepCopyAction(states.inventoryManager.rightHandWeapen, ActionInput.rb, ActionInput.rb);
            DeepCopyAction(states.inventoryManager.rightHandWeapen, ActionInput.rt, ActionInput.rt);
            if (states.inventoryManager.hasLeftHandWeapen)
            {
                DeepCopyAction(states.inventoryManager.leftHandWeapen, ActionInput.rb, ActionInput.lb, true);
                DeepCopyAction(states.inventoryManager.leftHandWeapen, ActionInput.rt, ActionInput.lt, true);
            }
            else
            {
                DeepCopyAction(states.inventoryManager.rightHandWeapen, ActionInput.lb, ActionInput.lb);
                DeepCopyAction(states.inventoryManager.rightHandWeapen, ActionInput.lt, ActionInput.lt);
            }
        }

        void DeepCopyAction(Weapen w, ActionInput inp, ActionInput assign,bool isLeftHand = false)
        {
            Action a = GetAction(assign);
            Action w_a = w.GetAction(w.actions, inp);
            if(w_a==null)
                return;
            a.targetAnim = w_a.targetAnim;
            a.type = w_a.type;
            a.canBenParried = w_a.canBenParried;
            a.changeSpeed = w_a.changeSpeed;
            a.animSpeed = w_a.animSpeed;
            a.canBackstab = w_a.canBackstab;
            if (isLeftHand)
            {
                a.mirror = true;
            }
        }

        public void UpdateActionsTwoHanded()
        {
            EmptyAllSlots();
            Weapen w = states.inventoryManager.rightHandWeapen;
            for (int i = 0; i < w.two_handenActions.Count; i++)
            {
                Action a = GetAction(w.two_handenActions[i].input);
                a.targetAnim = w.two_handenActions[i].targetAnim;
                a.type = w.two_handenActions[i].type;
            }
        }

        public void EmptyAllSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = GetAction((ActionInput) i);
                a.targetAnim = null;
                a.mirror = false;
                a.type = ActionType.attack;
            }
        }

        public Action GetActionSlot(StateManager st)
        {
            ActionInput input = GetActionInput(st);
            return GetAction(input);
        }

        private Action GetAction(ActionInput inp)
        {
            for (int i = 0; i < actionSlots.Count; i++)
            {
                if (actionSlots[i].input == inp)
                    return actionSlots[i];
            }
            return null;
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

    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public ActionType type;
        public string targetAnim;
        public bool mirror = false;
        public bool canBenParried = true;
        public bool changeSpeed = false;
        public float animSpeed = 1;
        public bool canBackstab = false;
    }

    [System.Serializable]
    public class ItemAction
    {
        public string targetAnim;
        public string item_id;
    }

}

