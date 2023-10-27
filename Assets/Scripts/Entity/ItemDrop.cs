using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;

public class ItemDrop : Entity
{
    public const string registeredID = "item_drop";

    public Item item { get; private set; }

    public void InitItemDrop(Item item)
    {
        if (item == null)
        {
            this.item = item;
        }
        else
        {
            Note.note.Warning("不可重复设置掉落物的物品");
        }
    }
}

public class ItemDropPrefab : EntityPrefab
{
    public const string registeredID = ItemDrop.registeredID;

    public override bool autoAddToPrefabList => true;
}
