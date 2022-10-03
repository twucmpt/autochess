using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum unitTypes
{
	MeleeZombie,
	BowSkeleton,
	HumanPeasent,
	Lich,
	Tombstone
}

/// <summary>
/// Base Class for all Unit Types
/// </summary>
public class UnitType {
	public string name;
	public unitTypes type;
	public UnitBaseStats stats;
	internal Dictionary<string, List<AudioClip>> sounds;


	public List<Ability> abilities = new();
	public int cost = 1;
	public float tierHealthMulti = 0;

	public UnitType() {
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
	public virtual void Attack(Entity target) {
	}
	/// <summary>
	/// A method that activates this units unique ability if it has one
	/// </summary>
	/// <param name="target"></param>
	public void ActivateAbility(Ability ability, Entity target) {
		if (ability == null)
			return;
		ability.Activate(target);
	}

	/// <summary>
	/// Reduce cooldown on abilities
	/// </summary>
	/// <param name="timePassed"></param>
	public void UpdateAbilityCooldown(float timePassed) {
		foreach (var ability in abilities) {
			ability.ReduceCooldown(timePassed);
		}

	}


	/// <summary>
	/// Fetches the sound for a given key, null if none 
	/// </summary>
	/// <param name="sound"></param>
	/// <returns></returns>
	public AudioClip GetSound(string sound) {
		if (sounds.ContainsKey(sound)) return sounds[sound][UnityEngine.Random.Range(0, sounds[sound].Count)];
		return null;
	}
}

public class MeleeZombie : UnitType {


	public override void Init() {
		base.Init();
		type = unitTypes.MeleeZombie;
		sounds = new() {
			["death"] = new() {
				Resources.Load<AudioClip>("SFX/zombiedeath1"),
				Resources.Load<AudioClip>("SFX/zombiedeath2"),
			},
			["placement"] = new() { Resources.Load<AudioClip>("SFX/zombiemoan"), null, null } // 33% chance to moan xD
		};
		cost = 1;

	}
}

public class BowSkeleton : UnitType {

	public override void Init() {
		base.Init();
		type = unitTypes.BowSkeleton;
		sounds = new() {
			["death"] = new() { Resources.Load<AudioClip>("SFX/skellyaction") },
			["placement"] = new() { Resources.Load<AudioClip>("SFX/skellydeath") },
		};
		cost = 2;
	}
}

public class Lich : UnitType {
	public override void Init() {
		base.Init();
		type = unitTypes.Lich;
		sounds = new() {
			["death"] = new() { Resources.Load<AudioClip>("SFX/skellyaction") },
			["placement"] = new() { Resources.Load<AudioClip>("SFX/skellydeath") },
		};
		cost = 6;
	}
}

public class Tombstone : UnitType {
	public override void Init() {
		base.Init();
		type = unitTypes.Tombstone;
		sounds = new() {
			//["death"] = new() { Resources.Load<AudioClip>("SFX/skellyaction") },
			//["placement"] = new() { Resources.Load<AudioClip>("SFX/skellydeath") },
		};
		cost = 10;
		tierHealthMulti = 0.2f;
	}
}


public class HumanPeasent : UnitType {

	public override void Init() {
		base.Init();
		type = unitTypes.HumanPeasent;
		sounds = new() { ["death"] = new() { Resources.Load<AudioClip>("SFX/humandeath1"), Resources.Load<AudioClip>("SFX/humandeath2") } };

	}
}