using UnityEngine;

public class Tile : MonoBehaviour
{
	[Header("Visual")]
	[SerializeField] private SpriteRenderer spriteRenderer;
	public TileData Data { get; private set; }
	public int X { get; private set; }
	public int Y { get; private set; }
	private Match3Board board;
	public void Setup(int x, int y, TileData data, Match3Board board)
	{
		SetGridPosition(x, y);

		Data = data;
		this.board = board;
		spriteRenderer.sprite = data.sprite;
	}
	private void OnMouseDown()
	{
		board.SelectTile(this);
	}
	public void SetSelected(bool selected)
	{
		spriteRenderer.color = selected ? Color.gray : Color.white;
	}
	public void SetGridPosition(int x, int y)
	{
		X = x;
		Y = y;
	}
}
public enum TileType
{
	Red,
	Blue,
	Green,
	Yellow,
	Purple
}