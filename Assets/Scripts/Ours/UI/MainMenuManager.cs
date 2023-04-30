using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame() => SceneManager.LoadScene("Level1");
    public void StartLevel(int lvlnr) => SceneManager.LoadScene("Level" + lvlnr);


    public void QuitGame() {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
    }

}
