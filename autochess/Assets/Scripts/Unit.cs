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
    Ability[] abilities;
    void Start() {
        var added = GameManager.Instance.AddUnit(this, Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        if (!added) Destroy(gameObject);

        abilities = GetComponentsInChildren<Ability>();

        animator = GetComponentInChildren<Animator>();
        animController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animController;
    }

    void Update() {
        // Orient according to facingRight (assuming default sprite faces right)
        if ((facingRight && Mathf.Sign(transform.localScale.x) < 0) || (!facingRight && Mathf.Sign(transform.localScale.x) > 0)) {
            Vector3 flipScale = transform.localScale;
            flipScale.x *= -1;
            transform.localScale = flipScale;
        }

        // Handle movement
        gridPos = GameManager.Instance.GetGridPos(this);
        var gridToWorldPos = new Vector3(gridPos.x, gridPos.y);
        animator.SetBool("Walking", gridToWorldPos != transform.position);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("walk")) transform.position = Vector3.MoveTowards(transform.position, gridToWorldPos, Time.deltaTime);

        // Reduce cooldown on abilities
        foreach (Ability ability in abilities) ability.ReduceCooldown(Time.deltaTime*speed);

        // Determine action
        // Animation state doesn't change right away so need to check animator booleans too
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle") && !animator.GetBool("Attacking") && !animator.GetBool("Walking")) {
            bool atLeastOneAbilityInRange = false;
            foreach (Ability ability in abilities) {
                if (atLeastOneAbilityInRange && ability.currentCooldown > 0) continue;
                var possibleTargets = ability.TargetsInRange(CompareTag("Player")? "Enemy" : "Player");
                if (possibleTargets.Count > 0) {
                    atLeastOneAbilityInRange = true;
                    if (ability.currentCooldown > 0) continue;
                    currentAbility = ability;
                    currentTarget = possibleTargets[0];
                    break;
                }
            }

            if (currentAbility is not null) {
                waitingOnCooldown = false;
                animController["attack"] = currentAbility.animation;
                animator.SetBool("Attacking", true);
                print(gameObject.name + " is attacking " + currentTarget.gameObject.name + " with " + currentAbility.name + ".");
            }
            else if (!atLeastOneAbilityInRange) {
                waitingOnCooldown = false;
                Advance();
            }
            else {
                if (!waitingOnCooldown) print(gameObject.name + " is waiting for ability to cooldown.");
                waitingOnCooldown = true;
            }
        }
    }

    void OnDestroy() {
        GameManager.Instance.RemoveUnit(this);
    }

    public void Advance() {
        var newPos = GameManager.Instance.GetGridPos(this);
        newPos += facingRight? Vector2Int.right : Vector2Int.left;
        GameManager.Instance.MoveUnit(this, newPos);
        print(gameObject.name + " is advancing to " + newPos.ToString() + ".");
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
