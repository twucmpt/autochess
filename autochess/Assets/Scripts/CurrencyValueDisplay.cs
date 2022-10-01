using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyValueDisplay : MonoBehaviour
{
    public string prefix = "Gold: ";
    TMPro.TMP_Text text;
    void Start()
    {
        text = GetComponent<TMPro.TMP_Text>();
    }

    void Update() {
        text.SetText(prefix + GameManager.Instance.currency.ToString());
    }
}
