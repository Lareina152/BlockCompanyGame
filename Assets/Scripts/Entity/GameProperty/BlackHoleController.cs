using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class BlackHoleController : GamePropertyController
{
    public BlackHole blackHole { get; private set; }

    [ShowInInspector]
    private float gravitationalRadius;

    [ShowInInspector]
    private float centripetalForce;

    [ShowInInspector]
    private float gravitationalDirectionRandomAngleRange;

    protected override void OnInit()
    {
        base.OnInit();

        blackHole = entity as BlackHole;

        Note.note.AssertIsNotNull(blackHole, nameof(blackHole));

        gravitationalRadius = blackHole.gravitationalRadius;
        centripetalForce = blackHole.centripetalForce;
        gravitationalDirectionRandomAngleRange = blackHole.gravitationalDirectionRandomAngleRange;
    }

    private void FixedUpdate()
    {
        var entityColliders = 
            Physics2D.OverlapCircleAll(transform.position.XY(), gravitationalRadius);

        foreach (var collider in entityColliders)
        {
            var entityCtrl = collider.GetComponent<EntityController>();

            if (entityCtrl == null || entityCtrl == this)
            {
                continue;
            }

            var rigidbody2D = entityCtrl.GetComponent<Rigidbody2D>();

            if (rigidbody2D == null)
            {
                continue;
            }

            var dir = transform.position.XY() - entityCtrl.transform.position.XY();

            var distance = dir.magnitude;

            dir = dir.normalized;

            dir = dir.ClockwiseRotate((-gravitationalDirectionRandomAngleRange).
                RandomRange(gravitationalDirectionRandomAngleRange));

            var force = 0f.Lerp(centripetalForce, distance / gravitationalRadius);

            rigidbody2D.AddForce(dir * force);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position.XY(), gravitationalRadius);
    }
}
