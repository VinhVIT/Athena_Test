using System.Collections.Generic;
using UnityEngine;

public class BoardShuffler
{
    private Tile[,] tiles;

    private int width;
    private int height;

    private MatchFinder matchFinder;

    private Match3Board board;

    public BoardShuffler(Tile[,] tiles, int width, int height, MatchFinder matchFinder, Match3Board board)
    {
        this.tiles = tiles;

        this.width = width;
        this.height = height;

        this.matchFinder = matchFinder;

        this.board = board;
    }

    public bool HasAnyValidMove()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile currentTile = tiles[x, y];

                if (currentTile == null)
                    continue;

                bool canMatchRight = CheckSwapCreatesMatch(x, y, x + 1, y);

                if (canMatchRight)
                    return true;

                bool canMatchUp = CheckSwapCreatesMatch(x, y, x, y + 1);

                if (canMatchUp)
                    return true;
            }
        }

        return false;
    }

    public void ShuffleBoard()
    {
        List<TileData> allTileData = new();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];

                if (tile == null)
                    continue;

                allTileData.Add(tile.Data);
            }
        }

        ShuffleList(allTileData);

        int index = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];

                if (tile == null)
                    continue;

                tile.Setup(x, y, allTileData[index], board);

                index++;
            }
        }

        List<Tile> matches = matchFinder.FindAllMatches();

        if (matches.Count > 0 || !HasAnyValidMove())
        {
            ShuffleBoard();
        }
    }

    private bool CheckSwapCreatesMatch(int x1, int y1, int x2, int y2)
    {
        if (!IsInsideBoard(x2, y2))
            return false;

        Tile firstTile = tiles[x1, y1];
        Tile secondTile = tiles[x2, y2];

        if (firstTile == null || secondTile == null)
            return false;

        SwapTiles(firstTile, secondTile);

        bool hasMatch = matchFinder.GetMatchTilesAt(firstTile.X, firstTile.Y).Count > 0 ||
            matchFinder.GetMatchTilesAt(secondTile.X, secondTile.Y).Count > 0;

        SwapTiles(firstTile, secondTile);

        return hasMatch;
    }

    private void SwapTiles(Tile a, Tile b)
    {
        int aX = a.X;
        int aY = a.Y;

        int bX = b.X;
        int bY = b.Y;

        tiles[aX, aY] = b;
        tiles[bX, bY] = a;

        a.SetGridPosition(bX, bY);
        b.SetGridPosition(aX, aY);
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private bool IsInsideBoard(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}