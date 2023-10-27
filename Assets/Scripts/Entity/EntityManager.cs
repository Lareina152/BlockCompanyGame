using System.Collections;
using System.Collections.Generic;
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
        return EntityController.Create(id, pos);
    }

    [Button("移除实体")]
    public static EntityController RemoveEntity(EntityController controller)
    {
        EntityController.RemoveEntity(controller);

        return controller;
    }

    [Button("创建多个实体")]
    private static void CreateSeveralEntities(
        [ValueDropdown("@GameSetting.entityGeneralSetting.GetPrefabNameList()")]
        string id, Vector2 pos, int count = 7)
    {
        count.DoActionNTimes(() => Create(id, pos));
    }
}
