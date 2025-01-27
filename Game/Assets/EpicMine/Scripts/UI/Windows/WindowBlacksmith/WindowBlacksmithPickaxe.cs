using System;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowBlacksmithPickaxe : MonoBehaviour
    {
        public Core.Pickaxe Pickaxe { get; private set; }

        [SerializeField] private GameObject _selected;
        [SerializeField] private GameObject _active;
        [SerializeField] private GameObject _locked;
        [SerializeField] private GameObject _ad;
        [SerializeField] private TextMeshProUGUI _adText;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _rarityBackground;
        [SerializeField] private GameObject _redDot;

        private Action<WindowBlacksmithPickaxe> _onClick;


        public void Initialize(Core.Pickaxe pickaxe, Action<WindowBlacksmithPickaxe> onClick)
        {
            Clear();
            Pickaxe = pickaxe;
            _onClick = onClick;

            var sprite = SpriteHelper.GetPickaxeImage(pickaxe.StaticPickaxe.Id);
            _icon.sprite = sprite;
            _icon.gameObject.SetActive(sprite != null);

            _icon.DOFade(0.3f, 0);

            if (Pickaxe.IsCreated)
            {
                RemoveLocked();
                _icon.DOFade(1f, 0);

                _rarityBackground.sprite =
                    SpriteHelper.GetPickaxeRarityBackgroundLuxAndUsual(Pickaxe);

                if (Pickaxe.StaticPickaxe.Id == App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe.Id)
                    SetSelected();

                return;
            }

            if (Pickaxe.IsUnlocked)
            {
                RemoveLocked();

                if (App.Instance.Controllers.RedDotsController.NewPickaxes.Contains(Pickaxe.StaticPickaxe.Id))
                    ShowRedDot();

                _rarityBackground.sprite = Pickaxe.StaticPickaxe.Type == PickaxeType.Donate
                    ? SpriteHelper.GetPickaxeLuxBackground(Pickaxe)
                    : SpriteHelper.GetPickaxeRarityBackground(pickaxe.StaticPickaxe.Rarity);

                if (Pickaxe.StaticPickaxe.Id == App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe.Id)
                    SetSelected();

                if (Pickaxe.StaticPickaxe.Type == PickaxeType.Ad)
                {
                    int currentVal;
                    App.Instance.Player.Blacksmith.AdPickaxes.TryGetValue(Pickaxe.StaticPickaxe.Id, out currentVal);
                    var costLeft = Pickaxe.StaticPickaxe.Cost - currentVal;

                    _ad.SetActive(true);
                    _adText.text = costLeft.ToString();
                }

                return;
            }

            SetLocked();
        }

        public void UpdateAdVal(int val)
        {
            var costLeft = Pickaxe.StaticPickaxe.Cost - val;
            _adText.text = costLeft.ToString();
        }


        public void Click()
        {
            _onClick?.Invoke(this);
        }

        public void SetSelected()
        {
            _selected.SetActive(true);
        }

        public void RemoveSelected()
        {
            _selected.SetActive(false);
        }

        public void SetActive()
        {
            _active.SetActive(true);
        }

        public void RemoveActive()
        {
            _active.SetActive(false);
        }

        public void SetLocked()
        {
            _icon.color = Color.black;
            _icon.DOFade(1f, 0);
            _locked.SetActive(true);
        }

        public void RemoveLocked()
        {
            _icon.color = Color.white;
            _locked.SetActive(false);
            _icon.DOFade(Pickaxe.IsCreated ? 1f : 0.3f, 0);
        }

        public void ShowRedDot()
        {
            _redDot.SetActive(true);
        }

        public void HideRedDot()
        {
            _redDot.SetActive(false);
        }

        public void SetCreated()
        {
            _icon.DOFade(1, 0);
            _ad.SetActive(false);
            Click();
        }


        private void OnDestroy()
        {
            Clear();
        }

        private void Clear()
        {
            _active.SetActive(false);
            _selected.SetActive(false);
            _locked.SetActive(false);
            _ad.SetActive(false);
            _adText.text = string.Empty;
            _icon.color = Color.white;
            _icon.DOFade(1, 0);
            HideRedDot();
        }
    }
}