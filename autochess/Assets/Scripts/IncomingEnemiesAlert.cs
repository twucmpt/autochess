using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingEnemiesAlert : MonoBehaviour
{
    public TMPro.TMP_Text text;
    public GameObject alertContainer;
    public int row;
    public float alertTime = 12;

    void Update()
    {
        if(GameManager.Instance.enemySpawnQueues == null || GameManager.Instance.enemySpawnQueues[row] == null) {
            alertContainer.SetActive(false);
            return;
        }

        int count = 0;
        foreach(var enemySpawn in GameManager.Instance.enemySpawnQueues[row]) {
            if (GameManager.Instance.totalTimeInCombat+alertTime < enemySpawn.time) break;
            count++;
        }
        alertContainer.SetActive(count > 0);
        text.SetText(count.ToString());
    }
}
