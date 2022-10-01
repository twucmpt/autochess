using System.Collections.Generic;

public class AttackTheWarlock : ProjectileAttack {
	public override List<Entity> TargetsInRange(string targetTag) {
        var possibleTargets = base.TargetsInRange(targetTag);
        if (possibleTargets.Contains(GameManager.Instance.warlock)) return new List<Entity>{GameManager.Instance.warlock};
        return new ();
    }
}