using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class InputHandler : MonoBehaviour
    {
        private float vertical;
        private float horizontal;
        private bool a_input;
        private bool b_input;
        private bool x_input;
        private bool y_input;
        private bool rb_input;
        private bool lb_input;
        private float rt_axis;
        private bool rt_input;
        private float lt_axis;
        private bool lt_input;
        private bool leftAxis_down;
        private bool rightAxis_down;

        private float d_x;
        private float d_y;
        private bool d_up;
        private bool d_down;
        private bool d_right;
        private bool d_left;
        private bool p_d_up;
        private bool p_d_down;
        private bool p_d_right;
        private bool p_d_left;

        private float b_timer;
        private float lt_timer;
        private float rt_timer;

        private float delta;
        private StateManager states;
        private CameraManager camManager;
	    // Use this for initialization
	    void Start ()
	    {
            UI.QuickSlot.singleton.Init();

	        states = GetComponent<StateManager>();
            states.Init();

	        camManager = CameraManager.singleton;
            camManager.Init(states);
	    }
	
	    void Update ()
	    {
	        delta = Time.deltaTime;
            states.Tick(delta);
            ResetInputNStates();
	    }

        void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            UpdateStates();
            states.FixedTick(Time.deltaTime);
            camManager.Tick(delta);
        }

        

        void GetInput()
        {
            vertical = Input.GetAxis(StaticStrings.Vertical);
            horizontal = Input.GetAxis(StaticStrings.Horizontal);
            b_input = Input.GetButton(StaticStrings.B);
            a_input = Input.GetButton(StaticStrings.A);
            y_input = Input.GetButtonUp(StaticStrings.Y);
            x_input = Input.GetButton(StaticStrings.X);

            rt_input = Input.GetButton(StaticStrings.RT);
            rt_axis = Input.GetAxis(StaticStrings.RT);
            if (rt_axis != 0)
                rt_input = true;

            lt_input = Input.GetButton(StaticStrings.LT);
            lt_axis = Input.GetAxis(StaticStrings.LT);
            if (lt_axis != 0)
                lt_input = true;

            lb_input = Input.GetButton(StaticStrings.LB);
            rb_input = Input.GetButton(StaticStrings.RB);

            rightAxis_down = Input.GetButtonUp(StaticStrings.L);//L is lockon

            if (b_input)
                b_timer += delta;

            d_x = Input.GetAxis(StaticStrings.Pad_X);
            d_y = Input.GetAxis(StaticStrings.Pad_Y);

            d_up = Input.GetKeyUp(KeyCode.Alpha1) || d_y > 0;
            d_down = Input.GetKeyUp(KeyCode.Alpha2) || d_y < 0;
            d_left = Input.GetKeyUp(KeyCode.Alpha3) || d_x < 0;
            d_right = Input.GetKeyUp(KeyCode.Alpha4) || d_x > 0;



        }

        void UpdateStates()
        {
            states.vertical = vertical;
            states.horizontal = horizontal;

            Vector3 v = vertical*camManager.transform.forward;
            Vector3 h = horizontal*camManager.transform.right;
            states.moveDir = (v + h).normalized;
            float m = Mathf.Abs(vertical) + Mathf.Abs(horizontal);
            states.moveAmount = Mathf.Clamp01(m);

            if (x_input)
                b_input = false;
            
            if (b_input && b_timer>0.5f)
            {
                states.run = states.moveAmount > 0;
            }
            if (b_input == false && b_timer > 0 && b_timer < 0.5f)
                states.rollInput = true;

            states.itemInput = x_input;
            states.rt = rt_input;
            states.rb = rb_input;
            states.lt = lt_input;
            states.lb = lb_input;

            if (y_input)
            {
                states.isTwoHanded = !states.isTwoHanded;
                states.HandleTwoHanded();
            }

            if (states.lockOnTarget != null)
            {
                if (states.lockOnTarget.eState.isDead)
                {
                    states.lockOn = false;
                    states.lockOnTarget = null;
                    states.LockonTransform = null;
                    camManager.lockonTarget = null;
                    camManager.lockon = false;
                }
            }
            else
            {
                states.lockOn = false;
                states.lockOnTarget = null;
                states.LockonTransform = null;
                camManager.lockonTarget = null;
                camManager.lockon = false;
            }

            if (rightAxis_down)
            {
                states.lockOn = !states.lockOn;
                states.lockOnTarget = EnemyManager.singleton.GetEnemyTarget(transform.position);
                if (states.lockOnTarget == null)
                    states.lockOn = false;
                
                camManager.lockonTarget = states.lockOnTarget;
                states.LockonTransform = states.lockOnTarget.GetTarget();
                camManager.LockonTransform = states.LockonTransform;
                camManager.lockon = states.lockOn;

            }

            HandleQuickSlotChanges();
        }

        private void HandleQuickSlotChanges()
        {
            if(states.usingItem||states.isSpellCasting)
                return;

            if (d_up)
            {
                if (!p_d_up)
                {
                    states.inventoryManager.ChangeToNextSpell();
                    p_d_up = true;
                }
            }
            if (!d_down)
                p_d_down = false;
            if (!d_up)
                p_d_up = false;

            if(states.canMove==false)
                return;
            if(states.isTwoHanded)
                return;
            
            if (d_left)
            {
                if (!p_d_left)
                {
                    states.inventoryManager.ChangeToNextWeapen(true);
                    p_d_left = true;
                }
            }
            if (d_right)
            {
                if (!p_d_right)
                {
                    states.inventoryManager.ChangeToNextWeapen(false);
                    p_d_right = true;
                }
            }
            if (!d_right)
                p_d_right = false;
            if (!d_left)
                p_d_left = false;
        }

        private void ResetInputNStates()
        {
            if (b_input == false)
                b_timer = 0;

            if (states.rollInput)
                states.rollInput = false;

            if (states.run)
                states.run = false;
        }
    }

}
