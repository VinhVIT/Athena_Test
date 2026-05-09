using UnityEngine;

[CreateAssetMenu(menuName = "Match3/Tile Data")]
public class TileData : ScriptableObject
{
    public TileType type;
    public Sprite sprite;
}