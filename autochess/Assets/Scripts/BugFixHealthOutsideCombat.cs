using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugFixHealthOutsideCombat : MonoBehaviour
{
    private Entity entity;
    void Start()
    {
        GetComponent<Entity>();
    }

    void Update()
    {
        if (GameManager.Instance.currentPhase == GamePhase.Planning) entity.currentHealth = entity.maxHealth;
    }
}
