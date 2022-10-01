using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum unitTypes
{
	MeleeZombie,
	BowSkeleton,
}

/// <summary>
/// Base Class for all Unit Types
/// </summary>
public class UnitType
{
	public string name;
	public unitTypes type;
	public UnitBaseStats stats;
	public Ability ability;

	public UnitType()
	{
		Init();
	}

	/// <summary>
	/// Initialization Method that can be called outside of Unity Start and Awake methods
	/// </summary>
	public virtual void Init() { }

	/// <summary>
	/// A method that handles the basic attack function of a unit type
	/// </summary>
	/// <param name="target"></param>
	public virtual void Attack(Entity target) { }
	/// <summary>
	/// A method that activates this units unique ability if it has one
	/// </summary>
	/// <param name="target"></param>
	public void ActivateAbility(Entity target) 
	{
		if (ability == null)
			return;

		ability.Activate(target); 
	}

	/// <summary>
	/// Reduce cooldown on abilities
	/// </summary>
	/// <param name="timePassed"></param>
	public void UpdateAbilityCooldown(float timePassed)
	{
		if (ability == null)
			return;

		ability.ReduceCooldown(timePassed);
	}


}

public class MeleeZombie : UnitType
{
	public override void Init()
	{
		base.Init();
	}
}

public class BowSkeleton : UnitType
{
	public override void Init()
	{
		base.Init();
		ability = new ProjectileAbility();
	}
}
