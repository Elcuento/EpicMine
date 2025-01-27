using System;
using System.Collections;
using BlackTemple.Common;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlackTemple.EpicMine
{
    public class SceneManager : Singleton<SceneManager>
    {
        public string PreviousScene { get; private set; }

        public string CurrentScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        public event EventHandler<string, string> OnSceneChange;

        public bool IsLoading { get; private set; }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneInternal(sceneName, new WindowFadeSettings(0.4f, Color.black)));
        }

        public void LoadScene(string sceneName, WindowFadeSettings fadeSettings)
        {
            StartCoroutine(LoadSceneInternal(sceneName, fadeSettings));
        }


        private IEnumerator LoadSceneInternal(string newSceneName, WindowFadeSettings fadeSettings)
        {

            IsLoading = true;

            PreviousScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            var windowFade = WindowManager.Instance.Show<WindowFade>(withSound: false);
            windowFade.FadeIn(fadeSettings);
            yield return new WaitForSecondsRealtime(fadeSettings.Time);
            GC.Collect();
            UnityEngine.SceneManagement.SceneManager.LoadScene(ScenesNames.Empty);
            WindowManager.Instance.Clear();

            var preloaded = false;

            switch (newSceneName)
            {
                case ScenesNames.Mine:
                    MineScenePreloader.Preload(() => { preloaded = true; });
                    break;
                default: preloaded = true;
                    break;
            }
        
            yield return new WaitUntil(()=> preloaded);

            UnityEngine.SceneManagement.SceneManager.LoadScene(newSceneName);

            windowFade.FadeOut(fadeSettings);
            yield return new WaitForSecondsRealtime(fadeSettings.Time);


            IsLoading = false;
            OnSceneChange?.Invoke(PreviousScene, newSceneName);
        }

        public void PreLoadScene(string sceneName)
        {
            switch (sceneName)
            {
                case ScenesNames.Mine:
                    
                    break;
            }
        }

    }
}