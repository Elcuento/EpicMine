using BlackTemple.EpicMine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoMinerSceneDropItem : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Image _unknownIcon;
    [SerializeField] private TextMeshProUGUI _chanceText;

    [SerializeField] private float _unknownIconTopPosition;
    [SerializeField] private float _unknownIconCenterPosition;

    private string _itemId;
    private bool _isItemUnlocked;

    public void Initialize(string itemStaticId, int value)
    {
        _itemId = itemStaticId;

        _itemIcon.sprite = SpriteHelper.GetIcon(_itemId);
        _itemIcon.gameObject.SetActive(true);
        _unknownIcon.gameObject.SetActive(false);

        _chanceText.text = value.ToString();
    }


    public void Initialize(string itemStaticId, float chance, bool itemUnlocked)
    {
        _itemId = itemStaticId;
        _isItemUnlocked = itemUnlocked;

        if (_isItemUnlocked)
        {
            _itemIcon.sprite = SpriteHelper.GetIcon(_itemId);
            _itemIcon.gameObject.SetActive(true);
            _unknownIcon.gameObject.SetActive(false);

            _chanceText.text = chance.ToString();
        }
        else
        {
            _itemIcon.gameObject.SetActive(false);
            _unknownIcon.gameObject.SetActive(true);
            _unknownIcon.rectTransform.DOAnchorPosY(_unknownIconTopPosition,0);

            _chanceText.text = "";
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isItemUnlocked)
            return;

        var window = WindowManager.Instance.Show<WindowItemPopup>(withSound: false);
        window.Initialize(_itemId, transform.position);
    }
}
