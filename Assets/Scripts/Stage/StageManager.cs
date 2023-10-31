using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IResettable
{
    public bool isLeft { get; protected set; }

    public bool areaInitialized { get; protected set; }

    public void SetArea(bool isLeft)
    {
        this.isLeft = isLeft;

        areaInitialized = true;
    }
}

public class StageManager : UniqueMonoBehaviour<StageManager>, IManagerBehaviour
{
    private static StageGeneralSetting stageSetting;

    [LabelText("左游戏道具生成区域")]
    [SerializeField]
    private RectangleFloat leftGamePropertyGenerationArea;

    [LabelText("右游戏道具生成区域")]
    [SerializeField]
    private RectangleFloat rightGamePropertyGenerationArea;

    [LabelText("左方块重置点")]
    [SerializeField]
    private Vector2 leftBlockResetPoint;

    [LabelText("右方块重置点")]
    [SerializeField]
    private Vector2 rightBlockResetPoint;

    [LabelText("左边胜利判定点")]
    [SerializeField]
    private Vector2 leftWinPoint;

    [LabelText("右边胜利判定点")]
    [SerializeField]
    private Vector2 rightWinPoint;

    [LabelText("胜利判定半径")]
    [SerializeField]
    [MinValue(0)]
    private float winRadius;

    [LabelText("管道吸收破坏半径")]
    [SerializeField]
    [MinValue(0)]
    private float tubeSuctionRadius;

    [Required]
    [SerializeField]
    private Animator leftTubeAnimator;

    [Required]
    [SerializeField]
    private Animator rightTubeAnimator;

    [ShowInInspector]
    private static float gamePropertyGenerationCooldown = 0f;

    [ShowInInspector]
    private static float leftBlockGenerationCooldown = 0f;

    [ShowInInspector]
    private static float rightBlockGenerationCooldown = 0f;

    [ShowInInspector]
    private static bool gameStarted = false;

    #region Manager Behaviour

    void IManagerBehaviour.OnEnterProcedure(string procedureID)
    {
        if (procedureID == GameInitializationProcedure.registeredID)
        {
            stageSetting = GameSetting.stageGeneralSetting;

            GameStateManager.OnGameEnded += () =>
            {
                ResetStage();
                gameStarted = false;
            };

            GameStateManager.OnGameStarted += () =>
            {
                gameStarted = true;

                gamePropertyGenerationCooldown = stageSetting.gamePropertyGenerationInitialCooldown;
                leftBlockGenerationCooldown = stageSetting.blockGenerationInitialCooldown;
                rightBlockGenerationCooldown = stageSetting.blockGenerationInitialCooldown;
            };
        }
    }

    #endregion

    private void Update()
    {
        if (gameStarted == false)
        {
            return;
        }

        WinCheck();

        if (hasWin)
        {
            return;
        }

        gamePropertyGenerationCooldown -= Time.deltaTime;

        if (gamePropertyGenerationCooldown <= 0f)
        {
            gamePropertyGenerationCooldown = stageSetting.gamePropertyGenerationInterval;

            GamePropertyItem leftGamePropertyItem = GetNextGamePropertyItem();

            Note.note.AssertIsNotNull(leftGamePropertyItem, nameof(leftGamePropertyItem));

            GamePropertyItem rightGamePropertyItem = GetNextGamePropertyItem();

            Note.note.AssertIsNotNull(rightGamePropertyItem, nameof(rightGamePropertyItem));

            CreateItemDrop(leftGamePropertyItem, isLeft:true);

            CreateItemDrop(rightGamePropertyItem, isLeft:false);
        }

        if (PlayerManager.playerCreated == false)
        {
            return;
        }

        leftBlockGenerationCooldown -= Time.deltaTime;

        if (leftBlockGenerationCooldown <= 0f)
        {
            if (PlayerManager.leftPlayer.HasItem() == false)
            {
                PlayerManager.leftPlayer.GiveItem(GetNextBlockItem());
                leftBlockGenerationCooldown = stageSetting.blockGenerationInterval;
            }
        }

        rightBlockGenerationCooldown -= Time.deltaTime;

        if (rightBlockGenerationCooldown <= 0f)
        {
            if (PlayerManager.rightPlayer.HasItem() == false)
            {
                PlayerManager.rightPlayer.GiveItem(GetNextBlockItem());
                rightBlockGenerationCooldown = stageSetting.blockGenerationInterval;
            }
        }
    }

    private void FixedUpdate()
    {
        if (hasWin)
        {
            WinAnimation();
        }
    }

    [Button]
    public static BlockItem GetNextBlockItem()
    {
        string blockItemID = stageSetting.blockItemGeneration;

        return Item.Create<BlockItem>(blockItemID);
    }

    [Button]
    public static GamePropertyItem GetNextGamePropertyItem()
    {
        string gamePropertyItemID = stageSetting.gamePropertyItemGeneration;

        return Item.Create<GamePropertyItem>(gamePropertyItemID);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        GizmosExt.DrawWireRect(leftGamePropertyGenerationArea.pivot,
            leftGamePropertyGenerationArea.size);
        GizmosExt.DrawWireRect(rightGamePropertyGenerationArea.pivot,
            rightGamePropertyGenerationArea.size);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(leftBlockResetPoint, 0.2f);
        Gizmos.DrawSphere(rightBlockResetPoint, 0.2f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(leftWinPoint, winRadius);
        Gizmos.DrawWireSphere(rightWinPoint, winRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(leftWinPoint, tubeSuctionRadius);
        Gizmos.DrawWireSphere(rightWinPoint, tubeSuctionRadius);
    }

    [Button]
    public static void ResetStage()
    {
        EntityManager.RemoveAllBlocks();
        EntityManager.RemoveAllGameProperties();
        EntityManager.RemoveAllItemDrops();

        ResetWin();
    }

    [Button("重置实体")]
    public static void ResetEntity(EntityController entityCtrl)
    {
        if (entityCtrl == null)
        {
            Note.note.Warning("试图重置一个空方块");
            return;
        }

        if (entityCtrl.entity is not IResettable resettable)
        {
            return;
        }

        if (resettable.areaInitialized == false)
        {
            return;
        }

        if (resettable.isLeft)
        {
            entityCtrl.transform.position = instance.leftBlockResetPoint;
        }
        else
        {
            entityCtrl.transform.position = instance.rightBlockResetPoint;
        }

        var rigidbody = entityCtrl.GetComponent<Rigidbody2D>();

        rigidbody.velocity = Vector2.zero;
    }

    #region Win

    [ShowInInspector]
    private static bool hasWin = false;

    [ShowInInspector]
    private static bool isLeftWin = false;

    [ShowInInspector]
    private static float leftWinCheckTime;

    [ShowInInspector]
    private static float rightWinCheckTime;

    private static readonly int IsShaking = Animator.StringToHash("isShaking");

    private static void WinCheck()
    {
        if (hasWin)
        {
            return;
        }

        if (gameStarted == false)
        {
            return;
        }

        if (PlayerManager.playerCreated == false)
        {
            return;
        }

        if (leftWinCheckTime >= stageSetting.winJudgeTime)
        {
            Win(true);
            return;
        }

        if (rightWinCheckTime >= stageSetting.winJudgeTime)
        {
            Win(false);
            return;
        }

        if (AreaHasStaticBlock(instance.leftWinPoint, instance.winRadius, true))
        {
            leftWinCheckTime += Time.deltaTime;
            instance.leftTubeAnimator.SetBool(IsShaking, true);
        }
        else
        {
            leftWinCheckTime = 0f;
            instance.leftTubeAnimator.SetBool(IsShaking, false);
        }

        if (AreaHasStaticBlock(instance.rightWinPoint, instance.winRadius, false))
        {
            rightWinCheckTime += Time.deltaTime;
            instance.rightTubeAnimator.SetBool(IsShaking, true);
        }
        else
        { 
            rightWinCheckTime = 0f;
            instance.rightTubeAnimator.SetBool(IsShaking, false);
        }

        bool AreaHasStaticBlock(Vector2 pivot, float radius, bool isLeft)
        {
            var targets = Physics2D.OverlapCircleAll(pivot, radius);

            foreach (var targetCollider in targets)
            {
                var entityCtrl = targetCollider.GetComponent<EntityController>();

                if (entityCtrl == null)
                {
                    continue;
                }

                if (entityCtrl.entity is Block block)
                {
                    if (block.isLeft != isLeft)
                    {
                        continue;
                    }

                    var rigidbody = entityCtrl.GetComponent<Rigidbody2D>();

                    if (rigidbody.bodyType == RigidbodyType2D.Static)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    [Button("胜利")]
    private static void Win(bool isLeftWin)
    {
        hasWin = true;

        StageManager.isLeftWin = isLeftWin;

        PlayerManager.DisablePlayerMovement();
        EntityManager.RemoveAllGameProperties();

        if (isLeftWin)
        {
            instance.leftTubeAnimator.SetBool(IsShaking, true);
        }
        else
        {
            instance.rightTubeAnimator.SetBool(IsShaking, true);
        }
    }

    private static void WinAnimation()
    {
        foreach (var entityCtrl in EntityController.GetAllEntityControllers())
        {
            var rigidbody = entityCtrl.GetComponent<Rigidbody2D>();

            var pivot = isLeftWin ? instance.leftWinPoint : instance.rightWinPoint;

            var dir = pivot - entityCtrl.transform.position.XY();

            dir = dir.normalized;

            rigidbody.AddForce(dir * stageSetting.tubeSuctionForce);

            rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        var suctionPivot = isLeftWin ? instance.leftWinPoint : instance.rightWinPoint;

        var targets = Physics2D.OverlapCircleAll(suctionPivot, instance.tubeSuctionRadius);

        foreach (var targetCollider in targets)
        {
            var entityCtrl = targetCollider.GetComponent<EntityController>();

            if (entityCtrl == null)
            {
                continue;
            }

            if (entityCtrl is PlayerController)
            {
                entityCtrl.gameObject.SetActive(false);
            }
            else
            {
                EntityManager.RemoveEntity(entityCtrl);
            }
        }
    }

    private static void ResetWin()
    {
        hasWin = false;

        instance.leftTubeAnimator.SetBool(IsShaking, false);
        instance.rightTubeAnimator.SetBool(IsShaking, false);

        leftWinCheckTime = 0f;
        rightWinCheckTime = 0f;
    }

    #endregion

    #region Create

    [Button("创建随机方块")]
    private static void CreateRandomBlocks(bool isLeft = true, int count = 10)
    {
        count.DoActionNTimes(() =>
        {
            var block = Block.CreateRandom<Block>();

            var resettable = (IResettable)block;

            resettable.SetArea(isLeft);

            Vector2 pos = isLeft ? 
                instance.leftGamePropertyGenerationArea.GetRandomPoint() :
                instance.rightGamePropertyGenerationArea.GetRandomPoint();

            var entityCtrl = EntityManager.Create(block, pos);
        });
    }

    [Button("创建道具")]
    private static void CreateProperty(
        [ValueDropdown("@GameSetting.entityGeneralSetting.GetPrefabNameList(typeof(GamePropertyPrefab))")]
        string propertyID,
        bool isLeft = true)
    {
        var property = GameProperty.Create<GameProperty>(propertyID);

        var resettable = (IResettable)property;

        resettable.SetArea(isLeft);

        Vector2 pos = isLeft ?
            instance.leftGamePropertyGenerationArea.GetRandomPoint() :
            instance.rightGamePropertyGenerationArea.GetRandomPoint();

        var entityCtrl = EntityManager.Create(property, pos);
    }

    public static void CreateItemDrop(Item item, bool isLeft)
    {
        Vector2 pos = isLeft ?
            instance.leftGamePropertyGenerationArea.GetRandomPoint() :
            instance.rightGamePropertyGenerationArea.GetRandomPoint();

        var entityCtrl = EntityManager.CreateItemDrop(item, pos);

        var resettable = entityCtrl.entity as IResettable;

        resettable?.SetArea(isLeft);
    }

    #endregion
}
