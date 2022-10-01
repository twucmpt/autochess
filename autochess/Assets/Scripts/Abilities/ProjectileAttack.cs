using UnityEngine;

public class ProjectileAttack : Ability {
    public GameObject projectilePrefab;
    public float speed = 1;


	public void Init()
	{
		speed = 1;
		angle = 45;
		power = 20;
		cooldown = 10;
		range = 4;
		rangeType = AttackRangeType.Cone;
		includeUser = false;
		friendlyFire = false;

		animation = Resources.Load<AnimationClip>("Animations/attack.anim");
		projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Fireball.prefab");
	}

	public override void AnimationImpactCallback(Entity target) {
        var spawnedProjectile = GameObject.Instantiate(projectilePrefab, user.transform.position, Quaternion.identity);
        var projectile = spawnedProjectile.GetComponent<Projectile>();
        projectile.ability = this;
        projectile.direction = (target.transform.position - user.transform.position).normalized;
    }

    public void OnProjectileHit(Projectile projectile, Collider2D collider) {
        Entity unit = collider.transform.root.GetComponent<Entity>();
        if (unit == user) return;
        if (!friendlyFire && unit.CompareTag(user.tag)) return;

        unit.TakeDamage(power);
        Destroy(projectile.gameObject);
    }
}