using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneAttackPointAttach : MonoBehaviour
    {
        public int Number;
        public bool IsBusy { get; private set; }

        public void SetBusy()
        {
            IsBusy = true;
        }

        public void SetFree()
        {
            IsBusy = false;
        }
    }
}