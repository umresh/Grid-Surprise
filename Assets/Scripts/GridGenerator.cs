using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;


public class GridGenerator : MonoBehaviour
{
    public static GridGenerator Instance;
    public int columns, rows, totalMoves = 20;
    public float cellSpacing = 48, cellSize = 45;
    public Sprite sprite, bonus;                                //Cell sprite image. To customize cells as we want. And customize the bonus/surprise image[Not visible in RELEASE]
    [HideInInspector]
    public ColorBlock buttonColor;                              //To set the hightlighting color for the selection of cells. Default button colorblock is not visible.
    [SerializeField]
    TMPro.TextMeshProUGUI movesCount;                           //Text box to show remaining moves.
    [SerializeField]
    bool isDebug = false;                                       //To show the hidden objects in the grid. For Checking/Testing purpose.
    [HideInInspector]
    public bool hasCanvas = false, canReset = false;

    int MAX_SURPRISE = 10;                                      //Number to surprise/bonus to hide/place
    private List<Cell> createdTiles = new List<Cell>();         //All the generateds cells.
    Cell[,] cellsArray;                                         //For validation.
    List<Cell> surpriceCellList = new List<Cell>();             //Need this to prevent hiding surprise/bonus in same cell.
    System.Random random = new System.Random();                 //To generate random numbers.

    /// <summary>
    /// To create a singleton for the generator
    /// </summary>
    private void Awake() => Instance = this;

    private void Start() => GenerateGrid();

    /// <summary>
    /// Creates grid with columns and rows count. Called only Once since grid is constant.
    /// </summary>
    public void GenerateGrid()
    {
        movesCount.text = $"Moves Left : {totalMoves}";

        int midColumns = columns / 2;
        int midRow = rows / 2;
        cellsArray = new Cell[columns, rows];
        for (int i = 0; i < columns; i++)
        {
            GameObject rowObject = new GameObject("Row-" + i);
            float currentYPos = (i - midColumns) * cellSpacing;
            rowObject.transform.SetParent(transform, false);

            RectTransform _rowRect = rowObject.AddComponent<RectTransform>();
            _rowRect.anchoredPosition = new Vector2(250, currentYPos);                     //Off setting rows to 250 in X-axis for formatting.
            for (int j = 0; j < rows; j++)
            {
                GameObject gameObj = new GameObject("Col-" + j);
                gameObj.transform.SetParent(rowObject.transform, false);
                RectTransform _colRect = gameObj.AddComponent<RectTransform>();
                _colRect.sizeDelta = Vector2.one * cellSize;
                _colRect.localPosition = new Vector3(cellSpacing * (j - midRow), 0, 0);// (i < midColumns ? -(midColumns - i) : midColumns - i);
                _colRect.localScale = Vector3.one;

                Cell _cell = gameObj.AddComponent<Cell>();
                _cell.SetGridPosition(i, j);
                createdTiles.Add(_cell);
                cellsArray[i, j] = _cell;
            }
        }
        //Placing surprise/bonus in random cells.
        PlaceSurprize();
    }
    /// <summary>
    /// Select cells at random and place surprises. Max number determines NO. of surprise to place in grid.
    /// </summary>
    public void PlaceSurprize()
    {
        if (createdTiles.Count == 0)
            return;

        //Select a cell for Interaction with Keyboards. Selected cell is [0,0] which is bottom left corner.
        EventSystem.current.SetSelectedGameObject(createdTiles[0].gameObject);

        surpriceCellList.Clear();
        while (surpriceCellList.Count < MAX_SURPRISE)
        {
            Cell _tempCell = createdTiles[random.Next(0, 400)];
            if (!surpriceCellList.Contains(_tempCell))
            {
                _tempCell.AddSuprise(bonus, isDebug);
                surpriceCellList.Add(_tempCell);
            }
        }
    }
    private void OnEnable() => PlaceSurprize();
    /// <summary>
    /// Used to reset/remove cells/surprises.
    /// </summary>
    private void OnDisable()
    {
        ResetCells();
    }
    private void ResetCells()
    {
        for (int i = 0; i < createdTiles.Count; i++)
            createdTiles[i].RemoveAndResetSurprise();
    }
    /// <summary>
    /// To validate and colorize the cells based on the RULES.
    /// </summary>
    /// <param name="_cell">Clicked or selected cell</param>
    public void ValidateClickedCell(Cell _cell)
    {
        if (!isDebug && totalMoves <= 0)
            return;
        int _X = _cell.x;
        int _Y = _cell.y;
        bool isInFirstLevel = false, isInSecondLevel = false;
        //For checking if surprise is one cell away.
        if (((_X + 1 < columns) ? cellsArray[_X + 1, _Y].HasSurprize : false) || ((_X - 1 >= 0) ? cellsArray[_X - 1, _Y].HasSurprize : false) ||
            ((_Y + 1 < rows) ? cellsArray[_X, _Y + 1].HasSurprize : false) || ((_Y - 1 >= 0) ? cellsArray[_X, _Y - 1].HasSurprize : false) ||
            (((_X - 1 >= 0) && (_Y + 1 < rows)) ? cellsArray[_X - 1, _Y + 1].HasSurprize : false) || (((_X - 1 >= 0) && (_Y - 1 >= 0)) ? cellsArray[_X - 1, _Y - 1].HasSurprize : false) ||
            (((_X + 1 < columns) && (_Y - 1 >= 0)) ? cellsArray[_X + 1, _Y - 1].HasSurprize : false) || (((_X + 1 < columns) && (_Y + 1 < rows)) ? cellsArray[_X + 1, _Y + 1].HasSurprize : false))
        {
            isInFirstLevel = true;
        }
        //For checking if surprise is two cells away.
        if (((_X + 2 < columns) ? cellsArray[_X + 2, _Y].HasSurprize : false) || ((_X - 2 >= 0) ? cellsArray[_X - 2, _Y].HasSurprize : false) ||
            ((_Y + 2 < rows) ? cellsArray[_X, _Y + 2].HasSurprize : false) || ((_Y - 2 >= 0) ? cellsArray[_X, _Y - 2].HasSurprize : false) ||
            (((_X - 2 >= 0) && (_Y + 2 < rows)) ? cellsArray[_X - 2, _Y + 2].HasSurprize : false) || (((_X - 2 >= 0) && (_Y - 2 >= 0)) ? cellsArray[_X - 2, _Y - 2].HasSurprize : false) ||
            (((_X + 2 < columns) && (_Y - 2 >= 0)) ? cellsArray[_X + 2, _Y - 2].HasSurprize : false) || (((_X + 2 < columns) && (_Y + 2 < rows)) ? cellsArray[_X + 2, _Y + 2].HasSurprize : false) ||
            (((_X + 2 < columns) && (_Y - 1 >= 0)) ? cellsArray[_X + 2, _Y - 1].HasSurprize : false) || (((_X + 2 < columns) && (_Y + 1 < rows)) ? cellsArray[_X + 2, _Y + 1].HasSurprize : false) ||
            (((_X - 2 >= 0) && (_Y - 1 >= 0)) ? cellsArray[_X - 2, _Y - 1].HasSurprize : false) || (((_X - 2 >= 0) && (_Y + 1 < rows)) ? cellsArray[_X - 2, _Y + 1].HasSurprize : false) ||
            (((_X - 1 >= 0) && (_Y - 2 >= 0)) ? cellsArray[_X - 1, _Y - 2].HasSurprize : false) || (((_X - 1 >= 0) && (_Y + 2 < rows)) ? cellsArray[_X - 1, _Y + 2].HasSurprize : false) ||
            (((_X + 1 < columns) && (_Y - 2 >= 0)) ? cellsArray[_X + 1, _Y - 2].HasSurprize : false) || (((_X + 1 < columns) && (_Y + 2 < rows)) ? cellsArray[_X + 1, _Y + 2].HasSurprize : false))
        {
            isInSecondLevel = true;
        }

        if (_cell.HasSurprize)
            _cell.ChangeCellColor(Color.red);                   //If the clicked cell has surprise.
        else if (isInFirstLevel || isInSecondLevel)
            _cell.ChangeCellColor(Color.yellow);                //If the clicked cell is one/two cell away.
        else
            _cell.ChangeCellColor(Color.green);                 //If the clicked cell doesn't have surprise of more than 2 cells away.
        totalMoves -= 1;
        movesCount.text = $"Moves Left : {totalMoves}";
        if (totalMoves <= 0)
        {
            canReset = true;
            movesCount.text = "Moves exhaused.\n\n-- Press any Key to Reset --";
        }
    }
    /// <summary>
    /// To restart game on any key press automatically after moves are exhausted.
    /// </summary>
    private void FixedUpdate()
    {
        if (isDebug)
            return;
        if (!canReset)
            return;
        if (Input.anyKeyDown)
        {
            /// Reset all the grid, cells and moves.
            totalMoves = 20;
            movesCount.text = $"Moves Left : {totalMoves}";
            canReset = false;
            ResetCells();
            PlaceSurprize();
        }
    }
}
