using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Basis;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BlackHole : GameProperty
{
    public const string registeredID = "block_hole";

    public float gravitationalRadius => origin.gravitationalRadius;

    public float centripetalForce => origin.centripetalForce;

    public float gravitationalDirectionRandomAngleRange =>
        origin.gravitationalDirectionRandomAngleRange;

    private new BlackHolePrefab origin;

    //private ParticleSystem particleSystem;

    [ShowInInspector]
    private float lifeTime;

    private AudioSource blackHoleAudioSource;

    protected override void OnCreate()
    {
        base.OnCreate();

        origin = GetOrigin<BlackHolePrefab>();
    }

    protected override void OnInit()
    {
        base.OnInit();

        ParticleSpawner.Spawn(origin.blackHoleParticleID,
            controller.transform.position);

        blackHoleAudioSource = AudioSpawner.
            Spawn(origin.blackHoleAudioID, controller.transform.position);

        blackHoleAudioSource.loop = true;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        //ParticleSpawner.Return(particleSystem);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        controller.transform.Rotate(Vector3.forward, origin.rotationSpeed * Time.deltaTime);

        if (isDestroyed)
        {
            return;
        }

        lifeTime += Time.deltaTime;

        if (lifeTime > origin.maxLifeTime)
        {
            Destroy();

            AudioSpawner.Return(blackHoleAudioSource);

            var spriteRenderer = controller.graphicsTransform.GetComponent<SpriteRenderer>();

            spriteRenderer.DOFade(0, origin.fadeOutTime).
                OnComplete(() => EntityManager.RemoveEntity(controller));
        }
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (isDestroyed || lifeTime > origin.maxLifeTime)
        {
            return;
        }

        var entityColliders =
            Physics2D.OverlapCircleAll(controller.transform.position.XY(), 
                gravitationalRadius);

        foreach (var collider in entityColliders)
        {
            var targetEntityCtrl = collider.GetComponent<EntityController>();

            if (targetEntityCtrl == null || targetEntityCtrl == controller)
            {
                continue;
            }

            var rigidbody2D = targetEntityCtrl.GetComponent<Rigidbody2D>();

            if (rigidbody2D == null)
            {
                continue;
            }

            var dir = controller.transform.position.XY() - 
                      targetEntityCtrl.transform.position.XY();

            var distance = dir.magnitude;

            dir = dir.normalized;

            dir = dir.ClockwiseRotate((-gravitationalDirectionRandomAngleRange).
                RandomRange(gravitationalDirectionRandomAngleRange));

            var force = centripetalForce.Lerp(0, distance / gravitationalRadius);

            rigidbody2D.AddForce(dir * force);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(controller.transform.position.XY(), gravitationalRadius);
    }
}

public class BlackHolePrefab : GamePropertyPrefab
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

    [LabelText("最大生命周期")]
    [MinValue(0)]
    public float maxLifeTime = 7f;

    [LabelText("旋转速度")]
    [MinValue(0)]
    public float rotationSpeed;

    [LabelText("黑洞粒子")]
    [ValueDropdown("@GameSetting.particleSpawnerGeneralSetting.GetPrefabNameList()")]
    [StringIsNotNullOrEmpty]
    public string blackHoleParticleID;

    [LabelText("黑洞声效")]
    [ValueDropdown("@GameSetting.audioGeneralSetting.GetPrefabNameList()")]
    [StringIsNotNullOrEmpty]
    public string blackHoleAudioID;

    [LabelText("淡出时间")]
    [MinValue(0)]
    public float fadeOutTime = 1f;
}