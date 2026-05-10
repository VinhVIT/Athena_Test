using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Match3/Base Level")]
public class LevelData : ScriptableObject
{
    public int width = 8;
    public int height = 8;
    public List<TileData> availableTiles;
    public int movesLimit = 25;
    public int targetScore = 1500;
}
