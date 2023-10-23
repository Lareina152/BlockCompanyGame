//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//using Sirenix.OdinInspector;

//using Basis;

//[AddComponentMenu("EventsCustomed/KeyEventTrigger", 0)]
//public class KeyEventTrigger : MonoBehaviour
//{
//    public static Note note = new Note("Events/KeyEventTrigger");

//    public static Dictionary<WindowEvent, Dictionary<string, KeyEventTrigger>> allTriggers {
//        get {return KeyEventController.allTriggers;}
//        set {KeyEventController.allTriggers = value;}
//    }
//    public static Dictionary<WindowEvent, Dictionary<string, List<UnityEvent<GameObject>>>> allKeyEvents {
//        get {return KeyEventController.allKeyEvents;}
//        set {KeyEventController.allKeyEvents = value;}
//    }
//    public static Dictionary<WindowEvent, Dictionary<string, List<KeyEventStruct>>> allBindedKeys {
//        get {return KeyEventController.allBindedKeys;}
//        set {KeyEventController.allBindedKeys = value;}
//    }

//    public bool isOn = true;

//    [Header("绑定窗口")]
//    [Required]
//    public WindowEvent bindedWindow = default(WindowEvent);

//    [Serializable]
//    public class KeyEventStructEditable {
//        public bool isAnyKey = false;
//        public List<KeyCode> keyCodes;
//        public KeyEventType keyEventType;
//    }

//    [Serializable]
//    public class KeyElement {
//        public string keyEventName = "";
//        public List<KeyEventStructEditable> keyEventStructs;
//        public UnityEvent<GameObject> eventToCall;
//    }

//    [Header("绑定键盘事件")]
//    public List<KeyElement> bindedKeyEvents = new List<KeyElement>();

//    void Reset() {
//        bindedWindow = GameObjectFunc.FindFirstParentComponent<WindowEvent>(gameObject);
//    }

//    void Awake() {
//        foreach (KeyElement element in bindedKeyEvents) {

//            KeyEventController.RegisterKeyEvent(bindedWindow, element.keyEventName, element.eventToCall);

//            KeyEventController.AddTrigger(bindedWindow, element.keyEventName, this);

//            foreach (KeyEventStructEditable structEditable in element.keyEventStructs) {
//                KeyEventController.AddBindedKey(bindedWindow, element.keyEventName, structEditable.isAnyKey, structEditable.keyCodes, structEditable.keyEventType);
//            }

//        }
//    }

//    //Only for Debugging
//    public void _DisplayThisObject() {
//        note.Log($"name:{gameObject.name}, tag:{gameObject.tag}, position: {transform.position}");
//    }
//}
