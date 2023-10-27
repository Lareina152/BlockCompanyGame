using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BlockItem : Item
{
    
}

public class BlockItemPrefab : ItemPrefab
{
    protected static Type bindInstanceType = typeof(BlockItem);

    [LabelText("方块实体")]
    [ValueDropdown(
        "@GameSetting.entityGeneralSetting.GetPrefabNameList(typeof(BlockPrefab))")]
    [StringIsNotNullOrEmpty]
    public string blockEntityId;
}
