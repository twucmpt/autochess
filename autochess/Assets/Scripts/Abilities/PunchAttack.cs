using System.Collections.Generic;
using UnityEngine;

public class PunchAttack : Ability
{
	public void Init()
	{
		angle = 0;
		power = 5;
		cooldown = 5;
		range = 1;
		rangeType = AttackRangeType.Linear;
		includeUser = false;
		friendlyFire = false;

		animation = Resources.Load<AnimationClip>("Animations/attack.anim");
	}
	public override void AnimationImpactCallback(Entity target)
	{
		List<Entity> unitsHit = new List<Entity>();
		switch (rangeType)
		{
			case AttackRangeType.Linear:
				unitsHit = TargetsInRange(null);
				break;
			case AttackRangeType.Cone:
				// Check they are still in range
				float targetAngle = Vector3.Angle(user.facingRight ? user.transform.right : -user.transform.right, (target.transform.root.position - user.transform.position));
				if (targetAngle > angle || Vector3.Distance(user.transform.position, target.transform.position) > range) break;

				// Hit all units in a line
				var linearHits = Physics2D.RaycastAll(user.transform.position, target.transform.position - user.transform.position, range, LayerMask.GetMask("Entities"));
				foreach (var hit in linearHits)
				{
					unitsHit.Add(hit.collider.transform.root.GetComponent<Entity>());
				}
				break;
		}
		foreach (Entity unit in unitsHit)
		{
			if (unit == user && !includeUser) continue;
			if (!friendlyFire && unit.CompareTag(user.tag)) continue;
			unit.TakeDamage(power);
		}
	}


}