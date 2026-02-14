using System;
using System.Collections.Generic;
using UnityEngine;
using RWCustom;
using HUD;
using static CustomStomachStorage.MyOptions;

namespace CustomStomachStorage
{
    internal class StomachHUD : HudPart
    {
        public static void HookAdd()
        {
            On.HUD.HUD.InitSinglePlayerHud += HUD_InitSinglePlayerHud;
            On.HUD.HUD.InitMultiplayerHud += HUD_InitMultiplayerHud;
        }
        public static void HookSubtract()
        {
            On.HUD.HUD.InitSinglePlayerHud -= HUD_InitSinglePlayerHud;
            On.HUD.HUD.InitMultiplayerHud -= HUD_InitMultiplayerHud;
        }

        // 单人游戏 HUD 初始化时调用
        private static void HUD_InitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
        {
            orig(self, cam);
            CreateStomachHUD(self, cam.room.game);
        }

        // 多人游戏 HUD 初始化时调用
        private static void HUD_InitMultiplayerHud(On.HUD.HUD.orig_InitMultiplayerHud orig, HUD.HUD self, ArenaGameSession session)
        {
            orig(self, session);
            CreateStomachHUD(self, session.room.game);
        }

        private static void CreateStomachHUD(HUD.HUD hud, RainWorldGame game)
        {
            if (!ModManager.JollyCoop)
            {
                // 单人模式
                if (game.Players.Count > 0 && game.Players[0].realizedCreature is Player player)
                {
                    var stomachHUD = new StomachHUD(hud, player, 0, 1);
                    hud.AddPart(stomachHUD);
                }
            }
            else
            {
                // 多人模式 - 为每个玩家创建独立的 HUD
                for (int i = 0; i < game.Players.Count; i++)
                {
                    if (game.Players[i].realizedCreature is Player player)
                    {
                        var stomachHUD = new StomachHUD(hud, player, player.playerState.playerNumber, game.Players.Count);
                        hud.AddPart(stomachHUD);
                    }
                }
            }
        }


        /// <summary>
        /// 构造函数，初始化胃部HUD
        /// </summary>
        /// <param name="hud">HUD实例</param>
        /// <param name="player">玩家实例</param>
        /// <param name="playerId">玩家ID</param>
        /// <param name="playerCount">玩家总数</param>
        public StomachHUD(HUD.HUD hud, Player player, int playerId, int playerCount)
            : base(hud)
        {
            this.container = new FContainer();
            this.player = player;
            this.options = MyOptions.Instance;
            this.iconSize = options.GetIconSize();

            hud.fContainers[1].AddChild(this.container);

            CreateIcons();
            PositionContainer(playerId, playerCount);
        }

        /// <summary>
        /// 清除所有精灵
        /// </summary>
        public override void ClearSprites()
        {
            base.ClearSprites();
            foreach (var icon in itemIcons)
            {
                icon?.ClearSprites();
            }
            container.RemoveFromContainer();
        }

        /// <summary>
        /// 每帧更新HUD状态
        /// </summary>
        public override void Update()
        {
            base.Update();

            var stomachContents = MyPlayer.ESS.GetStomachContents(player);

            // 更新图标数量
            if (stomachContents.Count != itemIcons.Count)
            {
                RebuildIcons(stomachContents.Count);
            }

            // 更新每个图标的内容
            for (int i = 0; i < stomachContents.Count && i < itemIcons.Count; i++)
            {
                itemIcons[i].SetObject(stomachContents[i], ObjectIcon.DisplayMode.Holding);
            }

            // 控制显示/隐藏
            bool shouldShow = options.GetGlobalVisibility() == GlobalVisibilityMode.Always ||
                             (player.mapInput.mp);//player.mapInput != null
            container.isVisible = shouldShow;
        }

        /// <summary>
        /// 获取绝对顶部Y坐标
        /// </summary>
        /// <returns>顶部Y坐标</returns>
        public float GetAbsoluteTopY()
        {
            return container.GetPosition().y + (itemIcons.Count > 0 ? iconSize : 0);
        }

        /// <summary>
        /// 创建图标
        /// </summary>
        private void CreateIcons()
        {
            var settings = CreateSettings();

            for (int i = 0; i < INITIAL_CAPACITY; i++)
            {
                var icon = new ObjectIcon(
                    container,
                    new Vector2(0, i * iconSize),  // 垂直排列
                    null,
                    ObjectIcon.SideLinePosition.Left,
                    Color.white,
                    settings
                );
                icon.isVisible = false;
                itemIcons.Add(icon);
            }
        }

        /// <summary>
        /// 重建图标
        /// </summary>
        /// <param name="newCount">新图标数量</param>
        private void RebuildIcons(int newCount)
        {
            // 清除现有图标
            foreach (var icon in itemIcons)
            {
                icon.ClearSprites();
            }
            itemIcons.Clear();

            // 创建新图标
            var settings = CreateSettings();
            for (int i = 0; i < newCount; i++)
            {
                var icon = new ObjectIcon(
                    container,
                    new Vector2(0, i * iconSize),
                    null,
                    ObjectIcon.SideLinePosition.Top,
                    Color.white,
                    settings
                );
                itemIcons.Add(icon);
            }

            // 重新定位
            PositionIcons();
        }

        /// <summary>
        /// 设置图标位置
        /// </summary>
        private void PositionIcons()
        {
            for (int i = 0; i < itemIcons.Count; i++)
            {
                itemIcons[i].position = new Vector2(0, i * iconSize);
            }
        }

        /// <summary>
        /// 定位容器
        /// </summary>
        /// <param name="playerId">玩家ID</param>
        /// <param name="playerCount">玩家总数</param>
        private void PositionContainer(int playerId, int playerCount)
        {
            float screenWidth = hud.rainWorld.screenSize.x;
            float screenHeight = hud.rainWorld.screenSize.y;
            float safeOffset = hud.rainWorld.options.SafeScreenOffset.y;

            // 放在屏幕右侧
            float xPos = screenWidth - iconSize - safeOffset - 10f;
            float yPos = safeOffset + iconSize;

            // 多人模式偏移
            if (playerCount > 1)
            {
                xPos -= (playerCount - playerId - 1) * (iconSize + HUD_GAP);
            }

            container.SetPosition(xPos, yPos);
        }

        /// <summary>
        /// 创建设置
        /// </summary>
        /// <returns>图标设置</returns>
        private ObjectIcon.Settings CreateSettings()
        {
            return new ObjectIcon.Settings
            {
                minAllowedSize = iconSize - 4,
                maxAllowedSize = iconSize,

                iconNoShader = hud.rainWorld.Shaders["Basic"],
                iconHoldingShader = hud.rainWorld.Shaders["Basic"],
                iconPickUpCandidateShader = hud.rainWorld.Shaders["GateHologram"],

                backgroundVisibilityMode = options.GetBackgroundVisibility(),
                backgroundOpacity = options.GetBackgroundOpacity(),

                sideLineVisibilityMode = options.GetSideLineVisibility(),
                sideLineOpacity = options.GetSideLineOpacity(),
            };
        }

        #region Items
        /// <summary>
        /// 容器
        /// </summary>
        private FContainer container;
        /// <summary>
        /// 玩家
        /// </summary>
        private Player player;
        /// <summary>
        /// 选项
        /// </summary>
        private MyOptions options;
        /// <summary>
        /// 物品图标列表
        /// </summary>
        private List<ObjectIcon> itemIcons = new List<ObjectIcon>();

        /// <summary>
        /// 图标大小
        /// </summary>
        int iconSize;

        /// <summary>
        /// 初始容量
        /// </summary>
        private const int INITIAL_CAPACITY = 5;
        /// <summary>
        /// HUD间距
        /// </summary>
        private const int HUD_GAP = 8;
        #endregion
    }
}