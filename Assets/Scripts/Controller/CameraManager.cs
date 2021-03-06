﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager singleton;
        public bool lockon;
        public float mouseSpeed = 2;
        public float controllerSpeed = 7;
        public float followSpeed = 9;

        public float turningSmooth = 0.1f;
        public float minAngle = -35;
        public float maxAngle = 35;

        private float smoothX;
        private float smoothY;
        private float smoothXvelocity;
        private float smoothYvelocity;
        public float lookAngle;
        public float tiltAngle;

        public Transform target;
        public EnemyTarget lockonTarget;
        public Transform LockonTransform;

        private bool useRightAxis;

        private bool changeTargetLeft;
        private bool changeTargetRight;

        [HideInInspector]
        public Transform pivot;
        [HideInInspector]
        public Transform camTrans;

        public StateManager states;

        public void Init(StateManager st)
        {
            target = st.transform;
            states = st;
            camTrans = Camera.main.transform;
            pivot = camTrans.parent;
        }

        public void Tick(float d)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            float c_h = Input.GetAxis("RightAxis X");
            float c_v = Input.GetAxis("RightAxis Y");

            float targetSpeed = mouseSpeed;
            if (c_h != 0 || c_v != 0)
            {
                h = c_h;
                v = c_v;
                targetSpeed = controllerSpeed;
            }

            changeTargetLeft = Input.GetKeyUp(KeyCode.V);
            changeTargetRight = Input.GetKeyUp(KeyCode.B);

            if (lockonTarget != null)
            {
                if (LockonTransform == null)
                {
                    LockonTransform = lockonTarget.GetTarget();
                    states.LockonTransform = LockonTransform;
                }
                if (Mathf.Abs(h) > 0.6f)
                {
                    if (!useRightAxis)
                    {
                        LockonTransform = lockonTarget.GetTarget(h>0);
                        states.LockonTransform = LockonTransform;
                        useRightAxis = true;
                    }
                }
                if (changeTargetLeft || changeTargetRight)
                {
                    LockonTransform = lockonTarget.GetTarget(changeTargetLeft);
                    states.LockonTransform = LockonTransform;
                }
            }

            if (useRightAxis)
            {
                if (Mathf.Abs(h) < 0.6f)
                {
                    useRightAxis = false;
                }
            }

            

            FollowTarget(d);
            HandleRotations(d,v,h,targetSpeed);

        }

        void FollowTarget(float d)
        {
            float speed = followSpeed*d;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }

        void HandleRotations(float d, float v, float h,float targetSpeed)
        {
            if (turningSmooth > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXvelocity, turningSmooth);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYvelocity, turningSmooth);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
            if (lockon && lockonTarget != null)
            {
                Vector3 targetDir = LockonTransform.position - transform.position;
                targetDir.Normalize();
                targetDir.y = 0;

                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, d*9);
                lookAngle = transform.eulerAngles.y;
                return;
            }
            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);

            
        }

        void Awake()
        {
            singleton = this;
        }
    }

}

