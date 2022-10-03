using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnPhase : MonoBehaviour
{
    public GamePhase phase;
    public GameObject component;
    void Update() {
        component.SetActive(GameManager.Instance.currentPhase == phase);
    }
}
