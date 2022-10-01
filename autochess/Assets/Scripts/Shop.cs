using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shop : MonoBehaviour
{
    public int rerollCost = 5;
    public List<GameObject> unitSelectionList = new ();
    public GameObject[] slots = new GameObject[5];

    public Bench bench;
    public Transform shopUnitRenderer;
    [SerializeField]
    private GameObject[] units = new GameObject[5];
    public UnityEvent<int> SlotUpdated = new UnityEvent<int>();

    public void Start() {
        RefillShop();
    }

    public void PurchaseSlot(int slot) {
        int cost = units[slot].GetComponent<Unit>().type.cost;
        if (GameManager.Instance.currency >= cost && bench.AddUnit(units[slot])) {
            GameManager.Instance.currency -= cost;

            SetShopUnit(slot, null);
            print("Purchase successful");
        }
        else {
            print("Purchase failed");
        }
    }

    public void SetShopUnit(int slot, GameObject prefab) {
        var slotRenderer = shopUnitRenderer.GetChild(slot);
        if (slotRenderer.childCount > 1) {
            Destroy(slotRenderer.GetChild(1).gameObject);
        }

        if (prefab == null) {
            slots[slot].SetActive(false);
        }
        else {
            slots[slot].SetActive(true);
            units[slot] = Instantiate(prefab, slotRenderer);
            units[slot].GetComponent<Unit>().Init();
            units[slot].GetComponent<Unit>().enabled = false;
            units[slot].GetComponent<UnitMoveToGrid>().enabled = false;
        }

        SlotUpdated.Invoke(slot);
    }

    public GameObject GetShopUnit(int slot) {
        return units[slot];
    }

    public void RerollShop() {
        if (GameManager.Instance.currency >= rerollCost) {
            GameManager.Instance.currency -= rerollCost;
            RefillShop();
            print("Rerolling successful");
        }
        else {
            print("Rerolling failed");
        }
    }

    void RefillShop() {
        // Todo do pick randomly
        for (int i = 0; i < units.Length; i++) {
            SetShopUnit(i, unitSelectionList[i]);
        }
    }
}
