using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;

public class ItemDrop : Entity, IResettable
{
    public const string registeredID = "item_drop";

    public Item item { get; private set; }

    protected override void OnInit()
    {
        base.OnInit();

        var spriteRender = controller.graphicsTransform.GetComponent<SpriteRenderer>();

        spriteRender.sprite = item.iconSprite;
    }

    public void InitItemDrop(Item item)
    {
        if (this.item == null)
        {
            this.item = item;
        }
        else
        {
            Note.note.Warning("不可重复设置掉落物的物品");
        }
    }

    bool IResettable.isLeft { get; set; }

    bool IResettable.areaInitialized { get; set; }
}

public class ItemDropPrefab : EntityPrefab
{
    public const string registeredID = ItemDrop.registeredID;

    public override bool autoAddToPrefabList => true;
}
