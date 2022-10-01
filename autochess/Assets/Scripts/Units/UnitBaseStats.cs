using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "ScriptableObjects/SpawnManagerUnitBaseStats", order = 1)]
public class UnitBaseStats : ScriptableObject
{
	public int health;
	public int moveSpeed;
}
