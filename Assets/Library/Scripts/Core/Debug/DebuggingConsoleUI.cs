using System.Collections;
using System.Collections.Generic;
using Basis;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebuggingConsoleUI : UIPanel
{
    public static TextMeshProUGUI noteText => GameCoreSettingBase.noteGeneralSetting.noteText;

    [LabelText("��������")]
    [Required, ChildGameObjectsOnly]
    public RectTransform container;

    [LabelText("��������")]
    [Required, ChildGameObjectsOnly]
    public ScrollRect scrollRect;

    [LabelText("Logѡ���")]
    [Required, ChildGameObjectsOnly]
    public CheckBoxWithLabel logToggle;

    [LabelText("Warningѡ���")]
    [Required, ChildGameObjectsOnly]
    public CheckBoxWithLabel warningToggle;

    [LabelText("Errorѡ���")]
    [Required, ChildGameObjectsOnly]
    public CheckBoxWithLabel errorToggle;

    [LabelText("�رհ�ť")]
    [Required, ChildGameObjectsOnly]
    public ButtonBase closeButton;

    [LabelText("�򿪰�ť")]
    [Required, ChildGameObjectsOnly]
    public ButtonBase openButton;

    [LabelText("��Ҫ����")]
    [Required, ChildGameObjectsOnly]
    public CanvasGroup mainGroup;

    private GameObjectLimitQueue<TextMeshProUGUI> queue;
    private readonly Dictionary<TextMeshProUGUI, Note.NoteType> noteTypes = new();

    private void Awake()
    {
        queue = new(GameCoreSettingBase.noteGeneralSetting.consoleNoteLimit);

        logToggle.onValueChanged.AddListener(ToggleLogs);
        warningToggle.onValueChanged.AddListener(ToggleWarnings);
        errorToggle.onValueChanged.AddListener(ToggleErrors);
    }

    protected override void OnShow()
    {
        mainGroup.DOKill();
        mainGroup.blocksRaycasts = true;
        mainGroup.DOFade(1, GameCoreSettingBase.noteGeneralSetting.fadeDuration);

        openButton.SetActive(false);
        closeButton.SetActive(true);
    }

    protected override void OnHide()
    {
        mainGroup.DOKill();
        mainGroup.blocksRaycasts = false;
        mainGroup.DOFade(0, GameCoreSettingBase.noteGeneralSetting.fadeDuration);

        openButton.SetActive(true);
        closeButton.SetActive(false);
    }

    private TextMeshProUGUI AddNoteText(string label, string content)
    {
        //bool scrollbarOnBottom = scrollbar.value < 0.01f;

        var newNoteText = queue.Add(noteText, container);
        container.SetHeight(container.childCount * noteText.GetComponent<RectTransform>().rect.height);

        scrollRect.verticalNormalizedPosition = 0;

        newNoteText.text = $"[{label}]{content.ColorTag(GameCoreSettingBase.noteGeneralSetting.contentColor)}";

        return newNoteText;
    }

    [Button("���Log", ButtonStyle.Box)]
    public void AddLog([LabelText("����")] string content = "Test")
    {
        var newNoteText = AddNoteText("Log".ColorTag(GameCoreSettingBase.noteGeneralSetting.logColor), content);
        noteTypes[newNoteText] = Note.NoteType.Log;
    }

    [Button("���Warning", ButtonStyle.Box)]
    public void AddWarning([LabelText("����")] string content = "Test")
    {
        var newNoteText = AddNoteText("Warning".ColorTag(GameCoreSettingBase.noteGeneralSetting.warningColor), content);
        noteTypes[newNoteText] = Note.NoteType.Warning;
    }

    [Button("���Error", ButtonStyle.Box)]
    public void AddError([LabelText("����")] string content = "Test")
    {
        var newNoteText = AddNoteText("Error".ColorTag(GameCoreSettingBase.noteGeneralSetting.errorColor), content);
        noteTypes[newNoteText] = Note.NoteType.Error;
    }

    [Button("ˢ��")]
    public void Refresh()
    {
        logToggle.SetLabelColor(GameCoreSettingBase.noteGeneralSetting.logColor);
        warningToggle.SetLabelColor(GameCoreSettingBase.noteGeneralSetting.warningColor);
        errorToggle.SetLabelColor(GameCoreSettingBase.noteGeneralSetting.errorColor);

        logToggle.SetActive(GameCoreSettingBase.noteGeneralSetting.enableConsoleLogOutput);
        warningToggle.SetActive(GameCoreSettingBase.noteGeneralSetting.enableConsoleWarningOutput);
        errorToggle.SetActive(GameCoreSettingBase.noteGeneralSetting.enableConsoleErrorOutput);
    }

    [Button("�����־")]
    public void ClearNote()
    {
        container.ClearChildren();
    }

    public void ToggleLogs(bool enable)
    {
        foreach (var log in noteTypes.GetKeysByValue(Note.NoteType.Log))
        {
            log.gameObject.SetActive(enable);
        }
    }

    public void ToggleWarnings(bool enable)
    {
        foreach (var warning in noteTypes.GetKeysByValue(Note.NoteType.Warning))
        {
            warning.gameObject.SetActive(enable);
        }
    }

    public void ToggleErrors(bool enable)
    {
        foreach (var error in noteTypes.GetKeysByValue(Note.NoteType.Error))
        {
            error.gameObject.SetActive(enable);
        }
    }
}
