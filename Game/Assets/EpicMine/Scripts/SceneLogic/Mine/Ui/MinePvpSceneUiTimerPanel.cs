using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class MinePvpSceneUiTimerPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerLabel;

        private bool _isGameEnd;

        private void Awake()
        {
            EventManager.Instance.Subscribe<PvpArenaTimeTickEvent>(OnGetTimeTick);
            EventManager.Instance.Subscribe<PvpArenaEndGameEvent>(OnGameEnd);
        }

        public void OnGameEnd(PvpArenaEndGameEvent ev)
        {
            _isGameEnd = true;
            _timerLabel.text = "00:00";
        }

        private void OnGetTimeTick(PvpArenaTimeTickEvent ev)
        {
            if (_isGameEnd) return;

            if (ev.ResoultTime <= 0)
            {
                _timerLabel.text = "00:00";
                return;
            }
            var minutes = Mathf.Floor(ev.ResoultTime / 60f).ToString("00");
            var seconds = (ev.ResoultTime % 60).ToString("00");

            _timerLabel.text = $"{minutes}:{seconds}";
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<PvpArenaTimeTickEvent>(OnGetTimeTick);
                EventManager.Instance.Unsubscribe<PvpArenaEndGameEvent>(OnGameEnd);
            }
        }
    }
}