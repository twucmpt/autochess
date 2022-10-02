using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xpRequirementDisplay : MonoBehaviour
{
    TMPro.TMP_Text text;
    void Start()
    {
        text = GetComponent<TMPro.TMP_Text>();
    }

    void Update() {
        text.SetText(string.Format("XP: {0}/{1}", GameManager.Instance.playerXP, GameManager.Instance.xpGainedPerRound * GameManager.Instance.maxPlacedUnits));
    }
}
