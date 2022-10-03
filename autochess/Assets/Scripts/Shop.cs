using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shop : MonoBehaviour
{
    public int rerollCost = 5;
    public int increaseMaxUnitsCost = 5;
    public List<GameObject> unitSelectionList = new ();
    public GameObject[] slots = new GameObject[5];
	public GameObject[] units = new GameObject[5];

	public Bench bench;
    public Transform shopUnitRenderer;
    [SerializeField]
    public UnityEvent<int> SlotUpdated = new UnityEvent<int>();

    public void Start() {
        RefillShop();
    }

    public void PurchaseSlot(int slot) {
        int cost = units[slot].GetComponent<Unit>().type.cost;
        if (GameManager.Instance.currency >= cost && bench.AddUnit(units[slot])) {
            GameManager.Instance.currency -= cost;
			GameManager.Instance.allPlayerUnits.Add(units[slot].GetComponent<Unit>());
            SetShopUnit(slot, null);
			GameManager.Instance.MergeLikeUnits(GameManager.Instance.allPlayerUnits);
            print("Purchase successful");
            GameManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/coinjanglelong"));
        }
        else {
            print("Purchase failed");
            GameManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/cantdothat"));

        }
    }

    public void SetShopUnit(int slot, GameObject prefab) {
        var slotRenderer = shopUnitRenderer.GetChild(slot).GetChild(0);
        if (slotRenderer.childCount > 0) {
            Destroy(slotRenderer.GetChild(0).gameObject);
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
            GameManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/roll"));
        }
        else {
            print("Rerolling failed");
            GameManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/cantdothat"));
        }
    }

    public void BuyXP() {
        if (GameManager.Instance.currency >= increaseMaxUnitsCost) {
            GameManager.Instance.currency -= increaseMaxUnitsCost;
            
            GameManager.Instance.AddXP(5);

            print("Increase xp successful");
            GameManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/coinjanglelong"));
        } else {
            print("Increase xp failed");
            GameManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/cantdothat"));
        }
    }

    void RefillShop() {
        // Todo do pick randomly
        for (int i = 0; i < slots.Length; i++) {
            SetShopUnit(i, unitSelectionList[i]);
        }
    }

}
