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
     screenPoint = Camera.main.WorldToScreenPoint(transform.position);
 
     offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
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
        if (GameManager.Instance.AddUnit(currentPosRounded, gameObject)) {
            transform.position = new Vector3(currentPosRounded.x, currentPosRounded.y);
            Graveyard.Instance.RemoveUnit(gameObject);
        }
        else transform.position = originalPos;
    }
    else if (Bench.Instance.Contains(gameObject)) {
        if (GameManager.Instance.AddUnit(currentPosRounded, gameObject)) {
            transform.position = new Vector3(currentPosRounded.x, currentPosRounded.y);
            Bench.Instance.RemoveUnit(gameObject);
        }
        else transform.position = originalPos;
    }
    else {
        if (currentPosRounded.x < 0 && Bench.Instance.AddUnit(gameObject)) {
            GameManager.Instance.RemoveUnit(unit);
            return;
        }
        if (GameManager.Instance.CheckValidPosition(currentPosRounded) && unit.MoveUnit(currentPosRounded)) transform.position = new Vector3(currentPosRounded.x, currentPosRounded.y);
        else transform.position = originalPos;
    }
    
 }

 Vector3 GetCurrentMousePos() {
    Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
 
    return Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
 }
 
 }