using DG.Tweening;
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
		spriteRenderer.color = selected ? new Color(1f, 1f, 0.8f) : Color.white;
		transform.DOKill();
		if (selected)
		{
			transform.DOScale(0.35f, 0.4f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
		}
		else
		{
			transform.DOScale(0.3f, 0.1f).SetEase(Ease.OutQuad);
		}
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