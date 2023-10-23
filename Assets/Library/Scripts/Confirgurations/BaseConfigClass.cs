using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;

[OnInspectorInit(@"@$value?.OnInspectorInit()")]
[HideDuplicateReferenceBox]
[HideReferenceObjectPicker]
[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public abstract class BaseConfigClass
{
    protected const string OPEN_SCRIPT_BUTTON_HORIZONTAL_GROUP = "OpenScriptButtonHorizontalGroup";

    public bool initDone { get; private set; } = false;

    protected virtual void OnInspectorInit()
    {

    }

    public virtual void CheckSettings()
    {

    }

    public void Init()
    {
        Note.note.Begin($"加载{this}");
        OnInit();
        Note.note.End();

        initDone = true;
    }

    protected virtual void OnInit()
    {

    }
}
