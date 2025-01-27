using BlackTemple.EpicMine;
using UnityEngine;
using UnityEngine.UI;

public class VillageSceneQuestPointer : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private GameObject _root;
    [SerializeField] private GameObject _leftArrow;
    [SerializeField] private GameObject _rightArrow;
    [SerializeField] private Image _questType;

    private GameObject _attached;
    private VillageSceneCharacter _character;
    private RectTransform _rect;

    private float _leftPosition;
    private float _rightPosition;

    private float _verticalPosition;
    private float _wide;

    private Image _arrowIcon;

    public void Initialize(VillageSceneCharacter character, GameObject questArrow, RectTransform rect)
    {
        _attached = questArrow;
        _character = character;

        _rect = rect;

        _arrowIcon = questArrow.GetComponent<Image>();

        _rightPosition = - _rect.rect.width / 2;
        _leftPosition = _rect.rect.width  / 2;

        _verticalPosition = transform.parent.InverseTransformPoint(_attached.transform.position).y;

        transform.localPosition = new Vector3(_rightPosition, _verticalPosition, transform.localPosition.z);
        _wide = _rectTransform.rect.width / 2;

        _questType.sprite = _arrowIcon.sprite;
    }

    public void OnClick()
    {
        _character.OnClickQuestTyp();
    }
    private void Update()
    {
        _questType.sprite = _arrowIcon.sprite;

        if (!_attached.activeSelf)
        {
            _root.SetActive(false);
            return;
        }

        var recPosition = _rect.InverseTransformPoint(_attached.transform.position);

        var isClose = _rect.rect.Contains(recPosition);

        if (isClose)
        {
            _root.SetActive(false);
        }
        else
        {
            _root.SetActive(true);

            if (transform.position.x > _attached.transform.position.x)
            {
                transform.localPosition = new Vector3(_rightPosition + _wide, _verticalPosition, transform.localPosition.z);

                _rightArrow.SetActive(false);
                _leftArrow.SetActive(true);
            }
            else
            {
                transform.localPosition = new Vector3(_leftPosition - _wide, _verticalPosition, transform.localPosition.z);

                _leftArrow.SetActive(false);
                _rightArrow.SetActive(true);
            }
        }
    }
}
