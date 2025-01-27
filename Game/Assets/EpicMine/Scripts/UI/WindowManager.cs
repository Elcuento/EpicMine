using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowManager : Singleton<WindowManager>
    {
        public float CanvasScale { get; private set; }

        private readonly List<WindowBase> _windows = new List<WindowBase>();

        private static int _sortingOrder;

        protected override void Awake()
        {
            CalculateProportions();
            base.Awake();
        }

        public override void Touch()
        {
            CalculateProportions();
        }

        private void CalculateProportions()
        {
            var matchW = Screen.width / 1920f;
            var matchH = Screen.height / 1080f;

            CanvasScale = matchW > matchH ? 1 : 0;
        }

        public T Show<T>(bool withPause = false, bool withCurrencies = false, bool withSound = true, bool withRating = false) where T : WindowBase
        {
        
            _sortingOrder++;
            var sortingOrder = _sortingOrder;

            var window = (T) _windows.FirstOrDefault(w => w is T);
            if (window == null)
            {
                var type = typeof(T);
                var windowPrefab = Resources.Load<T>(Paths.ResourcesWindowsPath + type.Name);
                window = Instantiate(windowPrefab, transform, false);
                window.Canvas.sortingOrder = sortingOrder;
                _windows.Add(window);
                window.OnShow(withPause, withCurrencies, withRating);

                window.Canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = CanvasScale;

                if (withSound)
                    AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.OpenWindow);
            }
            else
            {
                window.transform.SetAsLastSibling();
                window.Canvas.sortingOrder = sortingOrder;

                if (window.gameObject.activeSelf)
                    window.OnBecameCurrent();
                else
                {
                    window.gameObject.SetActive(true);
                    window.OnShow(withPause, withCurrencies);

                    if (withSound)
                        AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.OpenWindow);
                }
            }

            if (withPause)
                TimeManager.Instance.SetPause(true);

            return window;
        }

        
        public bool IsOpen<T>() where T : WindowBase
        {
            var window = (T) _windows.FirstOrDefault(w => w is T && w.gameObject.activeSelf);
            return window != null;
        }

        public bool IsClosed<T>() where T : WindowBase
        {
            var window = (T)_windows.FirstOrDefault(w => w is T && w.gameObject.activeSelf);
            return window == null;
        }

        public T Get<T>() where T : WindowBase
        {
            return (T)_windows.FirstOrDefault(w => w is T && w.gameObject.activeSelf);
        }

        public void Close<T>(bool withDestroy = false, bool withSound = true) where T : WindowBase
        {
            var window = _windows.LastOrDefault(w => w.gameObject.activeSelf);
            if (window != null && window is T)
                _sortingOrder--;
            else
                window = _windows.FirstOrDefault(w => w is T);

            CloseWindow(window, withDestroy, withSound);
        }
        
        public void Close(WindowBase windowBase, bool withDestroy = false, bool withSound = true)
        {
            var lastOpenedWindow = _windows.LastOrDefault(w => w.gameObject.activeSelf);
            if (lastOpenedWindow != null && lastOpenedWindow == windowBase)
                _sortingOrder--;

            CloseWindow(windowBase, withDestroy, withSound);
        }

        public void Clear()
        {
            var windowsToDestroy = new List<WindowBase>();
            
            foreach (var window in _windows)
            {

                switch (window.Canvas.sortingLayerName)
                {
                    case Layers.AboveWindowsSortingLayerName:
                    case Layers.SystemWindowsSortingLayerName:
                        continue;
                    default:
                        windowsToDestroy.Add(window);
                        break;
                }
            }

            foreach (var windowToDestroy in windowsToDestroy)
            {
                _windows.Remove(windowToDestroy);
                Destroy(windowToDestroy.gameObject);
            }

            _sortingOrder = 0;
        }


        private void CloseWindow(WindowBase window, bool withDestroy, bool withSound = true)
        {
            if (window == null || window.IsClosing)
                return;

            if (window.WithPause)
            {
                var anyPause = _windows.Find(x => x.WithPause && x != window && x.gameObject.activeSelf);

                if (anyPause == null)
                {
                    TimeManager.Instance.SetPause(false);
                }
            }
      

            window.OnClose();

            
            if (withSound)
                AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.CloseWindow);

            if (withDestroy)
            {
                _windows.Remove(window);
                Destroy(window.gameObject);
            }
            else
                window.gameObject.SetActive(false);

            var lastOpenedWindow = _windows.LastOrDefault(w => w.gameObject.activeSelf && !w.IsClosing);
            if (lastOpenedWindow != null)
            {
                if(lastOpenedWindow.WithPause)
                    TimeManager.Instance.SetPause(true);

                lastOpenedWindow.OnBecameCurrent();
            }

            var closeEvent = new WindowCloseEvent(window);
            EventManager.Instance.Publish(closeEvent);
        }
    }
}