using System.Collections;
using System.Collections.Generic;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;

public class VillageSceneChickenPumpkinEvent : MonoBehaviour
{
    [SerializeField] private List<string> _randomDrop;
    [SerializeField] private int _clickTarget = 10;
    [SerializeField] private int _fallVelocityTarget = 10;

    [SerializeField] private VillageSceneChicken _chicken;
    [SerializeField] private GameObject _root;

    private int currentsClicks = 0;

    private void Start()
    {
        if (_root == null)
            return;

        var random = Random.Range(0, 100) < 5;
        var isActive = App.Instance.GameEvents.IsActive(GameEventType.Halloween) && random;

        if (isActive && _root != null)
        {
            _root.SetActive(true);
            _chicken.OnFall += OnFallen;
            _chicken.OnClick += OnClick;
        }
        
    }

    private void OnDestroy()
    {
        if (_chicken == null)
            return;

        _chicken.OnFall -= OnFallen;
        _chicken.OnClick -= OnClick;
    }

    private void OnFallen(Vector2 velocity)
    {
        if (velocity.y >= _fallVelocityTarget)
        {
            GetReward();
        }
    }

    private void OnClick()
    {
        currentsClicks++;

        if (currentsClicks >= _clickTarget)
        {
            currentsClicks = 0;
            GetReward();
        }

    }

    private void GetReward()
    {
        foreach (var drop in _randomDrop)
        {
            var item = new Item(drop, Random.Range(1, 2));

            App.Instance.Player.Inventory.Add(item,IncomeSourceType.FromEvent);

            var cam = FindObjectOfType<Camera>();

            if (cam == null)
                return;

            var viewportPosition = cam.WorldToViewportPoint(transform.position);

            WindowManager.Instance.Show<WindowFlyingIcons>(withSound: false).Create(item, viewportPosition, Tags.InventoryButton, 0.1f);
        }

        if(_root != null)
            _root.SetActive(false);

        _chicken.OnClick -= OnClick;
        _chicken.OnFall -= OnFallen;
    }
}
