using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum unitTypes
{
	MeleeZombie,
	BowSkeleton,
	HumanPeasent,
}

/// <summary>
/// Base Class for all Unit Types
/// </summary>
public class UnitType
{
	public string name;
	public unitTypes type;
	public UnitBaseStats stats;
	public Dictionary<string,AudioClip> sounds;
	
	public AudioSource sfxBus;

	public List<Ability> abilities = new();
	public int cost = 1;

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
	public void ActivateAbility(Ability ability, Entity target) 
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
		foreach (var ability in abilities)
		{
			ability.ReduceCooldown(timePassed);
		}
		
	}

	public void PlaySound(string sound) {
		if (this.sounds.ContainsKey(sound))
			sfxBus.PlayOneShot(sounds[sound]);
    }
}

public class MeleeZombie : UnitType
{


	public override void Init() {
		base.Init();
		type = unitTypes.MeleeZombie;
		sounds = new() {
			["death"] = Resources.Load<AudioClip>("SFX/zombiedeath.wav"),
			["action"] = Resources.Load<AudioClip>("SFX/zombiemoan.wav")
		};

	}
}

public class BowSkeleton : UnitType
{

	public override void Init() {
			base.Init();
			type = unitTypes.BowSkeleton;
			sounds = new() {
				["death"] = Resources.Load<AudioClip>("SFX/zombiedeath.wav"),
				["action"] = Resources.Load<AudioClip>("SFX/zombiemoan.wav")
			};
	}
}

public class HumanPeasent : UnitType
{

	public override void Init() {
			base.Init();
			type = unitTypes.HumanPeasent;
			sounds = new() {
				["death"] = Resources.Load<AudioClip>("SFX/humandeath1.wav"),
				["action"] = Resources.Load<AudioClip>("SFX/humandeath2.wav")
			};

		}
	}
