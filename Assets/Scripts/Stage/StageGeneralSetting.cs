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

    [LabelText("游戏道具生成间隔")]
    [MinValue(0.1)]
    public float gamePropertyGenerationInterval = 5f;

    [LabelText("游戏道具生成初始剩余时间")]
    [PropertyRange(0, nameof(gamePropertyGenerationInterval))]
    public float gamePropertyGenerationInitialCooldown = 3f;

    [LabelText("方块生成")]
    public PrefabIDSetter<ItemPrefab, ItemGeneralSetting> blockItemGeneration = new();

    [LabelText("方块生成间隔")]
    [MinValue(0.1)]
    public float blockGenerationInterval = 3f;

    [LabelText("方块生成初始剩余时间")]
    [PropertyRange(0, nameof(blockGenerationInterval))]
    public float blockGenerationInitialCooldown = 1f;

    [LabelText("左玩家")]
    [ValueDropdown("@GameSetting.entityGeneralSetting.GetPrefabNameList(typeof(PlayerPrefab))")]
    public string leftPlayerID;

    [LabelText("右玩家")]
    [ValueDropdown("@GameSetting.entityGeneralSetting.GetPrefabNameList(typeof(PlayerPrefab))")]
    public string rightPlayerID;

    public LayerMask leftWallMask;

    public LayerMask rightWallMask;

    [LabelText("游戏胜利判定时间")]
    [MinValue(0)]
    public float winJudgeTime = 5f;

    [LabelText("管道吸力")]
    [MinValue(0)]
    public float tubeSuctionForce = 10f;

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
