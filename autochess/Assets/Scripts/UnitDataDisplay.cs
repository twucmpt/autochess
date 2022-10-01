using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDataDisplay : MonoBehaviour
{
    public Shop shop;
    public int slot;
    public TMPro.TMP_Text costText;

    void Awake() {
        shop.SlotUpdated.AddListener(OnSlotUpdated);
    }

    public void OnSlotUpdated(int slot) {
        Unit unit = shop.GetShopUnit(slot).GetComponent<Unit>();
        costText.SetText(unit.type.cost.ToString());
    }
}
