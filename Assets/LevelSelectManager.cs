using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;

    void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        level1Button.interactable = true;
        level2Button.interactable = unlockedLevel >= 2;
        level3Button.interactable = unlockedLevel >= 3;
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level3");
    }
}