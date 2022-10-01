using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;







public class Unit : Entity
{
    public int power = 10;
    public bool facingRight = true;
    public float speed = 1;
    public Vector2Int gridPos;

    Animator animator;
    AnimatorOverrideController animController;
    Ability currentAbility = null;
    Entity currentTarget = null;
    bool waitingOnCooldown = false;
	GameManager gameManager;

	public UnitType type;
	public unitTypes myType;

	void Start() 
	{
		Init();
    }

	public void Init()
	{
		gameManager = GameManager.Instance;

		var added = GameManager.Instance.AddUnit(this, Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
		if (!added) Destroy(gameObject);

		animator = GetComponentInChildren<Animator>();
		animController = new AnimatorOverrideController(animator.runtimeAnimatorController);
		animator.runtimeAnimatorController = animController;
		type = gameManager.GetUnitType(myType);
			
	}
	


	void Update() {
		UpdateFacingDirection();
		UpdatePosition();
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

	/// <summary>
	/// Updates the position and handles the movement of this unit
	/// </summary>
	private void UpdatePosition()
	{
		if (gridPos == null)
			return;

		var gridToWorldPos = new Vector3(gridPos.x, gridPos.y);
		animator.SetBool("Walking", gridToWorldPos != transform.position);
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("walk")) transform.position = Vector3.MoveTowards(transform.position, gridToWorldPos, Time.deltaTime);

	}

	public void Advance() {
        var newPos = gridPos;
        newPos += facingRight? Vector2Int.right : Vector2Int.left;

		MoveUnit(newPos);

    }

	/// <summary>
	/// Moves the Unit to a desired position
	/// </summary>
	private void MoveUnit(Vector2Int newPos)
	{
		if (!gameManager.CheckValidPosition(newPos))
			return;

		//Update Unit Positions
		gameManager.unitPositions.Remove(gridPos);
		gameManager.unitPositions.Add(newPos, this);

		gridPos = newPos;

		print(gameObject.name + " is advancing to " + newPos.ToString() + ".");
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


			//Temporary Needs to be removed
			List<Ability> abilities = new() { type.ability };

			foreach (Ability ability in abilities)
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

    public void DestroySelf() {
        Destroy(gameObject);
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
