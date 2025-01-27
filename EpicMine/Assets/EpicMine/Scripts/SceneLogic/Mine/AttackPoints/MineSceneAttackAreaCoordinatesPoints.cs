using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using BlackTemple.Common;
using BlackTemple.EpicMine.Core;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackTemple.EpicMine
{
    public class MineSceneAttackAreaCoordinatesPoints : MineSceneAttackArea
    {
        public Action<Dictionary<MineSceneAttackPointCoordinates, AttackPointHitType>, int> OnHits;

        [SerializeField] protected MineSceneAttackPointCoordinates _attackPointPrefab;
        //
        [SerializeField] protected MineSceneAttackLine _attackLine;

        [Header("Matrix")]
        private MineSceneAttackPoint[,] _pointMatrix;

        //
        public List<FieldFigure> Figures { get; private set; }
        public FieldPointArea PointArea { get; private set; }


        [Header("Combo")]
        protected float _comboTimer;
        public int Combo { get; protected set; }

        [Header("Points")]
        protected List<MineSceneAttackPointCoordinates> _attackPoints = new List<MineSceneAttackPointCoordinates>();
        public int PointMax  => PointArea.MaxPoints;
        public int PointLeft => PointArea.MaxPoints - _attackPoints.Count;


        [Header("Figures")]
        protected Coroutine _createFigure;


        [Header("Rules")]
        private int _destroyedPoints;


        public override void Initialize(MineSceneSection section, bool isHorizontalAttackLine, float moveTime)
        {

            base.Initialize(section, isHorizontalAttackLine, moveTime);
            Section = section;

            var selectedMine = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Mine>(RuntimeStorageKeys.SelectedMine);

            var selectedTier = App.Instance
                .Services
                .RuntimeStorage
                .Load<Core.Tier>(RuntimeStorageKeys.SelectedTier);

            var wallNumber = section.Number + 1;

            PointArea = (selectedTier != null && selectedMine != null)
                ? FieldHelper.GetSuitePointArea((selectedTier.Number + 1), (selectedMine.Number + 1), wallNumber)
                : FieldHelper.GetDefaultPointArea();

            Figures = FieldHelper.Figures;

            _pointMatrix = new MineSceneAttackPoint[PointArea.LengthX, PointArea.LengthY];

            _isStarted = true;

            FillArea();

            _attackLine.gameObject.SetActive(true);
            _attackLine.Initialize(isHorizontalAttackLine, moveTime, OnChangeDirection);
        }



        protected override void Update()
        {
            base.Update();

            UpdateComboTimer();
        }

        public override void FillArea()
        {
            var delay = 0.1f;

            var attackPoints = new List<MineSceneAttackPointCoordinates>();

            var sizeX = PointArea.Grid.GetLength(0);
            var sizeY = PointArea.Grid.GetLength(1);

            for (var i = 0; i < sizeX; i++)
            for (var j = 0; j < sizeY; j++)
            {
                if (PointArea.Grid[i, j].PointType != AttackPointType.Empty)
                {
                    delay += MineLocalConfigs.AttackPointSpawnInterval;
                    var point = CreateAttackPoint(PointArea.Grid[i, j], delay);
                    SetPoint(point, i, j);
                    attackPoints.Add(point);
                }
            }

            foreach (var mineSceneAttackPoint in attackPoints)
            {
                mineSceneAttackPoint.SetNeighborhoods(attackPoints);
            }
        }

        private void SetPoint(MineSceneAttackPointCoordinates point, int x, int y)
        {
            for (var i = x; i < (x + point.Size) - 1; i++)
            for (var j = y; j < (y + point.Size) - 1; j++)
            {
                _pointMatrix[i, j] = point;
            }
        }
        private void ClearPoint(int size, int x, int y)
        {
            for (var i = x; i < (x + size) - 1; i++)
            for (var j = y; j < (y + size) - 1; j++)
            {
                _pointMatrix[i, j] = null;
            }
        }

        private void CreateGarantRandomFigure()
        {
            if (_attackPoints.Count >= PointMax)
                return;

            var localFig = MineHelper.GetRandomFigure(Figures, PointArea, PointLeft);

            if (localFig == null)
                return;

            var place = MineHelper.GetGarantPlace(_pointMatrix, localFig);

            if (place.Length <= 0)
            {
                localFig = Figures.Find(x => x.Id == MineLocalConfigs.PointFigure);

                place = MineHelper.GetGarantPlace(_pointMatrix, localFig);

                if (place.Length <= 0)
                    return;
            }

            CreateFigure(localFig, place[0], place[1]);

        }

        protected override void OnChangeDirection()
        {
            if (!_isStarted)
                return;

            switch (PointArea.RulesType)
            {
                case FieldRulesType.Free:
                    if (PointLeft > 0)
                    {
                        CreateRandomFigure();
                    }
                    break;
                case FieldRulesType.FigureOnly:
                    if (_attackPoints.Count <= 0)
                    {
                        FillArea();
                    }
                    break;
                case FieldRulesType.StartFigure:
                    if (PointLeft > 0 && _destroyedPoints >= PointArea.GetPointsCount())
                    {
                        CreateRandomFigure();
                    }
                    break;
            }
        }

        public override void OnClick(bool force = false)
        {
           
        }

        public override void OnPointerUp(PointerEventData eventData)
        {

        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (Section.Hero.Pickaxe.HitCounter <= 0)
                return;

            var attackLinePosition = _attackLine.transform.position;

            var points = new Dictionary<MineSceneAttackPointCoordinates, float>();
            foreach (var attackPoint in _attackPoints)
            {
                if (points.ContainsKey(attackPoint))
                    continue;

                var distance = attackLinePosition - attackPoint.transform.position;
                var axisDistance = _attackLine.IsHorizontal
                    ? Mathf.Abs(distance.x)
                    : Mathf.Abs(distance.y);

                if (axisDistance < attackPoint.OuterBounds.x)
                {
                    points.Add(attackPoint, axisDistance);
                }
            }

            var hits = new Dictionary<MineSceneAttackPointCoordinates, AttackPointHitType>();

            if (points.Count > 0)
            {
                _comboTimer = 1f;

                foreach (var attackPoint in points)
                {
                    var hitType = AttackPointHitType.Outer;

                    if (attackPoint.Value < attackPoint.Key.InnerBounds.x)
                        hitType = AttackPointHitType.Inner;

                    if (!hits.ContainsKey(attackPoint.Key))
                    {
                        hits.Add(attackPoint.Key, hitType);
                        Combo++;

                        if (attackPoint.Key.Hit(hitType, _attackLine.IsHorizontal))
                            DestroyPoint(attackPoint.Key);
                    }
                }
            }
            else
            {
                Combo = 1;
                _comboTimer = 0f;

                WindowManager
                    .Instance
                    .Show<WindowVignette>()
                    .Initialize(_missVignetteColor, 0.5f);
            }

            if (Combo > 1)
            {
                WindowManager
                    .Instance
                    .Show<WindowCombo>()
                    .Initialize(Combo);
            }


            EventManager.Instance.Publish(new MineSceneWallSectionHitEvent(Section, hits, Combo, points.Count == 0));

            OnHits?.Invoke(hits, Combo);
            OnHit?.Invoke();
        }


        protected IEnumerator CreateRandomFiguresThread()
        {
            var finished = false;
            var figurePlaces = new KeyValuePair<int[], FieldFigure>();

            var pointFigure = Figures.Find(x => x.Id == MineLocalConfigs.PointFigure);

            while (true)
            {
                var thread = new Thread(() =>
                {
                    var timeOut = 50;

                    while (true)
                    {
                        timeOut -= 1;
                        if (timeOut > 50)
                            break;

                        if (_attackPoints.Count >= PointMax)
                            break;

                        var localFig = MineHelper.GetRandomFigure(Figures, PointArea, PointLeft);

                        var place = MineHelper.GetPlace(_pointMatrix, localFig);

                        if (place.Length <= 0)
                        {
                            localFig = pointFigure;

                            place = MineHelper.GetGarantPlace(_pointMatrix, localFig);

                            if (place.Length <= 0)
                                break;
                        }

                        var figureCopy = new FieldFigure(localFig);
                        var random = new System.Random(DateTime.Now.Millisecond).Next(0, 100);

                        if (random < 20)
                        {
                            figureCopy.Rotate();
                            figureCopy.Reverse();

                        }

                        if (random < 50)
                        {
                            figureCopy.Reverse();
                        }

                        if (random < 80)
                        {
                            figureCopy.Rotate();
                        }

                        figurePlaces = new KeyValuePair<int[], FieldFigure>(place, new FieldFigure(localFig));
                        break;
                    }


                    finished = true;
                });

                thread.Start();

                yield return new WaitUntil(() => finished);

                if (figurePlaces.Key != null && figurePlaces.Key.Length > 0)
                {
                    CreateFigure(figurePlaces.Value, figurePlaces.Key[0], figurePlaces.Key[1]);
                    figurePlaces = new KeyValuePair<int[], FieldFigure>();
                    finished = false;
                }
                else break;
            }

            _createFigure = null;
        }

        protected virtual void CreateRandomFigure()
        {
            if (_createFigure == null)
                _createFigure = StartCoroutine(CreateRandomFiguresThread());
        }

        protected virtual void CreateFigure(FieldFigure figure, int x = 0, int y = 0)
        {
            if (!_isStarted || PointLeft <= 0)
                return;

            var figurePoints = new List<MineSceneAttackPointCoordinates>();

            var dem1 = figure.LengthX;
            var dem2 = figure.LengthY;

            var a = 0;
            var delay = 0f;
            for (var i = x; i < dem1 + x; i++)
            {
                var b = -1;
                for (var j = y; j < dem2 + y; j++)
                {
                    b++;
                    var pointData = figure.Grid[a, b];

                    if (pointData.PointType == AttackPointType.Empty)
                    { continue; }

                    delay += MineLocalConfigs.AttackPointSpawnInterval;
                    var point = CreateAttackPoint(i, j, pointData.Size, delay);

                    SetPoint(point, i, j);

                    if (point.Size <= 2)
                        figurePoints.Add(point);
                    else point.DisableLinks();

                }
                a++;
            }

            foreach (var mineSceneAttackPoint in figurePoints)
            {
                mineSceneAttackPoint.SetNeighborhoods(new List<MineSceneAttackPointCoordinates>(figurePoints));
            }
        }

        private void SetPointPositionAndSize(MineSceneAttackPointCoordinates point, int x, int y, int baseSize)
        {
            var size = baseSize * MineLocalConfigs.PointPartSize;
            point.transform.localScale = new Vector3(size, size);

            // print(point._inner.bounds.size.x);
            var xOffset = -MineLocalConfigs.HorizontalAttackLineMaxXPosition * 0.8f;
            var yOffset = -MineLocalConfigs.VerticalAttackLineMaxYPosition * 0.8f;

            var extraXOffset = 0f;
            var extraYOffset = 0f;

            if (baseSize == 4)
            {
                extraXOffset += 0.95f;
                extraYOffset += 1.3f;
            }

            if (baseSize == 3)
            {
                extraXOffset += 0.54f;
                extraYOffset += 0.45f;
            }

            var xMax = (x + extraXOffset) / (float)PointArea.LengthX;
            var yMax = (y + extraYOffset) / (float)PointArea.LengthY;

            var spacingX = (MineLocalConfigs.PointSize * x) * 0.02f;
            var spacingY = (MineLocalConfigs.PointSize * y) * 0.05f;

            point.transform.localPosition = new Vector2(xOffset + (MineLocalConfigs.HorizontalFieldSize - 1) * xMax + spacingX,
                yOffset + (MineLocalConfigs.VerticalFieldSize - 1) * yMax + spacingY);
        }

        private MineSceneAttackPointCoordinates CreateAttackPoint(FieldAttackPoint pointData, float delay = 0)
        {
            var point = SingletonPool.Instance.FromPool<MineSceneAttackPointCoordinates>() ??
                        Instantiate(_attackPointPrefab, transform, false);

            point.transform.SetParent(transform);
            SetPointPositionAndSize(point, pointData.X, pointData.Y, pointData.Size);

            if (pointData.PointType == AttackPointType.Random)
            {
                AttackPointTypeProbabilityConfigs typeProbability;
                switch (App.Instance.Controllers.AttackPointProbabilityController.Type)
                {
                    case AttackPointTypeProbability.Default:
                        typeProbability = App.Instance.StaticData.Configs.Dungeon.Mines.AttackPoints.Probabilities.Types
                            .Default;
                        break;
                    case AttackPointTypeProbability.Donate:
                        typeProbability = App.Instance.StaticData.Configs.Dungeon.Mines.AttackPoints.Probabilities.Types
                            .Donate;
                        break;
                    case AttackPointTypeProbability.Help:
                        typeProbability = App.Instance.StaticData.Configs.Dungeon.Mines.AttackPoints.Probabilities.Types
                            .Help;
                        break;
                    case AttackPointTypeProbability.Braking:
                        typeProbability = App.Instance.StaticData.Configs.Dungeon.Mines.AttackPoints.Probabilities.Types
                            .Braking;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.LearnMiningBasic))
                    point.Initialize(AttackPointType.Default, pointData.X, pointData.Y, pointData.Size, delay);
                else
                {
                    var energyPointsAvailable = App.Instance.Player.Dungeon.LastOpenedTier.Number > 0;
                    var randomPercent = UnityEngine.Random.Range(0, 100f);
                    if (energyPointsAvailable && randomPercent <= typeProbability.EnergyPointChance)
                        point.Initialize(AttackPointType.Energy, pointData.X, pointData.Y, pointData.Size, delay);
                    else if (randomPercent <= typeProbability.EnergyPointChance + typeProbability.HealthPointChance)
                        point.Initialize(AttackPointType.Health, pointData.X, pointData.Y, pointData.Size, delay);
                    else
                        point.Initialize(AttackPointType.Default, pointData.X, pointData.Y, pointData.Size, delay);
                }
            }
            else
            {
                point.Initialize(pointData.PointType, pointData.X, pointData.Y, pointData.Size, delay);
            }

            _attackPoints.Add(point);

            return point;
        }

        public MineSceneAttackPointCoordinates CreateAttackPoint(int x, int y, int baseSize, float delay = 0)
        {
            AttackPointTypeProbabilityConfigs typeProbability;

            switch (App.Instance.Controllers.AttackPointProbabilityController.Type)
            {
                case AttackPointTypeProbability.Default:
                    typeProbability = App.Instance.StaticData.Configs.Dungeon.Mines.AttackPoints.Probabilities.Types.Default;
                    break;
                case AttackPointTypeProbability.Donate:
                    typeProbability = App.Instance.StaticData.Configs.Dungeon.Mines.AttackPoints.Probabilities.Types.Donate;
                    break;
                case AttackPointTypeProbability.Help:
                    typeProbability = App.Instance.StaticData.Configs.Dungeon.Mines.AttackPoints.Probabilities.Types.Help;
                    break;
                case AttackPointTypeProbability.Braking:
                    typeProbability = App.Instance.StaticData.Configs.Dungeon.Mines.AttackPoints.Probabilities.Types.Braking;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var point = SingletonPool.Instance.FromPool<MineSceneAttackPointCoordinates>() ?? Instantiate(_attackPointPrefab, transform, false);
            point.transform.SetParent(transform);


            SetPointPositionAndSize(point, x, y, baseSize);

            if (!App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.LearnMiningBasic))
                point.Initialize(AttackPointType.Default, x, y, baseSize, delay);
            else
            {
                var energyPointsAvailable = App.Instance.Player.Dungeon.LastOpenedTier.Number > 0;
                var randomPercent = UnityEngine.Random.Range(0, 100f);
                if (energyPointsAvailable && randomPercent <= typeProbability.EnergyPointChance)
                    point.Initialize(AttackPointType.Energy, x, y, baseSize, delay);
                else if (randomPercent <= typeProbability.EnergyPointChance + typeProbability.HealthPointChance)
                    point.Initialize(AttackPointType.Health, x, y, baseSize, delay);
                else
                    point.Initialize(AttackPointType.Default, x, y, baseSize, delay);
            }



            _attackPoints.Add(point);
            return point;
        }


        private void UpdateComboTimer()
        {
            if (_comboTimer <= 0)
                Combo = 0;
            else
                _comboTimer -= Time.deltaTime;
        }

        protected void DestroyPoint(MineSceneAttackPointCoordinates point)
        {
            ClearPoint(point.Size, point.X, point.Y);
            _attackPoints.Remove(point);
            _destroyedPoints++;
        }

        public override void Clear()
        {
            base.Clear();

            DOTween.Kill(_attackLine);
            _attackLine.gameObject.SetActive(false);

            foreach (var attackPoint in _attackPoints)
                SingletonPool.Instance.ToPool(attackPoint.gameObject);

            _attackPoints.Clear();
        }

        protected override void ResetField()
        {
            foreach (var mineSceneAttackPoint in _attackPoints)
            {
                SingletonPool.Instance.ToPool(mineSceneAttackPoint.gameObject);
            }

            _pointMatrix = new MineSceneAttackPoint[PointArea.LengthX, PointArea.LengthY];
            _attackPoints.Clear();
        }
    }
}

