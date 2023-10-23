using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : UniqueMonoBehaviour<CameraController>
{
    public new Camera camera => GetComponent<Camera>();

    [SerializeField]
    private float distance = 13f;

    [SerializeField]
    private Vector3 cameraRotation = Vector3.zero;

    [ShowInInspector]
    private float targetFOV = 60f;

    [SerializeField] 
    private Transform targetTransform;

    private void Start()
    {
        targetFOV = camera.fieldOfView;
    }

    private void Update()
    {
        var currentFOV = camera.fieldOfView;

        if (currentFOV.Distance(targetFOV) > 0.1f)
        {
            camera.fieldOfView = currentFOV.Lerp(targetFOV, GameSetting.cameraGeneralSetting.fovLerpSpeed);
        }

        if (targetTransform == null)
        {
            return;
        }

        var targetPosition = targetTransform.position + GetRelativeDisplacement();

        transform.position =
            transform.position.Lerp(targetPosition, GameSetting.cameraGeneralSetting.positionLerpSpeed);

        transform.eulerAngles = cameraRotation;
    }

    private Vector3 GetRelativeDisplacement()
    {
        Quaternion rotation = Quaternion.Euler(cameraRotation);

        // 计算位移向量
        Vector3 displacement = rotation * Vector3.forward * distance;

        return -displacement;
    }

    public static void SetFollowTarget(Transform targetTransform)
    {
        instance.targetTransform = targetTransform;
    }

    public static void SetFOV(float fov)
    {
        instance.targetFOV = fov;
    }
}
