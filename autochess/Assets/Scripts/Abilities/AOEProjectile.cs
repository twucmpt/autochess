using UnityEngine;

public class AOEProjectile : ProjectileAttack {
    public float radius = 1.5f;
    public GameObject effect;

    public override void OnProjectileHit(Projectile projectile, Collider collider) {
        Entity unit = collider.transform.GetComponent<Entity>();
        if (unit == user) return;
        if (!friendlyFire && unit.CompareTag(user.tag)) return;

        if (effect != null) Instantiate(effect, projectile.transform.position, Quaternion.identity);

        var hits = Physics.OverlapSphere(projectile.transform.position, radius, LayerMask.GetMask("Entities"));
        foreach (var hitCollider in hits) {
            unit = hitCollider.gameObject.GetComponent<Unit>();
            if (unit == user) continue;
            if (!friendlyFire && unit.CompareTag(user.tag)) continue;
            unit.TakeDamage(power);
        }

        Destroy(projectile.gameObject);
    }
}