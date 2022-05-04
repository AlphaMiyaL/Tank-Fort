using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TurretState
{
    protected Turret parent;

    public virtual void Enter(Turret parent) {
        this.parent = parent;
    }

    public virtual void Update() {
    }

    public virtual void Exit() {

    }

    public virtual void OnTriggerEnter(Collider other) {

    }

    // If the target exits the turret collider, change the target back to null and go to IdleState
    public virtual void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            parent.Target = null;
            parent.ChangeState(new IdleState());
        }
    }
}
