using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

using Basis;

public enum Collider2DEventBindingMode {
    FirstChild,
    AllChildren
}

[AddComponentMenu("EventsCustomed/Collider2DEvent", 0)]
public class Collider2DEvent : MonoBehaviour
{
    [FoldoutGroup("碰撞")]
    [Header("碰撞开始")]
    public UnityEvent<Collision2D> collisionEnterEvent;

    [FoldoutGroup("碰撞")]
    [Header("碰撞中")]
    public UnityEvent<Collision2D> collisionStayEvent;

    [FoldoutGroup("碰撞")]
    [Header("碰撞结束")]
    public UnityEvent<Collision2D> collisionExitEvent;

    [FoldoutGroup("触发")]
    [Header("触发开始")]
    public UnityEvent<Collider2D> triggerEnterEvent;

    [FoldoutGroup("触发")]
    [Header("触发中")]
    public UnityEvent<Collider2D> triggerStayEvent;

    [FoldoutGroup("触发")]
    [Header("触发结束")]
    public UnityEvent<Collider2D> triggerExitEvent;

    public bool isBindingMode = false;

    [EnableIf("isBindingMode")]
    public Collider2DEventBindingMode bindingMode = Collider2DEventBindingMode.FirstChild;

    public List<string> limitColliderNames = new List<string>();

    public void StartBind() {
        if (isBindingMode == true) {
            Action<Collider2DEvent> action = (Collider2DEvent trigger) => {
                //collider
                trigger.collisionEnterEvent.AddListener((Collision2D collision) => {
                    collisionEnterEvent.Invoke(collision);
                });
                trigger.collisionExitEvent.AddListener((Collision2D collision) => {
                    collisionExitEvent.Invoke(collision);
                });
                trigger.collisionStayEvent.AddListener((Collision2D collision) => {
                    collisionStayEvent.Invoke(collision);
                });

                //trigger
                trigger.triggerEnterEvent.AddListener((Collider2D collider) => {
                    triggerEnterEvent.Invoke(collider);
                });
                trigger.triggerExitEvent.AddListener((Collider2D collider) => {
                    triggerExitEvent.Invoke(collider);
                });
                trigger.triggerStayEvent.AddListener((Collider2D collider) => {
                    triggerStayEvent.Invoke(collider);
                });
            };

            if (bindingMode == Collider2DEventBindingMode.FirstChild) {
                Collider2DEvent bindedTarget = gameObject.FindFirstChildComponent<Collider2DEvent>();
                if (bindedTarget != null) {
                    action(bindedTarget);

                    bindedTarget.limitColliderNames.AddRange(limitColliderNames);
                }
            } else if (bindingMode == Collider2DEventBindingMode.AllChildren) {
                foreach (Collider2DEvent bindedTarget in gameObject.FindChildrenComponents<Collider2DEvent>()) {
                    action(bindedTarget);

                    bindedTarget.limitColliderNames.AddRange(limitColliderNames);
                }
            }
        }
    }

    //2d
    //碰撞开始
    public void OnCollisionEnter2D(Collision2D collision){
        if (isBindingMode == true) {
            return;
        }
        if (limitColliderNames.Count != 0 && !limitColliderNames.Contains(collision.gameObject.name)) {
            return;
        }
        collisionEnterEvent.Invoke(collision);
    }
    //碰撞中
    public void OnCollisionStay2D(Collision2D collision){
        if (isBindingMode == true) {
            return;
        }
        if (limitColliderNames.Count != 0 && !limitColliderNames.Contains(collision.gameObject.name)) {
            return;
        }
        collisionStayEvent.Invoke(collision);
    }

    //碰撞结束
    public void OnCollisionExit2D(Collision2D collision){
        if (isBindingMode == true) {
            return;
        }
        if (limitColliderNames.Count != 0 && !limitColliderNames.Contains(collision.gameObject.name)) {
            return;
        }
        collisionExitEvent.Invoke(collision);
    }

    //触发开始 只调用一次
    public void OnTriggerEnter2D(Collider2D collider){
        if (isBindingMode == true) {
            return;
        }
        if (limitColliderNames.Count != 0 && !limitColliderNames.Contains(collider.gameObject.name)) {
            return;
        }
        triggerEnterEvent.Invoke(collider);
    }

    //触发中 一直执行
    public void OnTriggerStay2D(Collider2D collider){
        if (isBindingMode == true) {
            return;
        }
        if (limitColliderNames.Count != 0 && !limitColliderNames.Contains(collider.gameObject.name)) {
            return;
        }
        triggerStayEvent.Invoke(collider);
    }

    //触发结束 只调用一次
    public void OnTriggerExit2D(Collider2D collider){
        if (isBindingMode == true) {
            return;
        }
        if (limitColliderNames.Count != 0 && !limitColliderNames.Contains(collider.gameObject.name)) {
            return;
        }
        triggerExitEvent.Invoke(collider);
    }


    public void _DisplayThisObject() {
        Note.note.Log($"name:{gameObject.name}, tag:{gameObject.tag}, position: {transform.position}");
    }
}
