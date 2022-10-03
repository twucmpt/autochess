using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugFixHealthOutsideCombat : MonoBehaviour
{
    private Unit unit;
    void Start()
    {
        unit = GetComponent<Unit>();
    }

    void Update()
    {
        if (GameManager.Instance.currentPhase == GamePhase.Planning) {
            unit.currentHealth = unit.maxHealth;
            unit.animator.SetBool("Dead", false);
        }
        if (GameManager.Instance.currentPhase != GamePhase.Combat) {
            unit.animator.SetBool("Attacking", false);
        }
    }
}
