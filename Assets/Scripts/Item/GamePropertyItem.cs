using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePropertyItem : Item
{
    
}

public class GamePropertyItemPrefab : ItemPrefab
{
    protected static Type bindInstanceType = typeof(GamePropertyItem);
}
