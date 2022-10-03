using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugFixHealthOutsideCombat : MonoBehaviour
{
    private Unit unit;
    void Start()
    {
        GetComponent<Unit>();
    }

    void Update()
    {
        if (GameManager.Instance.currentPhase == GamePhase.Planning) {
            unit.currentHealth = unit.maxHealth;
            unit.animator.SetBool("Dead", false);
        }
    }
}
