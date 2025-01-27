using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using BlackTemple.Common;
using CommonDLL.Static;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackTemple.EpicMine
{
    public class MineScenePickaxeHitController : MonoBehaviour
    {
        [SerializeField] private MineSceneBaseController _controller; 

        private MineSceneAttackSection _attackSection;
        private MeshRenderer _itemRender;

        private Color _backColor;
        private Color _frontColor;

        private bool _isEnable;

        private void Start()
        {
            CheckQuality();

             _backColor = Color.white;
            ColorUtility.TryParseHtmlString("#6A6A6A", out _frontColor);

            EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            EventManager.Instance.Subscribe<ChangeSettingsQualityEvent>(OnSettingsChange);


            Clear();

            _attackSection = _controller?.SectionProvider?.Sections[0] as MineSceneAttackSection;

            if (_attackSection != null)
            {
                _itemRender = _attackSection.Item.GetComponentInChildren<MeshRenderer>();
                _attackSection.OnPickaxeHit += OnGetHit; 

            }
        }

        private void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
            EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            EventManager.Instance.Unsubscribe<ChangeSettingsQualityEvent>(OnSettingsChange);

            if (_attackSection != null)
                _attackSection.OnPickaxeHit -= OnGetHit;

        }

        private void CheckQuality()
        {
            _isEnable = PlayerPrefsHelper.LoadDefault(PlayerPrefsType.WallCracks, false);// PlayerPrefs.GetInt("wallCrack", 0) == 1;
        }

        public void Clear()
        {
            if (_attackSection != null)
                _attackSection.OnPickaxeHit -= OnGetHit;

            _itemRender = null;
            _attackSection = null;
        }

        private void OnSettingsChange(ChangeSettingsQualityEvent eventData)
        {
            CheckQuality();
        }

        private void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
           Clear();
        }

        private void OnSectionReady(MineSceneSectionReadyEvent eventData)
        {
            _attackSection = eventData.Section as MineSceneAttackSection;

            if (_attackSection != null)
            {
                _itemRender = _attackSection.Item.GetComponentInChildren<MeshRenderer>();
                _attackSection.OnPickaxeHit += OnGetHit;
            }
        }

        private void OnGetHit(MineSceneAttackPoint point, AttackPointHitType pointHitType, AttackDamageType damageType,
            int damage, bool isCritical)
        {

            if (!_isEnable)
                return;

            if (_attackSection is MineScenePvpWallSection) // TODO IOS CRASH TEXTURES
                return;

            if (isCritical)
            {
                if (point != null)
                {
                    FireRay(point.transform.position, _attackSection.Item.transform.position,
                        App.Instance.ReferencesTables.Sprites.CriticalCrack);
                }
                else
                {
                    FireRay(App.Instance.ReferencesTables.Sprites.CriticalCrack);
                }
            }
            else
            {
                if (point != null)
                {
                    FireRay(point.transform.position, _attackSection.Item.transform.position,
                        pointHitType == AttackPointHitType.Inner
                            ? App.Instance.ReferencesTables.Sprites.InnerCrack
                            : App.Instance.ReferencesTables.Sprites.OuterCrack);
                }
                else
                {
                    FireRay(Random.Range(0,100)> 50 ? App.Instance.ReferencesTables.Sprites.InnerCrack : App.Instance.ReferencesTables.Sprites.OuterCrack);
                }
            }
        }

        private void FireRay(Vector3 from, Vector3 to, Texture2D[] textures)
        {
            var too = new Vector3(from.x, from.y, to.z);

            if (!Physics.Raycast(from, (too - from).normalized, out var hit, 20))
                return;

            var rend = hit.transform.GetComponent<Renderer>();

            if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null)
                return;

            var material = rend.material;
            var tex = material.mainTexture as Texture2D;

            if (tex == null)
                return;

            var pixelUV = hit.textureCoord;

            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;

            var x = (int)pixelUV.x;
            var y = (int)pixelUV.y;


            AddWatermark(material, x, y, textures);
        }

        private void FireRay(Texture2D[] textures)
        {
            var rend = _attackSection.Item.GetComponentInChildren<Renderer>();

            if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null)
                return;

            var material = rend.material;
            var tex = material.mainTexture as Texture2D;

            if (tex == null)
                return;

            AddWatermark(material, Random.Range(0 , tex.width), Random.Range(0, tex.height), textures);
        }


        public void DrawWaterMark(Material material, int i, int j, Texture2D background, Texture2D watermark)
        {

            // High performance block, rework it, if u know how to

           var backWidth = background.width;
           var backHeight = background.height;

           var markWidth = watermark.width;
           var markHeight = watermark.height;

            var result = new Texture2D(backWidth, backHeight, TextureFormat.ARGB32, false);

            var startX = i - markWidth / 2;
            var startY = j - markHeight / 2;

            var drawList = new List<Vector2>();

            for (var x = 0; x < backWidth; x++)
            {
                for (var y = 0; y < backHeight; y++)
                {
                    if (x >= startX && y >= startY && x < startX + markWidth && y < startY + markHeight)
                    {
                        var xX = x - startX;
                        var yY = y - startY;

                        var wmColor = watermark.GetPixel(xX, yY);
                        var bgColor = background.GetPixel(x, y);

                        if (wmColor.a == 1)
                        {
                            result.SetPixel(x, y - 1, Color.Lerp(bgColor, _backColor, 0.1f));
                            drawList.Add(new Vector2(x, y));
                        }
                        else
                        {
                            result.SetPixel(x, y, bgColor);
                        }
                    }
                    else
                    {
                        var bgColor = background.GetPixel(x, y);
                        result.SetPixel(x, y, bgColor);
                    }
                }
            }

            foreach (var vector2 in drawList)
            {
                var x = (int)vector2.x;
                var y = (int)vector2.y;

                var bgColor = background.GetPixel(x, y);
                var newColor = bgColor * _frontColor;

                result.SetPixel(x, y, newColor);
            }

            result.filterMode = FilterMode.Point;
            result.Apply();

            material.mainTexture = result;           
        }

        public void AddWatermark(Material material, int i, int j, Texture2D[] textures)
        {
            var background = _itemRender?.materials[0].mainTexture as Texture2D;

            if (background == null)
                return;

            var watermark = textures[Random.Range(0, textures.Length)];

            DrawWaterMark(material, i, j, background, watermark);
        }
    }
}