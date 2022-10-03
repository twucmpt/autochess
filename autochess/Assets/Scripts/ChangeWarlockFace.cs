using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWarlockFace : Singleton<ChangeWarlockFace>
{
    public GameObject confident;
    public GameObject flat;
    public GameObject sad;
    public float time = 0;
    public float timeToChange = 2;

    public void Update() {
        time = Mathf.Max(0, time-Time.deltaTime);
        if (time == 0) DisplayFlat();
    }

    public void DisplayConfident() {
        confident.SetActive(true);
        flat.SetActive(false);
        sad.SetActive(false);
        time = timeToChange;
    }

    public void DisplayFlat() {
        confident.SetActive(false);
        flat.SetActive(true);
        sad.SetActive(false);
    }

    public void DisplaySad() {
        confident.SetActive(false);
        flat.SetActive(false);
        sad.SetActive(true);
        time = timeToChange;
    }
}
