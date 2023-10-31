using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class BlackHoleController : GamePropertyController
{
    public BlackHole blackHole { get; private set; }

    protected override void OnInit()
    {
        base.OnInit();

        blackHole = entity as BlackHole;

        Note.note.AssertIsNotNull(blackHole, nameof(blackHole));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var entityCtrl = other.gameObject.GetComponent<EntityController>();

        if (entityCtrl != null)
        {
            if (entityCtrl.entity is IResettable targetResettable and not Player)
            {
                var blackHoleResettable = blackHole as IResettable;
                targetResettable.SetArea(blackHoleResettable.isLeft);
            }

            StageManager.ResetEntity(entityCtrl);
        }
    }
}
