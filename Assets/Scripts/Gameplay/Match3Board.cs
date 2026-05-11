using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Match3Board : MonoBehaviour
{
    [Header("Board")]
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private int scorePerMatch = 10;
    [SerializeField] private BoardView boardView;
    private LevelSession session;
    private LevelData currentLevelData;
    private Tile[,] tiles;
    private Tile selectedTile;
    private int width;
    private int height;
    private Vector2 tileSize;
    public bool IsShifting { get; set; }
    public event Action<bool> OnRunFinished;
    private MatchFinder matchFinder;
    private BoardRefiller boardRefiller;
    private BoardShuffler boardShuffler;
    public void Bind(LevelSession levelSession)
    {
        session = levelSession;
    }

    public void Build(LevelData levelData)
    {
        currentLevelData = levelData;

        width = currentLevelData.width;
        height = currentLevelData.height;

        tileSize = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;

        boardView.SetupScore(session.Score, currentLevelData.targetScore);

        ClearBoard();
        CreateBoard();
        boardView.SetupBoardBackground(width, height, tileSize, transform);
        matchFinder = new MatchFinder(tiles, width, height);
        boardShuffler = new BoardShuffler(tiles, width, height, matchFinder, this);
        boardRefiller = new BoardRefiller(tiles, width, height, tilePrefab, transform,
            tileSize, currentLevelData, this);
    }

    private void CreateBoard()
    {
        tiles = new Tile[width, height];

        float boardWidth = (width - 1) * tileSize.x;
        float boardHeight = (height - 1) * tileSize.y;

        float startX = transform.position.x - boardWidth / 2f;
        float startY = transform.position.y - boardHeight / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 spawnPosition = new Vector3(startX + (x * tileSize.x),
                    startY + (y * tileSize.y), 0);
                Tile spawned = Instantiate(tilePrefab, spawnPosition, Quaternion.identity,
                    transform);

                tiles[x, y] = spawned;

                SetupRandomTileWithoutMatch(spawned, x, y);
            }
        }
    }
    public void ClearBoard()
    {
        if (tiles == null)
            return;
        foreach (Tile tile in tiles)
        {
            if (tile != null)
            {
                Destroy(tile.gameObject);
            }
        }
        boardView.HideBoardBackground();
    }

    private void SetupRandomTileWithoutMatch(Tile tile, int x, int y)
    {
        List<TileData> availableTiles = new List<TileData>(currentLevelData.availableTiles);
        TileData selectedTileData = null;

        while (availableTiles.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableTiles.Count);
            TileData randomTile = availableTiles[randomIndex];
            bool createsMatch = CreatesMatchAt(x, y, randomTile.type);

            if (!createsMatch)
            {
                selectedTileData = randomTile;
                break;
            }
            availableTiles.RemoveAt(randomIndex);
        }

        if (selectedTileData == null)
        {
            selectedTileData = currentLevelData.availableTiles[0];
        }
        tile.Setup(x, y, selectedTileData, this);
    }

    private bool CreatesMatchAt(int x, int y, TileType type)
    //only check left and down because board is generated from bottom-left to top-right 
    {
        // Horizontal
        if (x >= 2)
        {
            Tile left1 = tiles[x - 1, y];
            Tile left2 = tiles[x - 2, y];
            if (left1 != null && left2 != null && left1.Data.type == type && left2.Data.type == type)
            {
                return true;
            }
        }
        // Vertical
        if (y >= 2)
        {
            Tile down1 = tiles[x, y - 1];
            Tile down2 = tiles[x, y - 2];
            if (down1 != null && down2 != null && down1.Data.type == type && down2.Data.type == type)
            {
                return true;
            }
        }
        return false;
    }
    public void SelectTile(Tile tile)
    {
        if (IsShifting)
            return;
        // First selection
        if (selectedTile == null)
        {
            selectedTile = tile;
            selectedTile.SetSelected(true);
            return;
        }
        // Click same tile
        if (selectedTile == tile)
        {
            selectedTile.SetSelected(false);
            selectedTile = null;
            return;
        }
        bool isAdjacent = IsAdjacent(selectedTile, tile);
        // Valid pair
        if (isAdjacent)
        {
            selectedTile.SetSelected(false);
            StartCoroutine(TrySwap(selectedTile, tile));
            selectedTile = null;
        }
        else
        {
            selectedTile.SetSelected(false);
            selectedTile = tile;
            selectedTile.SetSelected(true);
        }
    }

    private bool IsAdjacent(Tile a, Tile b)
    {
        int xDifference = Mathf.Abs(a.X - b.X);
        int yDifference = Mathf.Abs(a.Y - b.Y);

        return xDifference + yDifference == 1;
    }
    private IEnumerator TrySwap(Tile firstTile, Tile secondTile)
    {
        IsShifting = true;

        // Swap logical positions first
        SwapGrid(firstTile, secondTile);

        // Play swap animation
        yield return boardView.AnimateSwap(firstTile, secondTile);

        List<Tile> matchedTiles = new();

        matchedTiles.AddRange(matchFinder.GetMatchTilesAt(firstTile.X, firstTile.Y));

        matchedTiles.AddRange(matchFinder.GetMatchTilesAt(secondTile.X, secondTile.Y));

        // Invalid move
        if (matchedTiles.Count == 0)
        {
            // Revert logical swap
            SwapGrid(firstTile, secondTile);

            // Animate back
            yield return boardView.AnimateSwap(firstTile, secondTile);

            IsShifting = false;

            yield break;
        }

        session.ConsumeMove();

        yield return StartCoroutine(ProcessMatches(matchedTiles));

        CheckGameState();

        IsShifting = false;
    }
    private void SwapGrid(Tile a, Tile b)
    {
        int aX = a.X;
        int aY = a.Y;

        int bX = b.X;
        int bY = b.Y;

        // Swap inside board array
        tiles[aX, aY] = b;
        tiles[bX, bY] = a;

        // Update coordinates
        a.SetGridPosition(bX, bY);
        b.SetGridPosition(aX, aY);
    }
    private IEnumerator ProcessMatches(List<Tile> matchedTiles, int combo = 1)
    {
        SoundManager.Instance.PlayMatch();
        int gainedScore = matchedTiles.Count * scorePerMatch * combo;
        session.AddScore(gainedScore);
        boardView.UpdateScore(session.Score);

        Vector3 centerPosition = matchedTiles[0].transform.position;
        boardView.ShowScoreText(centerPosition, gainedScore);
        if (combo > 1)
        {
            boardView.ShowComboText(matchedTiles[0].transform.position, combo);
        }

        yield return boardView.AnimateDestroy(matchedTiles);
        boardRefiller.DestroyMatches(matchedTiles);
        boardView.ShakeCamera();

        yield return StartCoroutine(boardRefiller.CollapseColumns());
        boardRefiller.RefillBoard();

        yield return new WaitForSeconds(0.35f);
        List<Tile> cascadeMatches = matchFinder.FindAllMatches();

        if (cascadeMatches.Count > 0)
        {
            yield return StartCoroutine(ProcessMatches(cascadeMatches, combo + 1));
            yield break;
        }
        if (!boardShuffler.HasAnyValidMove())
        {
            boardShuffler.ShuffleBoard();
        }
    }
    private void CheckGameState()
    {
        if (session.Score >= currentLevelData.targetScore)
        {
            SoundManager.Instance.PlayWin();
            OnRunFinished?.Invoke(true);
            ClearBoard();
            return;
        }
        if (session.MovesLeft <= 0)
        {
            SoundManager.Instance.PlayLose();
            OnRunFinished?.Invoke(false);
            ClearBoard();

        }
    }
    public void ForceWin()
    {
        OnRunFinished?.Invoke(true);
        ClearBoard();
    }
}