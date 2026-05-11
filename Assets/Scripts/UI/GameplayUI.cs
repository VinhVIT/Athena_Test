using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePopup;
    public void Show()
    {
        pausePopup.SetActive(true);
    }
    public void Hide()
    {
        pausePopup.SetActive(false);
    }
}
