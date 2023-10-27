using System.Collections;
using System.Collections.Generic;
using Basis;
using Basis.GameItem;
using UnityEngine;

public class ItemGeneralSetting : 
    SimpleGameItemBundle<ItemPrefab, ItemGeneralSetting, Item>.GameItemGeneralSetting
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "物品" },
        { "English", "Item" }
    };

    public override StringTranslation prefabName => new()
    {
        { "Chinese", "物品" },
        { "English", "Item" }
    };

    public override StringTranslation prefabSuffixName => "";
}
