using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContextMenuProvider
{
    public IEnumerable<(string title, Action action)> GetContextMenuContent();
}
