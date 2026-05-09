using UnityEngine;
public class ScreenRouter : MonoBehaviour
{
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject gameplayScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    public void ShowMenu()
    {
        menuScreen.SetActive(true);
        gameplayScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public void ShowGameplay()
    {
        menuScreen.SetActive(false);
        gameplayScreen.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public void ShowResult(bool win)
    {
        gameplayScreen.SetActive(false);
        winScreen.SetActive(win);
        loseScreen.SetActive(!win);
    }
}

