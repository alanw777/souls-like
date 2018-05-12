using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class Projectile : MonoBehaviour
    {
        Rigidbody rigid;
        public float hSpeed = 10;
        public float vSpeed = 2;

        public Transform target;
        public GameObject explosionPrefab;

        public void Init()
        {
            rigid = GetComponent<Rigidbody>();
           
            Vector3 force = transform.forward*hSpeed;
            force += transform.up*vSpeed;

            rigid.AddForce(force,ForceMode.Impulse);
        }

        public void OnTriggerEnter(Collider other)
        {
            EnemyStates es = other.GetComponentInParent<EnemyStates>();
            if (es != null)
            {
                es.health -= 10;
                es.DoDamage_();
            }

            GameObject go = Instantiate(explosionPrefab,transform.position,transform.rotation) as GameObject;
            Destroy(this.gameObject);
        }

        
    }

}

