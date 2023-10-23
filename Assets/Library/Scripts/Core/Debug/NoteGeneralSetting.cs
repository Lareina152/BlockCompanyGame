using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Basis;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NoteGeneralSetting", menuName = "GameConfiguration/NoteGeneralSetting")]
public class NoteGeneralSetting : GeneralSettingBase
{
    public enum Corner
    {
        [LabelText("左上")]
        LeftTop,
        [LabelText("右上")]
        RightTop,
        [LabelText("左下")]
        LeftBottom,
        [LabelText("右下")]
        RightBottom,
    }

    public override StringTranslation settingName => new()
    {
        { "Chinese", "日志" },
        { "English", "Note" }
    };

    public override StringTranslation folderPath => coreCategory;

    #region ConsolePanel

    [LabelText("是否打开调试面板"), BoxGroup("调试面板")]
    [ToggleButtons("开启", "关闭")]
    [OnValueChanged(nameof(CreateDebuggingConsole))]
    public bool enableDebuggingConsole = true;

    [LabelText("日志文本UI预制体"), BoxGroup("调试面板")]
    [Required, AssetsOnly, AssetSelector(Paths = "Assets/Library/Resources")]
    [ShowIf(nameof(enableDebuggingConsole))]
    public TextMeshProUGUI noteText;

    [LabelText("调试面板UI预制体"), BoxGroup("调试面板")]
    [Required, AssetsOnly, AssetSelector(Paths = "Assets/Library/Resources")]
    [ShowIf(nameof(enableDebuggingConsole))]
    public DebuggingConsoleUI debuggingConsolePrefab;

    [LabelText("调试面板UI"), BoxGroup("调试面板")]
    [Required, SceneObjectsOnly]
    [ShowIf(nameof(enableDebuggingConsole))]
    public DebuggingConsoleUI debuggingConsole;

    [LabelText("位置"), BoxGroup("调试面板")]
    [EnumToggleButtons]
    [OnValueChanged(nameof(CreateDebuggingConsole))]
    [ShowIf(nameof(enableDebuggingConsole))]
    public Corner corner;

    [LabelText("面板透明度"), BoxGroup("调试面板")]
    [PropertyRange(0, 1)]
    [OnValueChanged(nameof(CreateDebuggingConsole))]
    [ShowIf(nameof(enableDebuggingConsole))]
    public float debuggingConsoleAlpha = 0.5f;

    [LabelText("内容颜色"), BoxGroup("调试面板")]
    [ShowIf(nameof(enableDebuggingConsole))]
    public Color contentColor = Color.black;

    [LabelText("Log前缀颜色"), BoxGroup("调试面板")]
    [ShowIf(nameof(enableDebuggingConsole))]
    [OnValueChanged(nameof(CreateDebuggingConsole))]
    public Color logColor = Color.gray;

    [LabelText("Warning前缀颜色"), BoxGroup("调试面板")]
    [ShowIf(nameof(enableDebuggingConsole))]
    [OnValueChanged(nameof(CreateDebuggingConsole))]
    public Color warningColor = new(0.8207547f, 0.4300702f, 0.01991047f, 1);

    [LabelText("Error前缀颜色"), BoxGroup("调试面板")]
    [ShowIf(nameof(enableDebuggingConsole))]
    [OnValueChanged(nameof(CreateDebuggingConsole))]
    public Color errorColor = Color.red;

    [LabelText("是否输出Log"), BoxGroup("调试面板")]
    [ShowIf(nameof(enableDebuggingConsole))]
    [OnValueChanged(nameof(CreateDebuggingConsole))]
    public bool enableConsoleLogOutput = true;

    [LabelText("是否输出Warning"), BoxGroup("调试面板")]
    [ShowIf(nameof(enableDebuggingConsole))]
    [OnValueChanged(nameof(CreateDebuggingConsole))]
    public bool enableConsoleWarningOutput = true;

    [LabelText("是否输出Error"), BoxGroup("调试面板")]
    [ShowIf(nameof(enableDebuggingConsole))]
    [OnValueChanged(nameof(CreateDebuggingConsole))]
    public bool enableConsoleErrorOutput = true;

    [LabelText("条目限制"), BoxGroup("调试面板")]
    [MinValue(1), MaxValue(10000)]
    [ShowIf(nameof(enableDebuggingConsole))]
    public int consoleNoteLimit = 500;

    [LabelText("面板淡入淡出时间"), BoxGroup("调试面板")]
    [PropertyRange(0, 1)]
    [ShowIf(nameof(enableDebuggingConsole))]
    public float fadeDuration = 0.2f;

    #endregion


    [LabelText("是否输出日志文件"), BoxGroup("日志文件")]
    [ToggleButtons("是", "否")]
    public bool enableNoteText = true;

    [LabelText("当前日志文件名称"), BoxGroup("日志文件")]
    [ShowInInspector, DisplayAsString]
    [ShowIf(nameof(enableNoteText))]
    public static string currentNoteFile = "";

    [LabelText("输出目录"), BoxGroup("日志文件")]
    [InfoBox(@"@""输出路径预览："" + outputDirectory")]
    [ShowIf(nameof(enableNoteText))]
    public string outputRelativeDirectory = "Logs";

    public string outputDirectory => IOFunc.cachePath.PathCombine(outputRelativeDirectory);

    #region GUI

    [Button("打开输出目录"), BoxGroup("日志文件")]
    [ShowIf(nameof(enableNoteText))]
    private void OpenOutputDirectory()
    {
        outputDirectory.CreateDirectory();
        outputDirectory.OpenDirectory();
    }

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        CreateDebuggingConsole();
    }

    private void CreateDebuggingConsole()
    {
//        if (enableDebuggingConsole == false)
//        {
//            if (debuggingConsole != null)
//            {
//                DestroyImmediate(debuggingConsole.gameObject);
//            }

//            return;
//        }


//        var uiObject = GameObjectFunc.FindOrCreateUIContainer();

//        var canvas = uiObject.transform.FindOrCreateCanvas();


//        if (debuggingConsolePrefab == null)
//        {
//#if UNITY_EDITOR
//            debuggingConsolePrefab = typeof(DebuggingConsoleUI).FindAssetOfType() as DebuggingConsoleUI;
//#endif
//        }

//        debuggingConsole = debuggingConsolePrefab.FindOrCreatePrefab(canvas.transform);

//        debuggingConsole.transform.SetAsLastSibling();

//        debuggingConsole.RT().pivot = corner switch
//        {
//            Corner.LeftTop => new(0, 1),
//            Corner.RightTop => new(1, 1),
//            Corner.LeftBottom => new(0, 0),
//            Corner.RightBottom => new(1, 0),
//            _ => throw new ArgumentOutOfRangeException()
//        };
//        Vector2 anchor = corner switch
//        {
//            Corner.LeftTop => new(0, 1),
//            Corner.RightTop => new(1, 1),
//            Corner.LeftBottom => new(0, 0),
//            Corner.RightBottom => new(1, 0),
//            _ => throw new ArgumentOutOfRangeException()
//        };
//        debuggingConsole.RT().anchorMin = anchor;
//        debuggingConsole.RT().anchorMax = anchor;
//        debuggingConsole.RT().ResetLocalArguments();

//        debuggingConsole.GetComponent<CanvasGroup>().alpha = debuggingConsoleAlpha;
        

//        debuggingConsole.name = "Debugging Console";

//        debuggingConsole.Refresh();

    }

    #endregion

    public override void CheckSettings()
    {
        base.CheckSettings();

        CreateDebuggingConsole();

        if (enableDebuggingConsole)
        {
            debuggingConsole.ClearNote();
        }
    }

    private void WriteToNoteFile(string content)
    {
        if (string.IsNullOrEmpty(currentNoteFile))
        {
            currentNoteFile = $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";
        }

        string filePath = outputDirectory.PathCombine(currentNoteFile);

        filePath.AppendFile(content + "\n");
    }

    public void AddLog(string content)
    {
        if (Application.isPlaying == false)
        {
            return;
        }
        if (enableDebuggingConsole && enableConsoleLogOutput)
        {
            debuggingConsole.AddLog(content);
        }

        if (enableNoteText)
        {
            WriteToNoteFile($"[Log]{content}");
        }
    }

    public void AddWarning(string content) {
        if (Application.isPlaying == false)
        {
            return;
        }
        if (enableDebuggingConsole && enableConsoleWarningOutput)
        {
            debuggingConsole.AddWarning(content);
        }

        if (enableNoteText)
        {
            WriteToNoteFile($"[Warning]{content}");
        }
    }

    public void AddError(string content)
    {
        if (Application.isPlaying == false)
        {
            return;
        }
        if (enableDebuggingConsole && enableConsoleErrorOutput)
        {
            debuggingConsole.AddError(content);
        }

        if (enableNoteText)
        {
            WriteToNoteFile($"[Error]{content}");
        }
    }
}
