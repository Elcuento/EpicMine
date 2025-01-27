using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Dto;
using DragonBones;
using UnityEngine;
using Transform = UnityEngine.Transform;

namespace BlackTemple.EpicMine
{
    public class WindowShopOffer : WindowBase
    {
        [SerializeField] private UnityArmatureComponent _blacksmith;
        [SerializeField] private UnityArmatureComponent _merchant;

        [SerializeField] private WindowShopSpecialOfferItem _itemPrefab;
        [SerializeField] private Transform _offerContainer;

        private List<ShopOffer> _shopOffers;


        private void Start()
        {
            Canvas.sortingLayerName = Layers.AboveWindowsSortingLayerName;

            EventManager.Instance.Subscribe<IapPurchaseCompleteEvent>(OnPurchaseCompleted);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<IapPurchaseCompleteEvent>(OnPurchaseCompleted);
        }

        public void OnPurchaseCompleted(IapPurchaseCompleteEvent purchase)
        {
            for(var i = 0; i < _shopOffers.Count; i++)
            {
                if(_shopOffers[i].Id == ShopHelper.GetPackByProductId(purchase.Product).Id)
                    Close();
            }
        }

        private IEnumerator BlacksmithCharactersAnimation()
        {
            _blacksmith.animation.Play("Release",1);
            yield return  new WaitForSecondsRealtime(_blacksmith.animation.animations["Release"].duration);
            _blacksmith.animation.FadeIn("Waiting_1", 0.1f);
        }

        private IEnumerator MerchantCharactersAnimation()
        {
            _merchant.animation.Play("Release",1);
            yield return new WaitForSecondsRealtime(_merchant.animation.animations["Release"].duration);
            _merchant.animation.FadeIn("Waiting", 0.1f);
        }

        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies, withRating);

            StopAllCoroutines();
            StartCoroutine(BlacksmithCharactersAnimation());
            StartCoroutine(MerchantCharactersAnimation());
        }

        public void Initialize(List<ShopOffer> offers, bool restart = false)
        {
            _shopOffers = offers;

            _offerContainer.ClearChildObjects();

            if (_shopOffers.Count > 0)
            {
                var offerCore = App.Instance.Player.Shop.ShopOffer.Find(x => x.Id == _shopOffers[0].Id);
                var res = Instantiate(_itemPrefab, _offerContainer.transform, false);
                res.Initialize(offerCore, OnBuy);
            }
            else Close();
        }

        public void OnBuy()
        {
            Close();
        }

        public override void OnClose()
        {
            base.OnClose();

            _shopOffers.Clear();
        }

        public override void Close()
        {
            if (_shopOffers.Count > 0)
            {
                _shopOffers.Remove(_shopOffers[0]);
            }

            if (_shopOffers.Count > 0)
            {
                Initialize(_shopOffers, true);
                return;
            }

            base.Close();
        }
    }
}
