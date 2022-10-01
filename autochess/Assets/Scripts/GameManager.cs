using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GamePhase
{
	Planning,
	Combat,
	Redeployment,
}

public class GameManager : Singleton<GameManager>
{
    public int gridWidth;
    public int gridHeight;
	public GamePhase currentPhase = GamePhase.Combat;

	public List<UnitListHelper> unitListHelpers = new();

	public Dictionary<Vector2Int, Unit> unitPositions = new();
	public List<Unit> enemyUnitCache = new();
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
	/// Time in Seconds
	/// </summary>
	public float time = 0;
	/// <summary>
	/// Time in Seconds
	/// </summary>
	private float totalTimeInCombat = 0;
	/// <summary>
	/// Time in Seconds
	/// </summary>
	private float maxTimeInCombat = 500;

	public void Update()
	{
		if (currentPhase == GamePhase.Combat)
		{
			time += Time.deltaTime;
			totalTimeInCombat += Time.deltaTime;
		}
		else if (currentPhase == GamePhase.Planning)
			totalTimeInCombat = 0;
		else if (currentPhase == GamePhase.Redeployment)
			return;
		if (totalTimeInCombat > maxTimeInCombat)
			OnRoundEndTimeout();

		if (CheckRoundState())
			OnRoundEnd();

		if (time >= 10)
		{
			time -= 10;
			OnStartRedeployment();
		}
	}

	/// <summary>
	/// A method that is called when the tactical phase ends
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public void OnPlanningPhaseEnd()
	{
		currentPhase = GamePhase.Combat;
		throw new NotImplementedException();
		// TODO: Things that need to happen in the combat phase that are triggered on this method call
		//	1. Dynamic Enemy Generation on a round to round basis (enemies may be continually responding)
		//	2. Players Units Activate to fight oncoming enemies
		//	3. Lockout placing/moving Units
	}


	/// <summary>
	/// Checks if round is over
	/// </summary>
	/// <returns>true = round is done, false = round is NOT done</returns>
	public bool CheckRoundState()
	{
		if (enemyUnitCache.Count > 0)
			return false;

		return true;
	}

	/// <summary>
	/// A method that is called when the Redeployment phase ends, and the Combat Phase resumes
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public void OnRedeploymentPhaseEnds()
	{
		currentPhase = GamePhase.Combat;
		throw new NotImplementedException();

		//TODO: Things that need to happen upon resuming combat
	}

	/// <summary>
	/// A method to handle the situation when the round was ended due to the round taking too long. 
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public void OnRoundEndTimeout()
	{
		currentPhase = GamePhase.Planning;
		throw new NotImplementedException();
	}

	/// <summary>
	/// A method to handle the end of a round due to all enemies being killed.
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public void OnRoundEnd()
	{
		currentPhase = GamePhase.Planning;
		throw new NotImplementedException();
	}

	/// <summary>
	/// This is the method that is called every 10 seconds to trigger the redeployment phase
	/// </summary>
	public void OnStartRedeployment()
	{
		currentPhase = GamePhase.Redeployment;
		throw new NotImplementedException();
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

		if (unit.isEnemy)
			enemyUnitCache.Add(unit);

		if (CheckValidPosition(pos)) return false;

		unitPositions.Add(pos, unit);
		return true;
	}
	public void RemoveUnit(Unit unit)
	{
		var pos = unit.gridPos;
		unitPositions.Remove(unit.gridPos);
		if (unit.isEnemy)
			enemyUnitCache.Remove(unit);
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
