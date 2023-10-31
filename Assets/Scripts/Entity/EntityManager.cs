using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [Button("创建实体")]
    public static EntityController Create(
        [ValueDropdown("@GameSetting.entityGeneralSetting.GetPrefabNameList()")]
        string id, Vector2 pos)
    {
        return EntityController.Create(Entity.Create(id), pos);
    }

    public static EntityController Create(Entity entity, Vector2 pos)
    {
        return EntityController.Create(entity, pos);
    }

    [Button("移除实体")]
    public static void RemoveEntity(EntityController controller)
    {
        EntityController.RemoveEntity(controller);
    }

    [Button("创建多个实体")]
    private static void CreateSeveralEntities(
        [ValueDropdown("@GameSetting.entityGeneralSetting.GetPrefabNameList()")]
        string id, Vector2 pos, int count = 7)
    {
        count.DoActionNTimes(() => Create(id, pos));
    }

    [Button("创建随机方块")]
    private static void CreateRandomBlocks(Vector2 pos, int count = 10)
    {
        count.DoActionNTimes(() =>
        {
            var block = Block.CreateRandom<Block>();

            Create(block, pos);
        });
    }

    public static EntityController CreateItemDrop(Item item, Vector2 pos)
    {
        var itemDrop = ItemDrop.Create<ItemDrop>(ItemDrop.registeredID);

        itemDrop.InitItemDrop(item);

        return Create(itemDrop, pos);
    }

    [Button("创建掉落物")]
    private static void CreateItemDrop(
        [ValueDropdown("@GameSetting.itemGeneralSetting.GetPrefabNameList()")]
        string itemID, Vector2 pos)
    {
        var item = Item.Create(itemID);

        CreateItemDrop(item, pos);
    }

    [Button("移除所有实体")]
    public static void RemoveAllEntities()
    {
        var allEntityCtrls = EntityController.GetAllEntityControllers().ToList();

        foreach (var controller in allEntityCtrls)
        {
            RemoveEntity(controller);
        }
    }

    public static void RemoveEntitiesWhere(Func<EntityController, bool> predicate)
    {
        var targetEntityCtrls = 
            EntityController.GetAllEntityControllers().Where(predicate).ToList();

        foreach (var controller in targetEntityCtrls)
        {
            RemoveEntity(controller);
        }
    }

    [Button("移除所有方块")]
    public static void RemoveAllBlocks()
    {
        RemoveEntitiesWhere(controller => controller.entity is Block);
    }

    [Button("移除所有游戏道具")]
    public static void RemoveAllGameProperties()
    {
        RemoveEntitiesWhere(controller => controller.entity is GameProperty);
    }

    [Button("移除所有掉落物")]
    public static void RemoveAllItemDrops()
    {
        RemoveEntitiesWhere(controller => controller.entity is ItemDrop);
    }
}
