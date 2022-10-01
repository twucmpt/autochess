using UnityEngine;

public class GameOverText : MonoBehaviour
{
    public TMPro.TMP_Text text;
    void Start()
    {
        text.SetText("Congratulations! You have survived " + GameData.roundsSurvived + " rounds.");
    }
}
