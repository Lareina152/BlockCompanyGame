using Basis.GameItem;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class Item :
    SimpleGameItemBundle<ItemPrefab, ItemGeneralSetting, Item>.GameItem
{
    public Sprite icon => origin.icon;

    protected bool isUsed = false;

    protected override void OnCreate()
    {
        base.OnCreate();

        isUsed = false;
    }

    public void Use()
    {
        if (isUsed)
        {
            Note.note.Error("不可重复使用物品");
        }

        OnUse();

        isUsed = true;
    }

    protected virtual void OnUse()
    {

    }
}

public class ItemPrefab :
    SimpleGameItemBundle<ItemPrefab, ItemGeneralSetting, Item>.GameItemPrefab
{
    [LabelText("图标")]
    [Required]
    public Sprite icon;
}
