using System;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
	public Action<Cell> CellClicked;

	public int Row;
	public int Column;
	public CellMode Mode;
	public CellType Type;
	public Button Button;
	public Image Image;

	private void Start()
	{
		Button.onClick.AddListener(OnCellClicked);

		Image = GetComponent<Image>();
	}

	private void OnCellClicked()
	{
		CellClicked?.Invoke(this);
	}
}
