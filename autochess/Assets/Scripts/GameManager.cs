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
	public GameObject pausedGameIndicator;
	public bool CanRedeployFromGrid = false;
	public bool CanRedeployFromBench = false;


	public int currency = 0;
	

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

		AddUnitFromPrefab(new Vector2Int(0, 0), zombie);
		AddUnitFromPrefab(new Vector2Int(0, 2), wizard);
		AddUnitFromPrefab(new Vector2Int(9, 0), human);
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
			time += Time.deltaTime;
		if (totalTimeInCombat > maxTimeInCombat)
			OnRoundEndTimeout();

		if (CheckRoundState())
			OnRoundEnd();

		if (time >= 10)
		{
			time -= 10;
			SwitchPhase();
		}
	}

	public void SwitchPhase() {
		switch(currentPhase) {
			case GamePhase.Combat:
				OnStartRedeployment();
				return;
			case GamePhase.Redeployment:
				OnRedeploymentPhaseEnds();
				return;
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
		pausedGameIndicator.SetActive(false);
		foreach(var unit in Graveyard.Instance.units) {
			unit.GetComponent<Draggable>().enabled = true;
		}
		foreach(var unit in unitPositions.Values) {
			unit.GetComponent<Unit>().enabled = true;
			if (unit.CompareTag("Player")) unit.GetComponent<Draggable>().enabled = false;
		}
		for(int i = 0; i < Bench.Instance.units.Length; i++) {
			if (Bench.Instance.units[i] == null) continue;
			Bench.Instance.units[i].GetComponent<Draggable>().enabled = false;
		}
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
		pausedGameIndicator.SetActive(true);

		foreach(var unit in Graveyard.Instance.units) {
			unit.GetComponent<Draggable>().enabled = true;
		}

		foreach(var unit in unitPositions.Values) {
			unit.GetComponent<Unit>().enabled = false;
			if (CanRedeployFromGrid) {
				if (unit.CompareTag("Player")) unit.GetComponent<Draggable>().enabled = true;
			}
		}

		if (CanRedeployFromBench) {
		for(int i = 0; i < Bench.Instance.units.Length; i++) {
			if (Bench.Instance.units[i] == null) continue;
			Bench.Instance.units[i].GetComponent<Draggable>().enabled = true;
		}
		}
	}

	/// <summary>
	/// Adds a Unit to the unit dictionary if the position is available
	/// </summary>
	public bool AddUnitFromPrefab(int x, int y, GameObject unitGO)
	{
		return AddUnitFromPrefab(new Vector2Int(x, y), unitGO);
	}
	/// <summary>
	/// Adds a Unit to the unit dictionary if the position is available
	/// </summary>
	public bool AddUnitFromPrefab( Vector2Int pos, GameObject unitPrefab)
	{
		if (!CheckValidPosition(pos)) return false;

		GameObject newUnitGO = Instantiate(unitPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
		return AddUnit(pos, newUnitGO);
	}

	public bool AddUnit(Vector2Int pos, GameObject unitGO) {
		if (!CheckValidPosition(pos)) return false;

		unitGO.transform.parent = transform;

		Unit unit = unitGO.GetComponent<Unit>();
		unit.gridPos = pos;

		if (unit.isEnemy)
			enemyUnitCache.Add(unit);

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
