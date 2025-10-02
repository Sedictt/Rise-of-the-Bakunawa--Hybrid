using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void Options()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void CardsGallery()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void Credits()
    {
        SceneManager.LoadSceneAsync(4);
    }
}
