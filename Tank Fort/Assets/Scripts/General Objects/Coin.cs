using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int addPointTo = -1;                 //Player that will get the extra coinpoint

    private void OnTriggerEnter(Collider other) {
        Rigidbody targetRigidbody = other.GetComponent<Rigidbody>();
        if (targetRigidbody) {
            TankMovement targetMovement = targetRigidbody.GetComponent<TankMovement>();
            // If TankMovement script attached to gameobject, continue
            if (targetMovement) {
                addPointTo = targetMovement.m_PlayerNumber;
                this.gameObject.GetComponent<BoxCollider>().enabled = false;
                foreach (Transform child in this.transform) {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}
