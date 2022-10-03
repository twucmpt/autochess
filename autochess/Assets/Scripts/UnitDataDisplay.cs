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
        if (this.slot != slot) return;
        Unit unit = shop.GetShopUnit(slot).GetComponent<Unit>();
        costText.SetText(unit.type.cost.ToString());
        nameText.SetText(unit.gameObject.name.ToString().Replace("(Clone)",""));
        healthText.SetText(unit.maxHealth.ToString());
        powerText.SetText(unit.power.ToString());
    }
}
