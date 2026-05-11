using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Match3/Base Level")]
public class LevelData : ScriptableObject
{
    [Range(1, 8)] public int width = 8;
    [Range(1, 8)] public int height = 8;
    public List<TileData> availableTiles;
    [Range(1, 50)] public int movesLimit = 25;
    [Range(1500, 9999)] public int targetScore = 1500;
}
