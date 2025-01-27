using System;
using System.IO;
using System.Linq;
using CommonDLL.Static;
using UnityEngine;
using UnityEngine.Purchasing;

namespace BlackTemple.EpicMine
{
    public static class SpriteHelper
    {
        public static Sprite GetFlagLongCode(string code)
        {
            Enum.TryParse(code, out SystemLanguage type);

            var shortCode = LocalizationHelper.ToCountryCode(type);
            var flags = Resources.LoadAll<Sprite>(Path.Combine(Paths.ResourcesSpriteAtlasesPath, "Flags"));
            var flag = flags.FirstOrDefault(p => p.name == shortCode);

            return flag == null ? flags.FirstOrDefault(p => p.name == "EN") : flag;
        }

        public static Sprite GetFlagShortCode(string code)
        {
            var flags = Resources.LoadAll<Sprite>(Path.Combine(Paths.ResourcesSpriteAtlasesPath, "Flags"));
            var flag = flags.FirstOrDefault(p => p.name == code);

            return flag == null ? flags.FirstOrDefault(p => p.name == "EN") : flag;
        }

        public static Sprite GetDiscoverPicture(string id)
        {
            var discovers = Resources.LoadAll<Sprite>(Path.Combine(Paths.ResourcesSpriteAtlasesPath, "discovers"));
            return discovers.FirstOrDefault(p => p.name == id.ToLower());
        }

        public static Sprite GetFeaturePicture(FeaturesType type)
        {
            var res = App.Instance.ReferencesTables.Sprites.Features.FirstOrDefault(x => x.Id == type);
            return res.Resource;
        }

        public static Sprite GetIcon(string id)
        {
            return Resources.Load<Sprite>(Path.Combine(Paths.ResourcesIconsPath, id.ToLower()));
        }

        public static Sprite GetArenaPreview(int num)
        {
            var pickaxes = Resources.LoadAll<Sprite>(Path.Combine(Paths.ResourcesSpriteAtlasesPath, "arena_preview_" + (num > 8 ? "9-12" : "1-8")));
            var resName = num.ToString("00");
            return pickaxes.FirstOrDefault(p => p.name == resName.ToLower());
        }
        public static Sprite GetPickaxeImage(string id)
        {
            var pickaxes = Resources.LoadAll<Sprite>(Path.Combine(Paths.ResourcesSpriteAtlasesPath, "pickaxes"));
            var pickaxe = pickaxes.FirstOrDefault(p => p.name == id.ToLower());
            if (pickaxe == null)
            {
                var pickaxes2 = Resources.LoadAll<Sprite>(Path.Combine(Paths.ResourcesSpriteAtlasesPath, "pickaxes2"));
                pickaxe = pickaxes2.FirstOrDefault(p => p.name == id.ToLower());
            }

            return pickaxe;
        }

        public static Sprite GetEffectTimerIcon(string effectId)
        {
            return App.Instance.ReferencesTables.Sprites.EffectIcons.FirstOrDefault(x => x.name == effectId);
        }

        public static Sprite GetPvpNumberDecal(int num)
        {
            var path = Path.Combine(Paths.ResourcesWallNumbersPath, (num+1).ToString());
            var number = Resources.Load<Sprite>(path);
            return number;
        }

        public static Sprite GetTorchImage(string id)
        {
            var pickaxes = Resources.LoadAll<Sprite>(Path.Combine(Paths.ResourcesSpriteAtlasesPath, "torches"));
            return pickaxes.FirstOrDefault(p => p.name == id.ToLower());
        }
        public static Sprite GetEmodji(EmoType emodji)
        {
            var strName = emodji.ToString().ToLower();
            var emodjis = Resources.LoadAll<Sprite>(Path.Combine(Paths.ResourcesSpriteAtlasesPath, "emodji"));
            return emodjis.FirstOrDefault(p => p.name == strName);
        }

        public static Sprite GetShopPackImage(string id)
        {
            var shopPacks = Resources.LoadAll<Sprite>(Path.Combine(Paths.ResourcesSpriteAtlasesPath, "shop"));
            var sprite = shopPacks.FirstOrDefault(p => p.name == id.ToLower());
            if (sprite == null)
            {
                shopPacks = Resources.LoadAll<Sprite>(Path.Combine(Paths.ResourcesSpriteAtlasesPath, "Shop3"));
                sprite = shopPacks.FirstOrDefault(p => p.name == id.ToLower());
            }
            return sprite;
        }

        public static Sprite GetPickaxeRarityBackground(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Simple:
                    return App.Instance.ReferencesTables.Sprites.Simple;
                case Rarity.Rare:
                    return App.Instance.ReferencesTables.Sprites.Rare;
                case Rarity.Legendary:
                    return App.Instance.ReferencesTables.Sprites.Legendary;
                default:
                    throw new ArgumentOutOfRangeException("rarity", rarity, null);
            }
        }

        public static Sprite GetPickaxeRarityBackgroundLuxAndUsual(string pickaxeId , int luxRang = 0)
        {
            var pic = App.Instance.StaticData.Pickaxes.Find(x => x.Id == pickaxeId);
            if (pic == null) return null;
            
            if (pic.Type == PickaxeType.Donate)
                return App.Instance.ReferencesTables.Sprites.ShopPackPickaxeBackgrounds[luxRang];

              return  GetPickaxeRarityBackground(pic.Rarity);
            
        }
        public static Sprite GetTorchBackground(string torch)
        {
            var staticTorch = App.Instance.StaticData.Torches.Find(x => x.Id == torch);
            
            switch (staticTorch.Rarity)
            {
                case Rarity.Simple:
                    return App.Instance.ReferencesTables.Sprites.Simple;
                case Rarity.Rare:
                    return App.Instance.ReferencesTables.Sprites.Rare;
                case Rarity.Legendary:
                    return App.Instance.ReferencesTables.Sprites.Legendary;
                default:
                    throw new ArgumentOutOfRangeException("rarity", staticTorch.Rarity, null);
            }
        }

        public static Sprite GetTorchBackground(Core.Torch torch)
        {
            switch (torch.StaticTorch.Rarity)
            {
                case Rarity.Simple:
                    return App.Instance.ReferencesTables.Sprites.Simple;
                case Rarity.Rare:
                    return App.Instance.ReferencesTables.Sprites.Rare;
                case Rarity.Legendary:
                    return App.Instance.ReferencesTables.Sprites.Legendary;
                default:
                    throw new ArgumentOutOfRangeException("rarity", torch.StaticTorch.Rarity, null);
            }
        }

        public static Sprite GetPickaxeRarityBackgroundLuxAndUsual(Core.Pickaxe pickaxe)
        {
            return pickaxe.StaticPickaxe.Type == PickaxeType.Donate
                ? GetPickaxeLuxBackground(pickaxe)
                : GetPickaxeRarityBackground(pickaxe.StaticPickaxe.Rarity);
        }

        public static Sprite GetPickaxeLuxBackground(Core.Pickaxe pickaxe)
        {
            var number = 0;

            foreach (var blPickaxe in App.Instance.Player.Blacksmith.Pickaxes)
            {
                if (blPickaxe == pickaxe)
                    break;

                if (blPickaxe.StaticPickaxe.Type == PickaxeType.Donate)
                    number++;
            }

            if (number > App.Instance.ReferencesTables.Sprites.ShopPackPickaxeBackgrounds.Length)
                number = number / App.Instance.ReferencesTables.Sprites.ShopPackPickaxeBackgrounds.Length;

            return App.Instance.ReferencesTables.Sprites.ShopPackPickaxeBackgrounds[number];
        }

        public static Sprite GetCurrencyIcon(CurrencyType currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.Gold:
                    return App.Instance.ReferencesTables.Sprites.GoldIcon;
                case CurrencyType.Crystals:
                    return App.Instance.ReferencesTables.Sprites.CrystalsIcon;
                default:
                    throw new ArgumentOutOfRangeException("currencyType", currencyType, null);
            }
        }

        public static Sprite GetRatingIcon(ViewRatingType type, bool isFull = false)
        {
            switch (type)
            {
                case ViewRatingType.Stars:
                    return isFull
                        ? App.Instance.ReferencesTables.Sprites.StarFullIcon
                        : App.Instance.ReferencesTables.Sprites.StarEmptyIcon;
                case ViewRatingType.Skulls:
                    return isFull
                        ? App.Instance.ReferencesTables.Sprites.SkullFullIcon
                        : App.Instance.ReferencesTables.Sprites.SkullEmptyIcon;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

       /* public static Sprite GetShopPackBackground(ShopPack shopPack)
        {
            switch (shopPack.Type)
            {
                case ShopPackType.Gold:
                    return App.Instance.ReferencesTables.Sprites.ShopPackGoldBackground;
                case ShopPackType.Chest:
                    if (shopPack.Id.Contains("1"))
                        return App.Instance.ReferencesTables.Sprites.ShopPackChestBackgrounds[0];
                    else if (shopPack.Id.Contains("2"))
                        return App.Instance.ReferencesTables.Sprites.ShopPackChestBackgrounds[1];
                    else
                        return App.Instance.ReferencesTables.Sprites.ShopPackChestBackgrounds[2];
                case ShopPackType.Potion:
                    return App.Instance.ReferencesTables.Sprites.ShopPackPotionBackground;
                case ShopPackType.Tnt:
                    return App.Instance.ReferencesTables.Sprites.ShopPackTntBackground;
                default:
                 //   throw new ArgumentOutOfRangeException("shopPackType", shopPack, null);
                    break;
            }
        }*/

        public static Sprite GetShopPackImage(ShopPack pack)
        {
            var shopPacks = Resources.LoadAll<Sprite>(Path.Combine(Paths.ResourcesSpriteAtlasesPath, "shop3"));
            return shopPacks.FirstOrDefault(p => p.name == pack.Id.ToLower());
        }

        public static Sprite GetShopPackBackground(Product product)
        {
            return App.Instance.ReferencesTables.Sprites.ShopPackCrystalsBackground;
        }

        public static Sprite GetShopPackBackground(ShopPack product)
        {
            return App.Instance.ReferencesTables.Sprites.ShopPackCrystalsBackground;
        }
        public static Sprite GetAbilityIcon(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.ExplosiveStrike:
                    return App.Instance.ReferencesTables.Sprites.ExplosiveStrikeAbilityIcon;
                case AbilityType.Freezing:
                    return App.Instance.ReferencesTables.Sprites.FreezingAbilityIcon;
                case AbilityType.Acid:
                    return App.Instance.ReferencesTables.Sprites.AcidAbilityIcon;
                case AbilityType.Torch:
                    return App.Instance.ReferencesTables.Sprites.Features
                        .FirstOrDefault(x => x.Id == FeaturesType.TorchAbility).Resource;
                default:
                    throw new ArgumentOutOfRangeException(nameof(abilityType), abilityType, null);
            }
        }

        public static Sprite GetSectionBuffIcon(MineSceneSectionBuff buff)
        {
            var freezingBuff = buff as MineSceneSectionFreezingBuff;
            if (freezingBuff != null)
                return App.Instance.ReferencesTables.Sprites.FreezingAbilityBuffIcon;

            var acidBuff = buff as MineSceneSectionAcidBuff;
            if (acidBuff != null)
                return App.Instance.ReferencesTables.Sprites.AcidAbilityBuffIcon;

            return null;
        }

        public static Sprite GetHeroBuffIcon(MineSceneHeroBuff buff)
        {
            var damagePotionBuff = buff as MineSceneHeroDamagePotionBuff;

            if (damagePotionBuff != null)
                return GetIcon(damagePotionBuff.Potion.Id);

            var acidBuff = buff as MineSceneHeroAcidBuff;

            if (acidBuff != null)
                return App.Instance.ReferencesTables.Sprites.AcidAbilityIcon;

            var prestigeBuff = buff as MineSceneHeroPrestigeBuff;

            if (prestigeBuff != null)
                return GetPrestigeIcon(App.Instance.Player.Prestige);

            return null;
        }

        public static Sprite GetPrestigeIcon(int level)
        {
            return App.Instance.ReferencesTables.Sprites.PrestigeIcons[level - 1];
        }

        public static Sprite GetSkillIcon(SkillType type)
        {
            return type == SkillType.Crit
                ? App.Instance.ReferencesTables.Sprites.CritIcon
                : type == SkillType.Damage
                    ? App.Instance.ReferencesTables.Sprites.DamageIcon
                    : App.Instance.ReferencesTables.Sprites.FortuneIcon;
        }
    }
}