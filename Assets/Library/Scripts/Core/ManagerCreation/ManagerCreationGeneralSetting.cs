using System;
using Basis;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ManagerType
{
    SettingCore,
    ProcedureCore,
    EventCore,
    UICore,
    NetworkCore,
    TestingCore,
    EnvironmentCore,
    OtherCore
}

public interface IManagerCreationProvider
{
    public IEnumerable<ManagerType> GetManagerTypes();

    public void HandleManagerCreation(ManagerType managerType, Transform managerContainer);
}

public class ManagerCreationGeneralSetting : GeneralSettingBase, IManagerCreationProvider
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "管理器创建" },
        { "English", "Manager Creation" }
    };

    public override string fullSettingName => settingName;

    public override bool hasIcon => true;

    public override SdfIconType icon => SdfIconType.Collection;

    public override StringTranslation folderPath => coreCategory;

    [LabelText("管理器总容器")]
    public ContainerChooser managerContainer = new();

    [LabelText("管理器类别子容器")]
    [DictionaryDrawerSettings(KeyLabel = "管理器类别", ValueLabel = "子容器")]
    public Dictionary<ManagerType, ContainerChooser> managerTypeContainers = new();

    protected override void Awake()
    {
        base.Awake();

        CreateManager();
    }

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        CreateManager();
    }

    protected override void OnPreInit()
    {
        base.OnPreInit();

        CreateManager();
    }

    [Button("创建管理器")]
    public void CreateManager()
    {
        managerContainer ??= new();

        managerContainer.SetDefaultContainerID("^Core");

        managerContainer.GetContainer().SetAsFirstSibling();

        foreach (ManagerType managerType in Enum.GetValues(typeof(ManagerType)))
        {
            managerTypeContainers.TryAdd(managerType, new ContainerChooser());

            managerTypeContainers[managerType].SetDefaultContainerID(managerType.ToString());

            managerTypeContainers[managerType].parentContainer = managerContainer;
        }

        var eventCoreContainer = managerTypeContainers[ManagerType.EventCore].GetContainer();

        eventCoreContainer.GetOrAddComponent<EventSystem>();
        eventCoreContainer.GetOrAddComponent<StandaloneInputModule>();

        var managerCreationList = new List<IManagerCreationProvider>();

        if (GameCoreSettingBase.gameCoreSettingsFileBase != null)
        {
            if (GameCoreSettingBase.gameCoreSettingsFileBase is IManagerCreationProvider managerCreation)
            {
                managerCreationList.Add(managerCreation);
            }
        }

        foreach (var generalSetting in GameCoreSettingBase.GetAllGeneralSettings())
        {
            if (generalSetting is IManagerCreationProvider managerCreation)
            {
                managerCreationList.Add(managerCreation);
            }
        }

        foreach (var managerCreation in managerCreationList)
        {
            foreach (var managerType in managerCreation.GetManagerTypes())
            {
                var container = managerTypeContainers[managerType].GetContainer();

                managerCreation.HandleManagerCreation(managerType, container);
            }
        }
    }

    public IEnumerable<ManagerType> GetManagerTypes()
    {
        yield return ManagerType.TestingCore;
    }

    public void HandleManagerCreation(ManagerType managerType, Transform managerContainer)
    {
        managerContainer.GetOrAddComponent<TempTestBench>();
    }
}
