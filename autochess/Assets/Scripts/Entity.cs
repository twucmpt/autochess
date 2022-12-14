using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    public int maxHealth = 100;
    public int baseHealth = 100;
    public int currentHealth = 100;
    public UnityEvent OnZeroHealth = new UnityEvent();
    public UnityEvent OnTakeDamage = new UnityEvent();
   

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) OnZeroHealth.Invoke();
        OnTakeDamage.Invoke();
    }
}
