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

	public int enemiesRemaining = 2;
	public float spawnInterval = 5;
	public int maxPlacedUnits = 1;
	public int currentNumberOfPlacedUnits {get{
		int count = Graveyard.Instance.units.Count;
		foreach (var unit in unitPositions.Values) {
			if (unit.CompareTag("Player")) count ++;
		}
		return count;
	}}

	private Dictionary<float, List<GameObject>> cachedEnemySelectionWeight = new();

	public List<EnemySpawn>[] enemySpawnQueues;

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

	}


	/// <summary>
	/// Time in Seconds
	/// </summary>
	public float time = 0;
	/// <summary>
	/// Time in Seconds
	/// </summary>
	public float totalTimeInCombat = 0;
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
			HandleIncomingEnemies();
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

		enemiesRemaining = 10;
		GenerateEnemies(currentDifficulty);
	}

	/// <summary>
	/// A method that is called when the tactical phase ends
	/// </summary>
	public void OnPlanningPhaseEnd()
	{
		currentPhase = GamePhase.Combat;
		CanRedeployFromGrid = false;
		CanRedeployFromBench = false;
		pausedGameIndicator.SetActive(false);
		startRoundButton.SetActive(false);
		DisableRedeployment();
		RecordOriginalPositions();
	}


	/// <summary>
	/// Checks if round is over
	/// </summary>
	/// <returns>true = round is done, false = round is NOT done</returns>
	public bool CheckRoundState()
	{
		return (enemyUnitCache.Count == 0 && enemiesRemaining == 0);
	}

	/// <summary>
	/// A method that is called when the Redeployment phase ends, and the Combat Phase resumes
	/// </summary>
	public void OnRedeploymentPhaseEnds()
	{
		currentPhase = GamePhase.Combat;
		pausedGameIndicator.SetActive(false);
		DisableRedeployment();
	}

	/// <summary>
	/// A method to handle the situation when the round was ended due to the round taking too long. 
	/// </summary>
	public void OnRoundEndTimeout()
	{
		currentPhase = GamePhase.Planning;
		OnPlanningPhaseStart();
	}

	/// <summary>
	/// A method to handle the end of a round due to all enemies being killed.
	/// </summary>
	public void OnRoundEnd()
	{
		currentPhase = GamePhase.Planning;
		round++;
		currency += currencyPerRound;
		OnPlanningPhaseStart();
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

	/// <summary>
	/// A method that handles spawning new enemies on the board at particular times
	/// </summary>
	public void HandleIncomingEnemies()
	{
		SpawnEnemies();
	}

	public void SpawnEnemies()
	{
		for (int i = 0; i < enemySpawnQueues.Length; i++)
		{
			if (enemySpawnQueues[i].Count <= 0)
				continue;
			
			if (enemySpawnQueues[i][0].time > totalTimeInCombat)
				continue;
			
			var pos = new Vector2Int(gridWidth, i);
			if (!CheckValidSpawn(pos))
				continue;

			AddUnitFromPrefab(pos, enemySpawnQueues[i][0].enemy, true);
			enemySpawnQueues[i].RemoveAt(0);
		}
	}

	public void GenerateEnemies(float difficulty)
	{
		enemySpawnQueues = new List<EnemySpawn>[gridHeight];
		for (int i = 0; i < enemySpawnQueues.Length; i++) enemySpawnQueues[i] = new();

		int wave = 0;
		int enemiesInWave = 0;
		int enemiesPerSpawnWave = UnityEngine.Random.Range(1,4);
		print("Wave " + wave);
		print("Enemies in wave: " + enemiesPerSpawnWave);
		for (int i = 0; i < enemiesRemaining; i++) {
			if (enemiesInWave >= enemiesPerSpawnWave) {
				wave++;
				enemiesInWave = 0;
				enemiesPerSpawnWave = UnityEngine.Random.Range(1,4);
				print("Wave " + wave);
				print("Enemies in wave: " + enemiesPerSpawnWave);
			}
			var selectedQueue = enemySpawnQueues[UnityEngine.Random.Range(0, enemySpawnQueues.Length)];
			var selectedEnemy = GetEnemyUnit(difficulty);
			print(selectedEnemy);
			selectedQueue.Add(new EnemySpawn(selectedEnemy, wave*spawnInterval));
			enemiesInWave++;
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
		if (!CheckValidPosition(pos, "Enemy")) return false;
		
		var hit = Physics2D.OverlapCircle(pos,0.45f);
		if (hit != null) return false;

		return true;
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

public class EnemySpawn {
	public GameObject enemy;
	public float time;
	public EnemySpawn(GameObject enemy, float time) {
		this.enemy = enemy;
		this.time = time;
	}
}