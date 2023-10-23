using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Basis;
using Basis.GameItem;
using ExtendedTilemap;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ExtRuleTileGeneralSetting : 
    GamePrefabCoreBundle<ExtRuleTile, ExtRuleTileGeneralSetting>.GameItemGeneralSetting
{
    public const string EMPTY_TILE_ID = "empty_tile";

    protected override bool showSetFirstPrefabIDAndNameToDefaultButton => false;

    public override StringTranslation settingName => new()
    {
        { "Chinese", "拓展瓦片" },
        { "English", "Extended Rule Tile" }
    };

    public override string fullSettingName => settingName;

    public override bool hasIcon => true;

    public override SdfIconType icon => SdfIconType.Grid3x3;

    public override StringTranslation folderPath => builtInCategory;

    public override StringTranslation prefabName => new()
    {
        { "Chinese", "拓展瓦片" },
        { "English", "Extended Rule Tile" }
    };

    public override StringTranslation prefabSuffixName => new()
    {
        { "Chinese", "预设" },
        { "English", "Preset" }
    };

    private TileBase _emptyTileBase;

    [LabelText("空TileBase")]
    [ShowInInspector]
    public TileBase emptyTileBase
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_emptyTileBase == null)
            {
                _emptyTileBase = CreateInstance<Tile>();
            }

            return _emptyTileBase;
        }
    }

    [LabelText("所有的TileBase")]
    [ShowInInspector]
    private Dictionary<Sprite, TileBase> allTileBases = new();

    [LabelText("默认的特定瓦片")]
    [ValueDropdown("@GameCoreSettingBase.extRuleTileGeneralSetting.GetPrefabNameList()")]
    [GUIColor(0.7f, 0.7f, 1)]
    public HashSet<string> defaultSpecificTiles = new();

    [LabelText("默认的非特定瓦片")]
    [ValueDropdown("@GameCoreSettingBase.extRuleTileGeneralSetting.GetPrefabNameList()")]
    [GUIColor(0.5f, 1f, 1)]
    public HashSet<string> defaultNotSpecificTiles = new();

    public void ClearBuffer()
    {
        foreach (var tileBase in allTileBases.Values)
        {
            DestroyImmediate(tileBase);
        }
        allTileBases.Clear();
    }

    protected override void Awake()
    {
        base.Awake();

        ClearBuffer();
    }

    #region GUI

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        AddPrefabToFirst(new()
        {
            id = EMPTY_TILE_ID,
            name =
            {
                { "Chinese", "空瓦片" }, 
                { "English", "Empty Tile" }
            }
        });
    }

    #endregion

    public TileBase GetTileBase(Sprite sprite)
    {
        if (sprite == null)
        {
            return emptyTileBase;
        }

        if (allTileBases.TryGetValue(sprite, out var tileBase))
        {
            return tileBase;
        }

        var newTileBase = CreateInstance<Tile>();
        newTileBase.sprite = sprite;

        allTileBases[sprite] = newTileBase;

        return newTileBase;
    }

    public Rule GetRule(string tileID, Neighbor neighbor)
    {
        var thisRuleTile = GetPrefabStrictly(tileID);

        var ruleSet = thisRuleTile.ruleSet;

        foreach (var rule in ruleSet)
        {
            if (rule.enable == false)
            {
                continue;
            }

            var neighborRuleTiles = new[]
            {
                neighbor.upperLeft, neighbor.upper, neighbor.upperRight,
                neighbor.left,                      neighbor.right,
                neighbor.lowerLeft, neighbor.lower, neighbor.lowerRight,
            };

            var neighborLimits = new[]
            {
                rule.upperLeft, rule.upper, rule.upperRight,
                rule.left,                      rule.right,
                rule.lowerLeft, rule.lower, rule.lowerRight,
            };

            if (neighborRuleTiles.Any((neighborIndex, item) =>
                    SatisfyLimit(thisRuleTile, item, neighborLimits[neighborIndex]) == false))
            {
                continue;
            }

            return rule;
        }

        if (thisRuleTile.ruleMode is RuleMode.Inheritance or RuleMode.Override)
        {
            if (thisRuleTile.parentRuleTileID.IsNullOrEmptyAfterTrim() == false &&
                thisRuleTile.parentRuleTileID != thisRuleTile.id)
            {
                return GetRule(thisRuleTile.parentRuleTileID, neighbor);
            }
        }

        return null;
    }

    public TileBase GetDefaultTileBase(string tileID)
    {
        var thisRuleTile = GetPrefabStrictly(tileID);

        if (thisRuleTile.ruleMode is RuleMode.Inheritance or RuleMode.Override)
        {
            if (thisRuleTile.parentRuleTileID.IsNullOrEmptyAfterTrim() == false &&
                thisRuleTile.parentRuleTileID != thisRuleTile.id)
            {
                return GetDefaultTileBase(thisRuleTile.parentRuleTileID);
            }
        }

        return GetTileBase(thisRuleTile.defaultSprite);
    }

    public static bool SatisfyLimit(ExtRuleTile thisRuleTile, ExtRuleTile neighborRuleTile, Limit limit)
    {
        Note.note.AssertIsNotNull(thisRuleTile, nameof(thisRuleTile));

        switch (limit.limitType)
        {
            case LimitType.None:
                return true;
            case LimitType.This:
                if (neighborRuleTile == null)
                {
                    return false;
                }

                if (neighborRuleTile.id != thisRuleTile.id)
                {
                    if (neighborRuleTile.ruleMode == RuleMode.Inheritance)
                    {
                        return SatisfyLimit(thisRuleTile, neighborRuleTile.parentRuleTile, limit);
                    }

                    return false;
                }

                return true;
            case LimitType.NotThis:
                if (neighborRuleTile == null)
                {
                    return true;
                }

                if (neighborRuleTile.id != thisRuleTile.id)
                {
                    if (neighborRuleTile.ruleMode == RuleMode.Inheritance)
                    {
                        return SatisfyLimit(thisRuleTile, neighborRuleTile.parentRuleTile, limit);
                    }

                    return true;
                }

                return false;
            case LimitType.SpecificTiles:
                if (neighborRuleTile == null)
                {
                    return false;
                }

                if (limit.specificTiles.Contains(neighborRuleTile.id) == false)
                {
                    if (neighborRuleTile.ruleMode == RuleMode.Inheritance)
                    {
                        return SatisfyLimit(thisRuleTile, neighborRuleTile.parentRuleTile, limit);
                    }

                    return false;
                }

                return true;
            case LimitType.NotSpecificTiles:
                if (neighborRuleTile == null)
                {
                    return true;
                }

                if (limit.specificTiles.Contains(neighborRuleTile.id) == false)
                {
                    if (neighborRuleTile.ruleMode == RuleMode.Inheritance)
                    {
                        return SatisfyLimit(thisRuleTile, neighborRuleTile.parentRuleTile, limit);
                    }

                    return true;
                }

                return false;
            case LimitType.IsEmpty:
                return neighborRuleTile == null || neighborRuleTile.id == EMPTY_TILE_ID;
            case LimitType.NotEmpty:
                return neighborRuleTile != null && neighborRuleTile.id != EMPTY_TILE_ID;
            case LimitType.ThisOrParent:

                if (neighborRuleTile == null)
                {
                    return false;
                }

                return SatisfyLimit(thisRuleTile.GetRoot(), neighborRuleTile, new()
                {
                    limitType = LimitType.This
                });

            case LimitType.NotThisAndParent:

                if (neighborRuleTile == null)
                {
                    return true;
                }

                return SatisfyLimit(thisRuleTile.GetRoot(), neighborRuleTile, new()
                {
                    limitType = LimitType.NotThis
                });
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
