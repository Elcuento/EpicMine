using System.Collections.Generic;
using CommonDLL.Static;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneAttackPointCoordinates : MineSceneAttackPoint
    {
        public int X { private set; get; }
        public int Y { private set; get; }

        [SerializeField] private Color _greenColor;
        [SerializeField] private Color _redColor;
        [SerializeField] private Color _yellowColor;

        [SerializeField] private MineSceneAttackPointLink[] _links;

        private List<MineSceneAttackPointCoordinates> _neighborhoods;

        public void Initialize(AttackPointType pointType, int x, int y, int size = 2, float delay = 0)
        {
            Clear();

            _inner.gameObject.SetActive(true);
            _outer.gameObject.SetActive(true);

            Size = size;
            X = x;
            Y = y;

            PointType = pointType;

            switch (pointType)
            {
                case AttackPointType.Energy:
                    SetColor(_yellowColor);
                    break;
                case AttackPointType.Health:
                    SetColor(_redColor);
                    break;
                default:
                    SetColor(_greenColor);
                    break;
            }

            var sortOrder = (int)(4 - PointType);

            _inner.sortingOrder = sortOrder;
            _outer.sortingOrder = sortOrder;

            foreach (var link in _links)
            {
                link.SetOrder(sortOrder);
            }

            PlaySpawnAnimation(delay);
        }


        public override bool Hit(AttackPointHitType hitType, bool isHorizontalHit = true)
        {
            if (!base.Hit(hitType, isHorizontalHit))
                return false;

            UnSetNeighborhoods();
            return true;
        }

        protected override void PlaySpawnAnimation(float delay = 0)
        {
            base.PlaySpawnAnimation(delay);

            foreach (var link in _links)
            {
                link.Show(1, 1);
            }
        }

        protected override void Clear()
        {
            base.Clear();

            _neighborhoods = new List<MineSceneAttackPointCoordinates>();

            foreach (var link in _links)
            {
                link.EnableDisable(false);
            }
        }

        public void SetNeighborhoods(List<MineSceneAttackPointCoordinates> neighborhoods)
        {
            _neighborhoods = neighborhoods;

            DisableLinks();
            SetLinks();
        }


        public void DisableLinks()
        {
            foreach (var link in _links)
            {
                link.EnableDisable(false);
            }
        }

        public void SetLinks()
        {
            if (Size > 2)
                return;

            foreach (var point in _neighborhoods)
            {

                var linkNumber = -1;

                if (point.X + 2 == X && point.Y == Y && point.Size <= 2)
                    linkNumber = 0;


                if (point.X == X + 2 && point.Y == Y && point.Size <= 2)
                    linkNumber = 1;


                if (point.X == X && point.Y == Y + 2 && point.Size <= 2)
                    linkNumber = 2;


                if (point.X == X && point.Y + 2 == Y && point.Size <= 2)
                    linkNumber = 3;


                if (linkNumber != -1)
                {
                    var link = _links[linkNumber];

                    link.EnableDisable(PointType <= point.PointType);
                    link.SetColor(PointType == AttackPointType.Energy ? _yellowColor :
                        PointType == AttackPointType.Health ? _redColor : _greenColor);
                }

            }
        }

        public void SetLinkColor(int linkNumber, Color col)
        {
            _links[linkNumber].SetColor(col);
        }

        public void UnSetNeighborhoods()
        {
            foreach (var link in _links)
            {
                link.EnableDisable(false);
            }

            _neighborhoods.Remove(this);

            foreach (var point in _neighborhoods)
            {
                if (point == null || point == this || !point.gameObject.activeSelf)
                    continue;

                point.OnDestroyNeighborhood(this);
            }
        }

        public void OnDestroyNeighborhood(MineSceneAttackPointCoordinates point)
        {
            if (point.X + 2 == X && point.Y == Y)
                _links[0].EnableDisable(false);
            if (point.X == X + 2 && point.Y == Y)
                _links[1].EnableDisable(false);
            if (point.X == X && point.Y == Y + 2)
                _links[2].EnableDisable(false);
            if (point.X == X && point.Y + 2 == Y)
                _links[3].EnableDisable(false);
        }
    }
}