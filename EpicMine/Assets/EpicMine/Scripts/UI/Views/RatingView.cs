using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class RatingView : MonoBehaviour
    {
        [SerializeField] private Image[] _items;

        public void Initialize(int value, ViewRatingType viewRatingType = ViewRatingType.Stars)
        {
            foreach (var image in _items)
                image.sprite = SpriteHelper.GetRatingIcon(viewRatingType);

            value = Mathf.Clamp(value, 0, _items.Length);

            for (var i = 0; i < value; i++)
                _items[i].sprite = SpriteHelper.GetRatingIcon(viewRatingType, isFull: true);


            //set
        }
    }

    public enum ViewRatingType
    {
        Stars,
        Skulls,
    }
}