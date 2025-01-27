using BlackTemple.EpicMine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowShopSpecialOfferHeaderResourceItem : MonoBehaviour
{
    [SerializeField] private Image _picture;
    [SerializeField] private TextMeshProUGUI _count;

    public void Initialize(string itemId, int count)
    {
        _picture.sprite = SpriteHelper.GetIcon(itemId);
        _count.text = count.ToString();
    }
}
