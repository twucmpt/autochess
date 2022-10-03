using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bench : Singleton<Bench>
{
    public GameObject[] units = new GameObject[5];

    public bool AddUnit(GameObject unit) {
        if (Contains(unit)) return false;
        for(int i = 0; i < units.Length; i++) {
            if (units[i] == null) {
                print("Adding unit to bench");
                units[i] = unit;
                unit.transform.SetParent(transform);
                unit.transform.localPosition = new Vector3(0, i, 0);
                if (unit.GetComponent<Unit>().animator != null) unit.GetComponent<Unit>().animator.SetBool("Walking", false);
                unit.GetComponent<UnitMoveToGrid>().enabled = false;
                if (GameManager.Instance.currentPhase == GamePhase.Planning) unit.GetComponent<Draggable>().enabled = true;
                return true;
            }
        }
        return false;
    }

    public void RemoveUnit(GameObject unit) {
        unit.GetComponent<UnitMoveToGrid>().enabled = true;
        for (int i = 0; i < units.Length; i++) {
            if (units[i] == unit) {
                print("Removing unit from bench");
                units[i] = null;
                return;
            }
        }
    }

    public bool Contains(GameObject unit) {
        foreach (var go in units) {
            if (go == unit) return true;
        }
        return false;
    }
}
