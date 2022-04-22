using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseSpeed : MonoBehaviour
{
    public float speedIncrease;
    public float duration;

    private void OnTriggerEnter(Collider other) {
        // If target has a rigidbody, continue
        Rigidbody targetRigidbody = other.GetComponent<Rigidbody>();
        if (targetRigidbody) {
            TankMovement targetMovement = targetRigidbody.GetComponent<TankMovement>();
            // If TankMovement script attached to gameobject, continue
            if (targetMovement) {
                this.transform.parent = null;
                StartCoroutine(targetMovement.IncreaseSpeed(speedIncrease, duration));
            }
        }
    }
}
