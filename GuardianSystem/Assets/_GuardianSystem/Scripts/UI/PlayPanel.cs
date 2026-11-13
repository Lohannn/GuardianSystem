using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayPanel : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("StageScene");
    }
}
