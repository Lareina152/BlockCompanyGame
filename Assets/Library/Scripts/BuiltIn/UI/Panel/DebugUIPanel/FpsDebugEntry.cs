using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;

public class FpsDebugEntry : DebugEntry
{
    public const string registeredID = "fps";

    public override bool autoAddToPrefabList => true;

    public override void OnTypeChanged()
    {
        base.OnTypeChanged();

        name = "FPS";
    }

    public override string OnHandleContentUpdate()
    {
        return (1 / Time.unscaledDeltaTime).ToString(0);
    }
}
