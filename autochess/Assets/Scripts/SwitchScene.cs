using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public void LoadScene(string scene) {
        SceneManager.LoadScene(scene);
    }
}
