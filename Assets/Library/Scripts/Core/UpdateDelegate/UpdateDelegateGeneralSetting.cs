using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "UpdateDelegateGeneralSetting", menuName = "GameConfiguration/UpdateDelegateGeneralSetting")]
public class UpdateDelegateGeneralSetting : GeneralSettingBase, IManagerCreationProvider
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "更新代理" },
        { "English", "Update Delegate" }
    };

    public override string fullSettingName => settingName;

    public override bool hasIcon => true;

    public override SdfIconType icon => SdfIconType.CloudFog;

    public override StringTranslation folderPath => coreCategory;

    [FormerlySerializedAs("updateDelegate")]
    [LabelText("更新代理")]
    [Required, SceneObjectsOnly]
    public UpdateDelegateManager updateDelegateManager;

    [LabelText("FixedUpdate事件代理数量")]
    [ShowInInspector]
    public int fixedUpdateEventCount => UpdateDelegateManager.fixedUpdateEventCount;

    [LabelText("Update事件代理数量")]
    [ShowInInspector]
    public int updateEventCount => UpdateDelegateManager.updateEventCount;

    [LabelText("LateUpdate事件代理数量")]
    [ShowInInspector]
    public int lateUpdateEventCount => UpdateDelegateManager.lateUpdateEventCount;

    [LabelText("OnGUI事件代理数量")]
    [ShowInInspector]
    public int onGUIEventCount => UpdateDelegateManager.onGUIEventCount;

    public IEnumerable<ManagerType> GetManagerTypes()
    {
        yield return ManagerType.EventCore;
    }

    public void HandleManagerCreation(ManagerType managerType, Transform managerContainer)
    {
        managerContainer.GetOrAddComponent<UpdateDelegateManager>();
    }
}
