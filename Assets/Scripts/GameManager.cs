using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public SpriteMapping ArtCollection;
	public GameObject GridRoot;
	public GameObject BuildingListRoot;
	public Cell SelectedBuildingPrefab;

	public Board Board;
	public int BuildingLimit;
	public int BuildsLeft;
	public Text BuildsLeftText;
	public string BuildsLeftTextPattern;
	public string ScoreTextPattern;
	public Button ScoreButton;
	public string RestartPatternText;
	public string ScoreBoardTextPattern;

	public GameObject TooltipsRoot;
	public GameObject HouseTooltip;
	public GameObject FiresideTooltip;
	public GameObject InnTooltip;
	public GameObject MarketTooltip;
	public GameObject WaterholeTooltip;
	public GameObject GraveYardTooltip;
	public GameObject ParkTooltip;
	public GameObject HospitalTooltip;
	public GameObject RoadTooltip;

	private bool _endGameTriggered = false;

	private void Start()
	{
		Board.InitBoard(GridRoot.transform, ArtCollection);
		Board.OnBoardCellClicked += OnBoardCellClicked;
		ScoreButton.onClick.AddListener(TriggerTheEndGame);

		AttachBuildingPrefabs();
		BuildsLeft = BuildingLimit;

		BuildsLeftText.text = string.Format(BuildsLeftTextPattern, BuildsLeft);
		SetTypeTooltip(SelectedBuildingPrefab.Type);
	}

	private void AttachBuildingPrefabs()
	{
		foreach (Transform child in BuildingListRoot.transform) {
			Cell cell = child.GetComponent<Cell>();
			if (cell != null) {
				cell.CellClicked += OnBuildingCellPrefabClicked;
			}
		}
	}

	private void OnBoardCellClicked(Cell cell)
	{
		if (SelectedBuildingPrefab.Type == CellType.House && Board.GetCellsFromType(CellType.House).Count > 15) {
			return;
		}

		if (SelectedBuildingPrefab.Type == CellType.Park && Board.GetCellsFromType(CellType.Park).Count > 10) {
			return;
		}

		if (_endGameTriggered) {
			return;
		}

		cell.Type = SelectedBuildingPrefab.Type;
		Board.RefreshBoardArt();

		BuildsLeft--;

		BuildsLeftText.text = string.Format(BuildsLeftTextPattern, BuildsLeft);

		if (BuildsLeft == 0) {
			TriggerTheEndGame();
		}
	}

	private void OnBuildingCellPrefabClicked(Cell buildingPrefab)
	{
		SelectedBuildingPrefab.Type = buildingPrefab.Type;
		SelectedBuildingPrefab.Image.sprite = ArtCollection.GetSpriteForCellType(SelectedBuildingPrefab.Type);

		SetTypeTooltip(buildingPrefab.Type);
	}

	private void SetTypeTooltip(CellType type)
	{
		foreach (Transform child in TooltipsRoot.transform) {
			child.gameObject.SetActive(false);
		}

		switch (type) {
			case CellType.Fireside:
				FiresideTooltip.gameObject.SetActive(true);
				break;
			case CellType.Graveyard:
				GraveYardTooltip.gameObject.SetActive(true);
				break;
			case CellType.Hospital:
				HospitalTooltip.gameObject.SetActive(true);
				break;
			case CellType.House:
				HouseTooltip.gameObject.SetActive(true);
				break;
			case CellType.Inn:
				InnTooltip.gameObject.SetActive(true);
				break;
			case CellType.Market:
				MarketTooltip.gameObject.SetActive(true);
				break;
			case CellType.Park:
				ParkTooltip.gameObject.SetActive(true);
				break;
			case CellType.Waterhole:
				WaterholeTooltip.gameObject.SetActive(true);
				break;
			case CellType.RoadCross:
			case CellType.RoadHor:
			case CellType.RoadVer:
				RoadTooltip.gameObject.SetActive(true);
				break;
		}
	}

	private void TriggerTheEndGame()
	{
		if (_endGameTriggered) {
			RestartGame();
		}
		else {
			ScoreButton.GetComponentInChildren<Text>().text = RestartPatternText;

			int sum = Board.EvaluateBoard();
			BuildsLeftText.text = string.Format(ScoreTextPattern, sum);

			_endGameTriggered = true;
		}
	}

	private void RestartGame()
	{
		ScoreButton.GetComponentInChildren<Text>().text = ScoreBoardTextPattern;
		BuildsLeft = BuildingLimit;
		BuildsLeftText.text = string.Format(BuildsLeftTextPattern, BuildsLeft);

		Board.ResetBoard();

		_endGameTriggered = false;
	}
}
