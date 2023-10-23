using Basis;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameSettingBase : SerializedScriptableObject
{
    public enum InitializationStage
    {
        None,
        PreInit,
        Init,
        PostInit,
        FinishInit,
    }

    public const string DEBUGGING_CATEGORY = "调试";

    public virtual bool isSettingUnmovable => false;

    public virtual string forcedFileName => null;

    [LabelText("是否加载完成"), TitleGroup(DEBUGGING_CATEGORY)]
    [ShowInInspector, ReadOnly]
    public bool initDone { get; private set; } = false;

    [LabelText("已完成初始化阶段"), TitleGroup(DEBUGGING_CATEGORY)]
    [ShowInInspector, ReadOnly]
    public InitializationStage finishedInitializationStage { get; private set; } = InitializationStage.None;

    protected virtual void Awake()
    {

    }

    public void PreInit()
    {
        initDone = false;
        finishedInitializationStage = InitializationStage.None;
        OnPreInit();
        finishedInitializationStage = InitializationStage.PreInit;
    }

    public void Init()
    {
        OnInit();
        finishedInitializationStage = InitializationStage.Init;
    }

    public void PostInit()
    {
        OnPostInit();
        finishedInitializationStage = InitializationStage.PostInit;
    }

    public void FinishInit()
    {
        OnFinishInit();
        finishedInitializationStage = InitializationStage.FinishInit;
        initDone = true;
    }

    protected virtual void OnPreInit()
    {

    }

    protected virtual void OnInit()
    {

    }

    protected virtual void OnPostInit()
    {

    }

    protected virtual void OnFinishInit()
    {

    }

    public virtual void CheckSettings()
    {

    }

    public virtual void CheckSettingsGUI()
    {
        CheckSettings();
    }

    private bool hasSettingError = false;
    private string checkSettingInfo = "未检查设置！！！";

    protected virtual void OnValidate()
    {
        checkSettingInfo = "设置已更新，请重新点击检查设置按钮！！！";
    }

    [Title("检查")]
    [InfoBox("此配置文件不可移动，不可创建多份", InfoMessageType.Warning, VisibleIf = "isSettingUnmovable")]
    [InfoBox(@"@""此配置文件不可改名，当前名称："" + name + ""，请改回名称："" + forcedFileName", 
        InfoMessageType.Warning, VisibleIf = @"@forcedFileName != null && name != forcedFileName")]
    [InfoBox("@" + nameof(checkSettingInfo), InfoMessageType.Warning)]
    [HideInInlineEditors]
    [GUIColor(nameof(GetSettingCheckColorGUI)), PropertySpace(SpaceBefore = 10), PropertyOrder(999)]
    [Button("检查设置", ButtonSizes.Large, Icon = SdfIconType.ArrowRightCircle)]
    private void CheckSettingsButton()
    {
        hasSettingError = true;
        checkSettingInfo = "设置有问题！";

        CheckSettingsGUI();

        checkSettingInfo = "设置无问题！";
        hasSettingError = false;
    }

    private Color GetSettingCheckColorGUI()
    {
        if (hasSettingError)
        {
            return new(1, 0, 0);
        }
        else
        {
            return new(0, 1, 0);
        }
    }
}


public abstract class GameSettingBaseInScene : SerializedMonoBehaviour
{

    public virtual bool isSettingUnmovable => false;

    protected virtual void Awake()
    {
        CheckSettings();
    }

    public virtual void Init()
    {

    }
    public virtual void CheckSettings()
    {

    }

    private const string nonErrorOutput = "无错误";
    private bool isSettingError = false;
    private string checkSettingInfo = "未检查设置！！！";

    protected virtual void OnValidate()
    {
        checkSettingInfo = "设置已更新，请重新点击检查设置按钮！！！";
    }

    [Title("检查")]
    [InfoBox("此配置文件不可移动，不可创建多份", InfoMessageType.Error, VisibleIf = "isSettingUnmovable")]
    [InfoBox("$checkSettingInfo", InfoMessageType.Warning)]
    [HideInInlineEditors]
    [GUIColor(0, 1, 0), PropertySpace(SpaceBefore = 10, SpaceAfter = 30), PropertyOrder(999)]
    [Button("检查设置", ButtonSizes.Large, Icon = SdfIconType.ArrowRightCircleFill)]
    private void CheckSettingsButton()
    {
        try
        {
            CheckSettings();
        }
        catch (Exception e)
        {
            checkSettingInfo = "设置有问题，请在Console控制台或错误报告查看具体信息";
            isSettingError = true;

            if (errorOutput == nonErrorOutput)
            {
                errorOutput = "错误报告已在子对象中显示";
            }

            throw e;
        }

        checkSettingInfo = "设置无问题！";
        errorOutput = nonErrorOutput;
        isSettingError = false;
    }

    [Title("错误报告")]
    [HideLabel]
    [DisplayAsString(false), ShowIf("isSettingError"), SerializeField, PropertyOrder(998), GUIColor("@Color.red")]
    private string errorOutput = "";

    protected void ShowSettingError(string error)
    {
        errorOutput = error;
        Note.note.Error(error);
    }

    public virtual void _DisplaySelf()
    {
        Note.note.Log(gameObject.name);
    }
}


public abstract class UniqueGameSettingBaseInScene<T> : GameSettingBaseInScene
    where T : UniqueGameSettingBaseInScene<T>
{
    public static T instance;

    protected override void Awake()
    {
        if (instance != null)
        {
            Note.note.Error($"重复添加组件{nameof(T)}");
        }

        instance = (T)this;

        base.Awake();
    }
}