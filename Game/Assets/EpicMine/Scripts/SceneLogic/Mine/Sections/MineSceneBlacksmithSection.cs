using System.Collections;
using DragonBones;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneBlacksmithSection : MineSceneSection
    {
        [SerializeField] private UnityArmatureComponent _armature;

        public override void SetReady()
        {
            base.SetReady();
            StartCoroutine(PlayAnimation());
        }

        private void Start()
        {
            _armature.animation.GotoAndStopByProgress("anim");
        }

        private IEnumerator PlayAnimation()
        {
            _armature.animation.Play("anim", 1);
            yield return new WaitForSeconds(0.6f);
            SetPassed();
            yield return new WaitForSeconds(0.25f);

            MineHelper.AddEndMiningEventToAnalytics(true);
            MineHelper.ClearTempStorage();
            SceneManager.Instance.LoadScene(ScenesNames.Village, fadeSettings: new WindowFadeSettings(1f, Color.white));
        }
    }
}