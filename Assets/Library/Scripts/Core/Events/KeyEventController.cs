//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.Events;

//using Basis;

//public struct KeyEventStruct {
//    public bool isAnyKey;
//    public HashSet<KeyCode> keyCodes;
//    public KeyEventType keyEventType;

//    public bool Equals(KeyEventStruct comparsion) {
//        if (isAnyKey != comparsion.isAnyKey) {
//            return false;
//        }

//        HashSet<KeyCode> temp = new HashSet<KeyCode>();

//        temp.UnionWith(keyCodes);
//        temp.ExceptWith(comparsion.keyCodes);

//        if (temp.Count > 0) {
//            return false;
//        }

//        if (keyEventType != comparsion.keyEventType) {
//            return false;
//        }

//        return true;
//    }

//    public override string ToString() {
//        if (isAnyKey == true) {
//            return $"(isAnyKey = true, KeyEventType = {Enum.GetName(typeof(KeyEventType), keyEventType)})";
//        } else {
//            string keyNames = "";
//            int count = 0;
//            foreach (KeyCode keyCode in keyCodes)
//            {
//                count++;
//                keyNames += keyCode.ToString();
//                if (count < keyCodes.Count)
//                {
//                    keyNames += ",";
//                }
//            }
//            return $"(isAnyKey = false, keyCodes = {keyNames}, keyEventType = {keyEventType})";
//        }
//    }
//}

//public enum KeyEventType {
//    KeyDown,
//    KeyUp,
//    KeyHold
//}

//[AddComponentMenu("EventsCustomed/KeyEventController", 0)]
//public class KeyEventController : MonoBehaviour {

//    public static Note note = new Note("Events/KeyEventController");

//    public static Dictionary<WindowEvent, Dictionary<string, KeyEventTrigger>> allTriggers = new Dictionary<WindowEvent, Dictionary<string, KeyEventTrigger>>();
//    public static Dictionary<WindowEvent, Dictionary<string, List<UnityEvent<GameObject>>>> allKeyEvents = new Dictionary<WindowEvent, Dictionary<string, List<UnityEvent<GameObject>>>>();
//    public static Dictionary<WindowEvent, Dictionary<string, List<KeyEventStruct>>> allBindedKeys = new Dictionary<WindowEvent, Dictionary<string, List<KeyEventStruct>>>();

//    void Update()
//    {
//        if (WindowEvent.focusedWindow == default(WindowEvent)) {
//            return;
//        }

//        if (!allKeyEvents.ContainsKey(WindowEvent.focusedWindow)) {
//            return;
//        }

//        if (!allBindedKeys.ContainsKey(WindowEvent.focusedWindow)) {
//            return;
//        }

//        foreach (KeyValuePair<string, List<KeyEventStruct>> kvp in allBindedKeys[WindowEvent.focusedWindow]) {
//            string keyEventName = kvp.Key;
//            List<KeyEventStruct> keyEventStructList = kvp.Value;

//            // Debug.Log(keyEventName);

//            foreach (KeyEventStruct eventStruct in keyEventStructList) {

//                if (eventStruct.isAnyKey == true) {

//                    if (eventStruct.keyEventType == KeyEventType.KeyDown) {
//                        if (Input.anyKeyDown) {
//                            TriggerKeyEvent(WindowEvent.focusedWindow, keyEventName);
//                            break;
//                        }
//                    } else if (eventStruct.keyEventType == KeyEventType.KeyHold) {
//                        if (Input.anyKey) {
//                            TriggerKeyEvent(WindowEvent.focusedWindow, keyEventName);
//                            break;
//                        }
//                    } else {
//                        note.Warning($"窗口 {WindowEvent.focusedWindow.gameObject.name} 下事件 {keyEventName} 的某一个触发按键isAnyKey=true的同时，KeyEventType设置为 {nameof(eventStruct.keyEventType)}");
//                    }

//                } else {
                    
//                    if (eventStruct.keyEventType == KeyEventType.KeyDown) {

//                        int holdCount = 0;
//                        bool isAllKeyDown = eventStruct.keyCodes.All(key =>
//                        {
//                            if (Input.GetKeyDown(key)) {
//                                return true;
//                            }

//                            if (Input.GetKey(key)) {
//                                holdCount++;
//                                return true;
//                            }

//                            return false;
//                        });

//                        if (isAllKeyDown && holdCount < eventStruct.keyCodes.Count) {
//                            TriggerKeyEvent(WindowEvent.focusedWindow, keyEventName);
//                            break;
//                        }

//                    } else if (eventStruct.keyEventType == KeyEventType.KeyUp) {

//                        int holdCount = 0;
//                        bool isAllKeyUpOrHold = eventStruct.keyCodes.All(key =>
//                        {
//                            if (Input.GetKeyUp(key)) {
//                                return true;
//                            }

//                            if (Input.GetKey(key)) {
//                                holdCount++;
//                                return true;
//                            }

//                            return false;
//                        });

//                        if (isAllKeyUpOrHold == true && holdCount < eventStruct.keyCodes.Count) {
//                            TriggerKeyEvent(WindowEvent.focusedWindow, keyEventName);
//                            break;
//                        }
//                    } else if (eventStruct.keyEventType == KeyEventType.KeyHold) {
//                        if (eventStruct.keyCodes.All(Input.GetKey)) {
//                            TriggerKeyEvent(WindowEvent.focusedWindow, keyEventName);
//                            break;
//                        }
//                    }

//                }
//            }
//        }
//    }

//    public static void TriggerKeyEvent(WindowEvent bindedWindow, string eventName) {
//        if (!allKeyEvents.ContainsKey(bindedWindow)) {
//            note.Log($"窗口 {bindedWindow.gameObject.name} 下没有注册任何事件，触发事件 {eventName} 失败");
//            return;
//        }

//        if (!allKeyEvents[bindedWindow].ContainsKey(eventName)) {
//            note.Log($"窗口 {bindedWindow.gameObject.name} 下不存在事件 {eventName} ,触发失败");
//            return;
//        }

//        if (allKeyEvents[bindedWindow][eventName] == default(List<UnityEvent<GameObject>>)) {
//            note.Log($"窗口 {bindedWindow.gameObject.name} 下事件 {eventName} 的UnityEvent列表无初始化，触发失败");
//        }

//        if (allKeyEvents[bindedWindow][eventName].Count <= 0) {
//            note.Log($"窗口 {bindedWindow.gameObject.name} 下事件 {eventName} 的UnityEvent列表长度为0，没有调用任何函数");
//        }

//        KeyEventTrigger trigger = GetTrigger(bindedWindow, eventName);

//        GameObject triggerObject = default(GameObject);
//        if (trigger != default(KeyEventTrigger)) {

//            if (trigger.isOn == false) {
//                return;
//            }

//            triggerObject = trigger.gameObject;
//        }

//        foreach (UnityEvent<GameObject> eventToCall in allKeyEvents[bindedWindow][eventName]) {
//            eventToCall.Invoke(triggerObject);
//        }
//    }

//    public static void AddTrigger(WindowEvent bindedWindow, string eventName, KeyEventTrigger trigger) {
//        if (!allTriggers.ContainsKey(bindedWindow)) {
//            allTriggers[bindedWindow] = new Dictionary<string, KeyEventTrigger>();
//        }

//        if (allTriggers[bindedWindow].ContainsKey(eventName)) {
//            note.Log($"窗口 {bindedWindow.gameObject.name} 下的按键事件 {eventName} 重复绑定trigger");
//        }

//        allTriggers[bindedWindow][eventName] = trigger;
//    }

//    public static KeyEventTrigger GetTrigger(WindowEvent bindedWindow, string eventName) {
//        if (!allTriggers.ContainsKey(bindedWindow)) {
//            return default(KeyEventTrigger);
//        }

//        if (!allTriggers[bindedWindow].ContainsKey(eventName)) {
//            return default(KeyEventTrigger);
//        }

//        return allTriggers[bindedWindow][eventName];
//    }

//    public static void RegisterKeyEvent(WindowEvent bindedWindow, string eventName, UnityEvent<GameObject> eventToCall) {
//        if (!allKeyEvents.ContainsKey(bindedWindow)) {
//            allKeyEvents[bindedWindow] = new Dictionary<string, List<UnityEvent<GameObject>>>();
//        }

//        if (!allKeyEvents[bindedWindow].ContainsKey(eventName)) {
//            allKeyEvents[bindedWindow][eventName] = new List<UnityEvent<GameObject>>();
//        }

//        allKeyEvents[bindedWindow][eventName].Add(eventToCall);
//    }

//    public static bool CheckKeyEventIsRegistered(WindowEvent bindedWindow, string eventName, bool debugging = true) {
//        if (!allKeyEvents.ContainsKey(bindedWindow)) {

//            if (debugging == true) {
//                note.Log($"没有检测到窗口 {bindedWindow.name} 相关的按键事件注册");
//            }

//            return false;
//        }

//        if (!allKeyEvents[bindedWindow].ContainsKey(eventName)) {

//            if (debugging == true) {
//                note.Log($"没有检测到窗口 {bindedWindow.name} 下的按键事件 {eventName} 被注册");
//            }

//            return false;
//        }

//        return true;
//    }

//    public static void AddBindedKey(WindowEvent bindedWindow, string eventName, KeyEventStruct eventStruct) {
//        if (!CheckKeyEventIsRegistered(bindedWindow, eventName)) {
//            return;
//        }

//        if (!allBindedKeys.ContainsKey(bindedWindow)) {
//            allBindedKeys[bindedWindow] = new Dictionary<string, List<KeyEventStruct>>();
//        }

//        if (!allBindedKeys[bindedWindow].ContainsKey(eventName)) {
//            allBindedKeys[bindedWindow][eventName] = new List<KeyEventStruct>();
//        }

//        foreach (KeyEventStruct bindedKey in allBindedKeys[bindedWindow][eventName]) {
//            if (bindedKey.Equals(eventStruct)) {

//                note.Log($"窗口 {bindedWindow.name} 下的按键事件 {eventName} 的 {eventStruct.ToString()} 已被绑定");

//                return;
//            }
//        }

//        allBindedKeys[bindedWindow][eventName].Add(eventStruct);
//    }

//    public static void AddBindedKey(WindowEvent bindedWindow, string eventName, bool isAnyKey, IEnumerable<KeyCode> keyCodes, KeyEventType keyEventType) {
//        KeyEventStruct temp;

//        temp.isAnyKey = isAnyKey;
//        temp.keyCodes = new HashSet<KeyCode>(keyCodes);
//        temp.keyEventType = keyEventType;

//        AddBindedKey(bindedWindow, eventName, temp);
//    }
//}
