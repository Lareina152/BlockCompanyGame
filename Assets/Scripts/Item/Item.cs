using Basis.GameItem;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class Item :
    SimpleGameItemBundle<ItemPrefab, ItemGeneralSetting, Item>.GameItem
{
    public Sprite iconSprite => origin.icon;

    public Texture2D iconTexture2D => origin.icon.texture;

    protected bool hasPlaced = false;

    protected override void OnCreate()
    {
        base.OnCreate();

        hasPlaced = false;
    }

    public EntityController Place(Vector2 pos)
    {
        if (hasPlaced)
        {
            Note.note.Error("不可重复使用物品");
        }

        var entityCtrl = EntityManager.Create(origin.entityId, pos);

        OnPlace();

        hasPlaced = true;

        return entityCtrl;
    }

    protected virtual void OnPlace()
    {

    }
}

public class ItemPrefab :
    SimpleGameItemBundle<ItemPrefab, ItemGeneralSetting, Item>.GameItemPrefab
{
    [LabelText("图标")]
    [Required]
    public Sprite icon;

    [LabelText("使用后生成的实体")]
    [ValueDropdown(
        "@GameSetting.entityGeneralSetting.GetPrefabNameList()")]
    [StringIsNotNullOrEmpty]
    public string entityId;
}
