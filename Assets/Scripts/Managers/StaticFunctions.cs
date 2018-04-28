using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public static class StaticFunctions
    {
        public static void DeepCopyWeapen(Weapen from, Weapen to)
        {
            to.icon = from.icon;
            to.oh_idle = from.oh_idle;
            to.th_idle = from.th_idle;
            to.actions = new List<Action>();
            for (int i = 0; i < from.actions.Count; i++)
            {
                Action a = new Action();
                a.weapenStats = new WeapenStats();
                DeepCopyActionToAction(a,from.actions[i]);
                to.actions.Add(a);
            }
            to.two_handenActions = new List<Action>();
            for (int i = 0; i < from.two_handenActions.Count; i++)
            {
                Action a = new Action();
                a.weapenStats = new WeapenStats();
                DeepCopyActionToAction(a, from.actions[i]);
                to.two_handenActions.Add(a);
            }
            to.parryMultiplier = from.parryMultiplier;
            to.backstabMultiplier = from.backstabMultiplier;
            to.leftHandMirror = from.leftHandMirror;
            to.modelPrefab = from.modelPrefab;
            to.l_model_pos = from.l_model_pos;
            to.l_model_eulers = from.l_model_eulers;
            to.r_model_pos = from.r_model_pos;
            to.r_model_eulers = from.r_model_eulers;
            to.model_scale = from.model_scale;

        }

        public static void DeepCopyActionToAction(Action a, Action w_a)
        {
            a.input = w_a.input;
            a.targetAnim = w_a.targetAnim;
            a.type = w_a.type;
            a.canParry = w_a.canParry;
            a.canBenParried = w_a.canBenParried;
            a.changeSpeed = w_a.changeSpeed;
            a.animSpeed = w_a.animSpeed;
            a.canBackstab = w_a.canBackstab;
            a.overrideDamageAnim = w_a.overrideDamageAnim;
            a.damageAnim = w_a.damageAnim;
            a.spellClass = w_a.spellClass;
          
            a.canParry = w_a.canParry;
            DeepCopyWeapenStats(w_a.weapenStats, a.weapenStats);
        }

        public static void DeepCopyAction(Weapen w, ActionInput inp, ActionInput assign, List<Action> actionSlots, bool isLeftHand = false)
        {
            Action a = GetAction(assign, actionSlots);
            Action w_a = w.GetAction(w.actions, inp);
            if (w_a == null)
                return;
            a.targetAnim = w_a.targetAnim;
            a.spellClass = w_a.spellClass;
            a.type = w_a.type;
            a.canBenParried = w_a.canBenParried;
            a.changeSpeed = w_a.changeSpeed;
            a.animSpeed = w_a.animSpeed;
            a.canBackstab = w_a.canBackstab;
            if (isLeftHand)
            {
                a.mirror = true;
            }
            a.overrideDamageAnim = w_a.overrideDamageAnim;
            a.damageAnim = w_a.damageAnim;
            a.parryMultiplier = w.parryMultiplier;
            a.backstabMultiplier = w.backstabMultiplier;
            a.canParry = w_a.canParry;
            DeepCopyWeapenStats(w_a.weapenStats, a.weapenStats);
        }

        public static void DeepCopyWeapenStats(WeapenStats from, WeapenStats to)
        {
            to.physical = from.physical;
            to.strike = from.strike;
            to.slash = from.slash;
            to.thrust = from.thrust;
            to.magic = from.magic;
            to.fire = from.fire;
            to.dark = from.dark;
            to.lighting = from.lighting;
        }

        public static Action GetAction(ActionInput inp, List<Action> actionSlots)
        {
            for (int i = 0; i < actionSlots.Count; i++)
            {
                if (actionSlots[i].input == inp)
                    return actionSlots[i];
            }
            return null;
        }

        public static void DeepCopySpell(Spell from, Spell to)
        {
            to.spellType = from.spellType;
            to.particlePrefab = from.particlePrefab;
            to.projectile = from.projectile;
            to.icon = from.icon;
            to.itemDescription = from.itemDescription;
            to.itemName = from.itemName;

        }
    }

}

