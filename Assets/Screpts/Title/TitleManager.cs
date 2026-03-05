using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // ゲーム開始
    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    // ゲーム終了
    public void QuitGame()
    {
        Debug.Log("ゲーム終了");

        Application.Quit();
    }
}
