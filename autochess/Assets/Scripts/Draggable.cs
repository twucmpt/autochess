 using UnityEngine;
 using System.Collections;
  
 public class Draggable : MonoBehaviour 
 { 
 private Vector3 originalPos;
 private Vector3 screenPoint;
 private Vector3 offset;
 private bool pickedUp = false;
 
 void Update() {}

 void OnMouseDown()
 {
    if (!enabled) return;
    originalPos = transform.position;
    pickedUp = true;
 }
 
 void OnMouseDrag()
 {
    if (!enabled && pickedUp) {
        transform.position = originalPos;
        pickedUp = false;
        return;
    }
    if (!enabled) return;
    transform.position = GetCurrentMousePos();
    if (GetComponent<Unit>().animator != null) GetComponent<Unit>().animator.SetBool("Walking", false);
 }

 
 void OnMouseUp()
 {
    if (!enabled) return;
    pickedUp = false;
    var currentPosRounded = Vector2Int.RoundToInt(GetCurrentMousePos());
    Unit unit = GetComponent<Unit>();
    Unit unitAtPos = GameManager.Instance.unitPositions.ContainsKey(currentPosRounded)? GameManager.Instance.unitPositions[currentPosRounded] : null;

    if (Graveyard.Instance.Contains(gameObject)) {
        if (currentPosRounded.x < GameManager.Instance.gridWidth / 2 && GameManager.Instance.AddUnit(currentPosRounded, gameObject)) {
            transform.position = new Vector3(currentPosRounded.x, currentPosRounded.y);
            Graveyard.Instance.RemoveUnit(gameObject);
            GameManager.Instance.PlaySFX(unit.type.GetSound("placement"));
        } else {
            transform.position = originalPos;
            GameManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/cantdothat"));
        }
    } else if (Bench.Instance.Contains(gameObject)) {
            if (GameManager.Instance.currentNumberOfPlacedUnits < GameManager.Instance.maxPlacedUnits && currentPosRounded.x < GameManager.Instance.gridWidth / 2 && GameManager.Instance.AddUnit(currentPosRounded, gameObject)) {
                transform.position = new Vector3(currentPosRounded.x, currentPosRounded.y);
                Bench.Instance.RemoveUnit(gameObject);
                GameManager.Instance.PlaySFX(unit.type.GetSound("placement"));
            } else {
                if (currentPosRounded.x < GameManager.Instance.gridWidth / 2 && unitAtPos != null && unitAtPos != unit && unitAtPos.CompareTag("Player")) {
                    SwapUnits(unit, unitAtPos);
                    return;
                }
                GameManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/cantdothat"));
                transform.position = originalPos;
            }
        }
    else {
        if ((currentPosRounded.x < -1 || currentPosRounded.y < 0) && Bench.Instance.AddUnit(gameObject)) {
            GameManager.Instance.RemoveUnit(unit);
            return;
        }
        else {
            var currentPosRoundedForBench = Vector2Int.RoundToInt(GetCurrentMousePos() + new Vector3(-1.5f, 0));
            if (currentPosRoundedForBench.y == -1 && currentPosRoundedForBench.x >= 0 && currentPosRoundedForBench.x < 5) {
                var unitAtBench = Bench.Instance.units[currentPosRoundedForBench.x];
                if (unitAtBench != null) {
                    SwapUnits(unit, unitAtBench.GetComponent<Unit>());
                    return;
                }
            }
        }
        if (GameManager.Instance.CheckValidPosition(currentPosRounded, tag) && currentPosRounded.x < GameManager.Instance.gridWidth / 2 && unit.MoveUnit(currentPosRounded)) transform.position = new Vector3(currentPosRounded.x, currentPosRounded.y);
        else {
            if (currentPosRounded.x < GameManager.Instance.gridWidth / 2 && unitAtPos != null && unitAtPos != unit && unitAtPos.CompareTag("Player")) {
                SwapUnits(unit, unitAtPos);
                return;
            }
            if (currentPosRounded.x != originalPos.x || currentPosRounded.y != originalPos.y) {
                    GameManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/cantdothat"));
            }
            transform.position = originalPos;
        }
    }
    
 }

 private void SwapUnits(Unit unit1, Unit unit2) {
    bool unit1Benched = Bench.Instance.Contains(unit1.gameObject);
    bool unit2Benched = Bench.Instance.Contains(unit2.gameObject);
    if (unit1Benched || unit2Benched) {
        if (GameManager.Instance.currentPhase == GamePhase.Planning) {
            GameManager.Instance.PlaySFX(Resources.Load<AudioClip>("SFX/cantdothat"));
            return;
        }
        Unit benchedUnit = null;
        Unit placedUnit = null;
        if (unit1Benched) {
            benchedUnit = unit1;
            placedUnit = unit2;
        }
        else {
            placedUnit = unit1;
            benchedUnit = unit2;
        }
        GameManager.Instance.RemoveUnit(placedUnit);

        // Add from bench
        GameManager.Instance.AddUnit(placedUnit.gridPos, benchedUnit.gameObject);
        benchedUnit.transform.position = new Vector3(placedUnit.gridPos.x, placedUnit.gridPos.y);
        Bench.Instance.RemoveUnit(benchedUnit.gameObject);
        GameManager.Instance.PlaySFX(unit1.type.GetSound("placement"));

        // Add to bench
        Bench.Instance.AddUnit(placedUnit.gameObject);
        return;
    }

    GameManager.Instance.RemoveUnit(unit1);
    GameManager.Instance.RemoveUnit(unit2);

    var temp = unit1.gridPos;
    GameManager.Instance.AddUnit(unit2.gridPos, unit1.gameObject);
    GameManager.Instance.AddUnit(temp, unit2.gameObject);

    unit1.transform.position = new Vector3(unit1.gridPos.x, unit1.gridPos.y);
    unit2.transform.position = new Vector3(unit2.gridPos.x, unit2.gridPos.y);
 }

 Vector3 GetCurrentMousePos() {
    RaycastHit hit;
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out hit, 10000.0f, LayerMask.GetMask("Board"))) {
        return hit.point;
    }
    throw new UnityException();
 }
 
 }