using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ShopUnitRenderer : MonoBehaviour
{
    public Shop shop;

    void Awake() {
        shop.SlotUpdated.AddListener(OnSlotUpdated);
    }

    void OnSlotUpdated(int slot) {
        var slotTransform = transform.GetChild(slot);
        if (slotTransform.childCount > 1) {
            Destroy(slotTransform.GetChild(1).gameObject);
        }
        GameObject unit = Instantiate(shop.GetShopUnit(slot), slotTransform);
        foreach (var component in unit.GetComponents<MonoBehaviour>()) component.enabled = false; 
    }
}
