using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graveyard : Singleton<Graveyard>
{
    public List<GameObject> units = new ();

    public bool AddUnit(GameObject unit) {
        if (Contains(unit)) return false;
        print("Adding unit to graveyard");
        int idx = units.Count;
        units.Add(unit);
        unit.transform.SetParent(transform);
        unit.transform.localPosition = new Vector3(0, idx, 0);
        var unitComp = unit.GetComponent<Unit>();
        unitComp.currentHealth = unitComp.maxHealth;
        unitComp.enabled = false;
        return true;
    }

    public void RemoveUnit(GameObject unit) {
        units.Remove(unit);
        for (int i = 0; i < units.Count; i++) {
            units[i].transform.localPosition = new Vector3(0, i, 0);
        }
    }

    public bool Contains(GameObject unit) {
        return units.Contains(unit);
    }
}
