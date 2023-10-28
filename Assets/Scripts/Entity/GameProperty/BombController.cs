using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class BombController : GamePropertyController
{
    private static readonly int IsBooming = Animator.StringToHash("isBooming");

    public Bomb bomb { get; private set; }

    [Required]
    [SerializeField]
    private Animator animator;

    [ShowInInspector]
    private bool hasSetExplodedAnimation = false;

    [ShowInInspector]
    private float cooldown;

    [ShowInInspector]
    private float explosionDestroyRadius;

    [ShowInInspector]
    private float explosionShockRadius;

    [ShowInInspector]
    private float shockStrength;

    private void Update()
    {
        if (initDone == false)
        {
            return;
        }

        cooldown -= Time.deltaTime;

        if (hasSetExplodedAnimation == false && cooldown < bomb.explosionAnimationTime)
        {
            animator.SetTrigger(IsBooming);

            hasSetExplodedAnimation = true;
        }

        if (cooldown < 0)
        {
            var brokenObjectColliders = Physics2D.
                OverlapCircleAll(transform.position.XY(), explosionDestroyRadius);

            foreach (var collider in brokenObjectColliders)
            {
                var entityCtrl = collider.GetComponent<EntityController>();

                if (entityCtrl != null && entityCtrl != this && entityCtrl.entity is Block)
                {
                    EntityManager.RemoveEntity(entityCtrl);
                }
            }

            var shockObjectColliders = Physics2D.
                OverlapCircleAll(transform.position.XY(), explosionShockRadius);

            foreach (var collider in shockObjectColliders)
            {
                var entityCtrl = collider.GetComponent<EntityController>();

                if (entityCtrl != null)
                {
                    var rigidbody2D = entityCtrl.GetComponent<Rigidbody2D>();

                    var dir = entityCtrl.transform.position.XY() - transform.position.XY();

                    dir = dir.normalized;

                    rigidbody2D.AddForce(dir * shockStrength);
                }
            }

            RemoveEntity(this);
        }
    }

    protected override void OnInit()
    {
        base.OnInit();

        bomb = entity as Bomb;

        Note.note.AssertIsNotNull(bomb, nameof(bomb));

        cooldown = bomb.cooldown;
        explosionDestroyRadius = bomb.explosionDestroyRadius;
        explosionShockRadius = bomb.explosionShockRadius;
        shockStrength = bomb.shockStrength;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionDestroyRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, explosionShockRadius);
    }
}
