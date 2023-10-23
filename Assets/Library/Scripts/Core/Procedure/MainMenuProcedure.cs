using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuProcedure : Procedure
{
    public const string registeredID = "main_menu_procedure";

    public override bool autoAddToPrefabList => true;

    public override void OnEnter()
    {
        base.OnEnter();

    }

    public override void OnLeave()
    {
        base.OnLeave();


    }

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        name = new()
        {
            { "Chinese", "主界面流程" },
            { "English", "Main Menu Procedure" }
        };
    }
}
