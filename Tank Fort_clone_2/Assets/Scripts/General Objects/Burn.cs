using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour
{
    public float damage;
    private void OnTriggerStay(Collider other) {
            Rigidbody targetRigidbody = other.GetComponent<Rigidbody>();
            if (targetRigidbody) {
                TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
                // If TankHealth script attached to gameobject, continue
                if (targetHealth) {
                    targetHealth.TakeDamage(damage);
                }
            }
        }
}
