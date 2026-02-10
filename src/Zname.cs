using BepInEx;
using SlugBase.Features;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;

namespace CustomStomachStorage
{
	internal class Zname
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


	}
}
