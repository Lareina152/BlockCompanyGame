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

    public virtual bool CanPlace(Player player)
    {
        return hasPlaced == false;
    }

    public EntityController Place(Vector2 pos, bool isLeft)
    {
        if (hasPlaced)
        {
            Note.note.Error("不可重复使用物品");
        }

        var entity = Entity.Create(origin.entityId);

        OnPrePlace(entity, isLeft);

        var entityCtrl = EntityManager.Create(entity, pos);

        OnPostPlace(entity, isLeft);

        hasPlaced = true;

        return entityCtrl;
    }

    protected virtual void OnPrePlace(Entity entity, bool isLeft)
    {
        if (entity is IResettable resettable)
        {
            resettable.SetArea(isLeft);
        }
    }

    protected virtual void OnPostPlace(Entity entity, bool isLeft)
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
