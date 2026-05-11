using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransition : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float duration = 0.25f;

    private Tween currentTween;

    private void Awake()
    {
        SetAlpha(0);
    }

    public void FadeOut(Action onComplete = null)
    {
        currentTween?.Kill();

        fadeImage.raycastTarget = true;

        currentTween = fadeImage
            .DOFade(1f, duration)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }

    public void FadeIn(Action onComplete = null)
    {
        currentTween?.Kill();

        currentTween = fadeImage
            .DOFade(0f, duration)
            .OnComplete(() =>
            {
                fadeImage.raycastTarget = false;
                onComplete?.Invoke();
            });
    }

    public void FadeTransition(Action middleAction)
    {
        FadeOut(() =>
        {
            middleAction?.Invoke();

            FadeIn();
        });
    }

    private void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}