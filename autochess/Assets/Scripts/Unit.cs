using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;







public class Unit : Entity
{
    public int power = 10;
    public bool facingRight = true;
	public bool isEnemy {get{return CompareTag("Enemy");}}
    public float speed = 1;
    public Vector2Int gridPos;

    public Animator animator;
    AnimatorOverrideController animController;
    Ability currentAbility = null;
    Entity currentTarget = null;
    bool waitingOnCooldown = false;
	GameManager gameManager;

	public UnitType type;
	public unitTypes myType;
    public Vector2Int originalPosition;
	public AudioSource sfxBus;


	void Start() 
	{
		Init();
    }

	public void Init()
	{
		gameManager = GameManager.Instance;
		animator = GetComponentInChildren<Animator>();
		animController = new AnimatorOverrideController(animator.runtimeAnimatorController);
		animator.runtimeAnimatorController = animController;
		type = gameManager.GetUnitType(myType);

		type.abilities = GetComponentsInChildren<Ability>().ToList();
		type.sfxBus = sfxBus;
	}
	


	void Update() {
		UpdateFacingDirection();
		type.UpdateAbilityCooldown(Time.deltaTime*speed);
		DetermineAction();
	}


    void OnDestroy() {
        GameManager.Instance.RemoveUnit(this);
    }

	/// <summary>
	/// Orient according to facingRight (assuming default sprite faces right)
	/// </summary>
	private void UpdateFacingDirection()
	{
		if ((facingRight && Mathf.Sign(transform.localScale.x) < 0) || (!facingRight && Mathf.Sign(transform.localScale.x) > 0))
		{
			Vector3 flipScale = transform.localScale;
			flipScale.x *= -1;
			transform.localScale = flipScale;
		}
	}

	public void Advance() {
        var newPos = gridPos;
        newPos += facingRight? Vector2Int.right : Vector2Int.left;

		MoveUnit(newPos);

    }

	/// <summary>
	/// Moves the Unit to a desired position
	/// </summary>
	public bool MoveUnit(Vector2Int newPos)
	{
		if (!gameManager.CheckValidPosition(newPos, tag)) {
			//print(gameObject.name + " wants to advance to " + newPos.ToString() + " but the position is not valid.");
			return false;
		}

		//Update Unit Positions
		gameManager.unitPositions.Remove(gridPos);
		gameManager.unitPositions.Add(newPos, this);

		gridPos = newPos;

		print(gameObject.name + " is advancing to " + newPos.ToString() + ".");
        return true;
	}

	


	/// <summary>
	/// Determines the next action for this unit
	/// Animation state doesn't change right away so need to check animator booleans too
	/// </summary>
	private void DetermineAction()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle") && !animator.GetBool("Attacking") && !animator.GetBool("Walking"))
		{
			bool atLeastOneAbilityInRange = false;

			foreach (Ability ability in type.abilities)
			{
				if (ability == null)
					continue;

				if (atLeastOneAbilityInRange && ability.currentCooldown > 0 ) continue;
				var possibleTargets = ability.TargetsInRange(CompareTag("Player") ? "Enemy" : "Player");
				if (possibleTargets.Count > 0)
				{
					atLeastOneAbilityInRange = true;
					if (ability.currentCooldown > 0) continue;
					currentAbility = ability;
					currentTarget = possibleTargets[0];
					break;
				}
			}

			if (currentAbility is not null)
			{
				waitingOnCooldown = false;
				if (currentAbility.animation == null)
				{
					Debug.LogError($"Missing attack animation on {currentAbility.name}");
					return;
				}

				animController["attack"] = currentAbility.animation;
				animator.SetBool("Attacking", true);
				print(gameObject.name + " is attacking " + currentTarget.gameObject.name + " with " + currentAbility.name + ".");
			}
			else if (!atLeastOneAbilityInRange)
			{
				waitingOnCooldown = false;
				Advance();
			}
			else
			{
				if (!waitingOnCooldown) print(gameObject.name + " is waiting for ability to cooldown.");
				waitingOnCooldown = true;
			}
		}
	}

	public void OnDying() {
		animator.SetBool("Dead", true);
	}

    public void OnDeath() {
        if (isEnemy) Destroy(gameObject);
        else {
            Graveyard.Instance.AddUnit(gameObject);
            gameManager.RemoveUnit(this);
        }
    }

    public void AnimationStartCallback() {
        currentAbility.Activate(currentTarget);
    }

    public void AnimationImpactCallback() {
        currentAbility.AnimationImpactCallback(currentTarget);
    }

    public void AnimationEndCallback() {
        currentAbility = null;
        currentTarget = null;
        animator.SetBool("Attacking", false);
    }
}
