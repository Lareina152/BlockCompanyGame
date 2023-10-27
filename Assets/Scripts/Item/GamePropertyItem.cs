using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePropertyItem : Item
{
    
}

public class GamePropertyItemPrefab : ItemPrefab
{
    protected static Type bindInstanceType = typeof(GamePropertyItem);

    [LabelText("游戏道具实体")]
    [ValueDropdown(
        "@GameSetting.entityGeneralSetting.GetPrefabNameList(typeof(GamePropertyPrefab))")]
    [StringIsNotNullOrEmpty]
    public string gamePropertyEntityId;
}
