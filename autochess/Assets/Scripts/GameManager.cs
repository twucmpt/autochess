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
	public List<EnemyListHelper> enemyListHelpers = new();

	public Dictionary<Vector2Int, Unit> unitPositions = new();
	public List<Unit> enemyUnitCache = new();
    public Dictionary<unitTypes, UnitType> unitTypeEnumToClass = new();
	public GameObject pausedGameIndicator;
	public GameObject startRoundButton;
	public bool CanRedeployFromGrid = false;
	public bool CanRedeployFromBench = false;
	public Entity warlock;

	public int enemiesRemaining = 10;

	private Dictionary<float, List<GameObject>> cachedEnemySelectionWeight = new();

	private Dictionary<Vector2Int, Stack<GameObject>> enemySpawnStacks = new();

	public int currency = 0;
	public int currencyPerRound = 10;
	public int round = 1;
	

    protected override void Awake() 
	{
		base.Awake();
		Init();

	}

	void Start() {
		OnPlanningPhaseStart();
	}

	public void Init()
	{
		unitTypeEnumToClass.Add(unitTypes.MeleeZombie, new MeleeZombie());
		unitTypeEnumToClass.Add(unitTypes.BowSkeleton, new BowSkeleton());
		unitTypeEnumToClass.Add(unitTypes.HumanPeasent, new HumanPeasent());

		enemySpawnStacks.Add(new Vector2Int(gridWidth, 0), new Stack<GameObject>());
		enemySpawnStacks.Add(new Vector2Int(gridWidth, 1), new Stack<GameObject>());
		enemySpawnStacks.Add(new Vector2Int(gridWidth, 2), new Stack<GameObject>());
		enemySpawnStacks.Add(new Vector2Int(gridWidth, 3), new Stack<GameObject>());
		enemySpawnStacks.Add(new Vector2Int(gridWidth, 4), new Stack<GameObject>());
		enemySpawnStacks.Add(new Vector2Int(gridWidth, 5), new Stack<GameObject>());

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
	public float currentDifficulty = 0;

	public void Update()
	{
		if (currentPhase == GamePhase.Combat)
		{
			time += Time.deltaTime;
			totalTimeInCombat += Time.deltaTime;
			HandleIncomingEnemies(currentDifficulty);
		}
		else if (currentPhase == GamePhase.Planning)
			totalTimeInCombat = 0;
		else if (currentPhase == GamePhase.Redeployment)
			time += Time.deltaTime;
		if (totalTimeInCombat > maxTimeInCombat)
			OnRoundEndTimeout();

		if (currentPhase != GamePhase.Planning && CheckRoundState())
			OnRoundEnd();

		if (time >= 10)
		{
			time -= 10;
			SwitchPhase();
		}

	}

	#region GameLoop Event Methods
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

	public void EnableRedeployment() {
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
	public void DisableRedeployment() {
		foreach(var unit in Graveyard.Instance.units) {
			unit.GetComponent<Draggable>().enabled = false;
		}
		foreach(var unit in unitPositions.Values) {
			unit.GetComponent<Unit>().enabled = true;
			if (unit.CompareTag("Player")) unit.GetComponent<Draggable>().enabled = false;
		}
		for(int i = 0; i < Bench.Instance.units.Length; i++) {
			if (Bench.Instance.units[i] == null) continue;
			Bench.Instance.units[i].GetComponent<Draggable>().enabled = false;
		}
	}

	public void RestoreUnitsToOriginalPlacement() {
		while (Graveyard.Instance.units.Count > 0) {
			var unit = Graveyard.Instance.units[0];
			Graveyard.Instance.units.RemoveAt(0);
			var ogPos = unit.GetComponent<Unit>().originalPosition;
			AddUnit(ogPos, unit);
			unit.transform.position = new Vector3(ogPos.x, ogPos.y, 0);
		}
		var units = unitPositions.Values.ToList<Unit>();
		foreach (var unit in units) {
			if (!unit.CompareTag("Player")) continue;
			RemoveUnit(unit);
			AddUnit(unit.originalPosition, unit.gameObject);
			unit.transform.position = new Vector3(unit.originalPosition.x, unit.originalPosition.y, 0);
		}
	}

	public void RecordOriginalPositions() {
		foreach(var kv in unitPositions) {
			kv.Value.originalPosition = kv.Key;
		}
	}

	public void OnPlanningPhaseStart() {
		currentPhase = GamePhase.Planning;
		CanRedeployFromGrid = true;
		CanRedeployFromBench = true;
		pausedGameIndicator.SetActive(true);
		startRoundButton.SetActive(true);
		RestoreUnitsToOriginalPlacement();
		EnableRedeployment();
	}

	/// <summary>
	/// A method that is called when the tactical phase ends
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public void OnPlanningPhaseEnd()
	{
		currentPhase = GamePhase.Combat;
		CanRedeployFromGrid = false;
		CanRedeployFromBench = false;
		pausedGameIndicator.SetActive(false);
		startRoundButton.SetActive(false);
		DisableRedeployment();
		RecordOriginalPositions();
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
		DisableRedeployment();
		//TODO: Things that need to happen upon resuming combat
	}

	/// <summary>
	/// A method to handle the situation when the round was ended due to the round taking too long. 
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public void OnRoundEndTimeout()
	{
		currentPhase = GamePhase.Planning;
		OnPlanningPhaseStart();
		throw new NotImplementedException();
	}

	/// <summary>
	/// A method to handle the end of a round due to all enemies being killed.
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public void OnRoundEnd()
	{
		currentPhase = GamePhase.Planning;
		round++;
		currency += currencyPerRound;
		OnPlanningPhaseStart();
		throw new NotImplementedException();
	}

	public void OnGameOver() {
		print("Congrats! You have surivived " + round + " rounds.");
		GameData.roundsSurvived = round;
		UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
	}

	/// <summary>
	/// This is the method that is called every 10 seconds to trigger the redeployment phase
	/// </summary>
	public void OnStartRedeployment()
	{
		currentPhase = GamePhase.Redeployment;
		pausedGameIndicator.SetActive(true);
		EnableRedeployment();
	}
	#endregion

	#region Add/Remove Units
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
	public bool AddUnitFromPrefab(Vector2Int pos, GameObject unitPrefab, bool EnableUnit = false)
	{
		if (!CheckValidPosition(pos, unitPrefab.tag)) return false;

		GameObject newUnitGO = Instantiate(unitPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
		return AddUnit(pos, newUnitGO, EnableUnit);
	}

	public bool AddUnit(Vector2Int pos, GameObject unitGO, bool EnableUnit = false) {
		if (!CheckValidPosition(pos, unitGO.tag)) return false;

		unitGO.transform.parent = transform;

		Unit unit = unitGO.GetComponent<Unit>();
		unit.gridPos = pos;
		unit.enabled = EnableUnit;

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
	#endregion


	private float spawnCD = 0;
	private float maxspawnCD = 5;

	/// <summary>
	/// A method that handles spawning new enemies on the board at particular times
	/// </summary>
	public void HandleIncomingEnemies(float difficulty)
	{
		if (currentPhase == GamePhase.Combat)
			spawnCD -= Time.deltaTime;

		if (spawnCD < 0)
		{
			spawnCD = maxspawnCD;
			GenerateEnemies(difficulty);
		}

		SpawnEnemies();
	}

	public void SpawnEnemies()
	{
		foreach (var loc in enemySpawnStacks)
		{
			if (loc.Value.Count <= 0)
				continue;

			if (!CheckValidSpawn(loc.Key))
				continue;

			AddUnitFromPrefab(loc.Key, loc.Value.Pop(), true);
		}
	}

	public void GenerateEnemies(float difficulty)
	{
		int unitsToSpawn = (int)Math.Min(enemiesRemaining, Math.Min(Math.Max(
				1,
				UnityEngine.Random.Range(0, Mathf.Ceil(difficulty / 3))
			), gridHeight - 1));

		List<Vector2Int> spawnLocations = Utility.TakeMultiple<Vector2Int>(enemySpawnStacks.Keys.ToList(), unitsToSpawn);

		foreach (var loc in spawnLocations)
		{
			Stack<GameObject> spawnStack = enemySpawnStacks[loc];
			spawnStack.Push(GetEnemyUnit(difficulty));
			enemiesRemaining = Math.Max(0,enemiesRemaining-1);
		}
	}


	/// <summary>
	/// Returns an enemy Gameobject prefab that depends on the difficulty passed
	/// </summary>
	/// <param name="difficulty"></param>
	/// <returns></returns>
	public GameObject GetEnemyUnit(float difficulty)
	{
		List<EnemyListHelper> newEnemy = enemyListHelpers.Where(x => x.difficulty <= difficulty).ToList();
		
		if (!cachedEnemySelectionWeight.ContainsKey(difficulty))
		{
			List<GameObject> enemySelectionList = new();
			foreach (var enemy in newEnemy)
			{
				for (int i = 0; i < enemy.weight; i++)
				{
					enemySelectionList.Add(enemy.unitPrefab);
				}
			}
			cachedEnemySelectionWeight.Add(difficulty, enemySelectionList);
		}

		return cachedEnemySelectionWeight[difficulty][UnityEngine.Random.Range(0, cachedEnemySelectionWeight[difficulty].Count-1)];
	}

	public bool CheckValidPosition(Vector2Int pos, string tag)
	{
		if (tag == "Player" && (pos.x < -1 || pos.x >= gridWidth || pos.y < 0 || pos.y >= gridHeight)) return false;
		if (tag == "Enemy" && (pos.x < 0 || pos.x >= gridWidth+1 || pos.y < 0 || pos.y >= gridHeight)) return false;

		if (unitPositions.ContainsKey(pos))
			return false;

		return true;
	}

	/// <summary>
	/// Used when determing if an enemy should be spawned onto the board
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	public bool CheckValidSpawn(Vector2Int pos)
	{
		bool validSpawn = true;
		validSpawn = CheckValidPosition(pos, "Enemy");
		return validSpawn;
	}

}

[System.Serializable]
public class UnitListHelper
{
	public unitTypes type;
	public GameObject unitPrefab;
}

[System.Serializable]
public class EnemyListHelper : UnitListHelper
{
	public float difficulty;
	public int weight;
}

