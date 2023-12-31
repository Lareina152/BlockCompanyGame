﻿using Basis.GameItem;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class Entity : 
    SimpleGameItemBundle<EntityPrefab, EntityGeneralSetting, Entity>.
    GameItem
{
    public GameObject prefab => origin.prefab;

    public EntityController controller { get; private set; }

    public bool isDestroyed { get; private set; }

    public bool initDone {get; private set; }

    public void InitEntity(EntityController controller)
    {
        this.controller = controller;
        
        Note.note.AssertIsNotNull(controller, nameof(controller));

        OnInit();

        initDone = true;
    }

    protected virtual void OnInit()
    {

    }

    public void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {

    }

    public void FixedUpdate()
    {
        OnFixedUpdate();
    }

    protected virtual void OnFixedUpdate()
    {

    }

    public void Destroy()
    {
        OnDestroy();

        isDestroyed = true;
    }

    protected virtual void OnDestroy()
    {

    }

    public void DrawGizmos()
    {
        OnDrawGizmos();
    }

    protected virtual void OnDrawGizmos()
    {

    }
}

public class EntityPrefab :
    SimpleGameItemBundle<EntityPrefab, EntityGeneralSetting, Entity>.
    GameItemPrefab
{
    [LabelText("预制体")]
    [Required]
    public GameObject prefab;
}
