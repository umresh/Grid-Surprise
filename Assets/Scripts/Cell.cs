using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Data for each cells like 
/// </summary>
public class Cell : MonoBehaviour
{
    public int x, y;
    public bool surprise = false, isValidated = false;

    /// <summary>
    /// Does cell has bonus/item/surprice to cell which is added at random from grid generator;
    /// </summary>
    public bool HasSurprize { get { return surprise; } set { surprise = value; } }

    private void Start()
    {
        Image _imageComp = gameObject.AddComponent<Image>();
        _imageComp.sprite = GridGenerator.Instance.sprite;
        Button _button = gameObject.AddComponent<Button>();
        _button.colors = GridGenerator.Instance.buttonColor;//.selectedColor = new Color(.8f,.8f,.8f);
        _button.onClick.AddListener(() =>
        {
            if (!isValidated)
                GridGenerator.Instance.ValidateClickedCell(this);
        });
    }
    public void SetGridPosition(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
    /// <summary>
    /// Adds suprise/Item/Bonus to cell
    /// </summary>
    public void AddSuprise(Sprite _bonusSprite, bool _isDebug)
    {
        HasSurprize = true;
        ///Add surprise/bonus for debugging/viewing purpose. Not needed for release version.
        if (_isDebug)
        {
            GameObject go = new GameObject("Surprise");
            go.transform.SetParent(transform, false);

            Image _imageComp = go.AddComponent<Image>();
            _imageComp.sprite = GridGenerator.Instance.bonus;
            go.GetComponent<RectTransform>().sizeDelta = Vector2.one * (GridGenerator.Instance.cellSize - 5);
        }
    }
    /// <summary>
    /// Clear/Remove all the hidden surprises for next play.
    /// </summary>
    public void RemoveAndResetSurprise()
    {
        if (HasSurprize)
        {
            HasSurprize = false;
            if (transform.childCount != 0)
                Destroy(transform.GetChild(0).gameObject);
        }
        isValidated = false;
        gameObject.GetComponent<Image>().color = Color.white;
    }
    /// <summary>
    /// Changes the cell color. Called from GridGenerator after validation.
    /// </summary>
    /// <param name="_color">Validated Color for cell</param>
    public void ChangeCellColor(Color _color)
    {
        //Added validated to avoid moves reducing if cell is already clicked by user/player.
        isValidated = true;
        gameObject.GetComponent<Image>().color = _color;
    }
}
