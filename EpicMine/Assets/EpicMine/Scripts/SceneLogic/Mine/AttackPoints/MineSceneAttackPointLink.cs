using DG.Tweening;
using UnityEngine;

public class MineSceneAttackPointLink : MonoBehaviour {

    [SerializeField] private SpriteRenderer[] _links;
    [SerializeField] private GameObject _root;

    public void EnableDisable(bool state)
    {
        _root.SetActive(state);
    }

    public void SetColor(Color col)
    {
        foreach (var t in _links)
        {
            t.color = col;
        }
    }

    public void Show(int speed = 1, float delay = 0)
    {
        foreach (var image in _links)
        {
            image.DOFade(1, speed)
                .SetDelay(delay);
        }
    }

    public void Hide(int speed = 1, float delay = 0)
    {
        foreach (var image in _links)
        {
            image.DOFade(0, speed)
                .SetDelay(delay);
        }
    }

    public void SetOrder(int sortOrder)
    {
        foreach (var image in _links)
        {
            image.sortingOrder = sortOrder;
        }
    }
}
