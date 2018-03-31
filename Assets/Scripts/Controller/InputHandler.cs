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

        private float b_timer;
        private float lt_timer;
        private float rt_timer;

        private float delta;
        private StateManager states;
        private CameraManager camManager;
	    // Use this for initialization
	    void Start ()
	    {
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
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
            b_input = Input.GetButton("B");
            a_input = Input.GetButton("A");
            y_input = Input.GetButtonUp("Y");
            x_input = Input.GetButton("X");

            rt_input = Input.GetButton("RT");
            rt_axis = Input.GetAxis("RT");
            if (rt_axis != 0)
                rt_input = true;

            lt_input = Input.GetButton("LT");
            lt_axis = Input.GetAxis("LT");
            if (lt_axis != 0)
                lt_input = true;

            lb_input = Input.GetButton("LB");
            rb_input = Input.GetButton("RB");

            rightAxis_down = Input.GetButtonUp("L");//L is lockon

            if (b_input)
                b_timer += delta;

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

            if (rightAxis_down)
            {
                states.lockOn = !states.lockOn;
                if (states.lockOnTarget == null)
                    states.lockOn = false;
                
                camManager.lockonTarget = states.lockOnTarget;
                states.LockonTransform = camManager.LockonTransform;
                camManager.lockon = states.lockOn;

            }


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
