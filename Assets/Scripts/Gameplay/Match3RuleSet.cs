using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Match3RuleSet", menuName = "Match3/Base Rule Set")]
public class Match3RuleSet : ScriptableObject
{
    public int width = 8;
    public int height = 8;
    public List<Sprite> tileType;
    public int movesLimit = 25;
    public int targetScore = 1500;
}
