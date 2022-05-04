using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        Rigidbody targetRigidbody = other.GetComponent<Rigidbody>();
        if (targetRigidbody) {
            TankMovement targetMovement = targetRigidbody.GetComponent<TankMovement>();
            // If TankMovement script attached to gameobject, continue
            if (targetMovement) {
                this.gameObject.GetComponent<BoxCollider>().enabled = false;
                foreach (Transform child in this.transform) {
                    child.gameObject.SetActive(false);
                    StartCoroutine(Destroying());
                }
            }
        }
    }

    private IEnumerator Destroying() {
        yield return new WaitForSeconds(this.gameObject.GetComponent<IncreaseSpeed>().duration+1);
        Destroy(this.gameObject);
    }
}
