using System;
using System.Collections.Generic;
using UnityEngine;

public class Match3Board : MonoBehaviour
{
    [Header("Board")]
    [SerializeField] private GameObject tile;
    private LevelSession session;
    private Match3RuleSet activeRuleSet;

    private GameObject[,] tiles;

    public bool IsShifting { get; set; }

    public event Action<bool> OnRunFinished;

    public void Bind(LevelSession levelSession)
    {
        session = levelSession;
    }
    //Build Board when level start
    public void Build(Match3RuleSet ruleSet)
    {
        activeRuleSet = ruleSet;

        ClearBoard();

        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;

        CreateBoard(activeRuleSet.width, activeRuleSet.height, activeRuleSet.tileType, offset.x, offset.y);
    }

    private void CreateBoard(int width, int height, List<Sprite> tileType, float xOffset, float yOffset)
    {
        tiles = new GameObject[width, height];

        float boardWidth = (width - 1) * xOffset;
        float boardHeight = (height - 1) * yOffset;

        float startX = transform.position.x - boardWidth / 2f;
        float startY = transform.position.y - boardHeight / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 spawnPosition = new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0);

                GameObject newTile = Instantiate(tile, spawnPosition, Quaternion.identity);
                tiles[x, y] = newTile;

                RandomTile(newTile, tileType);
            }
        }
    }

    private void ClearBoard()
    {
        if (tiles == null)
            return;

        foreach (GameObject tileObject in tiles)
        {
            if (tileObject != null)
            {
                Destroy(tileObject);
            }
        }
    }
    private void RandomTile(GameObject tile, List<Sprite> tileType)
    {
        tile.transform.parent = transform;
        Sprite newSprite = tileType[UnityEngine.Random.Range(0, tileType.Count)];
        tile.GetComponent<SpriteRenderer>().sprite = newSprite;
    }
}
