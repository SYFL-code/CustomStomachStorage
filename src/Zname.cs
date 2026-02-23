using BepInEx;
using SlugBase.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;

namespace CustomStomachStorage
{
	internal class Zname//Scrap 废案
	{
        #region Items
        #endregion
        #region Creatures
        #endregion


        //<DefineConstants>MYDEBUG</DefineConstants>

        /*public static readonly PlayerKeybind Explode = PlayerKeybind.Register(
				"example:explode",      // 唯一ID（格式：作者:功能）
				"Example Mod",          // 模组显示名称
				"Explode",              // 按键显示名称
				KeyCode.C,              // 键盘默认键（C键）
				KeyCode.JoystickButton3 // 手柄默认键（通常是RB或R1）
			);*/

        private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
		{
			if (self.room.world.game.rainWorld.ExpeditionMode)//在探险模式里开启冰盾能力
			{
				//GlobalVar.glacier2_iceshield_lock = false;
			}
			if (self.room.world.game.session is ArenaGameSession)//在竞技场模式里也开启冰盾能力
			{
				//GlobalVar.glacier2_iceshield_lock = false;
			}
		}


		private void A(Player player)
		{
            //mklink /j "D:\Steam\steamapps\common\Rain World\RainWorld_Data\StreamingAssets\mods\EnderPearl" "D:\Other\EnderPearl\mod"

            //存档字符串
            // 读取存档字符串：
            // "player_name<svB>玩家A<svA>level<svB>5<svA>coins<svB>100<svA>my_simple_data<svB>123<svA>"

            // 分割成：
            // ["player_name<svB>玩家A", "level<svB>5", "coins<svB>100", "my_simple_data<svB>123"]

            // 再分割每个部分：
            // "my_simple_data<svB>123" → ["my_simple_data", "123"]

            // 发现键是"my_simple_data"，值就是"123"

            //ID.-1.266<oB>0<oA>FlareBomb<oA>SL_S10.20.24.0<oA>-1<oA>-1，ID.-1.266<oB>0<oA>FlareBomb<oA>SL_S10.20.24.0<oA>-1<oA>-1，ID.-1.266<oB>0<oA>FlareBomb<oA>SL_S10.20.24.0<oA>-1<oA>-1

            //StomachStorage_ESS_SAVEFIELD<svB>Player0<svD>ID.-1.4206<oB>0<oA>OverseerCarcass<oA>HI_S05.16.16.0<oA>0.4470588<oA>0.9019608<oA>0.7686275<oA>0<oA>0,ID.-1.4206<oB>0<oA>OverseerCarcass<oA>HI_S05.16.16.0<oA>0.4470588<oA>0.9019608<oA>0.7686275<oA>0<oA>0,ID.-1.7342<oB>0<oA>DataPearl<oA>HI_S05.16.16.0<oA>131<oA>1<oA>Misc,ID.-1.7341<oB>0<oA>DataPearl<oA>HI_S05.16.16.0<oA>131<oA>0<oA>HI,ID.-1.3843<oB>0<oA>ScavengerBomb<oA>HI_S05.16.16.0,ID.-1.3840<oB>0<oA>ScavengerBomb<oA>HI_S05.16.16.0,ID.-1.3841<oB>0<oA>ScavengerBomb<oA>HI_S05.16.16.0<svC><svA><svA><svA>

            //层级 分隔符  作用
            //-------------------------------------------
            //顶级 <svA>   分隔主项
            //     <svB>   主项内的键值分隔
            //二级 <mwA>   分隔子项
            //	   <mwB>   子项内的键值分隔
            //三级 <slosA> 分隔子子项
            //     <slosB> 子子项内的键值分隔
            //四级 <svC>   分隔子子子项
            //	   <svD>   子子子项内的键值分隔

            // 最终保存格式：
            // ESS_savefield_name<svB>Player0<mwB>物品1,物品2<mwA>Player1<mwB>物品3<mwA><svA>

            //Debug
            //Debug.Log("普通消息");
            //Debug.LogWarning("警告消息");
            //Debug.LogError("错误消息");


            //room
            //player.room.game.Players
            //player.room.game.GetStorySession.Players
            //player.room.game.warpDeferPlayerSpawnRoomName
            //player.room.abstractRoom.name

        }

        //保存部分
        private static string SaveState_SaveToString(On.SaveState.orig_SaveToString orig, SaveState saveState)
		{
			// 获取原版存档
			string text = orig(saveState);

			// 移除原版存档中的"my_simple_data"字段
			text = State.RemoveField(text, "my_simple_data");

			// 保存"123"
			text += "my_simple_data<svB>123<svA>";

			return text;
		}
		//加载部分
		private static void SaveState_LoadGame(On.SaveState.orig_LoadGame orig, SaveState saveState, string str, RainWorldGame game)
		{
			// 先调用原版方法
			orig(saveState, str, game);

			// 查找保存的"123"数据
			string[] array = Regex.Split(str, "<svA>");
			foreach (var p in array)
			{
				string[] array2 = Regex.Split(p, "<svB>");
				if (array2.Length >= 2 && array2[0] == "my_simple_data")
				{
					// 找到并处理数据
					string savedData = array2[1];
					// 这里可以处理savedData（应该是"123"）
					UnityEngine.Debug.Log(savedData);
				}
			}
		}

		public static string GetSavePath(string modName = "CustomStomachStorage_Redlyn")
		{
			// 读取现有内容
			//string existingContent = File.ReadAllText(filePath);

			// 获取当前用户的LocalLow目录
			string localLowPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			localLowPath = Path.Combine(localLowPath, "..", "LocalLow");
			localLowPath = Path.GetFullPath(localLowPath);  // 规范化路径

			// 构建完整路径
			return Path.Combine(localLowPath, "Videocult", "Rain World", "ModConfigs", $"{modName}_data.txt");
		}

		private void Player_SwallowObject(On.Player.orig_SwallowObject orig, Player player, int grasp)
		{
			/*if (grasp < 0 || player.grasps[grasp] == null)
			{
				orig(player, grasp);
				return;
			}

			string filePath = GetSavePath();
			Console.WriteLine($"配置文件路径: {filePath}");

			string content = player.room.game.warpDeferPlayerSpawnRoomName + ":" + player.grasps[grasp].ToString() + "\n";
			File.WriteAllText(filePath, content);*/

			/*for (int i = 0; i < stomachItems.Count; i++)
			{
				UnityEngine.Debug.Log(stomachItems[i].ToString());
			}*/
		}

        /// <summary>
        /// 添加选择框的类型列
        /// </summary>
        /*private void AddCheckboxType(OpTab tab, ref float yPos, string title,
            IEnumerable<string> types, Dictionary<string, Configurable<bool>> configs,
            float xOffset = 0, string note = "")
        {
            if (!types.Any()) return;

            InGameTranslator translator = Custom.rainWorld.inGameTranslator;
            // 分类标题
            if (!string.IsNullOrEmpty(title))
            {
                var titleLabel = new OpLabel(TITLE_X + xOffset, yPos, translator.Translate(title), true)
                {
                    alignment = FLabelAlignment.Left
                };
                tab.AddItems(new UIelement[] { titleLabel });
                yPos -= SPACING * 1.5f;
            }
            float yTop = TITLE_Y;

            // 类型复选框
            foreach (var type in types)
            {
                if (!configs.ContainsKey(type)) continue;

                string displayName = translator.Translate(type);
                if (!string.IsNullOrEmpty(note))
                    displayName += $" {translator.Translate(note)}";

                displayName = translator.Translate(displayName);

                if (yPos < -1f)
                {
                    yPos = yTop;
                    xOffset += xOffset;
                }
                AddLabeledCheckbox(tab,
                    new Vector2(TITLE_X + xOffset, yPos),
                    displayName,
                    configs[type]);

                yPos -= SPACING;
            }

            yPos -= SPACING * 0.5f;
        }*/

        private void Options()
		{



			/*public readonly Configurable<bool> All;
//
public readonly Configurable<bool> Item;

public readonly Configurable<bool> Spear;
public readonly Configurable<bool> VultureMask;
public readonly Configurable<bool> NeedleEgg;

public readonly Configurable<bool> JokeRifle;
public readonly Configurable<bool> EnergyCell;
public readonly Configurable<bool> MoonCloak;

public readonly Configurable<bool> Boomerang;
//
public readonly Configurable<bool> Creature;
public readonly Configurable<bool> Lizard;
public readonly Configurable<bool> Vulture;
public readonly Configurable<bool> Centipede;
public readonly Configurable<bool> Spider;
public readonly Configurable<bool> DropBug;
public readonly Configurable<bool> BigEel;
public readonly Configurable<bool> MirosBird;
public readonly Configurable<bool> DaddyLongLegs;
public readonly Configurable<bool> Cicada;
public readonly Configurable<bool> Snail;
public readonly Configurable<bool> Scavenger;
public readonly Configurable<bool> LanternMouse;
public readonly Configurable<bool> JetFish;
public readonly Configurable<bool> TubeWorm;
public readonly Configurable<bool> Deer;
public readonly Configurable<bool> Yeek;
public readonly Configurable<bool> Inspector;
public readonly Configurable<bool> StowawayBug;
public readonly Configurable<bool> Loach;
public readonly Configurable<bool> BigMoth;
public readonly Configurable<bool> SkyWhale;
public readonly Configurable<bool> BoxWorm;
public readonly Configurable<bool> DrillCrab;
public readonly Configurable<bool> Tardigrade;
public readonly Configurable<bool> Barnacle;
public readonly Configurable<bool> Frog;*/

			/*private readonly List<Configurable<bool>> ItemsTypesList = new List<Configurable<bool>>();
		private readonly List<string> ItemsTypeName = new List<string>();
		private readonly List<Configurable<bool>> CreaturesTypesList = new List<Configurable<bool>>();
		private readonly List<string> CreaturesTypeName = new List<string>();*/

			/*ui.Add(new OpCheckBox(All, new Vector2(Title_X + (Creature_X / 2), OpBox_Y)));
ui.Add(new OpLabel(new Vector2(OpLabel_X + (Creature_X / 2), OpBox_Y), new Vector2(200f, 24f),
                               inGameTranslator.Translate("All"),
                               FLabelAlignment.Left, false, null));

for (int i = 0; i < ItemsTypesList.Count; i++)
{
    float yPos = OpBox_Y - (spacing * (i + 1));

    // 创建复选框
    OpCheckBox checkBox = new OpCheckBox(ItemsTypesList[i], new Vector2(Title_X, yPos));

    // 创建标签（使用存储的类型名称）
    OpLabel label = new OpLabel(new Vector2(OpLabel_X, yPos), new Vector2(200f, 24f),
                               inGameTranslator.Translate(ItemsTypeName[i]),
                               FLabelAlignment.Left, false, null);
    ui.Add(checkBox);
    ui.Add(label);
}

for (int i = 0; i < CreaturesTypesList.Count; i++)
{
    float yPos = OpBox_Y - (spacing * (i + 1));

    OpCheckBox checkBox;
    OpLabel label;
    if (i <= 14)
    {
        yPos = OpBox_Y - (spacing * (i + 1));
        // 创建复选框
        checkBox = new OpCheckBox(CreaturesTypesList[i], new Vector2(Title_X + Creature_X, yPos));

        // 创建标签（使用存储的类型名称）
        label = new OpLabel(new Vector2(OpLabel_X + Creature_X, yPos), new Vector2(200f, 24f),
                                   inGameTranslator.Translate(CreaturesTypeName[i]),
                                   FLabelAlignment.Left, false, null);
    }
    else
    {
        yPos = OpBox_Y - (spacing * (i + 1 - 15));
        // 创建复选框
        checkBox = new OpCheckBox(CreaturesTypesList[i], new Vector2(Title_X + (Creature_X * 2), yPos));

        // 创建标签（使用存储的类型名称）
        label = new OpLabel(new Vector2(OpLabel_X + (Creature_X * 2), yPos), new Vector2(200f, 24f),
                                   inGameTranslator.Translate(CreaturesTypeName[i]),
                                   FLabelAlignment.Left, false, null);
    }

    ui.Add(checkBox);
    ui.Add(label);
}*/

			/*typeTab.AddItems(new UIelement[]
{
    new OpCheckBox(Spear, new Vector2(Title_X, OpBox_Y - 0f)),
    new OpLabel(new Vector2(OpLabel_X, OpBox_Y - 0f), new Vector2(200f, 24f), inGameTranslator.Translate("Spear"), FLabelAlignment.Left, false, null),

    new OpCheckBox(VultureMask, new Vector2(Title_X, OpBox_Y - 40f)),
    new OpLabel(new Vector2(OpLabel_X, OpBox_Y - 40f), new Vector2(200f, 24f), inGameTranslator.Translate("VultureMask"), FLabelAlignment.Left, false, null),

    new OpCheckBox(NeedleEgg, new Vector2(Title_X, OpBox_Y - 80f)),
    new OpLabel(new Vector2(OpLabel_X, OpBox_Y - 80f), new Vector2(200f, 24f), inGameTranslator.Translate("NeedleEgg"), FLabelAlignment.Left, false, null),

    new OpCheckBox(Boomerang, new Vector2(Title_X, OpBox_Y - 120f)),
    new OpLabel(new Vector2(OpLabel_X, OpBox_Y - 120f), new Vector2(200f, 24f), inGameTranslator.Translate("Boomerang"), FLabelAlignment.Left, false, null),

    new OpCheckBox(JokeRifle, new Vector2(Title_X, OpBox_Y - 160f)),
    new OpLabel(new Vector2(OpLabel_X, OpBox_Y - 160f), new Vector2(200f, 24f), inGameTranslator.Translate("JokeRifle"), FLabelAlignment.Left, false, null),

    //new OpCheckBox(OpCheckBoxStunDuration, new Vector2(10, 420)),
    */

			//选项
			/*opTab.AddItems(new UIelement[]
            {
				new OpCheckBox(OpCheckBoxStunDuration, new Vector2(10, 420)),
				new OpTextBox(OpCheckBoxStunDuration, new Vector2(10, 420), 50f),
				new OpLabel(new Vector2(75f, 420f), new Vector2(200f, 24f), inGameTranslator.Translate("Stun duration"), FLabelAlignment.Left, false, null),

				new OpCheckBox(OpCheckBoxSaveIceData_conf, new Vector2(10, 390)),
				new OpLabel(new Vector2(50f, 390f), new Vector2(200f, 24f), inGameTranslator.Translate("Save Ice data to the next cycle(Save bug not fixed yet)"), FLabelAlignment.Left, false, null),
				new OpCheckBox(OpCheckBoxUnlockIceShieldNum_conf, new Vector2(10, 360)),
				new OpLabel(new Vector2(50f, 360f), new Vector2(200f, 24f), inGameTranslator.Translate("Unlock the maximum number of ice shields"), FLabelAlignment.Left, false, null),

				new OpLabel(new Vector2(50f, 420f), new Vector2(200f, 24f), inGameTranslator.Translate("If scavenger dies, the players continue playing"), FLabelAlignment.Left, false, null),
				radioButtonGroup,
				radioButton1,
				radioButton2
			});*/
		}

		private void InitializeSwallowType(string typeName, bool defaultValue = false)
		{
			//SwallowTypes[typeName] = config.Bind<bool>($"{typeName}_conf_CustomStomachStorage", defaultValue);

			/*// 获取字段
			FieldInfo field = typeof(Options).GetField(typeName,
				BindingFlags.Public | BindingFlags.Instance);

			if (field != null)
			{
				// 创建配置项
				Configurable<bool> configurable = config.Bind<bool>(
					$"{typeName}_conf_CustomStomachStorage",
					defaultValue);

				// 设置字段值
				field.SetValue(this, configurable);

				// 添加到列表
				if (ItemTypeNames.Contains(typeName))
				{
					ItemsTypesList.Add(configurable);
					ItemsTypeName.Add(typeName);
				}
				if (CreatureTypeNames.Contains(typeName))
				{
					CreaturesTypesList.Add(configurable);
					CreaturesTypeName.Add(typeName);
				}

			}*/
		}


        /*// 类型选项卡
            List<UIelement> S_ui = new List<UIelement>();

			float S_ItemPos_Y = OpBox_Y;
			int S_Itemi = 0;

			float S_CreaturePos_Y = OpBox_Y;
			int S_Creaturei = 0;

			HashSet<string> itemSet = new HashSet<string>(ItemTypeNames);
			HashSet<string> creatureSet = new HashSet<string>(CreatureTypeNames);

			foreach (var kvp in SwallowTypes)
			{
				OpCheckBox? cb = null;
				OpLabel? label = null;

				if (kvp.Key == "All")
				{
					cb = new OpCheckBox(kvp.Value, new Vector2(Title_X, OpBox_Y));
					label = new OpLabel(new Vector2(OpLabel_X, OpBox_Y), new Vector2(200f, 24f),
												inGameTranslator.Translate(kvp.Key),
												FLabelAlignment.Left, false, null);
				}

				else if (itemSet.Contains(kvp.Key))
				{
					S_ItemPos_Y = OpBox_Y - (spacing * (S_Itemi + 1));

					// 创建复选框
					cb = new OpCheckBox(kvp.Value, new Vector2(Title_X, S_ItemPos_Y));

					// 创建标签（使用存储的类型名称）
					label = new OpLabel(new Vector2(OpLabel_X, S_ItemPos_Y), new Vector2(200f, 24f),
											   inGameTranslator.Translate(kvp.Key),
											   FLabelAlignment.Left, false, null);
					S_Itemi += 1;
				}

				else if (creatureSet.Contains(kvp.Key))
				{
					if (S_Creaturei <= 14)
					{
						S_CreaturePos_Y = OpBox_Y - (spacing * (S_Creaturei + 1));
						// 创建复选框
						cb = new OpCheckBox(kvp.Value, new Vector2(Title_X + Creature_X, S_CreaturePos_Y));

						// 创建标签（使用存储的类型名称）
						label = new OpLabel(new Vector2(OpLabel_X + Creature_X, S_CreaturePos_Y), new Vector2(200f, 24f),
												   inGameTranslator.Translate(kvp.Key),
												   FLabelAlignment.Left, false, null);
					}
					else
					{
						S_CreaturePos_Y = OpBox_Y - (spacing * (S_Creaturei + 1 - 15));

						// 创建复选框
						cb = new OpCheckBox(kvp.Value, new Vector2(Title_X + (Creature_X * 2), S_CreaturePos_Y));

						// 创建标签（使用存储的类型名称）
						label = new OpLabel(new Vector2(OpLabel_X + (Creature_X * 2), S_CreaturePos_Y), new Vector2(200f, 24f),
												   inGameTranslator.Translate(kvp.Key),
												   FLabelAlignment.Left, false, null);
					}

					S_Creaturei += 1;
				}

				if (cb != null && label != null)
				{
					S_ui.Add(cb);
					S_ui.Add(label);
				}
			}

			// 添加到类型选项卡
			SwallowTypeTab.AddItems(S_ui.ToArray());



			// 抓握类型标题
			GrabTypeTab.AddItems(new UIelement[]
			{
				new OpLabel(Title_X, Title_Y, inGameTranslator.Translate("Types of graspable"), true)
				{
					alignment = FLabelAlignment.Left
				}
			});

			// 类型选项卡
			List<UIelement> G_ui = new List<UIelement>();

			float G_ItemPos_Y = OpBox_Y;
			int G_Itemi = 0;

			float G_CreaturePos_Y = OpBox_Y;
			int G_Creaturei = 0;

			foreach (var kvp in GrabTypes)
			{
				OpCheckBox? cb = null;
				OpLabel? label = null;

				if (kvp.Key == "OneHandGrabAll")
				{
					cb = new OpCheckBox(kvp.Value, new Vector2(Title_X, OpBox_Y));
					label = new OpLabel(new Vector2(OpLabel_X, OpBox_Y), new Vector2(200f, 24f),
												inGameTranslator.Translate("One hand can grab all"),
												FLabelAlignment.Left, false, null);
				}
				if (kvp.Key == "DragGrabAll")
				{
					cb = new OpCheckBox(kvp.Value, new Vector2(Title_X + Creature_X, OpBox_Y));
					label = new OpLabel(new Vector2(OpLabel_X + Creature_X, OpBox_Y), new Vector2(200f, 24f),
												inGameTranslator.Translate("At least able to drag all"),
												FLabelAlignment.Left, false, null);
				}

				else if (itemSet.Contains(kvp.Key))
				{
					G_ItemPos_Y = OpBox_Y - (spacing * (G_Itemi + 1));

					// 创建复选框
					cb = new OpCheckBox(kvp.Value, new Vector2(Title_X, G_ItemPos_Y));

					// 创建标签（使用存储的类型名称）
					label = new OpLabel(new Vector2(OpLabel_X, G_ItemPos_Y), new Vector2(200f, 24f),
											   inGameTranslator.Translate(kvp.Key),
											   FLabelAlignment.Left, false, null);
					G_Itemi += 1;
				}

				else if (creatureSet.Contains(kvp.Key))
				{
					if (G_Creaturei <= 14)
					{
						G_CreaturePos_Y = OpBox_Y - (spacing * (G_Creaturei + 1));
						// 创建复选框
						cb = new OpCheckBox(kvp.Value, new Vector2(Title_X + Creature_X, G_CreaturePos_Y));

						// 创建标签（使用存储的类型名称）
						label = new OpLabel(new Vector2(OpLabel_X + Creature_X, G_CreaturePos_Y), new Vector2(200f, 24f),
												   inGameTranslator.Translate(kvp.Key),
												   FLabelAlignment.Left, false, null);
					}
					else
					{
						G_CreaturePos_Y = OpBox_Y - (spacing * (G_Creaturei + 1 - 15));

						// 创建复选框
						cb = new OpCheckBox(kvp.Value, new Vector2(Title_X + (Creature_X * 2), G_CreaturePos_Y));

						// 创建标签（使用存储的类型名称）
						label = new OpLabel(new Vector2(OpLabel_X + (Creature_X * 2), G_CreaturePos_Y), new Vector2(200f, 24f),
												   inGameTranslator.Translate(kvp.Key),
												   FLabelAlignment.Left, false, null);
					}

					G_Creaturei += 1;
				}

				if (cb != null && label != null)
				{
					G_ui.Add(cb);
					G_ui.Add(label);
				}
			}

			// 添加到类型选项卡
			GrabTypeTab.AddItems(G_ui.ToArray());*/




        // 基础类型
        /*foreach (string typeName in otherTypeNames)
            InitializeSwallowType(typeName, false);
        foreach (string typeName in baseItemTypes)
            InitializeSwallowType(typeName, false);
        foreach (string typeName in baseCreatureTypes)
            InitializeSwallowType(typeName, false);
        ItemTypeNames.UnionWith(baseItemTypes);
        CreatureTypeNames.UnionWith(baseCreatureTypes);
        // MSC类型
        if (ModManager.MSC)
        {
            foreach (string typeName in mscItemTypes)
                InitializeSwallowType(typeName, false);
            foreach (string typeName in mscCreatureTypes)
                InitializeSwallowType(typeName, false);
            ItemTypeNames.UnionWith(mscItemTypes);
            CreatureTypeNames.UnionWith(mscCreatureTypes);
        }
        // Watcher类型
        if (ModManager.Watcher)
        {
            foreach (string typeName in watcherItemTypes)
                InitializeSwallowType(typeName, false);
            foreach (string typeName in watcherCreatureTypes)
                InitializeSwallowType(typeName, false);
            ItemTypeNames.UnionWith(watcherItemTypes);
            CreatureTypeNames.UnionWith(watcherCreatureTypes);
        }*/



        #region Items
        /*namespace CustomStomachStorage
        {
            public class MyOptions : OptionInterface
            {
                public static readonly MyOptions Instance = new MyOptions();

                public readonly Configurable<int> StomachCapacity;
                public readonly Configurable<bool> DebugMode;
                public readonly Configurable<bool> SpearmasterStoreItems;

                public readonly Dictionary<string, Configurable<bool>> SwallowConfigs = new();
                public readonly Dictionary<string, Configurable<string>> GrabConfigs = new();
                public readonly Dictionary<string, Configurable<bool>> GrabSpecialConfigs = new();

                public HashSet<string> ItemTypeNames { get; private set; } = new();
                public HashSet<string> CreatureTypeNames { get; private set; } = new();

                // 抓取模式枚举
                public enum GrabMode
                {
                    NotSelected,    // 未选择
                    OneHand,        // 单手可抓
                    BigOneHand,     // 单手但更大
                    TwoHands,       // 双手抓
                    Drag,           // 拖动
                    CantGrab,       // 不能抓取
                }
                // 抓取模式显示文本
                private static readonly Dictionary<GrabMode, string> GrabModeDisplay = new()
                {
                    { GrabMode.NotSelected, "NotSelected" },
                    { GrabMode.OneHand, "OneHand" },
                    { GrabMode.BigOneHand, "BigOneHand" },
                    { GrabMode.TwoHands, "TwoHands" },
                    { GrabMode.Drag, "Drag" },
                    { GrabMode.CantGrab, "CantGrab" },
                };
                private static readonly Dictionary<GrabMode, Player.ObjectGrabability> PlayerGrabDisplay = new()
                {
                    { GrabMode.NotSelected, Player.ObjectGrabability.CantGrab },
                    { GrabMode.OneHand, Player.ObjectGrabability.OneHand },
                    { GrabMode.BigOneHand, Player.ObjectGrabability.BigOneHand },
                    { GrabMode.TwoHands, Player.ObjectGrabability.TwoHands },
                    { GrabMode.Drag, Player.ObjectGrabability.Drag },
                    { GrabMode.CantGrab, Player.ObjectGrabability.CantGrab },
                };
                public List<ListItem> GrabModeItem = new List<ListItem> {
                    new ListItem("Not selected", "NotSelected"),
                    new ListItem("One hand", "OneHand"),
                    new ListItem("Big one hand", "BigOneHand"),
                    new ListItem("Two hands", "TwoHands"),
                    new ListItem("Drag", "Drag"),
                    new ListItem("Cannot grab", "CantGrab"),
                };


                /*public List<ListItem> GrabModeItem = new List<ListItem> {
			new ListItem("Not selected", NotSelected),
			new ListItem("One hand", "OneHand"),
			new ListItem("Big one hand", "BigOneHand"),
			new ListItem("Two hands", "TwoHands"),
			new ListItem("Drag", "Drag"),
			new ListItem("Cannot grab", "CantGrab"),
		};
		private static readonly Dictionary<string, Player.ObjectGrabability> PlayerGrabDisplay = new()
		{
			{ NotSelected, Player.ObjectGrabability.CantGrab },
			{ "OneHand", Player.ObjectGrabability.OneHand },
			{ "BigOneHand", Player.ObjectGrabability.BigOneHand },
			{ "TwoHands", Player.ObjectGrabability.TwoHands },
			{ "Drag", Player.ObjectGrabability.Drag },
			{ "CantGrab", Player.ObjectGrabability.CantGrab },
		};

		// ============ 访问器 ============
		public bool CanSwallow(string typeName) =>
			SwallowTypes.TryGetValue(typeName, out var config) && config.Value == true;
		public bool GetGrabSpecial(string typeName) =>
			GrabSpecialTypes.TryGetValue(typeName, out var config) && config.Value == true;
		public string GetGrabMode(string typeName) =>
			GrabTypes.TryGetValue(typeName, out var config) ? config.Value : NotSelected;
        public Player.ObjectGrabability GetPlayerGrab(string mode) =>
            PlayerGrabDisplay.TryGetValue(mode, out var value) ? value : Player.ObjectGrabability.CantGrab;*/

        // ============ 访问器 ============
        /*public bool CanSwallow(string type) =>
                    SwallowConfigs.TryGetValue(type, out var config) && config.Value == true;
                public bool GetGrabSpecial(string type) =>
                    GrabSpecialConfigs.TryGetValue(type, out var config) && config.Value == true;
                public GrabMode GetGrabMode(string type) =>
                    GrabConfigs.TryGetValue(type, out var config) ? GrabModeFromString(config.Value) : GrabMode.NotSelected;
                public string GrabModeToString(GrabMode mode) => GrabModeDisplay[mode];
                public Player.ObjectGrabability GetPlayerGrab(GrabMode mode) => PlayerGrabDisplay[mode];
                public GrabMode GrabModeFromString(string value)
                {
                    foreach (var pair in GrabModeDisplay)
                        if (pair.Value == value)
                            return pair.Key;
                    return GrabMode.NotSelected;
                }

                string[] specialTypeNames = {//45
                        "All",

                        "OneHandGrabAll",
                        "DragGrabAll",
                    };

                #region Items
                string[] baseItemTypes = {
                        "Item",
                        "Spear", 
                        "VultureMask", 
                        "NeedleEgg",
                        "OracleSwarmer", 
                        "SeedCob"
                    };
                string[] mscItemTypes = {
                        "LillyPuck",
                        "FireEgg", 
                        "JokeRifle", 
                        "EnergyCell",
                        "MoonCloak"
                    };
                string[] watcherItemTypes = {
                        "Boomerang",
                        "GraffitiBomb"
                    };
                #endregion
                #region Creature
                string[] baseCreatureTypes = {
                        "Creature",
                        "Lizard",
                        "Vulture",
                        "Centipede", 
                        "Spider", 
                        "DropBug",
                        "BigEel", 
                        "MirosBird", 
                        "DaddyLongLegs",
                        "Cicada", 
                        "Snail",
                        "Scavenger", 
                        "EggBug", 
                        "LanternMouse", 
                        "JetFish", 
                        "TubeWorm",
                        "Deer", 
                        "TempleGuard"
                    };
                string[] mscCreatureTypes = {
                        "Yeek", 
                        "Inspector", 
                        "StowawayBug"
                    };
                string[] watcherCreatureTypes = {
                        "Loach",
                        "BigMoth", 
                        "SkyWhale", 
                        "BoxWorm", 
                        "DrillCrab",
                        "Tardigrade",
                        "Barnacle", 
                        "Frog"
                    };
                #endregion

                // 常量

                MyOptions()
                {
                    //设置默认值
                    StomachCapacity = config.Bind<int>($"StomachCapacity_conf_{MOD_name}", 3, new ConfigAcceptableRange<int>(0, 500));

                    DebugMode = config.Bind<bool>($"DebugMode_conf_{MOD_name}", false);

                    SpearmasterStoreItems = config.Bind<bool>($"SpearmasterStoreItems_conf_{MOD_name}", false);

                    // 基础类型
                    foreach (string typeName in specialTypeNames)
                    {
                        InitializeSwallowType(typeName, false);
                        GrabSpecialConfigs[typeName] = config.Bind<bool>($"{typeName}_Grab_conf_{MOD_name}", false);
                    }
                    foreach (string typeName in baseItemTypes)
                        InitializeSwallowType(typeName, false);
                    foreach (string typeName in baseCreatureTypes)
                        InitializeSwallowType(typeName, false);
                    ItemTypeNames.UnionWith(baseItemTypes);
                    CreatureTypeNames.UnionWith(baseCreatureTypes);
                    // MSC类型
                    if (ModManager.MSC)
                    {
                        foreach (string typeName in mscItemTypes)
                            InitializeSwallowType(typeName, false);
                        foreach (string typeName in mscCreatureTypes)
                            InitializeSwallowType(typeName, false);
                        ItemTypeNames.UnionWith(mscItemTypes);
                        CreatureTypeNames.UnionWith(mscCreatureTypes);
                    }
                    // Watcher类型
                    if (ModManager.Watcher)
                    {
                        foreach (string typeName in watcherItemTypes)
                            InitializeSwallowType(typeName, false);
                        foreach (string typeName in watcherCreatureTypes)
                            InitializeSwallowType(typeName, false);
                        ItemTypeNames.UnionWith(watcherItemTypes);
                        CreatureTypeNames.UnionWith(watcherCreatureTypes);
                    }
                }

                private void InitializeSwallowType(string typeName, bool defaultValue = false)
                {
                    SwallowConfigs[typeName] = config.Bind<bool>($"{typeName}_Swallow_conf_{MOD_name}", defaultValue);
                    //SwallowTypes[typeName] = config.Bind<bool>($"{typeName}_Swallow_conf_{MOD_name}", defaultValue);
                    GrabConfigs[typeName] = config.Bind<string>($"{typeName}_Grab_conf_{MOD_name}", GrabModeToString(GrabMode.NotSelected));

                    *//*if (!ItemTypeNames.Contains(typeName))
                    {
                        GrabTypes[typeName] = config.Bind<bool>($"{typeName}_Grab_conf_{MOD_name}", defaultValue);
                    }*//*
                }


                public override void Initialize()
                {
                    // 创建选项卡
                    var optionsTab = new OpTab(this, "Options");
                    var swallowTab = new OpTab(this, "Swallowing");
                    var grabTab = new OpTab(this, "Grasping");
                    InGameTranslator inGameTranslator = Custom.rainWorld.inGameTranslator;
                    this.Tabs = new OpTab[]
                    {
                        optionsTab,
                        swallowTab,
                        grabTab,
                    };

                    const float Title_X = 10f;
                    const float Title_Y = 560;

                    const float Creature_X = 230f;

                    const float OpBox_Y = 450f;
                    const float OpLabel_X = 75f;

                    const float spacing = 30f; // 元素间距

                    const float ComboBox_width = 50f;

                    // 标题
                    optionsTab.AddItems(new UIelement[]
                    {
                        new OpLabel(Title_X, Title_Y, inGameTranslator.Translate("Custom Stomach Storage"), true)
                        {
                            alignment = FLabelAlignment.Left
                        }
                    });

                    //选项
                    optionsTab.AddItems(new UIelement[]
                    {
                        new OpTextBox(StomachCapacity, new Vector2(Title_X, OpBox_Y - 0f), 50f),
                        new OpLabel(new Vector2(OpLabel_X, OpBox_Y - 0f), new Vector2(200f, 24f), inGameTranslator.Translate("Stomach capacity"), FLabelAlignment.Left, false, null),

                        new OpCheckBox(SpearmasterStoreItems, new Vector2(Title_X, OpBox_Y - spacing)),
                        new OpLabel(new Vector2(OpLabel_X, OpBox_Y - spacing), new Vector2(200f, 24f), inGameTranslator.Translate("Allow Spearmaster to store items"), FLabelAlignment.Left, false, null),

                        new OpCheckBox(DebugMode, new Vector2(Title_X, 40f)),
                        new OpLabel(new Vector2(OpLabel_X, 40f), new Vector2(200f, 24f), inGameTranslator.Translate("DebugMode"), FLabelAlignment.Left, false, null)
                    });




                    // 吞咽类型标题
                    swallowTab.AddItems(new UIelement[]
                    {
                        new OpLabel(Title_X, Title_Y, inGameTranslator.Translate("Types of swallowing"), true)
                        {
                            alignment = FLabelAlignment.Left
                        }
                    });

                    // 类型选项卡
                    List<UIelement> S_ui = new List<UIelement>();

                    float S_ItemPos_Y = OpBox_Y;
                    int S_Itemi = 0;

                    float S_CreaturePos_Y = OpBox_Y;
                    int S_Creaturei = 0;

                    HashSet<string> itemSet = new HashSet<string>(ItemTypeNames);
                    HashSet<string> creatureSet = new HashSet<string>(CreatureTypeNames);

                    foreach (var kvp in SwallowConfigs)
                    {
                        OpCheckBox? cb = null;
                        OpLabel? label = null;

                        if (kvp.Key == "All")
                        {
                            cb = new OpCheckBox(kvp.Value, new Vector2(Title_X, OpBox_Y));
                            label = new OpLabel(new Vector2(OpLabel_X, OpBox_Y), new Vector2(200f, 24f),
                                                        inGameTranslator.Translate(kvp.Key),
                                                        FLabelAlignment.Left, false, null);
                        }

                        else if (itemSet.Contains(kvp.Key))
                        {
                            S_ItemPos_Y = OpBox_Y - (spacing * (S_Itemi + 1));

                            // 创建复选框
                            cb = new OpCheckBox(kvp.Value, new Vector2(Title_X, S_ItemPos_Y));

                            // 创建标签（使用存储的类型名称）
                            label = new OpLabel(new Vector2(OpLabel_X, S_ItemPos_Y), new Vector2(200f, 24f),
                                                       inGameTranslator.Translate(kvp.Key),
                                                       FLabelAlignment.Left, false, null);
                            S_Itemi += 1;
                        }

                        else if (creatureSet.Contains(kvp.Key))
                        {
                            if (S_Creaturei <= 14)
                            {
                                S_CreaturePos_Y = OpBox_Y - (spacing * (S_Creaturei + 1));
                                // 创建复选框
                                cb = new OpCheckBox(kvp.Value, new Vector2(Title_X + Creature_X, S_CreaturePos_Y));

                                // 创建标签（使用存储的类型名称）
                                label = new OpLabel(new Vector2(OpLabel_X + Creature_X, S_CreaturePos_Y), new Vector2(200f, 24f),
                                                           inGameTranslator.Translate(kvp.Key),
                                                           FLabelAlignment.Left, false, null);
                            }
                            else
                            {
                                S_CreaturePos_Y = OpBox_Y - (spacing * (S_Creaturei + 1 - 15));

                                // 创建复选框
                                cb = new OpCheckBox(kvp.Value, new Vector2(Title_X + (Creature_X * 2), S_CreaturePos_Y));

                                // 创建标签（使用存储的类型名称）
                                label = new OpLabel(new Vector2(OpLabel_X + (Creature_X * 2), S_CreaturePos_Y), new Vector2(200f, 24f),
                                                           inGameTranslator.Translate(kvp.Key),
                                                           FLabelAlignment.Left, false, null);
                            }

                            S_Creaturei += 1;
                        }

                        if (cb != null && label != null)
                        {
                            S_ui.Add(cb);
                            S_ui.Add(label);
                        }
                    }

                    // 添加到类型选项卡
                    swallowTab.AddItems(S_ui.ToArray());



                    // 抓握类型标题
                    grabTab.AddItems(new UIelement[]
                    {
                        new OpLabel(Title_X, Title_Y, inGameTranslator.Translate("Types of graspable"), true)
                        {
                            alignment = FLabelAlignment.Left
                        }
                    });

                    // 类型选项卡
                    List<UIelement> G_ui = new List<UIelement>();

                    float G_ItemPos_Y = OpBox_Y;
                    int G_Itemi = 0;

                    float G_CreaturePos_Y = OpBox_Y;
                    int G_Creaturei = 0;

                    foreach (var kvp in GrabSpecialConfigs)
                    {
                        UIconfig? cb = null;
                        OpLabel? label = null;

                        if (kvp.Key == "OneHandGrabAll")
                        {
                            cb = new OpCheckBox(kvp.Value, new Vector2(Title_X, OpBox_Y));
                            label = new OpLabel(new Vector2(OpLabel_X, OpBox_Y), new Vector2(200f, 24f),
                                                        inGameTranslator.Translate("One hand can grab all"),
                                                        FLabelAlignment.Left, false, null);
                        }
                        if (kvp.Key == "DragGrabAll")
                        {
                            cb = new OpCheckBox(kvp.Value, new Vector2(Title_X + Creature_X, OpBox_Y));
                            label = new OpLabel(new Vector2(OpLabel_X + Creature_X, OpBox_Y), new Vector2(200f, 24f),
                                                        inGameTranslator.Translate("At least able to drag all"),
                                                        FLabelAlignment.Left, false, null);
                        }

                        if (cb != null && label != null)
                        {
                            G_ui.Add(cb);
                            G_ui.Add(label);
                        }
                    }

                    foreach (var kvp in GrabConfigs)
                    {
                        UIconfig? cb = null;
                        OpLabel? label = null;

                        if (itemSet.Contains(kvp.Key))
                        {
                            G_ItemPos_Y = OpBox_Y - (spacing * (G_Itemi + 1));

                            // 创建复选框
                            cb = new OpComboBox(kvp.Value, new Vector2(Title_X, G_ItemPos_Y), ComboBox_width, GrabModeItem);

                            // 创建标签（使用存储的类型名称）
                            label = new OpLabel(new Vector2(OpLabel_X, G_ItemPos_Y), new Vector2(200f, 24f),
                                                       inGameTranslator.Translate(kvp.Key),
                                                       FLabelAlignment.Left, false, null);
                            G_Itemi += 1;
                        }

                        else if (creatureSet.Contains(kvp.Key))
                        {
                            if (G_Creaturei <= 14)
                            {
                                G_CreaturePos_Y = OpBox_Y - (spacing * (G_Creaturei + 1));
                                // 创建复选框
                                cb = new OpComboBox(kvp.Value, new Vector2(Title_X + Creature_X, G_CreaturePos_Y), ComboBox_width, GrabModeItem);

                                // 创建标签（使用存储的类型名称）
                                label = new OpLabel(new Vector2(OpLabel_X + Creature_X, G_CreaturePos_Y), new Vector2(200f, 24f),
                                                           inGameTranslator.Translate(kvp.Key),
                                                           FLabelAlignment.Left, false, null);
                            }
                            else
                            {
                                G_CreaturePos_Y = OpBox_Y - (spacing * (G_Creaturei + 1 - 15));

                                // 创建复选框
                                cb = new OpComboBox(kvp.Value, new Vector2(Title_X + (Creature_X * 2), G_CreaturePos_Y), ComboBox_width, GrabModeItem);

                                // 创建标签（使用存储的类型名称）
                                label = new OpLabel(new Vector2(OpLabel_X + (Creature_X * 2), G_CreaturePos_Y), new Vector2(200f, 24f),
                                                           inGameTranslator.Translate(kvp.Key),
                                                           FLabelAlignment.Left, false, null);
                            }

                            G_Creaturei += 1;
                        }

                        if (cb != null && label != null)
                        {
                            G_ui.Add(cb);
                            G_ui.Add(label);
                        }
                    }

                    // 添加到类型选项卡
                    grabTab.AddItems(G_ui.ToArray());

                }


            }
        }*/
        #endregion



    }
}
