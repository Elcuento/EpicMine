using System.Collections;
using DragonBones;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneGodSection : MineSceneSection
    {
        [SerializeField] private UnityArmatureComponent _armature;

        public override void Initialize(int number, MineSceneHero hero)
        {
            base.Initialize(number, hero);
            _armature.gameObject.SetActive(false);
        }

        public override void SetReady()
        {
            base.SetReady();
            StartCoroutine(PlayAnimation());
        }


        private IEnumerator PlayAnimation()
        {
            _armature.gameObject.SetActive(true);
            _armature.animation.Play("animtion0", 1);
            yield return new WaitForSeconds(1.85f);
            _armature.animation.Play("cycle");

            WindowManager.Instance.Show<WindowPrestige>();
        }
    }
}