using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class IDBasedConfigContainer<TConfig> : BaseConfigClass 
    where TConfig : IDBasedConfig
{
    [LabelText("设置")]
    [ListDrawerSettings(ShowFoldout = false)]
    [SerializeField]
    private List<TConfig> configs = new();

    [ShowInInspector]
    [HideInEditorMode]
    private Dictionary<string, TConfig> configsRuntime = new();

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        configs ??= new();
    }

    public override void CheckSettings()
    {
        base.CheckSettings();

        configs.ForEach(config => config.CheckSettings());
    }

    protected override void OnInit()
    {
        base.OnInit();

        configs.ForEach(config => config.Init());

        foreach (var config in configs)
        {
            var id = config.GetID();

            if (configsRuntime.TryAdd(id, config) == false)
            {
                Note.note.Warning($"{id}重复");
            }
        }
    }

    public IEnumerable<TConfig> GetAllConfigs()
    {
        if (initDone)
        {
            return configsRuntime.Values;
        }

        return configs;
    }

    public TConfig GetConfig(string id)
    {
        if (initDone)
        {
            if (configsRuntime.TryGetValue(id, out var config))
            {
                return config;
            }

            return null;
        }

        foreach (var config in configs)
        {
            if (config.GetID() == id)
            {
                return config;
            }
        }

        return null;
    }
}

public abstract class IDBasedConfig : BaseConfigClass
{
    public abstract string GetID();
}
