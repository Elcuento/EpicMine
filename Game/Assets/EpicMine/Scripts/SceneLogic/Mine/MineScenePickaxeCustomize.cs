using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MineScenePickaxeCustomize : MonoBehaviour
{
    [SerializeField] private Light Left;
    [SerializeField] private Light Right;

    public void BlinkEffect(Color color)
    {

        StartCoroutine(Blink(color,5));
    }

    private IEnumerator Blink(Color color, float speed)
    {

        Left.color = color;
        Left.range = 0;
  
        var timer = 0f;
        var isRevers = false;
        var to = 5;

        while (true)
        {
            timer += 0.2f * (isRevers ? -1 : 1) ;

            Left.range = timer;

            if (isRevers && Left.range <= 0)
            {
                yield break;
            }

            if (!isRevers && Left.range >= to)
            {
                isRevers = true;

                to = 0;
            }
            
            yield return null;
        }
    }
}
