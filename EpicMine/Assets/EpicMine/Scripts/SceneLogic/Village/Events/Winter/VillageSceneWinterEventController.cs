using System.Collections.Generic;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;
using UnityEngine.UI;

public class VillageSceneWinterEventController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _winterContent;

    [SerializeField] private Material _winterMountainBackMaterial;
    [SerializeField] private Material _winterMountainFrontMaterial;

    [SerializeField] private Image _winterMountainBack;
    [SerializeField] private Image _winterMountainFront;

    [SerializeField] private GameObject _fallingSnow;
    [SerializeField] private GameObject _layingSnow;

    private void Start()
    {

        if (App.Instance.GameEvents.IsActive(GameEventType.Winter))
        {
            foreach (var o in _winterContent)
            {
                o.SetActive(true);
            }

            _winterMountainBack.material = _winterMountainBackMaterial;
            _winterMountainFront.material = _winterMountainFrontMaterial;

            _layingSnow.SetActive(true);
            _fallingSnow.SetActive(true);
        }
    }
}
