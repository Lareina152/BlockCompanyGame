using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializationProcedure : LoadingProcedure
{
    public const string registeredID = "game_initialization_procedure";

    public override bool autoAddToPrefabList => true;

    public override void OnEnter()
    {
        base.OnEnter();

        EnterNextProcedure();
    }

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        name = new()
        {
            { "Chinese", "游戏初始化流程" },
            { "English", "Game Initialization Procedure" }
        };
    }
}
