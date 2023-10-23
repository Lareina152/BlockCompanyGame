using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class ContextMenuUIController : TracingUIPanelController<ContextMenuUIController>
{
    private struct ContextMenuEntryInfo
    {
        public VisualElement title;
    }

    protected ContextMenuUIPreset contextMenuUIPreset { get; private set; }

    [ShowInInspector]
    private VisualElement contextMenuEntryContainer;

    [ShowInInspector]
    private VisualTreeAsset contextMenuEntryAsset;

    [ShowInInspector]
    private List<ContextMenuEntryInfo> entryInfos = new();

    [ShowInInspector]
    private IContextMenuProvider provider;

    protected override void OnPreInit(UIPanelPreset preset)
    {
        base.OnPreInit(preset);

        contextMenuUIPreset = preset as ContextMenuUIPreset;

        Note.note.AssertIsNotNull(contextMenuUIPreset, nameof(contextMenuUIPreset));

        contextMenuEntryAsset = contextMenuUIPreset.contextMenuEntryAsset;

        Note.note.AssertIsNotNull(contextMenuEntryAsset, nameof(contextMenuEntryAsset));
    }

    protected override void OnInit(UIPanelPreset preset)
    {
        base.OnInit(preset);

        OnPanelOnMouseClickChanged += (oldPanel, currentPanel) =>
        {
            if (currentPanel != this)
            {
                Close();
            }
        };

        foreach (var globalEventID in contextMenuUIPreset.globalEventIDsToClose)
        {
            GlobalEventManager.AddEvent(globalEventID, Close);
        }
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        contextMenuEntryContainer = rootVisualElement.Q(contextMenuUIPreset.contextMenuEntryContainerName);

        Note.note.AssertIsNotNull(contextMenuEntryContainer, nameof(contextMenuEntryContainer));
    }

    protected override void OnClose()
    {
        base.OnClose();

        entryInfos.Clear();

        provider = null;
    }

    public static void Open(IContextMenuProvider provider, UIPanelController source)
    {
        ((UIPanelController)instance).Close();

        var entryContents = provider.GetContextMenuContent().ToArray();

        if (entryContents.Length == 0)
        {
            return;
        }

        if (entryContents.Length == 1 && instance.contextMenuUIPreset.autoExecuteIfOnlyOneEntry)
        {
            entryContents[0].action.Invoke();
            return;
        }

        instance.Open(source);

        instance.provider = provider;

        foreach (var (titleString, action) in entryContents)
        {
            var entry = instance.contextMenuEntryAsset.CloneTree();

            instance.AddVisualElement(instance.contextMenuEntryContainer, entry);

            var title = entry.Q<Label>(instance.contextMenuUIPreset.contextMenuEntryTitleName);

            Note.note.AssertIsNotNull(title, nameof(title));

            title.text = titleString;

            entry.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (instance.contextMenuUIPreset.clickMouseButtonType.HasMouseButton(evt.button))
                {
                    action.Invoke();
                    Close(provider);
                }
            });

            instance.entryInfos.Add(new ContextMenuEntryInfo()
            {
                title = title
            });
        }
    }

    public static void Close(IContextMenuProvider provider)
    {
        if (instance.provider == provider)
        {
            ((UIPanelController)instance).Close();
        }
    }
}
