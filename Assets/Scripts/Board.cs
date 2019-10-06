using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
	public Action<Cell> OnBoardCellClicked;

	public List<List<Cell>> Cells;
	public int Columns;

	private SpriteMapping _artMapping;

	public void InitBoard(Transform root, SpriteMapping artMapping)
	{
		List<Cell> cells = GetRawCells(root);
		PrepareGrid(cells);

		_artMapping = artMapping;
	}

	public List<Cell> GetRawCells(Transform root)
	{
		var cells = new List<Cell>();

		foreach (Transform child in root) {
			var cell = child.GetComponent<Cell>();
			if (cell != null) {
				cells.Add(cell);

				cell.CellClicked += OnCellClicked;
			}
		}

		return cells;
	}

	public void PrepareGrid(List<Cell> cells)
	{
		Cells = new List<List<Cell>>();

		int i = 0;
		var tempCells = new List<Cell>();
		int elementIndex = 0;
		foreach (Cell cell in cells) {
			int tempIndex = cells.IndexOf(cell);
			cell.Row = i;
			cell.Column = elementIndex;

			tempCells.Add(cell);
			elementIndex++;
			if ((tempIndex + 1) % Columns == 0) {
				Cells.Add(tempCells);
				tempCells = new List<Cell>();

				i++;
				elementIndex = 0;
			}
		}
	}

	public Cell GetUp(Cell cell)
	{
		return GetCellFromArray(cell.Row - 1, cell.Column);
	}

	public Cell GetDown(Cell cell)
	{
		return GetCellFromArray(cell.Row + 1, cell.Column);
	}

	public Cell GetLeft(Cell cell)
	{
		return GetCellFromArray(cell.Row, cell.Column - 1);
	}

	public Cell GetRight(Cell cell)
	{
		return GetCellFromArray(cell.Row, cell.Column + 1);
	}

	public Cell GetUpLeft(Cell cell)
	{
		return GetCellFromArray(cell.Row - 1, cell.Column - 1);
	}

	public Cell GetUpRight(Cell cell)
	{
		return GetCellFromArray(cell.Row - 1, cell.Column + 1);
	}

	public Cell GetDownLeft(Cell cell)
	{
		return GetCellFromArray(cell.Row + 1, cell.Column - 1);
	}

	public Cell GetDownRight(Cell cell)
	{
		return GetCellFromArray(cell.Row + 1, cell.Column + 1);
	}

	public Cell GetCellFromArray(int row, int column)
	{
		try {
			return Cells[row][column];
		} catch {
			return null;
		}
	}

	public List<Cell> GetAllSurroundings(Cell cell)
	{
		var surroundings = new List<Cell>();
		surroundings.Add(GetUp(cell));
		surroundings.Add(GetDown(cell));
		surroundings.Add(GetLeft(cell));
		surroundings.Add(GetRight(cell));
		surroundings.Add(GetUpLeft(cell));
		surroundings.Add(GetUpRight(cell));
		surroundings.Add(GetDownLeft(cell));
		surroundings.Add(GetDownRight(cell));

		return surroundings.Where(e => e != null).ToList();
	}

	public List<Cell> GetCellsFromRow(Cell cell)
	{
		var cells = Cells[cell.Row];
		return cells.Where(c => c != cell).ToList();
	}

	public List<Cell> GetCellsFromColumn(Cell cell)
	{
		var cells = new List<Cell>();
		foreach (List<Cell> cellRow in Cells) {
			cells.Add(cellRow[cell.Column]);
		}

		return cells.Where(c => c != cell).ToList();
	}

	public void RefreshBoardArt()
	{
		foreach (List<Cell> cellRow in Cells) {
			foreach (Cell cell in cellRow) {

				if (cell.Type == CellType.RoadHor || cell.Type == CellType.RoadVer || cell.Type == CellType.RoadCross) {
					TryFixRoad(cell);
				}

				Sprite art = _artMapping.GetSpriteForCellType(cell.Type);
				cell.Image.sprite = art;
			}
		}
	}

	public void ResetBoard()
	{
		foreach (List<Cell> cellRow in Cells) {
			foreach (Cell cell in cellRow) {
				cell.Type = CellType.None;
			}
		}

		RefreshBoardArt();
	}

	private void TryFixRoad(Cell cell)
	{
		Cell topCell = GetUp(cell);
		Cell leftCell = GetLeft(cell);
		Cell rightCell = GetRight(cell);
		Cell bottomCell = GetDown(cell);

		byte i = 1;
		if (topCell?.Type == CellType.RoadHor || topCell?.Type == CellType.RoadVer || topCell?.Type == CellType.RoadCross) {
			i++;
		}

		if (leftCell?.Type == CellType.RoadHor || leftCell?.Type == CellType.RoadVer || leftCell?.Type == CellType.RoadCross) {
			i++;
		}

		if (rightCell?.Type == CellType.RoadHor || rightCell?.Type == CellType.RoadVer || rightCell?.Type == CellType.RoadCross) {
			i++;
		}

		if (bottomCell?.Type == CellType.RoadHor || bottomCell?.Type == CellType.RoadVer || bottomCell?.Type == CellType.RoadCross) {
			i++;
		}

		if (i >= 2) {
			cell.Type = CellType.RoadCross;
		}
	}

	private void OnCellClicked(Cell cell)
	{
		OnBoardCellClicked?.Invoke(cell);
	}

	public int EvaluateBoard()
	{
		int sum = 0;
		foreach (List<Cell> cellRow in Cells) {
			foreach (Cell cell in cellRow) {
				sum += EvaluateCell(cell);
			}
		}

		int graveyards = 0;
		int hospitals = 0;
		foreach (List<Cell> cellRow in Cells) {
			foreach (Cell cell in cellRow) {
				if (cell.Type == CellType.Graveyard) {
					graveyards++;
				}
				else if (cell.Type == CellType.Hospital) {
					hospitals++;
				}
			}
		}

		if (graveyards == 2) {
			sum += 20;
		}

		if (hospitals == 2) {
			sum += 20;
		}

		return sum;
	}

	private int EvaluateCell(Cell cell)
	{
		switch (cell.Type) {
			case CellType.Fireside:
				return EvaluateFireside(cell);
			case CellType.Graveyard:
				return EvaluateGraveyard(cell);
			case CellType.Hospital:
				return EvaluateHospital(cell);
			case CellType.House:
				return EvaluateHouse(cell);
			case CellType.Inn:
				return EvaluateInn(cell);
			case CellType.Market:
				return EvaluateMarket(cell);
			case CellType.Park:
				return EvaluatePark(cell);
			case CellType.Waterhole:
				return EvaluateWaterhole(cell);
			default:
				return 0;
		}
	}

	private int EvaluateFireside(Cell cell)
	{
		int i = 0;

		foreach (Cell surroundingCell in GetAllSurroundings(cell)) {
			if (surroundingCell.Type == CellType.Graveyard) {
				i -= 5;
			}
		}

		return i;
	}

	private int EvaluateGraveyard(Cell cell)
	{
		int i = 0;

		foreach (Cell surroundingCell in GetAllSurroundings(cell)) {
			if (surroundingCell.Type == CellType.RoadCross || surroundingCell.Type == CellType.RoadHor || surroundingCell.Type == CellType.RoadVer) {
				i++;
			}
			else if (surroundingCell.Type == CellType.Park) {
				i += 2;
			} else if (surroundingCell.Type == CellType.Hospital) {
				i -= 5;
			}
		}

		var rowCells = GetCellsFromRow(cell);
		var columnCells = GetCellsFromColumn(cell);

		foreach (Cell surCell in rowCells) {
			if (surCell.Type == CellType.Hospital) {
				i -= 5;
				break;
			}
		}

		foreach (Cell surCell in columnCells) {
			if (surCell.Type == CellType.Hospital) {
				i -= 5;
				break;
			}
		}

		return i;
	}

	private int EvaluateHospital(Cell cell)
	{
		int i = 0;

		return i;
	}

	private int EvaluateHouse(Cell cell)
	{
		int i = 0;

		foreach (Cell surroundingCell in GetAllSurroundings(cell)) {
			if (surroundingCell.Type == CellType.House) {
				i++;
			} else if (surroundingCell.Type == CellType.Fireside) {
				i += 2;
			} else if (surroundingCell.Type == CellType.RoadCross || surroundingCell.Type == CellType.RoadHor || surroundingCell.Type == CellType.RoadVer) {
				i++;
			} else if (surroundingCell.Type == CellType.Inn) {
				i -= 2;
			} else if (surroundingCell.Type == CellType.Market) {
				i -= 2;
			} else if (surroundingCell.Type == CellType.Waterhole) {
				i++;
			} else if (surroundingCell.Type == CellType.Graveyard) {
				i -= 5;
			} else if (surroundingCell.Type == CellType.Park) {
				i++;
			}
		}

		var rowCells = GetCellsFromRow(cell);
		var columnCells = GetCellsFromColumn(cell);

		foreach (Cell surCell in rowCells) {
			if (surCell.Type == CellType.Inn) {
				i += 2;
				break;
			}
		}

		foreach (Cell surCell in columnCells) {
			if (surCell.Type == CellType.Inn) {
				i += 2;
				break;
			}
		}

		foreach (Cell surCell in rowCells) {
			if (surCell.Type == CellType.Market) {
				i += 2;
				break;
			}
		}

		foreach (Cell surCell in columnCells) {
			if (surCell.Type == CellType.Market) {
				i += 2;
				break;
			}
		}

		foreach (Cell surCell in rowCells) {
			if (surCell.Type == CellType.Hospital) {
				i++;
				break;
			}
		}

		foreach (Cell surCell in columnCells) {
			if (surCell.Type == CellType.Hospital) {
				i++;
				break;
			}
		}

		return i;
	}

	private int EvaluateInn(Cell cell)
	{
		int i = 0;

		foreach (Cell surroundingCell in GetAllSurroundings(cell)) {
			if (surroundingCell.Type == CellType.House) {
				i--;
			}
			else if (surroundingCell.Type == CellType.RoadCross || surroundingCell.Type == CellType.RoadHor || surroundingCell.Type == CellType.RoadVer) {
				i++;
			}
			else if (surroundingCell.Type == CellType.Inn) {
				i--;
			}
			else if (surroundingCell.Type == CellType.Market) {
				i++;
			}
			else if (surroundingCell.Type == CellType.Graveyard) {
				i -= 5;
			}
			else if (surroundingCell.Type == CellType.Hospital) {
				i -= 2;
			}
		}

		return i;
	}

	private int EvaluateMarket(Cell cell)
	{
		int i = 0;

		var rowCells = GetCellsFromRow(cell);
		var columnCells = GetCellsFromColumn(cell);

		foreach (Cell surCell in rowCells) {
			if (surCell.Type == CellType.House) {
				i++;
			}
		}

		foreach (Cell surCell in columnCells) {
			if (surCell.Type == CellType.House) {
				i++;
			}
		}

		foreach (Cell surroundingCell in GetAllSurroundings(cell)) {
			if (surroundingCell.Type == CellType.RoadCross || surroundingCell.Type == CellType.RoadHor || surroundingCell.Type == CellType.RoadVer) {
				i++;
			}
			else if (surroundingCell.Type == CellType.Inn) {
				i--;
			}
			else if (surroundingCell.Type == CellType.Graveyard) {
				i -= 5;
			}
			else if (surroundingCell.Type == CellType.Hospital) {
				i -= 2;
			}
		}

		return i;
	}

	private int EvaluatePark(Cell cell)
	{
		int i = 0;

		foreach (Cell surroundingCell in GetAllSurroundings(cell)) {
			if (surroundingCell.Type == CellType.RoadCross || surroundingCell.Type == CellType.RoadHor || surroundingCell.Type == CellType.RoadVer) {
				i++;
			}
			else if (surroundingCell.Type == CellType.Park) {
				i += 1;
			}
			else if (surroundingCell.Type == CellType.Hospital) {
				i += 2;
			}
			else if (surroundingCell.Type == CellType.Waterhole) {
				i += 1;
			}
		}

		return i;
	}

	private int EvaluateWaterhole(Cell cell)
	{
		int i = 0;

		foreach (Cell surroundingCell in GetAllSurroundings(cell)) {
			if (surroundingCell.Type == CellType.RoadCross || surroundingCell.Type == CellType.RoadHor || surroundingCell.Type == CellType.RoadVer) {
				i++;
			}
		}

		return i;
	}
}
