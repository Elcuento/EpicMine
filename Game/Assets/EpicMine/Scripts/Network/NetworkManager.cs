using System;
using System.Collections;
using BlackTemple.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace BlackTemple.EpicMine
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        public bool IsInternetAvailable = true;

        public const string CheckInternetUrl = "https://epicmine-live.firebaseapp.com/internetChecker.txt";

       // public const string CheckInternetResponse = "1";

        private int _requestWithPreloaderCount;

        // Some devices cannot send WebRequest either WWW , dunno why, unity bug on 2018.x still on.
        private bool _useInternetChecker;

        protected void Start()
        {
            StartCoroutine(SendCheckInternet());
        }

       /* public void Send<TResponse>(RequestBase request, Action<TResponse> onComplete = null,
            Action<string> onError = null) where TResponse : ResponseBase
        {
            try
            {
                StartCoroutine(SendInternal(request, onComplete, onError));

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
         
        }*/

        IEnumerator SendCheckInternet()
        {
            var checkTime = 5;

            while (true)
            {

                yield return new WaitForSeconds(checkTime);

                var request = new UnityWebRequest(CheckInternetUrl)
                {
                    downloadHandler = new DownloadHandlerBuffer(),
                    timeout = 30,
                    chunkedTransfer = false,                   
                };

                yield return request.SendWebRequest();

                if (request.isHttpError || request.isNetworkError)
                {
                    IsInternetAvailable = !_useInternetChecker;
                }
                else
                {
                    IsInternetAvailable = true;
                    _useInternetChecker = true;
                }
            }
        }
   /*
        private IEnumerator SendInternal<TResponse>(RequestBase request, Action<TResponse> onComplete = null, Action<string> onError = null) where TResponse : ResponseBase
        {
            if (request.WithWindowPreloader)
                OnRequestWithPreloaderStart();

            if (_useInternetChecker)
            {
                var webRequest = new UnityWebRequest(CheckInternetUrl)
                {
                    downloadHandler = new DownloadHandlerBuffer(),
                    timeout = 30,
                    chunkedTransfer = false,
                };

                yield return webRequest.SendWebRequest();

                if (webRequest.isHttpError || webRequest.isNetworkError)
                {
                    if (request.WithWindowNoInternet)
                        ShowWindowErrorNoInternet();

                    if (request.WithWindowPreloader)
                        OnRequestWithPreloaderComplete();

                    onError?.Invoke("No internet connection");
                    yield break;
                }
            }

            try
            {
                request.Send();
            }
            catch (Exception e)
            {
                onError?.Invoke(e.Message);

                if (request.WithWindowPreloader)
                    OnRequestWithPreloaderComplete();

                yield break;
            }

            yield return new WaitUntil(() => request.IsCompleted);

            if (request.WithWindowPreloader)
                OnRequestWithPreloaderComplete();

            if (request.IsError)
            {
                onError?.Invoke(request.ErrorMessage);
                yield break;
            }

            onComplete?.Invoke(request.Response as TResponse);
        }
        */
        private void ShowWindowErrorNoInternet()
        {
            var windowError = WindowManager.Instance.Show<WindowInformation>();
            windowError.Initialize(
                "window_no_internet_header",
                "window_no_internet_description",
                "window_no_internet_button");
        }

        private void OnRequestWithPreloaderStart()
        {
            _requestWithPreloaderCount++;
            WindowManager.Instance.Show<WindowPreloader>(withSound: false);
        }

        private void OnRequestWithPreloaderComplete()
        {
            _requestWithPreloaderCount--;

            if (_requestWithPreloaderCount <= 0)
            {
                _requestWithPreloaderCount = 0;
                WindowManager.Instance.Close<WindowPreloader>(withSound: false);
            }
        }
    }
}