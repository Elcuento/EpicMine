using System;
using BlackTemple.Common;
using UnityEngine;
using UnityEngine.Video;

namespace BlackTemple.EpicMine
{
    public class WindowVideo : WindowBase
    {
        [SerializeField] private VideoPlayer _videoPlayer;

        private Action _onComplete;
        private float _currentTimer;
        private bool _isStarted;
        private bool _isCompleted;


        public void Initialize(string videoName, Action onComplete = null)
        {
            _onComplete = onComplete;

            var video = Resources.Load<VideoClip>(videoName);
            if (video != null)
            {
                _currentTimer = (float)video.length;
                _videoPlayer.source = VideoSource.VideoClip;
                _videoPlayer.clip = video;
                _videoPlayer.loopPointReached += OnVideoEnded;
                _videoPlayer.errorReceived += OnVideoErrorReceived;
                _videoPlayer.Play();
                _isStarted = true;
                return;
            }

            Complete();
        }

        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);
         //   _videoPlayer.targetCamera = Canvas.worldCamera;
        }

        private void Update()
        {
            if (_isCompleted || !_isStarted)
                return;

            if (_currentTimer > 0)
            {
                _currentTimer -= Time.unscaledDeltaTime;
                return;
            }

            Complete();
        }

        private void OnVideoEnded(VideoPlayer source)
        {
            Complete();
        }

        private void OnVideoErrorReceived(VideoPlayer source, string message)
        {
            Complete();
        }

        private void Complete()
        {
            if (_isCompleted)
                return;

            _onComplete?.Invoke();

            WindowManager.Instance.Close(this, withSound: false);
            App.Instance.Controllers.AudioController.RefreshSceneSounds();
            _isCompleted = true;
        }
    }
}