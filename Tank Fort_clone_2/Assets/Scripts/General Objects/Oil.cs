using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : MonoBehaviour
{
    public float speedDecrease;
    private void OnTriggerEnter(Collider other) {
        // If target has a rigidbody, continue
        Rigidbody targetRigidbody = other.GetComponent<Rigidbody>();
        if (targetRigidbody) {
            TankMovement targetMovement = targetRigidbody.GetComponent<TankMovement>();
            // If TankMovement script attached to gameobject, continue
            if (targetMovement) {
                targetMovement.m_Speed -= speedDecrease;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        // If target has a rigidbody, continue
        Rigidbody targetRigidbody = other.GetComponent<Rigidbody>();
        if (targetRigidbody) {
            TankMovement targetMovement = targetRigidbody.GetComponent<TankMovement>();
            // If TankMovement script attached to gameobject, continue
            if (targetMovement) {
                targetMovement.m_Speed += speedDecrease;
            }
        }
    }
}
