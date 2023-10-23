using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

using Basis;

public enum ColliderEventBindingMode {
    FirstChild,
    AllChildren
}

[AddComponentMenu("EventsCustomed/ColliderEvent", 0)]
public class ColliderEvent : MonoBehaviour
{

    [FoldoutGroup("碰撞")]
    [Header("碰撞开始")]
    public UnityEvent<Collision> collisionEnterEvent;

    [FoldoutGroup("碰撞")]
    [Header("碰撞中")]
    public UnityEvent<Collision> collisionStayEvent;

    [FoldoutGroup("碰撞")]
    [Header("碰撞结束")]
    public UnityEvent<Collision> collisionExitEvent;

    [FoldoutGroup("触发")]
    [Header("触发开始")]
    public UnityEvent<Collider> triggerEnterEvent;

    [FoldoutGroup("触发")]
    [Header("触发中")]
    public UnityEvent<Collider> triggerStayEvent;

    [FoldoutGroup("触发")]
    [Header("触发结束")]
    public UnityEvent<Collider> triggerExitEvent;

    public bool isBindingMode = false;

    [EnableIf("isBindingMode")]
    public ColliderEventBindingMode bindingMode = ColliderEventBindingMode.FirstChild;

    public List<string> limitColliderNames = new List<string>();

    public void StartBind() {
        if (isBindingMode == true) {
            Action<ColliderEvent> action = (ColliderEvent trigger) => {
                //collider
                trigger.collisionEnterEvent.AddListener((Collision collision) => {
                    collisionEnterEvent.Invoke(collision);
                });
                trigger.collisionExitEvent.AddListener((Collision collision) => {
                    collisionExitEvent.Invoke(collision);
                });
                trigger.collisionStayEvent.AddListener((Collision collision) => {
                    collisionStayEvent.Invoke(collision);
                });

                //trigger
                trigger.triggerEnterEvent.AddListener((Collider collider) => {
                    triggerEnterEvent.Invoke(collider);
                });
                trigger.triggerExitEvent.AddListener((Collider collider) => {
                    triggerExitEvent.Invoke(collider);
                });
                trigger.triggerStayEvent.AddListener((Collider collider) => {
                    triggerStayEvent.Invoke(collider);
                });
            };

            if (bindingMode == ColliderEventBindingMode.FirstChild) {
                ColliderEvent bindedTarget = gameObject.FindFirstChildComponent<ColliderEvent>();
                if (bindedTarget != null) {
                    action(bindedTarget);
                }
            } else if (bindingMode == ColliderEventBindingMode.AllChildren) {
                foreach (ColliderEvent bindedTarget in gameObject.FindChildrenComponents<ColliderEvent>()) {
                    action(bindedTarget);
                }
            }
        }
    }

    //3d
    //碰撞开始
    public void OnCollisionEnter(Collision collision){
        if (isBindingMode == true) {
            return;
        }
        if (limitColliderNames.Count != 0 && !limitColliderNames.Contains(collision.gameObject.name)) {
            return;
        }
        collisionEnterEvent.Invoke(collision);
    }
    //碰撞中
    public void OnCollisionStay(Collision collision){
        if (isBindingMode == true) {
            return;
        }
        if (limitColliderNames.Count != 0 && !limitColliderNames.Contains(collision.gameObject.name)) {
            return;
        }
        collisionStayEvent.Invoke(collision);
    }

    //碰撞结束
    public void OnCollisionExit(Collision collision){
        if (isBindingMode == true) {
            return;
        }
        if (limitColliderNames.Count != 0 && !limitColliderNames.Contains(collision.gameObject.name)) {
            return;
        }
        collisionExitEvent.Invoke(collision);
    }

    //触发开始 只调用一次
    public void OnTriggerEnter(Collider collider){
        // _DisplayThisObject();

        if (isBindingMode == true) {
            return;
        }
        if (limitColliderNames.Count != 0 && !limitColliderNames.Contains(collider.gameObject.name)) {
            return;
        }
        triggerEnterEvent.Invoke(collider);
    }

    //触发中 一直执行
    public void OnTriggerStay(Collider collider){
        if (isBindingMode == true) {
            return;
        }
        if (limitColliderNames.Count != 0 && !limitColliderNames.Contains(collider.gameObject.name)) {
            return;
        }
        triggerStayEvent.Invoke(collider);
    }

    //触发结束 只调用一次
    public void OnTriggerExit(Collider collider){
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
