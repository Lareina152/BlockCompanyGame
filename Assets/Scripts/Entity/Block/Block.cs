using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Block : Entity, IResettable
{
    bool IResettable.isLeft { get; set; }

    bool IResettable.areaInitialized { get; set; }

    public bool isLeft => (this as IResettable).isLeft;

    protected override void OnInit()
    {
        base.OnInit();

        Debug.Log($"Fore:{isLeft}");

        var rigidbody = controller.GetComponent<Rigidbody2D>();

        rigidbody.excludeLayers = isLeft ?
            GameSetting.stageGeneralSetting.leftWallMask :
            GameSetting.stageGeneralSetting.rightWallMask;

        Debug.Log($"Post:{isLeft},{rigidbody.excludeLayers.value}");
    }
}

public class BlockPrefab : EntityPrefab
{
    protected static Type bindInstanceType = typeof(Block);
}
