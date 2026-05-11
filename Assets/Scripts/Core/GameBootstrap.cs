using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private const string CurrentLevelKey = "CURRENT_LEVEL";
    [SerializeField] private ScreenRouter screenRouter;
    [SerializeField] private ScreenTransition screenTransition;
    [SerializeField] private HUDPresenter hudPresenter;
    [SerializeField] private Match3Board board;
    [SerializeField] private LevelData[] levels;

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
        screenTransition.FadeTransition(() =>
        {
            currentLevel = Mathf.Clamp(levelIndex, 0, levels.Length - 1);
            SaveCurrentLevel();

            var levelData = levels[currentLevel];
            levelSession.Start(currentLevel, levelData.movesLimit);
            board.Build(levelData);
            stateMachine.ChangeState(GameFlowState.Playing);
        });
    }
    public void ContinueGame()
    {
        if (!HasSave()) return;

        int savedLevel = LoadCurrentLevel();
        StartLevel(savedLevel);
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
    private void SaveCurrentLevel()
    {
        PlayerPrefs.SetInt(CurrentLevelKey, currentLevel);
        PlayerPrefs.Save();
    }

    private int LoadCurrentLevel()
    {
        int savedLevel = PlayerPrefs.GetInt(CurrentLevelKey, 0);

        return Mathf.Clamp(savedLevel, 0, levels.Length - 1);
    }
    public bool HasSave()
    {
        return PlayerPrefs.HasKey(CurrentLevelKey);
    }
    public void BackToMenu()
    {
        screenTransition.FadeTransition(() =>
        {
            stateMachine.ChangeState(GameFlowState.Menu);
            board.ClearBoard();
        });

    }
    public void OnBoardRunFinished(bool win)
    {
        screenTransition.FadeTransition(() =>
        {
            levelSession.Finish(win);
            stateMachine.ChangeState(GameFlowState.Result);
            screenRouter.ShowResult(win);
        });
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(CurrentLevelKey);
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
