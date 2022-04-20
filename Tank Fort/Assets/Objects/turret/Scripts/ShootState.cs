using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootState : TurretState
{
    // When Entering ShootState, turn on shooting animation(will also shoot the projectile as well)
    public override void Enter(Turret parent) {
        base.Enter(parent);
        parent.Animator.SetBool("Shoot", true);
    }

    public override void Update() {
        // While target still exists, rotate rotator to the target
        if (parent.Target != null) {
            parent.Rotator.LookAt(parent.Target.position + parent.AimOffset);
        }
        // Shooting a raycast from the gun barrel to the player, if it doesn't hit(meaning the player is behind a wall,building,etc), change state to IdleState
        if (!parent.CanSeeTarget(parent.GunBarrels[0].forward, parent.Rotator.position, "Player")) {
            parent.ChangeState(new IdleState());
        }
    }
}
