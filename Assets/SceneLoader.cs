using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public AudioSource musicSource;

    public void LoadLevelSelect()
    {
        Debug.Log("KEKLIK START 🔥");

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }

    public void LoadLevel1()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevel2()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level2");
    }

    public void LoadLevel3()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level3");
    }
}