using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDataDisplay : MonoBehaviour
{
    public Shop shop;
    public int slot;
    public TMPro.TMP_Text costText, nameText, healthText, powerText;

    void Awake() {
        shop.SlotUpdated.AddListener(OnSlotUpdated);
    }

    public void OnSlotUpdated(int slot) {
        Unit unit = shop.GetShopUnit(slot).GetComponent<Unit>();
        costText.SetText(unit.type.cost.ToString());
        //nameText.SetText(unit.type.name.ToString());
        //healthText.SetText(unit.type.stats.health.ToString());
        powerText.SetText(unit.power.ToString());
    }
}
