using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleGameObjectToRect : MonoBehaviour
{
    public RectTransform rect;

    void Update() {
        Vector3[] v = new Vector3[4];
        rect.GetLocalCorners(v);
        Debug.Log(v[0] + " " + v[1] + " " + v[2] + " " + v[3]);
    }

}
