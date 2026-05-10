using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BoardRefiller
{
    private Match3Board board;
    private Tile[,] tiles;
    private int width;
    private int height;
    private Tile tilePrefab;
    private Transform boardParent;
    private Vector2 tileSize;
    private LevelData levelData;

    public BoardRefiller(Tile[,] tiles, int width, int height, Tile tilePrefab,
        Transform boardParent, Vector2 tileSize, LevelData levelData, Match3Board board)
    {
        this.tiles = tiles;
        this.width = width;
        this.height = height;
        this.tilePrefab = tilePrefab;
        this.boardParent = boardParent;
        this.tileSize = tileSize;
        this.levelData = levelData;
        this.board = board;
    }

    public void DestroyMatches(List<Tile> matchedTiles)
    {
        foreach (Tile tile in matchedTiles)
        {
            if (tile == null)
                continue;

            int x = tile.X;
            int y = tile.Y;

            tiles[x, y] = null;
            Object.Destroy(tile.gameObject);
        }
    }

    public IEnumerator CollapseColumns()
    {
        List<Tween> tweens = new();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] != null)
                    continue;

                for (int aboveY = y + 1; aboveY < height; aboveY++)
                {
                    Tile aboveTile = tiles[x, aboveY];
                    if (aboveTile == null)
                        continue;

                    Tween moveTween = MoveTileTo(aboveTile, x, y);
                    tweens.Add(moveTween);
                    break;
                }
            }
        }
        foreach (Tween tween in tweens)
        {
            yield return tween.WaitForCompletion();
        }
    }

    private Tween MoveTileTo(Tile tile, int newX, int newY)
    {
        tiles[tile.X, tile.Y] = null;
        tiles[newX, newY] = tile;

        tile.SetGridPosition(newX, newY);

        Vector3 targetPosition = GetWorldPosition(newX, newY);
        return tile.transform.DOMove(targetPosition, 0.4f).SetEase(Ease.OutBounce);
    }

    public void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] != null)
                    continue;

                SpawnNewTile(x, y);
            }
        }
    }

    private void SpawnNewTile(int x, int y)
    {
        Vector3 targetPosition = GetWorldPosition(x, y);
        Vector3 spawnPosition = targetPosition + Vector3.up * 2f;

        Tile spawned = Object.Instantiate(tilePrefab, spawnPosition, Quaternion.identity, boardParent);
        tiles[x, y] = spawned;

        SetupRandomTile(spawned, x, y);
        spawned.transform.DOMove(targetPosition, 0.25f).SetEase(Ease.OutBounce);
    }

    private void SetupRandomTile(Tile tile, int x, int y)
    {
        int randomIndex = Random.Range(0, levelData.availableTiles.Count);
        TileData tileData = levelData.availableTiles[randomIndex];
        tile.Setup(x, y, tileData, board);
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        float boardWidth = (width - 1) * tileSize.x;
        float boardHeight = (height - 1) * tileSize.y;

        float startX = boardParent.position.x - boardWidth / 2f;
        float startY = boardParent.position.y - boardHeight / 2f;

        return new Vector3(startX + (x * tileSize.x), startY + (y * tileSize.y), 0);
    }
}