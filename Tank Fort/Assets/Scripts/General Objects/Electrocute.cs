using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electrocute : MonoBehaviour{
    public bool electrocuting = false;   // Boolean determining whether it electric floor is on
    public float onOffDelay = 3f;        // Time spent on/off before swapping to the other
    public GameObject electric;          // Electric object to be turned on or off
    public float damage;                 // Damage taken each second in the electric

    private WaitForSeconds onOffWait;

    private void Start() {
        onOffWait = new WaitForSeconds(onOffDelay);
        StartCoroutine(electricLoop());
    }

    private IEnumerator electricLoop() {
        reverseElectric();
        yield return onOffWait;

        StartCoroutine(electricLoop());
    }

    private void OnTriggerStay(Collider other) {
        // If electrocuting is true and there exists a rigidbody to the target, continue
        if (electrocuting) {
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

    private void reverseElectric() {
        electrocuting = !electrocuting;
        electric.SetActive(electrocuting);
    }
}
