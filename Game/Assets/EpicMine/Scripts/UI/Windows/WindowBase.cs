using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    [RequireComponent(typeof(Canvas))]
    public class WindowBase : MonoBehaviour
    {
        public Canvas Canvas { get; private set; }

        public bool WithPause { get; private set; }

        public bool WithCurrencies { get; private set; }

        public bool WithRating { get; private set; }

        public bool IsClosing { get; private set; }

        public bool IsReady { get; private set; }


        public virtual void Close()
        {
            WindowManager.Instance.Close(this);
        }



        public virtual void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            WithPause = withPause;
            WithCurrencies = withCurrencies;
            WithRating = withRating;

            IsClosing = false;

            var openEvent = new WindowOpenEvent(this);
            EventManager.Instance.Publish(openEvent);

            if (WithCurrencies)
                WindowManager.Instance.Show<WindowCurrencies>(withSound: false);

            if (withRating)
                WindowManager.Instance.Show<WindowRating>(withSound: false);
        }

        public virtual void OnBecameCurrent()
        {
            if (WithCurrencies)
                WindowManager.Instance.Show<WindowCurrencies>(withSound: false);

            if (WithRating)
                WindowManager.Instance.Show<WindowRating>(withSound: false);
        }

        public virtual void OnClose()
        {
            IsClosing = true;

            if (WithCurrencies)
                WindowManager.Instance.Close<WindowCurrencies>(withSound: false);

            if (WithRating)
                WindowManager.Instance.Close<WindowRating>(withSound: false);
        }


        protected virtual void Awake()
        {
            Canvas = GetComponent<Canvas>();

            Camera worldCamera = null;

            var uiCameraGo = GameObject.FindWithTag(Tags.UICamera);
            if (uiCameraGo != null)
                worldCamera = uiCameraGo.GetComponent<Camera>();

            if (worldCamera == null)
                worldCamera = Camera.main;

            Canvas.worldCamera = worldCamera;
        }

        protected virtual void Ready()
        {
            IsReady = true;
            OnReady();
        }

        protected virtual void OnReady() { }
    }
}