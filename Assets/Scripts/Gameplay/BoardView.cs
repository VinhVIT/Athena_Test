using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private FloatingText floatingTextPrefab;
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private SpriteRenderer boardBackground;
    [SerializeField] private float padding = 0.3f;

    [Header("UI")]
    [SerializeField] private Slider scoreSlider;

    public void SetupBoardBackground(int width, int height, Vector2 tileSize,
         Transform boardTransform)
    {
        if (boardBackground == null)
            return;

        float boardWidth = width * tileSize.x;
        float boardHeight = height * tileSize.y;

        boardBackground.transform.position = boardTransform.position;

        Sprite sprite = boardBackground.sprite;

        float spriteWidth = sprite.bounds.size.x;
        float spriteHeight = sprite.bounds.size.y;

        boardBackground.transform.localScale = new Vector3((boardWidth + padding) / spriteWidth,
             (boardHeight + padding) / spriteHeight, 1f);

        boardBackground.gameObject.SetActive(true);
    }

    public void HideBoardBackground()
    {
        if (boardBackground != null)
        {
            boardBackground.gameObject.SetActive(false);
        }
    }

    public void SetupScore(int currentScore, int targetScore)
    {
        scoreSlider.maxValue = targetScore;
        scoreSlider.value = currentScore;
    }

    public void UpdateScore(int score)
    {
        scoreSlider.DOValue(score, 0.25f);
    }

    public IEnumerator AnimateSwap(Tile a, Tile b)
    {
        Vector3 aPosition = a.transform.position;
        Vector3 bPosition = b.transform.position;

        Tween moveA = a.transform.DOMove(bPosition, 0.15f).SetEase(Ease.OutQuad);

        Tween moveB = b.transform.DOMove(aPosition, 0.15f).SetEase(Ease.OutQuad);

        yield return moveA.WaitForCompletion();
        yield return moveB.WaitForCompletion();
    }

    public IEnumerator AnimateDestroy(List<Tile> matchedTiles)
    {
        List<Tween> tweens = new();

        foreach (Tile tile in matchedTiles)
        {
            if (tile == null)
                continue;

            tile.transform.DOPunchScale(Vector3.one * 0.2f, 0.15f);

            Tween scaleTween = tile.transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack);
            tweens.Add(scaleTween);
        }

        foreach (Tween tween in tweens)
        {
            yield return tween.WaitForCompletion();
        }
    }

    public void ShowScoreText(Vector3 worldPosition, int score)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        FloatingText text = Instantiate(floatingTextPrefab, screenPosition,
             Quaternion.identity, worldCanvas.transform);

        text.Setup($"+{score}", Color.white);
    }

    public void ShowComboText(Vector3 worldPosition, int combo)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition + Vector3.up * 0.5f);
        FloatingText text = Instantiate(floatingTextPrefab, screenPosition,
             Quaternion.identity, worldCanvas.transform);
        text.Setup($"COMBO x{combo}", Color.yellow);
    }

    public void ShakeCamera()
    {
        mainCamera.transform.DOShakePosition(0.1f, 0.08f);
    }
}