using System;
using Basis;
using System.Collections;
using System.Collections.Generic;
using Basis.GameItem;
using Sirenix.OdinInspector;
using UnityEngine;

public class LoadingProcedure : Procedure
{
    [LabelText("加载结束进入流程"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
    [ValueDropdown("@GameCoreSettingBase.procedureGeneralSetting.GetPrefabNameList()")]
    [SerializeField]
    protected string autoEnterProcedure;

    protected void EnterNextProcedure()
    {
        LeaveThisProcedure();
        ProcedureManager.EnterProcedure(autoEnterProcedure);
    }
}

public class Procedure : GamePrefabCoreBundle<Procedure, ProcedureGeneralSetting>.GameItemPrefab
{
    protected override string preferredIDSuffix => "procedure";

    public event Action OnEnterEvent, OnLeaveEvent; 

    public virtual void OnEnter()
    {
        OnEnterEvent?.Invoke();
    }

    public virtual void OnLeave()
    {
        OnLeaveEvent?.Invoke();
    }

    protected virtual void LeaveThisProcedure()
    {
        ProcedureManager.LeaveProcedure(id);
    }
}

public sealed class ProcedureGeneralSetting : 
    GamePrefabCoreBundle<Procedure, ProcedureGeneralSetting>.GameItemGeneralSetting, 
    IManagerCreationProvider
{
    public const string DEFAULT_INITIAL_PROCEDURE_ID = GameInitializationProcedure.registeredID;

    public override StringTranslation settingName => new()
    {
        { "Chinese", "流程" },
        { "English", "Procedure" }
    };

    public override string fullSettingName => settingName;

    public override StringTranslation folderPath => coreCategory;

    public override bool hasIcon => true;

    public override SdfIconType icon => SdfIconType.ClockHistory;

    public override StringTranslation prefabName => new()
    {
        { "Chinese", "流程" },
        { "English", "Procedure" }
    };

    public override StringTranslation prefabSuffixName => "";

    public override bool gameTypeAbandoned => true;

    [LabelText("起始流程")]
    [ValueDropdown("@" + nameof(GetPrefabNameList) + "()")]
    [StringIsNotNullOrEmpty]
    public string initialProcedureID = DEFAULT_INITIAL_PROCEDURE_ID;

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        if (initialProcedureID.IsNullOrEmptyAfterTrim())
        {
            initialProcedureID = DEFAULT_INITIAL_PROCEDURE_ID;
        }
    }

    protected override void OnPreInit()
    {
        base.OnPreInit();

        if (initialProcedureID.IsNullOrEmptyAfterTrim())
        {
            initialProcedureID = DEFAULT_INITIAL_PROCEDURE_ID;
        }
    }

    public IEnumerable<ManagerType> GetManagerTypes()
    {
        return new[] { ManagerType.ProcedureCore, ManagerType.NetworkCore };
    }

    public void HandleManagerCreation(ManagerType managerType, Transform managerContainer)
    {
        if (managerType == ManagerType.ProcedureCore)
        {
            managerContainer.GetOrAddComponent<ProcedureManager>();
        }
        else if (managerType == ManagerType.NetworkCore)
        {
            managerContainer.GetOrAddComponent<NetworkProcedureTriggerController>();
        }
    }
}
