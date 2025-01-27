using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;

public class TestLightSceneController : MonoBehaviour
{
    public MineSceneTorchCustomize Torch;

    private bool isMoving;
    private bool isUseTorch;
    public GameObject Hand;

    public void OnPressTorch()
    {
        Hand.SetActive(Torch._torchType == TorchHandingType.Flying);
        //if (!isMoving)
        //{
          //  isUseTorch = true;
            Torch.UseTorch();
      //  }

    }

    public void OnReleaseTorch()
    {
        Torch.EndUseTorch();
       // isUseTorch = false;
    }

    public void IsMoving(bool state)
    {
        if (state)
        {
            if(isUseTorch)
            Torch.EndUseTorch();
        }
        isMoving = state;

    }
}
