using System;
using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;


using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class MinePvpSceneUiEmodjiPanel : MonoBehaviour
    {
        [SerializeField] private Transform EmodjiPanel;
        [SerializeField] private Button[] _emodjiList;


        [SerializeField] private Image LeftPlayerEmodjiContainer;
        [SerializeField] private Image RightPlayerEmodjiContainer;
        [SerializeField] private Image LeftPlayerEmodji;
        [SerializeField] private Image RightPlayerEmodji;

        [SerializeField] private Button _openButton;
        [SerializeField] private Image _openButtonIcon;

        private bool _isOpen;


        public void OnGetEmodji(PvpArenaGetEmodjiEvent data)
        {
            CancelInvoke("HideRightPlayerEmodji");
            RightPlayerEmodjiContainer.gameObject.SetActive(true);
            RightPlayerEmodji.sprite = SpriteHelper.GetEmodji((EmoType) data.EmodjiNumber);
            Invoke("HideRightPlayerEmodji", 3);
        }

        public void ShowSelfEmodji(int number)
        {
            CancelInvoke("HideLeftPlayerEmodji");
            LeftPlayerEmodjiContainer.gameObject.SetActive(true);
            LeftPlayerEmodji.sprite = SpriteHelper.GetEmodji((EmoType)number);
            SetState(false);
            RefreshButtonSprite();
            EmodjiPanel.gameObject.SetActive(false);
            Invoke("HideLeftPlayerEmodji", 3);
        }

        public void HideLeftPlayerEmodji()
        {
            LeftPlayerEmodjiContainer.gameObject.SetActive(false);
        }

        public void HideRightPlayerEmodji()
        {
            RightPlayerEmodjiContainer.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<PvpArenaGetEmodjiEvent>(OnGetEmodji);
            }
        }
    
        public void Start()
        {
            EventManager.Instance.Subscribe<PvpArenaGetEmodjiEvent>(OnGetEmodji);

            LeftPlayerEmodjiContainer.gameObject.SetActive(false);
            RightPlayerEmodjiContainer.gameObject.SetActive(false);
            EmodjiPanel.gameObject.SetActive(false);

            var emodjiTypes =  Enum.GetValues(typeof(EmoType));

            for (var i = 0; i < emodjiTypes.Length; i++)
            {
                if (i < _emodjiList.Length)
                {
                    var ii = i;
                    var emodji = (EmoType)ii;
                    _emodjiList[i].image.sprite = SpriteHelper.GetEmodji(emodji);
                    _emodjiList[i].onClick.AddListener(() => {  OnClickEmodji(ii);});
                }
                else _emodjiList[i].enabled = false;
            }
            EmodjiPanel.gameObject.SetActive(false);
        }

        public void SetState(bool state)
        {
            _isOpen = state;
        }

        public void RefreshButtonSprite()
        {
            _openButtonIcon.sprite = _isOpen
                ? App.Instance.ReferencesTables.Sprites.CloseIcon
                : App.Instance.ReferencesTables.Sprites.ChatIcon;
        }

        public void OnClickOpenButton()
        {
            SetState(!_isOpen);

            EmodjiPanel.gameObject.SetActive(_isOpen); 
            RefreshButtonSprite();
        }
        
        public void OnClickEmodji(int i)
        {
            EventManager.Instance.Publish(new PvpArenaSendEmodjiEvent(i));
            ShowSelfEmodji(i);
           
        }
    }
}
