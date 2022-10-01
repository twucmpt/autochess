using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunParticles : MonoBehaviour
{

    public GameObject stunParticles;
    void Update()
    {
        if (CompareTag("Enemy")) stunParticles.SetActive(GameManager.Instance.currentPhase == GamePhase.Redeployment);
    }
}
