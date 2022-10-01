using UnityEngine;

public class UnitMoveToGrid : MonoBehaviour
{
    private Unit unit;
    void Start() {
        unit = GetComponent<Unit>();
    }
    void Update()
    {
        if (unit.animator == null) return;
		var gridToWorldPos = new Vector3(unit.gridPos.x, unit.gridPos.y);
		unit.animator.SetBool("Walking", gridToWorldPos != transform.position);
		if (unit.animator.GetCurrentAnimatorStateInfo(0).IsName("walk")) transform.position = Vector3.MoveTowards(transform.position, gridToWorldPos, Time.deltaTime);
    }
}
