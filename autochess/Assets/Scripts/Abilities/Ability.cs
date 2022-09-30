using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    new public string name;
    new public AnimationClip animation;
    public float range;
    public AttackRangeType rangeType;
    public float angle;
    public int power;
    public float cooldown;
    public float currentCooldown;
    public bool includeUser = false;
    public bool friendlyFire = false;

    protected Unit user;

    void Start() {
        user = GetComponentInParent<Unit>();
    }

    public List<Entity> TargetsInRange(string targetTag) {
        var targets = new List<Entity>();
        switch (rangeType) {
            case AttackRangeType.Linear:
                var linearHits = Physics2D.RaycastAll(user.transform.position, user.facingRight ? Vector2.right : Vector2.left, range, LayerMask.GetMask("Entities"));
                foreach (var hit in linearHits) {
                    if (targetTag is null || hit.collider.transform.root.CompareTag(targetTag)) {
                        Entity unit = hit.collider.transform.root.GetComponent<Entity>();
                        if (includeUser || unit != user) targets.Add(unit);
                    }
                }
                return targets;
            case AttackRangeType.Cone:
                var circleHits = Physics2D.OverlapCircleAll(user.transform.position, range, LayerMask.GetMask("Entities"));
                foreach(var collider in circleHits) {
                    if (targetTag is not null && !collider.transform.root.CompareTag(targetTag)) continue;
                    if ((user.facingRight && collider.transform.root.position.x < user.transform.position.x) || (!user.facingRight && collider.transform.root.position.x > user.transform.position.x)) continue;
                    float targetAngle = Vector3.Angle(user.facingRight? user.transform.right : -user.transform.right, (collider.transform.root.position - user.transform.position));
                    if (targetAngle > angle) continue;

                    Entity unit = collider.transform.root.GetComponent<Entity>();
                    if (includeUser || unit != user) targets.Add(unit);
                }
                return targets;
        }
        return targets;
    }

    public virtual void Activate(Entity target) {
        currentCooldown = cooldown;
    }
    public virtual void AnimationImpactCallback(Entity target) {
        List<Entity> unitsHit = new List<Entity>();
        switch (rangeType) {
            case AttackRangeType.Linear:
                unitsHit = TargetsInRange(null);
                break;
            case AttackRangeType.Cone:
                // Check they are still in range
                float targetAngle = Vector3.Angle(user.facingRight? user.transform.right : -user.transform.right, (target.transform.root.position - user.transform.position));
                if (targetAngle > angle || Vector3.Distance(user.transform.position, target.transform.position) > range) break;

                // Hit all units in a line
                var linearHits = Physics2D.RaycastAll(user.transform.position, target.transform.position - user.transform.position, range, LayerMask.GetMask("Entities"));
                foreach (var hit in linearHits) {
                    unitsHit.Add(hit.collider.transform.root.GetComponent<Entity>());
                }
                break;
        }
        foreach (Entity unit in unitsHit) {
            if (unit == user && !includeUser) continue;
            if (!friendlyFire && unit.CompareTag(user.tag)) continue;
            unit.TakeDamage(power);
        }
    }

    public void ReduceCooldown(float seconds) {
        currentCooldown -= seconds;
        currentCooldown = Mathf.Max(currentCooldown, 0);
    }
}

public enum AttackRangeType {
    Linear, Cone
}