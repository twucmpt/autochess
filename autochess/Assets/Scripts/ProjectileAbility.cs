using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu()]
public class ProjectileAbility : Ability {
    public GameObject projectilePrefab;

    public override void AnimationImpactCallback(Unit user, Entity target) {
        var spawnedProjectile = GameObject.Instantiate(projectilePrefab, user.transform.position, Quaternion.identity);
        var projectile = spawnedProjectile.GetComponent<Projectile>();
        projectile.direction = (target.transform.position - user.transform.position).normalized;
        projectile.damage = power;
        projectile.user = user;
    }
}