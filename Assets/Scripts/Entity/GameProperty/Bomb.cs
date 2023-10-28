using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Bomb : Entity
{
    public const string registeredID = "bomb";

    protected new BombPrefab origin;

    protected override void OnCreate()
    {
        base.OnCreate();

        origin = GetOrigin<BombPrefab>();
    }

    public float cooldown => origin.cooldown;

    public float explosionDestroyRadius => origin.explosionDestroyRadius;

    public float explosionShockRadius => origin.explosionShockRadius;

    public float shockStrength => origin.shockStrength;

    public float explosionAnimationTime => origin.explosionAnimationTime;
}

public class BombPrefab : EntityPrefab
{
    public const string registeredID = Bomb.registeredID;

    public override bool autoAddToPrefabList => true;

    [LabelText("倒计时时间"), SuffixLabel("秒")]
    [MinValue(0)]
    public float cooldown = 3f;

    [LabelText("爆炸破坏半径")]
    [MinValue(0)]
    public float explosionDestroyRadius = 1f;

    [LabelText("爆炸冲击半径")]
    [MinValue(0)]
    public float explosionShockRadius = 3f;

    [LabelText("爆炸冲击强度")]
    [MinValue(0)]
    public float shockStrength = 10f;

    [LabelText("爆炸动画时间")]
    [MinValue(0)]
    public float explosionAnimationTime = 0.3f;
}
