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
 }

 
 void OnMouseUp()
 {
    if (!enabled) return;
    pickedUp = false;
    var currentPosRounded = Vector2Int.RoundToInt(GetCurrentMousePos());
    Unit unit = GetComponent<Unit>();
    if (Graveyard.Instance.Contains(gameObject)) {
        if (currentPosRounded.x < GameManager.Instance.gridWidth/2 && GameManager.Instance.AddUnit(currentPosRounded, gameObject)) {
            transform.position = new Vector3(currentPosRounded.x, currentPosRounded.y);
            Graveyard.Instance.RemoveUnit(gameObject);
        }
        else transform.position = originalPos;
    }
    else if (Bench.Instance.Contains(gameObject)) {
        if (GameManager.Instance.currentNumberOfPlacedUnits < GameManager.Instance.maxPlacedUnits && currentPosRounded.x < GameManager.Instance.gridWidth/2 && GameManager.Instance.AddUnit(currentPosRounded, gameObject)) {
            transform.position = new Vector3(currentPosRounded.x, currentPosRounded.y);
            Bench.Instance.RemoveUnit(gameObject);
        }
        else transform.position = originalPos;
    }
    else {
        if ((currentPosRounded.x < -1 || currentPosRounded.y < 0) && Bench.Instance.AddUnit(gameObject)) {
            GameManager.Instance.RemoveUnit(unit);
            return;
        }
        if (GameManager.Instance.CheckValidPosition(currentPosRounded, tag) && currentPosRounded.x < GameManager.Instance.gridWidth/2 && unit.MoveUnit(currentPosRounded)) transform.position = new Vector3(currentPosRounded.x, currentPosRounded.y);
        else transform.position = originalPos;
    }
    
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