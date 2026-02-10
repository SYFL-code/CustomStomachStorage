using BepInEx;
using CoralBrain;
using Expedition;
using HUD;
using JollyCoop;
using JollyCoop.JollyMenu;
using MoreSlugcats;
using Noise;
using RWCustom;
using SlugBase;
using SlugBase.Features;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
//using UDebug =  UnityEngine.Debug;


namespace CustomStomachStorage
{
	[BepInPlugin(MOD_ID, "Custom Stomach Storage", "0.1.0")]
	class Plugin : BaseUnityPlugin
	{
		public const string MOD_ID = "CustomStomachStorage.Redlyn";


		// Add hooks-添加钩子
		public void OnEnable()
		{
			playerStomachsDict = new Dictionary<int, List<string>>();
			gameRef = null;

			On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

			// Put your custom hooks here!-在此放置你自己的钩子
			On.PlayerGraphics.Update += PlayerGraphics_Update;
			On.Player.GrabUpdate += Player_GrabUpdate;
			On.Player.SwallowObject += Player_SwallowObject;
			On.Player.Regurgitate += Player_Regurgitate;
			//On.SaveState.BringUpToDate += SaveState_BringUpToDate;
			//On.Player.SaveStomachObjectInPlayerState += Player_SaveStomachObjectInPlayerState;
			On.Player.ctor += Player_ctor;
			//On.Player.Update += Player_Update;
			//On.Player.Jump += Player_Jump;    
			//在玩家触发跳跃时执行Player_Jump
			//On.Player.Die += Player_Die;
			//On.Lizard.ctor += Lizard_ctor;

			//保存数据
			On.SaveState.SaveToString += SaveState_SaveToString;
			//读取数据
			On.SaveState.LoadGame += SaveState_LoadGame;
		}


		// add this to do the opposite of whatever you did in OnEnable()
		//添加此命令以执行与您在Enable()中所做的相反的操作
		// otherwise you'll wind up with two methods being called
		//否则，您将会调用两个方法
		public void OnDisable()
		{
			On.RainWorld.OnModsInit -= Extras.WrapInit(LoadResources);

			// Put your custom hooks here!-在此放置你自己的钩子
			On.PlayerGraphics.Update -= PlayerGraphics_Update;
			On.Player.GrabUpdate -= Player_GrabUpdate;
			On.Player.SwallowObject -= Player_SwallowObject;
			On.Player.Regurgitate -= Player_Regurgitate;
			//On.SaveState.BringUpToDate -= SaveState_BringUpToDate;
			//On.Player.SaveStomachObjectInPlayerState -= Player_SaveStomachObjectInPlayerState;
			On.Player.ctor -= Player_ctor;
			//On.Player.Update -= Player_Update;
			//On.Player.Jump -= Player_Jump;
			//On.Player.Die -= Player_Die;
			//On.Lizard.ctor -= Lizard_ctor;

			//保存数据
			On.SaveState.SaveToString -= SaveState_SaveToString;
			//读取数据
			On.SaveState.LoadGame -= SaveState_LoadGame;

			playerStomachsDict?.Clear();
			gameRef = null;
		}


		// Load any resources, such as sprites or sounds-加载任何资源 包括图像素材和音效
		private void LoadResources(RainWorld rainWorld)
		{
			MachineConnector.SetRegisteredOI(MOD_ID, Options.Instance);
		}



		// 管理扩展的胃部存储
		public class ESS//ExtendedStomachStorage
		{
			public static readonly ConditionalWeakTable<Player, List<AbstractPhysicalObject>> stomachContents =
				new ConditionalWeakTable<Player, List<AbstractPhysicalObject>>();

			public static List<AbstractPhysicalObject> GetStomachContents(Player player)
			{
				if (!stomachContents.TryGetValue(player, out var contents))
				{
					contents = new List<AbstractPhysicalObject>();
					stomachContents.Add(player, contents);
				}
				return contents;
			}

			// 获取第一个物品
			public static AbstractPhysicalObject GetFirstStomachItem(Player player)
			{
				var contents = GetStomachContents(player);
				return contents.Count > 0 ? contents[0] : null;
			}

			// 获取最后一个物品
			public static AbstractPhysicalObject GetLastStomachItem(Player player)
			{
				var contents = GetStomachContents(player);
				return contents.Count > 0 ? contents[contents.Count - 1] : null;
			}

			// 移除指定位置的物品
			public static AbstractPhysicalObject RemoveStomachItem(Player player, int index)
			{
				var contents = GetStomachContents(player);
				if (index >= 0 && index < contents.Count)
				{
					var item = contents[index];
					contents.RemoveAt(index);

					return item;
				}
				return null;
			}

			// 获取胃部容量
			public static int GetStomachCapacity(Player player)
			{
				int Capacity = 3;

				if (Options.Instance?.StomachCapacity != null)
				{
					Capacity = Options.Instance.StomachCapacity.Value;
				}
				if (ModManager.MSC && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand)
				{
					Capacity += 2;
				}

				return Capacity;
			}

			// 检查是否有空间
			public static bool HasSpace(Player player)
			{
				return GetStomachContents(player).Count < GetStomachCapacity(player);
			}
		}

		// 修改SwallowObject方法
		private void Player_SwallowObject(On.Player.orig_SwallowObject orig, Player player, int grasp)
		{
			UDebug.Log("SwallowObject_B");
			if (!ESS.HasSpace(player))
			{
				UDebug.Log("胃部已满，无法吞咽！");
				return;
			}
			player.objectInStomach = null;

			orig(player, grasp);

			if (player.objectInStomach != null)
			{
				var stomachContents = ESS.GetStomachContents(player);
				stomachContents.Add(player.objectInStomach);

				//player.objectInStomach = null;

				UDebug.Log($"吞咽成功！胃部物品数量: {stomachContents.Count}");
				for (int i = 0; i < stomachContents.Count; i++)
				{
					UDebug.Log(stomachContents[i].ToString());
				}
			}
			else
			{
				UDebug.Log("原版吞咽函数没有处理物品");
			}
			UDebug.Log("SwallowObject_A");
		}

		// 修正Regurgitate方法
		private void Player_Regurgitate(On.Player.orig_Regurgitate orig, Player player)
		{
			UDebug.Log("Regurgitate_B");
			var stomachContents = ESS.GetStomachContents(player);

			if (stomachContents.Count == 0)
			{
				UDebug.Log("胃部为空");

				if (player.isGourmand)
				{
					orig(player);
				}
				return;
			}

			if (ModManager.MSC && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear)
			{
				UDebug.Log("矛大师珍珠");

				orig(player);
				return;
			}

			player.objectInStomach = stomachContents[stomachContents.Count - 1];
			orig(player);
			stomachContents.RemoveAt(stomachContents.Count - 1);
			if (stomachContents.Count > 0)
			{
				player.objectInStomach = stomachContents[stomachContents.Count - 1];
			}
			else
			{
				player.objectInStomach = null;
			}
			//player.objectInStomach = null;

			for (int i = 0; i < stomachContents.Count; i++)
			{
				UDebug.Log(stomachContents[i].ToString());
			}
			UDebug.Log("Regurgitate_A");
		}

		public void Player_GrabUpdate(On.Player.orig_GrabUpdate orig, Player player, bool eu)
		{
			if (player.objectInStomach == null)
			{
				var stomachContents = ESS.GetStomachContents(player);
				//UDebug.Log($">>> [2] {stomachContents.Count}_GrabUpdate");
				if (stomachContents.Count > 0)
				{
					UDebug.Log($">>> [3] {stomachContents[stomachContents.Count - 1]}_GrabUpdate");
					player.objectInStomach = stomachContents[stomachContents.Count - 1];
				}
			}

			// 判断玩家是否处于某种特定状态，例如没有移动、跳跃或投掷物品，并且根据 ModManager 的条件进行进一步判断
			bool flag = ((player.input[0].x == 0 && player.input[0].y == 0 && !player.input[0].jmp && !player.input[0].thrw) || 
				(ModManager.MMF && player.input[0].x == 0 && player.input[0].y == 1 && !player.input[0].jmp && !player.input[0].thrw && 
				(player.bodyMode != Player.BodyModeIndex.ClimbingOnBeam || player.animation == Player.AnimationIndex.BeamTip || 
				player.animation == Player.AnimationIndex.StandOnBeam))) && 
				(player.mainBodyChunk.submersion < 0.5f || player.isRivulet);
			bool flag3 = false;
			bool craftingObject = false;

			// 如果满足 flag 条件，进一步判断玩家是否按下了 pckp 键，并根据 ModManager 和玩家状态判断是否可以进行抓取或制作物品
			if (flag)
			{
				if (player.input[0].pckp)
				{
					flag3 = true;
					if (ModManager.MSC && (player.FreeHand() == -1 || player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Artificer) && player.GraspsCanBeCrafted())
					{
						craftingObject = true;
					}
				}
			}

			// 如果玩家被水淹没且 MMF 模组启用，则禁止执行某些操作
			if (ModManager.MMF && player.mainBodyChunk.submersion >= 0.5f)
			{
				flag3 = false;
			}

			// 如果满足 flag3 条件，根据 craftingObject 状态执行不同的操作，例如抓取物品或进行物品制作
			if (flag3)
			{
				if (craftingObject)
				{
				}
				// 如果不是 MMF 模组启用或玩家没有向上的移动输入，则尝试将抓取的物品吞下
				else if (!ModManager.MMF || player.input[0].y == 0)
				{
					if (ESS.HasSpace(player))
					{
						int num13 = 0;
						while (num13 < 2)
						{
							if (player.grasps[num13] != null && player.CanBeSwallowed(player.grasps[num13].grabbed))
							{
								player.objectInStomach = null;
								UDebug.Log($">>> [4] {player.objectInStomach}_GrabUpdate");
								//player.SwallowObject(num13);
								break;
							}
							else
							{
								num13++;
							}
						}
					}

					// 根据玩家状态和 stomach 内容判断是否可以进行物品反刍
					var stomachContents = ESS.GetStomachContents(player);
					if ((player.objectInStomach != null && stomachContents.Count > 0) || player.isGourmand || (ModManager.MSC && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear))
					{
						bool flag6 = false;
						if (player.isGourmand && stomachContents.Count == 0)
						{
							flag6 = true;
						}
						if (!flag6 || (flag6 && player.FoodInStomach >= 1))
						{
							UDebug.Log($">>> [5] {stomachContents.Count > 0}_GrabUpdate");
							if (stomachContents.Count > 0)
							{
								player.objectInStomach = stomachContents[stomachContents.Count - 1];
							}
							else
							{
								player.objectInStomach = null;
							}
							//player.Regurgitate();
						}
					}


				}

			}
			if (player.swallowAndRegurgitateCounter > 0)
			{
				UDebug.Log($">>> [5] {player.objectInStomach}_GrabUpdate");
			}

			// 调用原始方法
			orig(player, eu);
		}


		private void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics playerGra)
		{
			orig(playerGra);
		}



		//数据保存字段
		public const string ESS_savefield_name = "StomachStorage_ESS_SAVEFIELD";

		public const string svA = "<svA>";
		public const string svB = "<svB>";
		public const string svC = "<svC>";
		public const string svD = "<svD>";
		public const string PlayerStr = "Player";
		//最后一次的存档数据
		public static Dictionary<int, List<string>> playerStomachsDict = new Dictionary<int, List<string>>();
		//全局系统变量
		public static WeakReference<RainWorldGame> gameRef = null;

		private static void SaveState_LoadGame(On.SaveState.orig_LoadGame orig, SaveState saveState, string str, RainWorldGame game_)
		{
			UDebug.Log($">>> [0] 方法入口_{playerStomachsDict.Count}_LoadGame");

			UDebug.Log(">>> [1] 方法入口");

			// 先调用原版
			try
			{
				UDebug.Log(">>> [2] 调用 orig");
				orig(saveState, str, game_);
				UDebug.Log(">>> [3] orig 完成");
			}
			catch (Exception e)
			{
				UDebug.LogError($">>> [ERR] orig 失败: {e}");
				throw;
			}
			//return;

			// 检查 str
			UDebug.Log(">>> [4] 检查 str");
			if (string.IsNullOrEmpty(str))
			{
				UDebug.LogError(">>> [ERR] str 为空");
				return;
			}

			UDebug.Log($">>> [5] str 长度: {str.Length}");
			UDebug.Log($"{str}_LoadGame_B");

			try
			{
				UDebug.Log(">>> [9] 进入 try 块");

				// 检查字典
				UDebug.Log(">>> [10] 检查字典");
				if (playerStomachsDict == null)
				{
					UDebug.LogWarning(">>> [WARN] 字典为 null，重新创建");
					playerStomachsDict = new Dictionary<int, List<string>>();
				}
				else
				{
					UDebug.Log(">>> [11] 清空字典");
					playerStomachsDict.Clear();
				}

				// 检查是否包含我们的数据
				UDebug.Log(">>> [12] 检查 ESS_savefield_name");
				if (!str.Contains(ESS_savefield_name))
				{
					UDebug.Log(">>> [13] 未找到数据，返回");
					return;
				}

				// 分割字符串
				UDebug.Log(">>> [14] 分割 sA");
				string[] sA = Regex.Split(str, svA);
				UDebug.Log($">>> [15] sA 长度: {sA?.Length ?? -1}");

				if (sA == null || sA.Length == 0)
				{
					UDebug.LogError(">>> [ERR] sA 为空");
					return;
				}

				foreach (var sA_ in sA)
				{
					UDebug.Log($">>> [16] 处理 sA_ 块，长度: {sA_?.Length ?? -1}");

					if (string.IsNullOrEmpty(sA_))
					{
						UDebug.Log(">>> [17] sA_ 为空，跳过");
						continue;
					}

					UDebug.Log(">>> [18] 分割 sB");
					string[] sB = Regex.Split(sA_, svB);
					UDebug.Log($">>> [19] sB 长度: {sB?.Length ?? -1}");

					if (sB == null || sB.Length < 2)
					{
						UDebug.Log(">>> [20] sB 长度不足，跳过");
						continue;
					}

					UDebug.Log($">>> [21] sB[0] = {sB[0]}");
					if (sB[0] != ESS_savefield_name)
					{
						UDebug.Log(">>> [22] 不是数据，跳过");
						continue;
					}

					string savedData = sB[1];
					UDebug.Log($">>> [23] savedData 长度: {savedData?.Length ?? -1}");
					UDebug.Log($"{savedData}_LoadGame_SD");

					if (string.IsNullOrEmpty(savedData))
					{
						UDebug.LogWarning(">>> [WARN] savedData 为空");
						continue;
					}

					UDebug.Log(">>> [24] 分割 sC");
					string[] sC = Regex.Split(savedData, svC);
					UDebug.Log($">>> [25] sC 长度: {sC?.Length ?? -1}");

					if (sC == null || sC.Length == 0)
					{
						UDebug.LogError(">>> [ERR] sC 为空");
						continue;
					}

					foreach (var sC_ in sC)
					{
						UDebug.Log($">>> [26] 处理 sC_ 块");

						if (string.IsNullOrEmpty(sC_))
						{
							UDebug.Log(">>> [27] sC_ 为空，跳过");
							continue;
						}

						UDebug.Log(">>> [28] 分割 sD");
						string[] sD = Regex.Split(sC_, svD);
						UDebug.Log($">>> [29] sD 长度: {sD?.Length ?? -1}");

						if (sD == null || sD.Length < 2)
						{
							UDebug.Log(">>> [30] sD 长度不足，跳过");
							continue;
						}

						UDebug.Log($">>> [31] sD[0] = {sD[0]}");
						if (!sD[0].Contains(PlayerStr))
						{
							UDebug.Log(">>> [32] 不包含 Player，跳过");
							continue;
						}

						string savedDataOther = sD[1];
						UDebug.Log($">>> [33] savedDataOther 长度: {savedDataOther?.Length ?? -1}");

						// 解析玩家编号
						UDebug.Log(">>> [34] 解析玩家编号");
						string playerNumStr = sD[0].Replace(PlayerStr, "");
						if (!int.TryParse(playerNumStr, out int N))
						{
							UDebug.LogWarning($">>> [WARN] 无法解析玩家编号: {sD[0]}");
							continue;
						}
						UDebug.Log($">>> [35] 玩家编号 N = {N}");

						if (string.IsNullOrEmpty(savedDataOther))
						{
							UDebug.LogWarning($">>> [WARN] Player{N} 数据为空");
							continue;
						}

						UDebug.Log(">>> [36] 分割物品列表");
						List<string> result = savedDataOther.Split(',').ToList();
						UDebug.Log($">>> [37] 物品数量: {result?.Count ?? -1}");

						if (result.Count > 0 && N >= 0)
						{
							UDebug.Log($">>> [44] 保存 Player{N} 的 {result.Count} 个物品");
							playerStomachsDict[N] = result;
						}
						else
						{
							UDebug.Log($">>> [45] Player{N} 无有效物品");
						}


					}
				}

				UDebug.Log(">>> [46] 处理完成");
			}
			catch (Exception e)
			{
				UDebug.LogError($">>> [ERR] 外层异常: {e}");
			}

			UDebug.Log(">>> [47] 方法结束");
		}

		private static string SaveState_SaveToString(On.SaveState.orig_SaveToString orig, SaveState saveState)
		{
			UDebug.Log(">>> [1] 方法入口_SaveState");
			string text = orig(saveState);
			UDebug.Log($"{text}_SaveToString_B");

			//return text;

			try
			{

				if (gameRef == null || !gameRef.TryGetTarget(out var game) || game?.world == null || game.Players == null)
				{
					return text;
				}

				text = RemoveField(text, ESS_savefield_name);

				string extendedText = "";
				foreach (var absc in game.Players)
				{
					if (absc == null || absc.realizedCreature == null)
					{
						continue;
					}
					if (absc.realizedCreature is Player player)
					{
						int N = player.playerState.playerNumber;
						List<AbstractPhysicalObject> stomach = ESS.GetStomachContents(player);
						if (stomach != null && stomach.Count > 0)
						{
							string strings = "";
							foreach (var item in stomach)
							{
								AbstractCreature abstractCreature3 = item as AbstractCreature;
								if (abstractCreature3 != null)
								{
									if (game.world.GetAbstractRoom(abstractCreature3.pos.room) == null)
									{
										abstractCreature3.pos = player.coord;
									}
									strings += SaveState.AbstractCreatureToStringStoryWorld(abstractCreature3);
								}
								else
								{
									strings += item.ToString();
								}
								strings += ",";
							}
							strings = strings.Substring(0, strings.Length - 1);
							extendedText += string.Format(CultureInfo.InvariantCulture, $"{PlayerStr}{N}{svD}{strings}{svC}");
						}

					}
				}

				if (extendedText != null && extendedText != "")
				{
					text += string.Format(CultureInfo.InvariantCulture, $"{ESS_savefield_name}{svB}{extendedText}{svA}");
				}

				// 保存"123"
				//text += "my_simple_data<svB>123<svA>";
			}
			catch (Exception e)
			{
				UDebug.LogError($"{e}_SaveToString_e");
			}
			UDebug.Log($"{text}_SaveToString_A");
			return text;
		}

		public static string RemoveField(string dataText, string fieldName)
		{
			if (string.IsNullOrEmpty(dataText) || string.IsNullOrEmpty(fieldName))
				return dataText;

			// 查找字段名在文本中的首次出现位置
			int index_start = dataText.IndexOf(fieldName);

			// 循环处理所有匹配的字段（因为可能多次出现）
			while (index_start != -1)
			{
				// 查找字段值结束标签"<svA>"的位置
				// 从字段名位置开始搜索，确保找到的是当前字段对应的结束标签
				int endTagIndex = dataText.IndexOf(svA, index_start);
				if (endTagIndex == -1) break; // 找不到结束标签，退出

				int index_end = endTagIndex + 5;
				// 注意：+5是为了包含"<svA>"标签本身（标签长度为5个字符）

				// 计算要移除的文本长度（从字段名开始到结束标签之后）
				int removeLength = index_end - index_start;

				// 移除字段名及其对应的值（包括结束标签）
				dataText = dataText.Remove(index_start, removeLength);

				// 查找下一个匹配的字段名位置
				index_start = dataText.IndexOf(fieldName);
			}

			// 返回处理后的文本
			return dataText;
		}

		private void Player_ctor(On.Player.orig_ctor orig, Player player, AbstractCreature abstractCreature, World world)
		{
			// 先调用原版构造函数
			orig(player, abstractCreature, world);

			//赋值给全局变量供其他函数使用
			RainWorldGame game = player?.room?.world?.game;
			if (game == null)
			{
				UDebug.LogError($">>> [ERR] 设置 game 失败");
				return;
			}
			UDebug.Log(">>> [8] 设置 gameRef");
			try
			{
				gameRef = new WeakReference<RainWorldGame>(game);
			}
			catch (Exception ex)
			{
				UDebug.LogError($">>> [ERR] 设置 gameRef 失败: {ex.Message}");
				return;
			}

			var stomachContents = ESS.GetStomachContents(player);
			stomachContents.Clear();

			int N = player.playerState.playerNumber;
			if (playerStomachsDict.TryGetValue(N, out var stomachStr))
			{
				if (stomachStr != null && stomachStr.Count > 0)
				{
					bool repeat = false;
					string PlayerStomachStrings = "";
					if (player.objectInStomach != null)
					{
						if (player.objectInStomach is AbstractCreature abstractCreature3)
						{
							PlayerStomachStrings = SaveState.AbstractCreatureToStringStoryWorld(abstractCreature3);
						}
						else
						{
							PlayerStomachStrings = player.objectInStomach.ToString();
						}
					}

					foreach (var item in stomachStr)
					{
						UDebug.Log($">>> [38] 处理物品: {item?.Substring(0, Math.Min(50, item?.Length ?? 0))}...");
						if (string.IsNullOrWhiteSpace(item))
						{
							UDebug.Log(">>>[39] 物品为空：跳过");
							continue;
						}
						if (!string.IsNullOrWhiteSpace(PlayerStomachStrings))
						{
							if (item == PlayerStomachStrings)
							{
								repeat = true;
							}
						}

						AbstractPhysicalObject obj = null;
						try
						{
							if (item.Contains("<oA>"))
							{
								UDebug.Log(">>> [40] 解析物品对象");
								obj = SaveState.AbstractPhysicalObjectFromString(game.world, item);
							}
							else if (item.Contains("<cA>"))
							{
								UDebug.Log(">>> [41] 解析生物对象");
								obj = SaveState.AbstractCreatureFromString(game.world, item, false, default(WorldCoordinate));
							}
							else
							{
								UDebug.Log($">>> [42] 未知物品类型: {item.Substring(0, Math.Min(30, item.Length))}");
							}
						}
						catch (Exception ex)
						{
							UDebug.LogWarning($">>> [WARN] 解析物品失败: {ex.Message}");
							continue;
						}

						if (obj != null)
						{
							UDebug.Log(">>> [43] 添加物品到列表");
							if (obj != null)
							{
								obj.pos = abstractCreature.pos;
							}
							stomachContents.Add(obj);
							//stomach.Add(obj);
						}
						else
						{
							UDebug.LogWarning($">>> [WARN] 物品解析结果为 null");
						}
					}
                    if (!repeat && player.objectInStomach != null)
					{
                        UDebug.Log(">>> 加载 objectInStomach");
                        stomachContents.Add(player.objectInStomach);
                    }



                }
			}

			if (player.objectInStomach != null && stomachContents.Count == 0)
			{
				UDebug.Log(">>> 使用原版 objectInStomach");
				stomachContents.Add(player.objectInStomach);
			}

		}



	}
}