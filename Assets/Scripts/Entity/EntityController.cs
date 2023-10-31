using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    [Required]
    public Transform graphicsTransform;

    public Animator animator;

    [ShowInInspector]
    public Entity entity { get; private set; }

    public bool initDone { get; private set; }

    private void Init()
    {
        entity.InitEntity(this);
        OnInit();

        initDone = true;
    }

    protected virtual void OnInit()
    {

    }

    protected virtual void Update()
    {
        if (initDone)
        {
            entity.Update();
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (initDone)
        {
            entity.DrawGizmos();
        }
    }

    protected void FixedUpdate()
    {
        if (initDone)
        {
            entity.FixedUpdate();
        }
    }

    #region Manager

    [ShowInInspector]
    private static HashSet<EntityController> allEntityCtrls = new();

    public static void RemoveEntity(EntityController controller)
    {
        if (allEntityCtrls.Remove(controller) == false)
        {
            Note.note.Warning($"未知错误：移除未记录的实体: {controller},{controller.entity}");
        }

        controller.entity.Destroy();

        Destroy(controller.gameObject);
    }

    public static EntityController Create(Entity entity, Vector2 pos)
    {
        var instanceObject = Instantiate(entity.prefab);

        var controller = instanceObject.GetOrAddComponent<EntityController>();

        controller.name = entity.name;

        controller.entity = entity;

        controller.transform.position = pos;

        controller.Init();

        allEntityCtrls.Add(controller);

        return controller;
    }

    public static IReadOnlyCollection<EntityController> GetAllEntityControllers()
    {
        return allEntityCtrls;
    }

    #endregion
}
