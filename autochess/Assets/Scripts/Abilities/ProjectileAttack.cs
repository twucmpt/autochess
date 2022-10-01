using UnityEngine;

public class ProjectileAttack : Ability {
    public GameObject projectilePrefab;
    public float speed = 1;

	public override void AnimationImpactCallback(Entity target) {
        var spawnedProjectile = GameObject.Instantiate(projectilePrefab, user.transform.position, Quaternion.identity);
        var projectile = spawnedProjectile.GetComponent<Projectile>();
        projectile.ability = this;
        projectile.direction = (target.transform.position - user.transform.position).normalized;
    }

    public void OnProjectileHit(Projectile projectile, Collider2D collider) {
        Entity unit = collider.transform.GetComponent<Entity>();
        if (unit == user) return;
        if (!friendlyFire && unit.CompareTag(user.tag)) return;

        unit.TakeDamage(power);
        Destroy(projectile.gameObject);
    }
}