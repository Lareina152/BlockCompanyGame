using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [Button]
    public static EntityController Create(
        [ValueDropdown("@GameSetting.entityGeneralSetting.GetPrefabNameList()")]
        string id, Vector2 pos)
    {
        return EntityController.Create(id, pos);
    }
}
