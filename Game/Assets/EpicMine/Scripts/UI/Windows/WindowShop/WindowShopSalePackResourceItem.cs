using BlackTemple.EpicMine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowShopSalePackResourceItem : MonoBehaviour
{
    [SerializeField] private Image _itemPicture;
    [SerializeField] private TextMeshProUGUI _itemAmount;
    [SerializeField] private TextMeshProUGUI _itemName;

    public void Initialize(string itemId, int amount)
    {
        _itemPicture.sprite = SpriteHelper.GetIcon(itemId);
        _itemAmount.text = amount.ToString();
        _itemName.text = LocalizationHelper.GetLocale(itemId);
    }
    public void Initialize(string id, Sprite picture, long amount)
    {
        _itemPicture.sprite = picture;
        _itemAmount.text = amount.ToString();
        _itemName.text = LocalizationHelper.GetLocale(id);
    }
}
