using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Basis;
using Basis.GameItem;
using ConfigurationBasis;
using ObjectAnimationBasis;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public enum ShowType
{
    [LabelText("无动画")]
    None,
    [LabelText("淡入")]
    Fade
}

public enum HideType
{
    [LabelText("无动画")]
    None,
    [LabelText("淡出")]
    Fade,
    [LabelText("跳跃淡出")]
    Leap
}

public class ObjectParticlePrefab<TPrefab, TGeneralSetting> : 
    GamePrefabCoreBundle<TPrefab, TGeneralSetting>.GameItemPrefab
    where TPrefab : ObjectParticlePrefab<TPrefab, TGeneralSetting>
    where TGeneralSetting : ObjectParticleGeneralSetting<TPrefab, TGeneralSetting>
{
    protected virtual Type requireType => typeof(ObjectParticleBase<TPrefab, TGeneralSetting>);

    [Title("描述", Bold = false), HideLabel]
    [MultiLineProperty(2)]
    [ShowIf("showDescription")]
    public string description;

    #region Basic

    [FoldoutGroup("基本设置")]
    [LabelText("动画更新类型")]
    [EnumToggleButtons]
    public UpdateType updateType = UpdateType.LateUpdate;

    [FoldoutGroup("基本设置")]
    [LabelText("持续追踪")]
    [ToggleButtons("是", "否")]
    public bool continuousTracing = false;

    [FoldoutGroup("基本设置")]
    [LabelText("追踪插值")]
    [ToggleButtons("开", "关")]
    [ShowIf("continuousTracing")]
    public bool tracingLerp = true;

    [FoldoutGroup("基本设置")]
    [LabelText("插值速度")]
    [MinValue(0.001)]
    [ShowIf("@tracingLerp && continuousTracing")]
    public float lerpSpeed = 0.5f;

    [FoldoutGroup("基本设置")]
    [LabelText("每个追踪对象的激活UI数量")]
    [ToggleButtons("限制", "不限制")]
    public bool limitAmountOnSameTarget = false;

    [FoldoutGroup("基本设置")]
    [LabelText("每个追踪对象的激活数量限制")]
    [MinValue(1), MaxValue(150)]
    [ShowIf("limitAmountOnSameTarget")]
    public int sameTargetLimitedAmount = 15;

    [FoldoutGroup("基本设置")]
    [LabelText("全局对象池容量")]
    [MinValue(1), MaxValue(150)]
    public int globalPoolCapacity = 30;

    [FoldoutGroup("基本设置")]
    [LabelText("全局激活的数量限制")]
    [MinValue(1), MaxValue(300)]
    public int globalLimitedAmount = 30;

    [FoldoutGroup("基本设置")]
    [LabelText("溢出替换模式")]
    [ToggleButtons("开启", "关闭")]
    public bool replaceMode = false;

    [FoldoutGroup("基本设置")]
    [LabelText("预制体")]
    [AssetsOnly, AssetSelector(Paths = "Assets/Resources"), Required]
    [InfoBox(@"@""此预制体不包含"" + requireType.ToString() + ""或其子类""", InfoMessageType.Error,
        @"@PrefabObjectContainsValidComponent()")]
    public ObjectParticleBase<TPrefab, TGeneralSetting> prefabObject;

    #endregion

    #region DisplaySetting

    [FoldoutGroup("显示设置")]
    [LabelText("出场额外偏移")]
    [HideIf("@continuousTracing && tracingLerp == false")]
    public Vector3Setter extraShowOffset = new();

    [FoldoutGroup("显示设置")]
    [LabelText("是否自动隐藏")]
    [ToggleButtons("是", "否")]
    public bool autoHide = true;

    //[FoldoutGroup("显示设置")]
    //[LabelText("显示持续时间(秒)")]
    //[ShowIf("autoHide")]
    //[DisableObjectChooserType("Lerp")]
    //[MinValue(0)]
    //public FloatSetter displayDuration = 0.7f;

    #endregion

    [FoldoutGroup("动画设置")]
    [LabelText("容器动画")]
    public ObjectAnimation containerAnimation = new();

    [HideInInspector]
    public bool showDescription = true;

    protected bool PrefabObjectContainsValidComponent()
    {
        if (prefabObject == null)
        {
            return false;
        }
        return !(prefabObject.GetType() == requireType || prefabObject.GetType().IsSubclassOf(requireType));
    }

    #region GUI

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        containerAnimation ??= new();
    }

    #endregion
}

public class ObjectParticleGeneralSetting<TPrefab, TGeneralSetting> : 
    GamePrefabCoreBundle<TPrefab, TGeneralSetting>.GameItemGeneralSetting
    where TPrefab : ObjectParticlePrefab<TPrefab, TGeneralSetting>
    where TGeneralSetting : ObjectParticleGeneralSetting<TPrefab, TGeneralSetting>
{
    public override StringTranslation prefabName => new()
    {
        { "Chinese", "Object粒子" },
        { "English", "Object Particle" }
    };

    [LabelText("显示描述")]
    [ToggleButtons("是", "否")]
    [OnValueChanged("OnShowDescriptionChanged")]
    public bool showDescription = true;

    #region GUI

    private void OnShowDescriptionChanged()
    {
        allGameItemPrefabs.ForEach(prefab => prefab.showDescription = showDescription);
    }

    #endregion
}
