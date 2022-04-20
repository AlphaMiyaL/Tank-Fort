using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : TurretState
{
    public override void Enter(Turret parent) {
        base.Enter(parent);
        parent.Animator.SetBool("Shoot", false);
    }

    // For when all player targets leave the turret collider, and the turret is now resetting back to default
    public override void Update() {
        // If the rotator rotation is not the default rotation, slowly turn back to default rotation
        if (parent.DefaultRotation != parent.Rotator.rotation) {
            parent.Rotator.rotation = Quaternion.RotateTowards(parent.Rotator.rotation, parent.DefaultRotation, Time.deltaTime * parent.RotationSpeed);
        }

        // (If target still exists while in IdleMode, the last target hid behind something, but is still in the collider space)
        // Send raycasst from gunbarrel to player, and if hit, change state to FindTargetState
        if (parent.Target != null) {
            if (parent.CanSeeTarget(((parent.Target.position+parent.AimOffset) - parent.GunBarrels[0].position), parent.GunBarrels[0].position, "Player")){
                parent.ChangeState(new FindTargetState());
            }
        }
    }

    // If collider with the player tag enters the turret collider area, it will set that collider as the target and change to FindTargetState
    public override void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            parent.Target = other.transform;
            parent.ChangeState(new FindTargetState());
        }
    }
}
