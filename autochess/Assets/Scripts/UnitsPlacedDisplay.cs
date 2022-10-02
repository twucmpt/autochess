using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsPlacedDisplay : MonoBehaviour
{
    TMPro.TMP_Text text;
    void Start()
    {
        text = GetComponent<TMPro.TMP_Text>();
    }

    void Update() {
        text.SetText(string.Format("Units Placed: {0}/{1}", GameManager.Instance.currentNumberOfPlacedUnits, GameManager.Instance.maxPlacedUnits));
    }
}
