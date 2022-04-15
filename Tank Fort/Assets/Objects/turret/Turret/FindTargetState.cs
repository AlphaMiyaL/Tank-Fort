using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTargetState : TurretState
{
    public override void Update() {
        // Makes the "GhostRotator" look directly at the target, and slowly turns the real rotator over time based on rotation speed
        parent.GhostRotator.LookAt(parent.Target.position+parent.AimOffset);
        parent.Rotator.rotation = Quaternion.RotateTowards(parent.Rotator.rotation, parent.GhostRotator.rotation, Time.deltaTime * parent.RotationSpeed);

        //if the rotator is pointing straight at the target, change to ShootState
        if (parent.GhostRotator.rotation.y == parent.Rotator.rotation.y) {
            parent.ChangeState(new ShootState());
        }
    }
}
