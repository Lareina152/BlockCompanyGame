using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class Glue : GameProperty
{
    public const string registeredID = "glue";

    private new GluePrefab origin;

    private float lifeTime;

    protected override void OnCreate()
    {
        base.OnCreate();

        origin = GetOrigin<GluePrefab>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        lifeTime += Time.deltaTime;

        if (lifeTime > origin.maxLifeTime)
        {
            var targets = Physics2D.OverlapCircleAll(controller.transform.position.XY(),
                origin.viscosityRadius);

            foreach (var collider in targets)
            {
                var targetEntityCtrl = collider.GetComponent<EntityController>();

                if (targetEntityCtrl == null || targetEntityCtrl == controller)
                {
                    continue;
                }

                if (targetEntityCtrl.entity is Block)
                {
                    var rigidbody = targetEntityCtrl.GetComponent<Rigidbody2D>();

                    rigidbody.bodyType = RigidbodyType2D.Static;
                }
            }

            EntityManager.RemoveEntity(controller);

            AudioSpawner.Spawn(origin.glueAudioID, controller.transform.position);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(controller.transform.position, origin.viscosityRadius);
    }
}

public class GluePrefab : GamePropertyPrefab
{
    public const string registeredID = Glue.registeredID;

    public override bool autoAddToPrefabList => true;

    [LabelText("粘性半径")]
    [MinValue(0)]
    public float viscosityRadius = 1f;

    [LabelText("最大生命周期")]
    [MinValue(0)]
    public float maxLifeTime = 1f;

    [LabelText("胶水声效")]
    [ValueDropdown("@GameSetting.audioGeneralSetting.GetPrefabNameList()")]
    [StringIsNotNullOrEmpty]
    public string glueAudioID;
}
