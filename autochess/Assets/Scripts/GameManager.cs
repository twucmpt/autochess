using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int gridWidth;
    public int gridHeight;
    public Unit[,] grid;

    private Dictionary<Unit, Vector2Int> unitPositions = new Dictionary<Unit, Vector2Int>();
    
    protected override void Awake() {
        base.Awake();
        grid = new Unit[gridWidth,gridHeight];
    }

    public bool MoveUnit(Unit unit, int x, int y) {
        return MoveUnit(unit, new Vector2Int(x,y));
    }

    public bool MoveUnit(Unit unit, Vector2Int newPos) {
        if (grid[newPos.x, newPos.y] is not null || !unitPositions.ContainsKey(unit)) return false;

        var pos = unitPositions[unit];
        grid[pos.x, pos.y] = null;

        grid[newPos.x,newPos.y] = unit;
        unitPositions[unit] = newPos;
        return true;
    }

    public void RemoveUnit(Unit unit) {
        var pos = unitPositions[unit];
        grid[pos.x, pos.y] = null;
        unitPositions.Remove(unit);
    }

    public bool AddUnit(Unit unit, int x, int y) {
        if (grid[x,y] is not null || unitPositions.ContainsKey(unit)) return false;
        
        grid[x,y] = unit;
        unitPositions.Add(unit, new Vector2Int(x, y));
        return true;
    }

    public Vector2Int GetGridPos(Unit unit) {
        return unitPositions[unit];
    }
}
