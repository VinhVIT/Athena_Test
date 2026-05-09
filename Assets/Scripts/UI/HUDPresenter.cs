using TMPro;
using UnityEngine;

public class HUDPresenter : MonoBehaviour
{
    [SerializeField] private TMP_Text movesText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text levelText;
    public void Bind(LevelSession session)
    {
        session.OnMovesChanged += value => movesText.text = $"Moves: {value}";
        session.OnScoreChanged += value => scoreText.text = $"Score: {value}";
        session.OnLevelChanged += value => levelText.text = $"Level: {value + 1}";
    }
}

