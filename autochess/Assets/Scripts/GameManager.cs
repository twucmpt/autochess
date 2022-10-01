using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int gridWidth;
    public int gridHeight;

    public Dictionary<Vector2Int, Unit> unitPositions = new();
    public Dictionary<unitTypes, UnitType> unitTypeEnumToClass = new();


    protected override void Awake() 
	{
        base.Awake();
    }

	public void Init()
	{
		unitTypeEnumToClass.Add(unitTypes.MeleeZombie, new MeleeZombie());
		unitTypeEnumToClass.Add(unitTypes.BowSkeleton, new BowSkeleton());

	}

	/// <summary>
	/// Adds a Unit to the unit dictionary if the position is available
	/// </summary>
	public bool AddUnit(Unit unit, int x, int y)
	{
		return AddUnit(unit, new Vector2Int(x, y));
	}
	/// <summary>
	/// Adds a Unit to the unit dictionary if the position is available
	/// </summary>
	public bool AddUnit(Unit unit, Vector2Int pos)
	{


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
		}

		return new MeleeZombie();
	}
	public bool CheckValidPosition(Vector2Int pos)
	{
		if (pos.x >= 0 && pos.x < gridWidth)
			return false;
		if (pos.y >= 0 && pos.y < gridHeight)
			return false;

		if (!unitPositions.ContainsKey(pos))
			return false;
		return true;
	}

}
