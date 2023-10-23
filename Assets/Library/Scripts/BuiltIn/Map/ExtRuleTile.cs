using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Basis;
using Basis.GameItem;
using ConfigurationBasis;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ExtendedTilemap
{
    public enum RuleMode
    {
        [LabelText("普通")]
        Normal,
        [LabelText("继承")]
        Inheritance,
        [LabelText("覆写")]
        Override
    }

    public enum LimitType
    {
        [LabelText("无约束")]
        None,
        [LabelText("此瓦片")]
        This,
        [LabelText("非此瓦片")]
        NotThis,
        [LabelText("空瓦片")]
        IsEmpty,
        [LabelText("非空瓦片")]
        NotEmpty,
        [LabelText("特定瓦片")]
        SpecificTiles,
        [LabelText("非特定瓦片")]
        NotSpecificTiles,
        [LabelText("此瓦片或父瓦片")]
        ThisOrParent,
        [LabelText("非此瓦片和父瓦片")]
        NotThisAndParent,
    }

    [Serializable]
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    [GUIColor("@$value.GetLimitColor()")]
    public class Limit
    {
        [HideLabel]
        [OnInspectorInit(nameof(OnInspectorInit))]
        [OnValueChanged(nameof(OnLimitTypeChanged))]
        public LimitType limitType;

        [LabelText("特定瓦片列表")]
        [ShowIf(@"@limitType == LimitType.SpecificTiles || limitType == LimitType.NotSpecificTiles")]
        [ValueDropdown("@GameCoreSettingBase.extRuleTileGeneralSetting.GetPrefabNameList()")]
        [InfoBox("至少添加一个", InfoMessageType.Warning, "@specificTiles.Count == 0")]
        [ListDrawerSettings(DefaultExpandedState = false)]
        public List<string> specificTiles = new();

        #region GUI

        private void OnInspectorInit()
        {
            specificTiles ??= new();
        }

        private Color GetLimitColor()
        {
            return limitType switch
            {
                LimitType.None => Color.white,
                LimitType.This => new(0.5f, 1, 0.5f, 1),
                LimitType.NotThis => new(1, 0.5f, 0.5f, 1),
                LimitType.SpecificTiles => new(0.7f, 0.7f, 1, 1),
                LimitType.NotSpecificTiles => new(0.5f, 1f, 1, 1),
                LimitType.IsEmpty => new(1, 1, 0.5f, 1),
                LimitType.NotEmpty => new (1, 0.5f, 1, 1),
                LimitType.ThisOrParent => new(0.6f, 0.9f, 0.8f, 1),
                LimitType.NotThisAndParent => new(0.9f, 0.7f, 0.5f, 1),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void OnLimitTypeChanged()
        {
            if (limitType == LimitType.SpecificTiles)
            {
                if (specificTiles == null || specificTiles.Count == 0)
                {
                    specificTiles = new();
                    specificTiles.AddRange(GameCoreSettingBase.extRuleTileGeneralSetting.defaultSpecificTiles);
                }
            }

            if (limitType == LimitType.NotSpecificTiles)
            {
                if (specificTiles == null || specificTiles.Count == 0)
                {
                    specificTiles = new();
                    specificTiles.AddRange(GameCoreSettingBase.extRuleTileGeneralSetting.defaultNotSpecificTiles);
                }
            }
        }

        #endregion

        public bool Equals(Limit other)
        {
            if (limitType != other.limitType)
            {
                return false;
            }

            if (limitType is LimitType.SpecificTiles or LimitType.NotSpecificTiles)
            {
                if (specificTiles.Count != other.specificTiles.Count)
                {
                    return false;
                }

                if (specificTiles.ToArray() != other.specificTiles.ToArray())
                {
                    return false;
                }
            }

            return true;
        }

        public void CopyFrom(Limit other)
        {
            limitType = other.limitType;

            if (limitType is LimitType.SpecificTiles or LimitType.NotSpecificTiles)
            {
                specificTiles = new();
                specificTiles.AddRange(other.specificTiles);
            }
        }

        public static implicit operator Limit(LimitType limitType)
        {
            return new()
            {
                limitType = limitType
            };
        }
    }

    [Serializable]
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    public class Rule
    {
        [LabelText("是否开启")]
        [HorizontalGroup("Config")]
        public bool enable = true;

        [LabelText("是否开启动画")]
        [HorizontalGroup("Config")]
        public bool enableAnimation = false;

        [HideLabel]
        [HorizontalGroup("Rule")]
        [OnInspectorInit(nameof(OnInspectorInit))]
        [HideIf(nameof(enableAnimation))]
        public SpriteSetter sprite = new();

        [LabelText("动画列表")]
        [HorizontalGroup("Rule")]
        [ShowIf(nameof(enableAnimation))]
        public SpriteListSetter animationSprites = new();

        [HideLabel]
        [VerticalGroup("Rule/Limit")]
        [HorizontalGroup("Rule/Limit/Upper")]
        public Limit upperLeft = new();
        [HideLabel]
        [HorizontalGroup("Rule/Limit/Upper")]
        public Limit upper = new();
        [HideLabel]
        [HorizontalGroup("Rule/Limit/Upper")]
        public Limit upperRight = new();
        [HideLabel]
        [HorizontalGroup("Rule/Limit/Center")]
        public Limit left = new();
        [HideLabel]
        [HorizontalGroup("Rule/Limit/Center")]
        [ShowInInspector, ReadOnly]
        private Limit center = new();
        [HideLabel]
        [HorizontalGroup("Rule/Limit/Center")]
        public Limit right = new();
        [HideLabel]
        [HorizontalGroup("Rule/Limit/Lower")]
        public Limit lowerLeft = new();
        [HideLabel]
        [HorizontalGroup("Rule/Limit/Lower")]
        public Limit lower = new();
        [HideLabel]
        [HorizontalGroup("Rule/Limit/Lower")]
        public Limit lowerRight = new();

        [LabelText("动画间隔")]
        [ShowIf(nameof(enableAnimation))]
        [MinValue(0)]
        public float gap = 0.2f;

        [LabelText("开始时自动播放动画")]
        [ShowIf(nameof(enableAnimation))]
        public bool autoPlayOnStart = true;

        #region GUI

        protected virtual void OnInspectorInit()
        {
            sprite ??= new();

            upperLeft ??= new();
            upper ??= new();
            upperRight ??= new();
            left ??= new();
            right ??= new();
            lowerLeft ??= new();
            lower ??= new();
            lowerRight ??= new();
        }

        #endregion

        public IEnumerable<Limit> GetAllLimits()
        {
            return new[]
            {
                upperLeft, upper, upperRight,
                left, right,
                lowerLeft, lower, lowerRight
            };
        }

        #region Preset

        public static Rule UpperLeftOnly(LimitType upperLowerLeftRightType, LimitType upperLeft)
        {
            return new()
            {
                left = upperLowerLeftRightType,
                right = upperLowerLeftRightType,
                upper = upperLowerLeftRightType,
                lower = upperLowerLeftRightType,
                upperLeft = upperLeft
            };
        }

        public static Rule UpperRightOnly(LimitType upperLowerLeftRightType, LimitType upperRight)
        {
            return new()
            {
                left = upperLowerLeftRightType,
                right = upperLowerLeftRightType,
                upper = upperLowerLeftRightType,
                lower = upperLowerLeftRightType,
                upperRight = upperRight
            };
        }

        public static Rule LowerLeftOnly(LimitType upperLowerLeftRightType, LimitType lowerLeft)
        {
            return new()
            {
                left = upperLowerLeftRightType,
                right = upperLowerLeftRightType,
                upper = upperLowerLeftRightType,
                lower = upperLowerLeftRightType,
                lowerLeft = lowerLeft
            };
        }

        public static Rule LowerRightOnly(LimitType upperLowerLeftRightType, LimitType lowerRight)
        {
            return new()
            {
                left = upperLowerLeftRightType,
                right = upperLowerLeftRightType,
                upper = upperLowerLeftRightType,
                lower = upperLowerLeftRightType,
                lowerRight = lowerRight
            };
        }

        #endregion
    }

    [Serializable]
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    public class ExtRuleTile : 
        GamePrefabCoreBundle<ExtRuleTile, ExtRuleTileGeneralSetting>.GameItemPrefab, IParentOnlyNode<ExtRuleTile>
    {
        protected override string preferredIDSuffix => "tile";

        [LabelText("是否启用实时贴图更新")]
        public bool enableUpdate = true;

        [LabelText("模式")]
        [EnumToggleButtons, GUIColor(nameof(GetRuleModeColor))]
        public RuleMode ruleMode = RuleMode.Normal;

        [LabelText("父规则瓦片ID")]
        [ValueDropdown("@GameCoreSettingBase.extRuleTileGeneralSetting.GetPrefabNameList()")]
        [InfoBox("ID不能为空", InfoMessageType.Warning, "@StringFunc.IsNullOrEmpty(parentRuleTileID)")]
        [InfoBox("不能为自己的ID", InfoMessageType.Error, "@parentRuleTileID == id")]
        [ShowIf("@ruleMode == RuleMode.Inheritance || ruleMode == RuleMode.Override")]
        public string parentRuleTileID;

        public ExtRuleTile _parentRuleTile;

        public ExtRuleTile parentRuleTile
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                _parentRuleTile ??= GameCoreSettingBase.extRuleTileGeneralSetting.GetPrefabStrictly(parentRuleTileID);

                return _parentRuleTile;
            }
        }

        [LabelText("默认贴图")]
        [HideIf("@ruleMode == RuleMode.Inheritance || ruleMode == RuleMode.Override")]
        public SpriteSetter defaultSprite = new();

        [LabelText("规则集")]
        [ListDrawerSettings(NumberOfItemsPerPage = 4, DefaultExpandedState = false)]
        public List<Rule> ruleSet = new();

        #region GUI

        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();
            
            defaultSprite ??= new();
            ruleSet ??= new();
        }

        private Color GetRuleModeColor()
        {
            return ruleMode switch
            {
                RuleMode.Normal => Color.white,
                RuleMode.Inheritance => new(1, 1, 0.5f, 1),
                RuleMode.Override => new(0.5f, 1, 0.5f, 1),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        #region AddPreset

        [Button("添加带边框的瓦片预设（上下左右四种边框，共16种）")]
        [FoldoutGroup("工具", false)]
        [PropertyOrder(99)]
        private void AddBorderTilePreset16()
        {
            var combinations = new List<LimitType>() { LimitType.This, LimitType.NotThis }.
                GenerateCombinations(4).ToList();

            combinations.Sort(item =>
            {
                return item.Count(LimitType.This) switch
                {
                    1 => 1,
                    3 => 2,
                    2 => 3,
                    0 => 4,
                    4 => 4,
                    _ => throw new ArgumentOutOfRangeException()
                };
            });

            foreach (var combination in combinations)
            {
                ruleSet.Add(new()
                {
                    left = combination[0],
                    right = combination[1],
                    upper = combination[2],
                    lower = combination[3]
                });
            }
        }

        [Button("添加4种小正方形组成的瓦片预设（除去对角正方形和没有或全是正方形的情况，12种）")]
        [FoldoutGroup("工具", false)]
        [PropertyOrder(99)]
        private void Add4SubRectangleTilePreset12()
        {
            ruleSet.Add(Rule.UpperLeftOnly(LimitType.This, LimitType.NotThis));
            ruleSet.Add(Rule.UpperRightOnly(LimitType.This, LimitType.NotThis));
            ruleSet.Add(Rule.LowerLeftOnly(LimitType.This, LimitType.NotThis));
            ruleSet.Add(Rule.LowerRightOnly(LimitType.This, LimitType.NotThis));

            var combinations = new List<LimitType>() { LimitType.This, LimitType.NotThis }.
                GenerateCombinations(4).ToList();

            combinations.Remove(item => (item[0] == LimitType.NotThis && item[1] == LimitType.NotThis) ||
                                        (item[2] == LimitType.NotThis && item[3] == LimitType.NotThis));

            combinations.Remove(combination => combination.All(item => item == LimitType.This));

            combinations.Sort(item =>
            {
                return item.Count(LimitType.This) switch
                {
                    1 => 1,
                    3 => 2,
                    2 => 3,
                    0 => 4,
                    4 => 4,
                    _ => throw new ArgumentOutOfRangeException()
                };
            });

            foreach (var combination in combinations)
            {
                ruleSet.Add(new()
                {
                    left = combination[0],
                    right = combination[1],
                    upper = combination[2],
                    lower = combination[3]
                });
            }
        }

        #endregion


        [LabelText("操作的规则范围")]
        [MinMaxSlider(0, "@ruleSet.Count - 1")]
        [FoldoutGroup("工具", false)]
        [PropertyOrder(100)]
        [SerializeField]
        private Vector2Int operativeRange;

        [LabelText("替换前的束缚")]
        [HorizontalGroup("工具/Replace")]
        [SerializeField]
        [PropertyOrder(101)]
        private Limit limitBeforeReplace = new();

        [LabelText("替换后的束缚")]
        [HorizontalGroup("工具/Replace")]
        [SerializeField]
        [PropertyOrder(102)]
        private Limit limitAfterReplace = new();

        [LabelText("是否比较特定瓦片列表里的内容")]
        [FoldoutGroup("工具", false)]
        [SerializeField]
        [PropertyOrder(103)]
        private bool compareSpecificTiles = true;

        [FoldoutGroup("工具", false)]
        [Button("替换规则集中的束缚")]
        [PropertyOrder(104)]
        private void ReplaceLimit()
        {
            for (var index = operativeRange.x; index <= operativeRange.y; index++)
            {
                var rule = ruleSet[index];

                foreach (var limit in rule.GetAllLimits())
                {
                    var isEqual = false;

                    if (compareSpecificTiles)
                    {
                        isEqual = limit.Equals(limitBeforeReplace);
                    }
                    else
                    {
                        isEqual = limit.limitType == limitBeforeReplace.limitType;
                    }

                    if (isEqual)
                    {
                        limit.CopyFrom(limitAfterReplace);
                    }
                }
            }
        }

        [FoldoutGroup("工具", false)]
        [Button("启用部分规则")]
        private void EnableRules()
        {
            for (var index = operativeRange.x; index <= operativeRange.y; index++)
            {
                var rule = ruleSet[index];

                rule.enable = true;
            }
        }

        [FoldoutGroup("工具", false)]
        [Button("禁用部分规则")]
        private void DisableRules()
        {
            for (var index = operativeRange.x; index <= operativeRange.y; index++)
            {
                var rule = ruleSet[index];

                rule.enable = false;
            }
        }

        #endregion

        public ExtRuleTile GetParent()
        {
            if (ruleMode != RuleMode.Inheritance)
            {
                return null;
            }

            return parentRuleTile;
        }
    }
}


