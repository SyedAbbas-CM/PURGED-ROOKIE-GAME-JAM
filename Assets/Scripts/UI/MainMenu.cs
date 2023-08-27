using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public string levelToLoad = "Level1";

    public void onPlayButton()
    {
        SceneManager.LoadScene(levelToLoad);
    }
    public void onQuitButton()
    {

        Application.Quit(); 

    }
    public void onLevelSelectButton()
    {
        SceneManager.LoadScene("LevelSelect");
    }

}
