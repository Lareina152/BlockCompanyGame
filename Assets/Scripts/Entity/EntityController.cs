using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class EntityController : MonoBehaviour
{
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

    public static void RemoveEntity(EntityController controller)
    {
        controller.entity.Destroy();

        Destroy(controller.gameObject);
    }

    public static EntityController Create(string id, Vector2 pos)
    {
        var gameProperty = Entity.Create(id);

        var instanceObject = Instantiate(gameProperty.prefab);

        var controller = instanceObject.GetOrAddComponent<EntityController>();

        controller.name = gameProperty.name;

        controller.entity = gameProperty;

        controller.transform.position = pos;

        controller.Init();

        return controller;
    }
}
