using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bench : MonoBehaviour
{
    public GameObject[] units = new GameObject[5];

    public bool AddUnit(GameObject unit) {
        for(int i = 0; i < units.Length; i++) {
            if (units[i] == null) {
                units[i] = unit;
                unit.transform.SetParent(transform);
                units[i].transform.localPosition = new Vector3(0, i, 0);
                return true;
            }
        }
        return false;
    }
}
