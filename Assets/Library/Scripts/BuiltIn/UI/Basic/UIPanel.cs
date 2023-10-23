using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour, IUniversalTree<UIPanel>
{
    [LabelText("�����")]
    public UIPanel parent = null;
    [LabelText("�����")]
    public HashSet<UIPanel> children = new();

    [LabelText("��ʼ״̬")]
    [SerializeField]
    [ToggleButtons("����", "�ر�")]
    private bool isOn = true;

    private void Start()
    {
        if (isOn)
        {
            Open();
        }
        else
        {
            Hide();
        }
    }

    public void Show()
    {
        OnShow();

        isOn = true;
    }

    protected virtual void OnShow()
    {
        
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        OnHide();

        isOn = false;
    }

    protected virtual void OnHide()
    {
        gameObject.SetActive(false);
    }

    public virtual void Disable()
    {

    }

    public virtual void Enable()
    {

    }

    public void Open()
    {
        Open(this, null);
    }

    [Button("�������", ButtonStyle.Box)]
    public void Open([LabelText("�����"), SceneObjectsOnly] UIPanel newPanel)
    {
        Open(newPanel, this);
    }

    public static void Open(UIPanel newPanel, UIPanel parent)
    {
        if (newPanel == null)
        {
            return;
        }

        if (parent != null)
        {
            parent.children.Add(newPanel);

            newPanel.parent = parent;
        }

        newPanel.Show();
    }

    public IEnumerable<UIPanel> GetChildren()
    {
        return children;
    }

    public UIPanel GetParent()
    {
        return parent;
    }

    public bool DirectEquals(UIPanel other)
    {
        return this == other;
    }
}
