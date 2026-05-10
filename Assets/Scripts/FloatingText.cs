using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void Setup(string message, Color color)
    {
        text.text = message;
        text.color = color;

        transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(1f, 0.15f).SetEase(Ease.OutBack));
        sequence.Join(transform.DOMoveY(transform.position.y + 1f, 0.8f));
        sequence.Join(text.DOFade(0f, 1f));

        sequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}