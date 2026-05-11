using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePopup;
    [Header("Animation")]
    [SerializeField] private float duration = 0.2f;
    private CanvasGroup canvasGroup;
    private RectTransform panel;
    private Tween currentTween;

    private void Awake()
    {
        canvasGroup = pausePopup.GetComponent<CanvasGroup>();
        panel = pausePopup.GetComponent<RectTransform>();

        pausePopup.SetActive(false);
        canvasGroup.alpha = 0f;
        panel.localScale = Vector3.one * 0.8f;
    }

    public void Show()
    {
        currentTween?.Kill();
        pausePopup.SetActive(true);
        canvasGroup.alpha = 0f;
        panel.localScale = Vector3.one * 0.8f;

        Sequence sequence = DOTween.Sequence();
        sequence.Join(canvasGroup.DOFade(1f, duration));
        sequence.Join(panel.DOScale(1f, duration)
                .SetEase(Ease.OutBack));

        currentTween = sequence;
    }

    public void Hide()
    {
        currentTween?.Kill();
        Sequence sequence = DOTween.Sequence();
        sequence.Join(canvasGroup.DOFade(0f, duration));

        sequence.Join(panel.DOScale(0.8f, duration)
                .SetEase(Ease.InBack));
        sequence.OnComplete(() =>
        {
            pausePopup.SetActive(false);
        });
        currentTween = sequence;
    }
}