using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableStartRoundWithNoUnits : MonoBehaviour
{
    public UnityEngine.UI.Button button;

    void Update()
    {
        button.interactable = GameManager.Instance.currentNumberOfPlacedUnits > 0;
    }

}
