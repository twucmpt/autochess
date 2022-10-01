using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shop : MonoBehaviour
{
    public List<GameObject> unitList = new ();

    public Bench bench;
    [SerializeField]
    private GameObject[] units = new GameObject[5];
    public UnityEvent<int> SlotUpdated = new UnityEvent<int>();

    public void Start() {
        for (int i = 0; i < units.Length; i++) {
            SetShopUnit(i, units[i]);
        }
    }

    public void PurchaseSlot(int slot) {
        int cost = units[slot].GetComponent<Unit>().type.cost;
        if (GameManager.Instance.currency > cost && bench.AddUnit(units[slot])) {
            GameManager.Instance.currency -= cost;
            print("Purchase successful");
        }
        else {
            print("Purchase failed");
        }
    }

    public void SetShopUnit(int slot, GameObject unit) {
        units[slot] = unit;
        SlotUpdated.Invoke(slot);
    }

    public GameObject GetShopUnit(int slot) {
        return units[slot];
    }
}
