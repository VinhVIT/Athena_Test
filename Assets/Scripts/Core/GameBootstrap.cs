using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private ScreenRouter screenRouter;
    [SerializeField] private HUDPresenter hudPresenter;
    [SerializeField] private Match3Board board;
    [SerializeField] private Match3RuleSet[] levels;

    private readonly GameStateMachine stateMachine = new();
    private readonly LevelSession levelSession = new();

    private int currentLevel;

    private void Awake()
    {
        stateMachine.OnStateChanged += OnStateChanged;
        hudPresenter.Bind(levelSession);
        board.Bind(levelSession);
        board.OnRunFinished += OnBoardRunFinished;

        stateMachine.ChangeState(GameFlowState.Menu);
    }

    public void StartLevel(int levelIndex)
    {
        currentLevel = Mathf.Clamp(levelIndex, 0, levels.Length - 1);
        var rules = levels[currentLevel];

        levelSession.Start(currentLevel, rules.movesLimit);
        board.Build(rules);
        stateMachine.ChangeState(GameFlowState.Playing);
    }

    public void RetryLevel()
    {
        StartLevel(currentLevel);
    }

    public void NextLevel()
    {
        var next = Mathf.Min(currentLevel + 1, levels.Length - 1);
        StartLevel(next);
    }
    public void BackToMenu()
    {
        stateMachine.ChangeState(GameFlowState.Menu);
    }
    public void OnBoardRunFinished(bool win)
    {
        levelSession.Finish(win);
        stateMachine.ChangeState(GameFlowState.Result);
        screenRouter.ShowResult(win);
    }

    private void OnStateChanged(GameFlowState previous, GameFlowState next)
    {
        switch (next)
        {
            case GameFlowState.Menu:
                screenRouter.ShowMenu();
                break;
            case GameFlowState.Playing:
                screenRouter.ShowGameplay();
                break;
        }
    }
}
