using System;

public enum GameFlowState
{
    Boot,
    Menu,
    Playing,
    Result
}

public sealed class GameStateMachine
{
    public GameFlowState CurrentState { get; private set; } = GameFlowState.Boot;

    public event Action<GameFlowState, GameFlowState> OnStateChanged;

    public void ChangeState(GameFlowState next)
    {
        if (next == CurrentState)
            return;

        var previous = CurrentState;
        CurrentState = next;
        OnStateChanged?.Invoke(previous, next);
    }
}

