using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRenderRow : MonoBehaviour
{
    private int indexOffset = 1;
    private SpriteRenderer[] sprites;

    void Start() {
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }
    void Update() {
        int row = Mathf.RoundToInt(transform.position.y);
        int id = SortingLayer.NameToID("Background");
        if (row < 0) id = SortingLayer.NameToID("Default");
        try {
            id = SortingLayer.layers[GameManager.Instance.gridHeight-row+indexOffset-1].id;
        }
        catch {}
        foreach(var sprite in sprites) {
            sprite.sortingLayerID = id;
        }
    }
}
