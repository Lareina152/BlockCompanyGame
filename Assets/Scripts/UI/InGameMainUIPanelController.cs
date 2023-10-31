using System.Collections;
using System.Collections.Generic;
using Basis;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameMainUIPanelController : UIToolkitPanelController
{
    public InGameMainUIPanelPreset inGameMainUIPanelPreset { get; private set; }

    [ShowInInspector]
    private VisualElement resetButton;

    [ShowInInspector]
    private VisualElement leftPlayerItemPanel;

    [ShowInInspector]
    private VisualElement rightPlayerItemPanel;

    protected override void OnPreInit(UIPanelPreset preset)
    {
        base.OnPreInit(preset);

        inGameMainUIPanelPreset = preset as InGameMainUIPanelPreset;
        
        Note.note.AssertIsNotNull(inGameMainUIPanelPreset, nameof(inGameMainUIPanelPreset));
    }

    protected override async void OnOpen()
    {
        base.OnOpen();

        resetButton = rootVisualElement.Q(inGameMainUIPanelPreset.resetButtonName);

        Note.note.AssertIsNotNull(resetButton, nameof(resetButton));

        resetButton.style.backgroundImage =
            inGameMainUIPanelPreset.resetButtonReleasedIcon;

        resetButton.RegisterCallback<MouseDownEvent>(e =>
        {
            resetButton.style.backgroundImage =
                inGameMainUIPanelPreset.resetButtonPressedIcon;
        });

        resetButton.RegisterCallback<MouseUpEvent>(e =>
        {
            resetButton.style.backgroundImage =
                inGameMainUIPanelPreset.resetButtonReleasedIcon;

            GameStateManager.RestartGame();
        });

        leftPlayerItemPanel = rootVisualElement.Q(inGameMainUIPanelPreset.leftPlayerItemPanelName);

        Note.note.AssertIsNotNull(leftPlayerItemPanel, nameof(leftPlayerItemPanel));

        rightPlayerItemPanel = rootVisualElement.Q(inGameMainUIPanelPreset.rightPlayerItemPanelName);

        Note.note.AssertIsNotNull(rightPlayerItemPanel, nameof(rightPlayerItemPanel));

        await UniTask.WaitUntil(() => PlayerManager.playerCreated);

        PlayerManager.leftPlayer.OnItemChangedEvent += OnLeftPlayerItemChanged;

        PlayerManager.rightPlayer.OnItemChangedEvent += OnRightPlayerItemChanged;
    }

    private void OnLeftPlayerItemChanged(Item item)
    {
        OnPlayerItemChanged(PlayerManager.leftPlayer, item, leftPlayerItemPanel);
    }

    private void OnRightPlayerItemChanged(Item item)
    {
        OnPlayerItemChanged(PlayerManager.rightPlayer, item, rightPlayerItemPanel);
    }

    private void OnPlayerItemChanged(Player player, Item item, VisualElement playerItemPanel)
    {
        if (item != null)
        {
            playerItemPanel.style.backgroundImage = new StyleBackground(item.iconSprite);
        }
        else
        {
            playerItemPanel.style.backgroundImage = null;
        }
    }
}
