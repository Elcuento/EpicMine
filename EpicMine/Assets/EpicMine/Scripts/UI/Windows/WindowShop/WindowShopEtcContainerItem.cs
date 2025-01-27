using UnityEngine;
using UnityEngine.UI;

public class WindowShopEtcContainerItem : MonoBehaviour {

    [SerializeField] private GridLayoutGroup _grid;

    public void SetGridCount(int col = 3, int sizeX = 480, int sizeY = 610)
    {
        _grid.constraintCount = col;
        _grid.cellSize = new Vector2(sizeX, sizeY);
    }
}
