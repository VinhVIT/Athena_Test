using System.Collections.Generic;
using UnityEngine;

public class MatchFinder
{
    private Tile[,] tiles;
    private int width;
    private int height;

    public MatchFinder(Tile[,] tiles, int width, int height)
    {
        this.tiles = tiles;
        this.width = width;
        this.height = height;
    }

    public List<Tile> GetMatchTilesAt(int x, int y)
    {
        List<Tile> matchedTiles = new();

        Tile centerTile = tiles[x, y];

        if (centerTile == null)
            return matchedTiles;

        TileType type = centerTile.Data.type;

        List<Tile> horizontalMatches = new();
        List<Tile> verticalMatches = new();

        horizontalMatches.Add(centerTile);
        verticalMatches.Add(centerTile);

        horizontalMatches.AddRange(CollectMatches(x, y, -1, 0, type));
        horizontalMatches.AddRange(CollectMatches(x, y, 1, 0, type));

        verticalMatches.AddRange(CollectMatches(x, y, 0, -1, type));
        verticalMatches.AddRange(CollectMatches(x, y, 0, 1, type));

        if (horizontalMatches.Count >= 3)
            matchedTiles.AddRange(horizontalMatches);

        if (verticalMatches.Count >= 3)
            matchedTiles.AddRange(verticalMatches);

        return matchedTiles;
    }

    public List<Tile> FindAllMatches()
    {
        List<Tile> allMatches = new();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];

                if (tile == null)
                    continue;

                List<Tile> matches = GetMatchTilesAt(x, y);

                foreach (Tile match in matches)
                {
                    if (!allMatches.Contains(match))
                    {
                        allMatches.Add(match);
                    }
                }
            }
        }

        return allMatches;
    }

    private List<Tile> CollectMatches(int startX, int startY,
        int directionX, int directionY, TileType targetType)
    {
        List<Tile> matchedTiles = new();

        int x = startX + directionX;
        int y = startY + directionY;

        while (IsInsideBoard(x, y))
        {
            Tile tile = tiles[x, y];

            if (tile == null)
                break;

            if (tile.Data.type != targetType)
                break;

            matchedTiles.Add(tile);

            x += directionX;
            y += directionY;
        }

        return matchedTiles;
    }

    private bool IsInsideBoard(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}