using BepInEx;
using SlugBase.Features;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;

namespace CustomStomachStorage
{
	internal class Zname//Scrap 废案
	{

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
			text = Plugin.RemoveField(text, "my_simple_data");

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

			/*for (int i = 0; i < stomachContents.Count; i++)
			{
				UnityEngine.Debug.Log(stomachContents[i].ToString());
			}*/
		}

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




	}
}
