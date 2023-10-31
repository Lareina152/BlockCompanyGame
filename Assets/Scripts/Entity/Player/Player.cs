using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Player : Entity, IResettable
{
    private new PlayerPrefab origin;

    public string horizontalMovementInputMappingID => origin.horizontalMovementInputMappingID;

    public string jumpInputMappingID => origin.jumpInputMappingID;

    public string placeInputMappingID => origin.placeInputMappingID;

    public float placeItemYOffset => origin.placeItemYOffset;

    public Item item { get; private set; }

    public event Action<Item> OnItemChangedEvent;

    protected override void OnCreate()
    {
        base.OnCreate();

        origin = GetOrigin<PlayerPrefab>();

        ((IResettable)this).SetArea(origin.isLeft);
    }

    protected override void OnInit()
    {
        base.OnInit();

        GlobalEventManager.AddEvent(origin.placeInputMappingID, PlaceItem);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        GlobalEventManager.RemoveEvent(origin.placeInputMappingID, PlaceItem);
    }

    public void PlaceItem()
    {
        if (item == null)
        {
            CheckPickup();
            return;
        }

        if (isDebugging)
        {
            Note.note.Warning($"{this}放置物品:{item}");
        }

        if (item.CanPlace(this))
        {
            var targetPos = controller.transform.position.XY();

            targetPos = targetPos.AddY(origin.placeItemYOffset);

            item.Place(targetPos, origin.isLeft);

            ParticleSpawner.Spawn(origin.particleIDWhenPlacing, targetPos);

            AudioSpawner.Spawn(origin.audioIDWhenPlacing, targetPos);

            item = null;

            OnItemChangedEvent?.Invoke(null);
        }
    }

    public bool HasItem()
    {
        return item != null;
    }

    public void GiveItem(Item item)
    {
        if (this.item != null)
        {
            Note.note.Error("玩家已经有了物品");
            return;
        }

        this.item = item;
        
        OnItemChangedEvent?.Invoke(item);
    }

    private void CheckPickup()
    {
        var colliders = Physics2D.OverlapCircleAll(
            controller.transform.position + new Vector3(0, origin.pickupPositionYOffset, 0),
            origin.pickupRadius);

        foreach (var collider in colliders)
        {
            var entityCtrl = collider.GetComponent<EntityController>();

            if (entityCtrl != null && entityCtrl.entity is ItemDrop itemDrop)
            {
                GiveItem(itemDrop.item);

                EntityManager.RemoveEntity(entityCtrl);

                return;
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            controller.transform.position + new Vector3(0, origin.pickupPositionYOffset, 0),
            origin.pickupRadius);
    }

    bool IResettable.isLeft { get; set; }

    bool IResettable.areaInitialized { get; set; }
}

public class PlayerPrefab : EntityPrefab
{
    protected static Type bindInstanceType = typeof(Player);

    public override bool autoAddToPrefabList => true;

    [LabelText("哪边的玩家")]
    [ToggleButtons("左边玩家", "右边玩家")]
    public bool isLeft = false;

    [LabelText("放置物品Y偏移")]
    public float placeItemYOffset;

    [LabelText("水平移动输入映射")]
    [ValueDropdown("@GameSetting.globalEventSystemGeneralSetting." +
                   "GetPrefabNameList(typeof(InputEventConfigOfFloatArg))")]
    [StringIsNotNullOrEmpty]
    public string horizontalMovementInputMappingID;

    [LabelText("跳跃输入映射")]
    [ValueDropdown("@GameSetting.globalEventSystemGeneralSetting." +
                   "GetPrefabNameList(typeof(InputEventConfigOfBoolArg))")]
    [StringIsNotNullOrEmpty]
    public string jumpInputMappingID;

    [LabelText("放置输入映射")]
    [ValueDropdown("@GameSetting.globalEventSystemGeneralSetting." +
                   "GetPrefabNameList(typeof(InputEventConfigOfBoolArg))")]
    [StringIsNotNullOrEmpty]
    public string placeInputMappingID;

    [LabelText("放置时的粒子特效")]
    [ValueDropdown("@GameSetting.particleSpawnerGeneralSetting.GetPrefabNameList()")]
    [StringIsNotNullOrEmpty]
    public string particleIDWhenPlacing;

    [LabelText("放置时的声效")]
    [ValueDropdown("@GameSetting.audioGeneralSetting.GetPrefabNameList()")]
    [StringIsNotNullOrEmpty]
    public string audioIDWhenPlacing;

    [LabelText("捡起物品的检测位置Y偏移")]
    public float pickupPositionYOffset;

    [LabelText("捡起物品的检测半径")]
    [MinValue(0)]
    public float pickupRadius;
}
