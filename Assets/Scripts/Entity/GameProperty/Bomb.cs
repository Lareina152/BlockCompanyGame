using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Bomb : GameProperty
{
    public const string registeredID = "bomb";

    private static readonly int IsBooming = Animator.StringToHash("isBooming");

    protected new BombPrefab origin;

    public float explosionDestroyRadius => origin.explosionDestroyRadius;

    public float explosionShockRadius => origin.explosionShockRadius;

    public float shockStrength => origin.shockStrength;

    public float explosionAnimationTime => origin.explosionAnimationTime;

    [ShowInInspector]
    private bool hasSetExplodedAnimation = false;

    [ShowInInspector]
    private float lifeTime;

    [ShowInInspector]
    private float fuseParticleCooldown;

    private AudioSource fuseAudioSource;

    protected override void OnCreate()
    {
        base.OnCreate();

        origin = GetOrigin<BombPrefab>();

        fuseParticleCooldown = origin.bombFuseParticleSpawnInterval;
    }

    protected override void OnInit()
    {
        base.OnInit();

        fuseAudioSource = AudioSpawner.
            Spawn(origin.bombFuseAudioID, controller.transform.position);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        lifeTime += Time.deltaTime;

        if (hasSetExplodedAnimation == false && 
            origin.maxLifeTime - lifeTime < origin.explosionAnimationTime)
        {
            controller.animator.SetTrigger(IsBooming);

            hasSetExplodedAnimation = true;

            AudioSpawner.Return(fuseAudioSource);
        }

        if (hasSetExplodedAnimation == false)
        {
            fuseParticleCooldown -= Time.deltaTime;

            if (fuseParticleCooldown <= 0)
            {
                fuseParticleCooldown = origin.bombFuseParticleSpawnInterval;

                ParticleSpawner.Spawn(origin.bombFuseParticleID,
                    controller.transform.position.AddY(origin.bombFuseParticleSpawnYOffset));
            }
        }

        if (lifeTime >= origin.maxLifeTime)
        {
            var brokenObjectColliders = Physics2D.
                OverlapCircleAll(controller.transform.position.XY(), explosionDestroyRadius);

            foreach (var collider in brokenObjectColliders)
            {
                var entityCtrl = collider.GetComponent<EntityController>();

                if (entityCtrl != null && entityCtrl != controller && entityCtrl.entity is Block)
                {
                    EntityManager.RemoveEntity(entityCtrl);
                }
            }

            var shockObjectColliders = Physics2D.
                OverlapCircleAll(controller.transform.position.XY(), explosionShockRadius);

            foreach (var collider in shockObjectColliders)
            {
                var entityCtrl = collider.GetComponent<EntityController>();

                if (entityCtrl != null)
                {
                    var rigidbody2D = entityCtrl.GetComponent<Rigidbody2D>();

                    var dir = entityCtrl.transform.position.XY() - 
                              controller.transform.position.XY();

                    dir = dir.normalized;

                    rigidbody2D.AddForce(dir * shockStrength);
                }
            }

            foreach (var explosionParticleID in origin.explosionParticleIDs)
            {
                ParticleSpawner.Spawn(explosionParticleID, controller.transform.position);
            }

            foreach (var explosionAudioID in origin.explosionAudioIDs)
            {
                AudioSpawner.Spawn(explosionAudioID, controller.transform.position);
            }

            EntityManager.RemoveEntity(controller);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controller.transform.position, explosionDestroyRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(controller.transform.position, explosionShockRadius);
    }
}

public class BombPrefab : GamePropertyPrefab
{
    public const string registeredID = Bomb.registeredID;

    public override bool autoAddToPrefabList => true;

    [LabelText("倒计时时间"), SuffixLabel("秒")]
    [MinValue(0)]
    public float maxLifeTime = 3f;

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

    [LabelText("爆炸粒子特效")]
    [ValueDropdown("@GameSetting.particleSpawnerGeneralSetting.GetPrefabNameList()")]
    public List<string> explosionParticleIDs;

    [LabelText("炸弹引线粒子")]
    [ValueDropdown("@GameSetting.particleSpawnerGeneralSetting.GetPrefabNameList()")]
    public string bombFuseParticleID;

    [LabelText("炸弹引线粒子生成间隔")]
    [PropertyRange(0, nameof(maxLifeTime))]
    public float bombFuseParticleSpawnInterval = 0.8f;

    [LabelText("炸弹引线粒子生成Y轴偏移")]
    public float bombFuseParticleSpawnYOffset = 0.5f;

    [LabelText("爆炸声效")]
    [ValueDropdown("@GameSetting.audioGeneralSetting.GetPrefabNameList()")]
    public List<string> explosionAudioIDs;

    [LabelText("引线声效")]
    [ValueDropdown("@GameSetting.audioGeneralSetting.GetPrefabNameList()")]
    public string bombFuseAudioID;

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();
        
        explosionParticleIDs ??= new();

        explosionAudioIDs ??= new();
    }
}
