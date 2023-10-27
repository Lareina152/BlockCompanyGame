using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BlackHole : Entity
{
    public const string registeredID = "block_hole";

    protected new BlackHolePrefab origin;

    protected override void OnCreate()
    {
        base.OnCreate();

        origin = GetOrigin<BlackHolePrefab>();
    }

    public float gravitationalRadius => origin.gravitationalRadius;

    public float centripetalForce => origin.centripetalForce;

    public float gravitationalDirectionRandomAngleRange => 
        origin.gravitationalDirectionRandomAngleRange;
}

public class BlackHolePrefab : EntityPrefab
{
    public const string registeredID = BlackHole.registeredID;

    public override bool autoAddToPrefabList => true;

    [LabelText("引力半径")]
    [MinValue(0)]
    public float gravitationalRadius = 5f;

    [LabelText("最大向心力")]
    [MinValue(0)]
    public float centripetalForce = 50f;

    [LabelText("引力方向随机角度范围")]
    [PropertyRange(0, 90)]
    public float gravitationalDirectionRandomAngleRange = 0f;
}