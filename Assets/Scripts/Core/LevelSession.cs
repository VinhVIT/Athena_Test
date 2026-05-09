using System;
public sealed class LevelSession
{
    public int LevelIndex { get; private set; }
    public int MovesLeft { get; private set; }
    public int Score { get; private set; }
    public bool IsCompleted { get; private set; }

    public event Action<int> OnMovesChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int> OnLevelChanged;

    public event Action<bool> OnFinished;

    public void Start(int levelIndex, int initialMoves)
    {
        LevelIndex = levelIndex;
        MovesLeft = initialMoves;
        Score = 0;
        IsCompleted = false;

        OnMovesChanged?.Invoke(MovesLeft);
        OnScoreChanged?.Invoke(Score);
        OnLevelChanged?.Invoke(levelIndex);
    }

    public void ConsumeMove()
    {
        if (IsCompleted || MovesLeft <= 0)
            return;

        MovesLeft--;
        OnMovesChanged?.Invoke(MovesLeft);
    }

    public void AddScore(int delta)
    {
        if (IsCompleted || delta <= 0)
            return;

        Score += delta;
        OnScoreChanged?.Invoke(Score);
    }

    public void Finish(bool success)
    {
        if (IsCompleted)
            return;

        IsCompleted = true;
        OnFinished?.Invoke(success);
    }
}
