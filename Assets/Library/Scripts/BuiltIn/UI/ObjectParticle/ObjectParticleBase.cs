using System;
using Basis;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UpdateType = Basis.UpdateType;

public abstract class ObjectParticleBase<TPrefab, TGeneralSetting> : MonoBehaviour
    where TPrefab : ObjectParticlePrefab<TPrefab, TGeneralSetting>
    where TGeneralSetting : ObjectParticleGeneralSetting<TPrefab, TGeneralSetting>
{
    public static Dictionary<GameObject, Dictionary<string, List<ObjectParticleBase<TPrefab, TGeneralSetting>>>> allValueHUDs = new();
    public static Dictionary<string, GameObjectLimitPool<ObjectParticleBase<TPrefab, TGeneralSetting>>> globalPools = new();
    public static Dictionary<string, List<ObjectParticleBase<TPrefab, TGeneralSetting>>> globalValueHUDs = new();

    [LabelText("容器"), BoxGroup("基本设置")]
    [ChildGameObjectsOnly]
    [SerializeField, Required]
    protected Transform container;

    [LabelText("数值显示预设"), FoldoutGroup("Only For Debugging")]
    [SerializeField]
    protected ObjectParticlePrefab<TPrefab, TGeneralSetting> objectParticlePrefab;

    [LabelText("绑定对象"), FoldoutGroup("Only For Debugging")]
    [SerializeField]
    protected GameObject bindTarget;

    [LabelText("跟踪偏移"), FoldoutGroup("Only For Debugging")]
    [SerializeField, DisplayAsString]
    protected Vector2 tracingOffset = Vector2.zero;

    [LabelText("跟踪状态"), FoldoutGroup("Only For Debugging")]
    [SerializeField, DisplayAsString]
    protected bool enableTracing = false;

    [LabelText("速度"), FoldoutGroup("Only For Debugging")]
    [SerializeField, DisplayAsString]
    protected Vector3 speed = default;

    private void FixedUpdate()
    {
        if (objectParticlePrefab.updateType == UpdateType.FixedUpdate)
        {
            Trace();
        }
    }

    private void Update()
    {
        if (objectParticlePrefab.updateType == UpdateType.Update)
        {
            Trace();
        }
    }

    private void LateUpdate()
    {
        if (objectParticlePrefab.updateType == UpdateType.LateUpdate)
        {
            Trace();
        }
    }

    private void OnGUI()
    {
        if (objectParticlePrefab.updateType == UpdateType.OnGUI)
        {
            Trace();
        }
    }

    protected void Trace()
    {
        if (enableTracing && objectParticlePrefab.continuousTracing == true && bindTarget != null)
        {
            if (objectParticlePrefab.tracingLerp)
            {
                transform.LerpTo(GetBindTargetPosition(),
                    objectParticlePrefab.lerpSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = GetBindTargetPosition();
            }

        }
    }

    protected virtual Vector3 GetBindTargetPosition()
    {
        return bindTarget.transform.position;
    }

    public void Stop()
    {
        objectParticlePrefab.containerAnimation.Kill(container);

        OnReturn();
    }

    protected virtual void OnCreate()
    {
        enableTracing = true;

        objectParticlePrefab.containerAnimation.Run(container);

        if (objectParticlePrefab.autoHide)
        {
            objectParticlePrefab.containerAnimation.totalDuration.DelayAction(OnReturn);
        }
    }

    protected virtual void OnReturn()
    {
        enableTracing = false;

        allValueHUDs[bindTarget][objectParticlePrefab.id].Remove(this);

        globalValueHUDs[objectParticlePrefab.id].Remove(this);

        globalPools[objectParticlePrefab.id].Return(this);
    }

    public void HideAllWithoutAnimation(Action onHide = default)
    {
        container.gameObject.SetActive(false);
        onHide?.Invoke();
    }

    protected static ObjectParticleBase<TPrefab, TGeneralSetting> Create(ObjectParticlePrefab<TPrefab, TGeneralSetting> prefab, 
        GameObject target, Vector2 tracingOffset = default)
    {
        var id = prefab.id;

        if (target == null)
        {
            Note.note.Warning($"试图绑定的目标为null");
            return null;
        }

        if (!allValueHUDs.ContainsKey(target))
        {
            allValueHUDs[target] = new();
        }

        if (!allValueHUDs[target].ContainsKey(id))
        {
            allValueHUDs[target][id] = new();
        }

        if (prefab.limitAmountOnSameTarget == true)
        {
            if (allValueHUDs[target][id].Count >= prefab.sameTargetLimitedAmount)
            {
                if (prefab.replaceMode == true)
                {
                    allValueHUDs[target][id].Choose().Stop();
                }
                else
                {
                    Note.note.Warning($"在生成ValueHUD:{prefab.id}时失败，因为超出对同一个对象追踪的最大数量");
                    return null;
                }
                
            }
        }

        if (!globalPools.ContainsKey(id))
        {
            globalPools[id] = new(prefab.globalPoolCapacity);
        }

        if (!globalValueHUDs.ContainsKey(id))
        {
            globalValueHUDs[id] = new();
        }

        if (globalValueHUDs[id].Count >= prefab.globalLimitedAmount)
        {
            if (prefab.replaceMode == true)
            {
                globalValueHUDs[id].Choose().Stop();
            }
            else
            {
                Note.note.Warning($"在生成ValueHUD:{prefab.id}时失败，因为超出全局最大数量");
                return null;
            }
        }

        var newObjectParticle = globalPools[id].Get();

        if (newObjectParticle == null)
        {
            newObjectParticle = Instantiate(prefab.prefabObject.gameObject).
                GetComponent<ObjectParticleBase<TPrefab, TGeneralSetting>>();
        }
        

        allValueHUDs[target][id].Add(newObjectParticle);

        globalValueHUDs[id].Add(newObjectParticle);

        newObjectParticle.objectParticlePrefab = prefab;

        newObjectParticle.gameObject.SetActive(true);

        newObjectParticle.bindTarget = target;

        newObjectParticle.tracingOffset = tracingOffset;

        newObjectParticle.transform.position = target.GetRealPositionOnScreen(tracingOffset) + prefab.extraShowOffset.GetValue();

        newObjectParticle.container.localPosition = default;

        newObjectParticle.OnCreate();

        return newObjectParticle;
    }
}
