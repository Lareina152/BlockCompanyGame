using Basis;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class StageGeneralSetting : GeneralSettingBase, IManagerCreationProvider
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "关卡" },
        { "English", "Stage" }
    };

    [LabelText("游戏道具生成")]
    public PrefabIDSetter<ItemPrefab, ItemGeneralSetting> gamePropertyItemGeneration = new();

    [LabelText("方块生成")]
    public PrefabIDSetter<ItemPrefab, ItemGeneralSetting> blockItemGeneration = new();

    #region GUI

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        gamePropertyItemGeneration ??= new();
        blockItemGeneration ??= new();
    }

    #endregion

    #region Manager Provider

    public IEnumerable<ManagerType> GetManagerTypes()
    {
        yield return ManagerType.OtherCore;
    }

    public void HandleManagerCreation(ManagerType managerType, Transform managerContainer)
    {
        if (managerType == ManagerType.OtherCore)
        {
            managerContainer.gameObject.GetOrAddComponent<StageManager>();
        }
    }

    #endregion
}
