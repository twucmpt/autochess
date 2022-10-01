using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int gridWidth;
    public int gridHeight;

	public List<UnitListHelper> unitListHelpers = new();

	public Dictionary<Vector2Int, Unit> unitPositions = new();
    public Dictionary<unitTypes, UnitType> unitTypeEnumToClass = new();

	

    protected override void Awake() 
	{
		base.Awake();
		Init();

	}

	public void Init()
	{
		unitTypeEnumToClass.Add(unitTypes.MeleeZombie, new MeleeZombie());
		unitTypeEnumToClass.Add(unitTypes.BowSkeleton, new BowSkeleton());
		unitTypeEnumToClass.Add(unitTypes.HumanPeasent, new HumanPeasent());

		InitTest();

	}

	private void InitTest()
	{
		GameObject zombie = unitListHelpers.Where(x => x.type == unitTypes.MeleeZombie).FirstOrDefault().unitPrefab;
		GameObject wizard = unitListHelpers.Where(x => x.type == unitTypes.BowSkeleton).FirstOrDefault().unitPrefab;
		GameObject human = unitListHelpers.Where(x => x.type == unitTypes.HumanPeasent).FirstOrDefault().unitPrefab;

		AddUnit(new Vector2Int(0, 0), zombie);
		AddUnit(new Vector2Int(0, 2), wizard);
		AddUnit(new Vector2Int(9, 0), human);
	}

	/// <summary>
	/// Adds a Unit to the unit dictionary if the position is available
	/// </summary>
	public bool AddUnit(int x, int y, GameObject unitGO)
	{
		return AddUnit(new Vector2Int(x, y), unitGO);
	}
	/// <summary>
	/// Adds a Unit to the unit dictionary if the position is available
	/// </summary>
	public bool AddUnit( Vector2Int pos, GameObject unitPrefab)
	{
		GameObject newUnitGO = Instantiate(unitPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
		newUnitGO.gameObject.transform.parent = transform;

		Unit unit = newUnitGO.GetComponent<Unit>();
		unit.gridPos = pos;

		if (CheckValidPosition(pos)) return false;

		unitPositions.Add(pos, unit);
		return true;
	}
	public void RemoveUnit(Unit unit)
	{
		var pos = unit.gridPos;
		unitPositions.Remove(unit.gridPos);
	}

	public UnitType GetUnitType(unitTypes type)
	{
		switch (type)
		{
			case unitTypes.MeleeZombie:
				return new MeleeZombie();
			case unitTypes.BowSkeleton:
				return new BowSkeleton();
			case unitTypes.HumanPeasent:
				return new HumanPeasent();

		}

		return new MeleeZombie();
	}
	public bool CheckValidPosition(Vector2Int pos)
	{
		if (pos.x < 0 || pos.x >= gridWidth)
			return false;
		if (pos.y < 0 || pos.y >= gridHeight)
			return false;

		if (unitPositions.ContainsKey(pos))
			return false;

		return true;
	}

}

[System.Serializable]
public class UnitListHelper
{
	public unitTypes type;
	public GameObject unitPrefab;
}
