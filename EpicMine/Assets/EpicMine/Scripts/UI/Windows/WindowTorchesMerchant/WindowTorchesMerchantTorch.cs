using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
   public class WindowTorchesMerchantTorch : MonoBehaviour
    {
        public Core.Torch Torch { get; private set; }

        [SerializeField] private GameObject _selected;
        [SerializeField] private GameObject _active;
        [SerializeField] private GameObject _locked;
        [SerializeField] private GameObject _ad;
        [SerializeField] private TextMeshProUGUI _adText;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _rarityBackground;
        [SerializeField] private GameObject _redDot;

        private Action<WindowTorchesMerchantTorch> _onClick;


        public void Initialize(Core.Torch torch, Action<WindowTorchesMerchantTorch> onClick)
        {
            Clear();
            Torch = torch;
            _onClick = onClick;

            var sprite = SpriteHelper.GetTorchImage(torch.StaticTorch.Id);
            _icon.sprite = sprite;
            _icon.gameObject.SetActive(sprite != null);

            _icon.DOFade(0.3f, 0);

            if (Torch.IsCreated)
            {
                RemoveLocked();
                _icon.DOFade(1f, 0);

                _rarityBackground.sprite =
                    SpriteHelper.GetTorchBackground(Torch);


                if (Torch.StaticTorch.Id == App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch.Id)
                    SetSelected();

                return;
            }

            if (Torch.IsUnlocked)
            {
                RemoveLocked();

              //  if (App.Instance.Controllers.RedDotsController.NewPickaxes.Contains(Torch.StaticTorch.Id))
               //     ShowRedDot();

               _rarityBackground.sprite = SpriteHelper.GetTorchBackground(Torch);

                if (Torch.StaticTorch.Id == App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch.Id)
                    SetSelected();


                if (Torch.StaticTorch.Type == TorchType.Ad)
                {
                    int currentVal;
                    App.Instance.Player.TorchesMerchant.AdTorches.TryGetValue(Torch.StaticTorch.Id, out currentVal);
                    var costLeft = Torch.StaticTorch.Cost - currentVal;

                    _ad.SetActive(true);
                    _adText.text = costLeft.ToString();
                }

                return;
            }

            SetLocked();
        }

        public void UpdateAdVal(int val)
        {
            var costLeft = Torch.StaticTorch.Cost - val;
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
            _icon.DOFade(Torch.IsCreated ? 1f : 0.3f, 0);
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
