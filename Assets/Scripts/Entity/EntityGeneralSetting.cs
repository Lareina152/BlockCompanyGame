using System.Collections;
using System.Collections.Generic;
using Basis;
using Basis.GameItem;
using UnityEngine;

public class EntityGeneralSetting :
    SimpleGameItemBundle<EntityPrefab, EntityGeneralSetting, Entity>.
    GameItemGeneralSetting, IManagerCreationProvider
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "实体" },
        { "English", "Entity" }
    };

    public override StringTranslation prefabName => new()
    {
        { "Chinese", "实体" },
        { "English", "Entity" }
    };

    public override StringTranslation prefabSuffixName => "";

    public override bool gameTypeAbandoned => true;


    public IEnumerable<ManagerType> GetManagerTypes()
    {
        yield return ManagerType.OtherCore;
    }

    public void HandleManagerCreation(ManagerType managerType, Transform managerContainer)
    {
        if (managerType == ManagerType.OtherCore)
        {
            managerContainer.GetOrAddComponent<EntityManager>();
        }
    }
}
