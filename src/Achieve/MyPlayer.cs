using BepInEx;
using CoralBrain;
using Expedition;
using HUD;
using ImprovedInput;
using JollyCoop;
using JollyCoop.JollyMenu;
using MoreSlugcats;
using Noise;
using RWCustom;
using SlugBase;
using SlugBase.Features;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Watcher;
using static CustomStomachStorage.MyOptions;
using static CustomStomachStorage.Plugin;
using static CustomStomachStorage.Extended;
using static Player.ObjectGrabability;
using static SlugBase.Features.FeatureTypes;
//using UDebug =  UnityEngine.Debug;


namespace CustomStomachStorage
{
	public class MyPlayer
	{

		public static void HookAdd()
		{
			On.Player.ctor += Player_ctor;
			On.Player.Update += Player_Update;
			On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
			On.PlayerGraphics.Update += PlayerGraphics_Update;
			On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
			On.Player.GrabUpdate += Player_GrabUpdate;
			On.Player.Grabability += Player_Grabability;
			On.Player.CanBeSwallowed += Player_CanBeSwallowed;
			On.Player.SwallowObject += Player_SwallowObject;
			On.Player.Regurgitate += Player_Regurgitate;
		}
		public static void HookSubtract()
		{
			On.Player.ctor -= Player_ctor;
			On.Player.Update -= Player_Update;
			On.PlayerGraphics.InitiateSprites -= PlayerGraphics_InitiateSprites;
			On.PlayerGraphics.Update -= PlayerGraphics_Update;
			On.PlayerGraphics.DrawSprites -= PlayerGraphics_DrawSprites;
			On.Player.GrabUpdate -= Player_GrabUpdate;
			On.Player.Grabability -= Player_Grabability;
			On.Player.CanBeSwallowed -= Player_CanBeSwallowed;
			On.Player.SwallowObject -= Player_SwallowObject;
			On.Player.Regurgitate -= Player_Regurgitate;
		}

		private static void Player_ctor(On.Player.orig_ctor orig, Player player, AbstractCreature abstractCreature, World world)
		{
#if MYDEBUG
			try
			{
#endif

				// 先调用原版构造函数
				orig(player, abstractCreature, world);

				// 添加日志记录传入参数
				UDebug.Log($">>> Player_ctor 开始 - player: {player.ToString() ?? "null"}, abstractCreature: {abstractCreature.ToString()}, world: {world?.ToString() ?? "null"}");
				UDebug.Log($">>> player.playerState.playerNumber: {player.playerState.playerNumber}");

				//赋值给全局变量供其他函数使用
				GlobalVar._game = abstractCreature.world?.game;
				UDebug.Log($">>> GlobalVar._game 设置: {(GlobalVar._game != null ? "成功" : "失败 - 可能为 null")}");

				//玩家变量初始化
				var pv = new PlayerVar();
				GlobalVar.playerVar.Add(player, pv);
				//调试图像
				//pv.myDebug = new MyDebug(player);
				if (UDebug.ShouldLog)
				{
					pv.myDebug = new MyDebug(player);
				}

				if (abstractCreature.world?.game.session is StoryGameSession sto && sto.saveState.malnourished)
				{
					UDebug.Log($">>> 挨饿轮回，");
				}

				if (abstractCreature.world?.game.session is ArenaGameSession)//在竞技场模式
				{
					UDebug.Log(">>> 竞技场模式，跳过胃部加载");
					try
					{
						ESS.stomachItems.Clear();
						GlobalVar.playersStrStomachs.Clear();
					}
					catch (Exception ex)
					{
						UDebug.Log($">>> 竞技场模式清除失败: {ex}");
					}
					return;
				}

				if (abstractCreature.world?.game?.manager?.menuSetup?.startGameCondition == ProcessManager.MenuSetup.StoryGameInitCondition.New)//新剧情模式
				{
					UDebug.Log(">>> 新剧情，跳过胃部加载");
					ESS.stomachItems.Clear();
					GlobalVar.playersStrStomachs.Clear();
				}

				var stomachItems = ESS.GetstomachItems(player);
				UDebug.Log($">>> 获取 stomachItems, Count: {stomachItems.Count}");

				int N = player.playerState.playerNumber;
				UDebug.Log($">>> 尝试获取 playersStrStomachs 中玩家 {N} 的数据");
				UDebug.Log($">>> GlobalVar.playersStrStomachs 当前状态: Count={GlobalVar.playersStrStomachs.Count}, Keys={string.Join(", ", GlobalVar.playersStrStomachs.Keys)}");

				bool hasValue = GlobalVar.playersStrStomachs.TryGetValue(N, out var strStomach);
				UDebug.Log($">>> TryGetValue 结果: {hasValue}, strStomach 是否为 null: {strStomach == null}");

				if (hasValue)
				{
					UDebug.Log($">>> 进入 strStomach 处理块");
                    stomachItems.Clear();

					// 添加对 strStomach 的详细检查
					if (strStomach == null)
					{
						UDebug.Log($">>> [错误] strStomach 为 null，跳过处理");
					}
					else
					{
						try
						{
							UDebug.Log($">>> strStomach 类型: {strStomach.GetType()}, Count: {strStomach.Count}");

							if (strStomach.Count > 0)
							{
								UDebug.Log($">>> strStomach 包含 {strStomach.Count} 个项目");

								// 遍历 strStomach 前先记录所有项目
								for (int idx = 0; idx < strStomach.Count; idx++)
								{
									UDebug.Log($">>> strStomach[{idx}] = {strStomach[idx] ?? "null"}");
								}

								bool found = false;
								string? strings = null;

								// 检查 story session 和 swallowedItems
								if (abstractCreature.world?.game.session is StoryGameSession story)
								{
									UDebug.Log($">>> 当前是故事模式，检查 swallowedItems");
									if (story.saveState?.swallowedItems != null)
									{
										UDebug.Log($">>> swallowedItems 数组长度: {story.saveState.swallowedItems.Length}");
										if (player.playerState?.playerNumber < story.saveState.swallowedItems.Length)
										{
											strings = story.saveState.swallowedItems[player.playerState.playerNumber];
											UDebug.Log($">>> [38] 处理物品_objectInStomach: {strings ?? "null"}");
										}
										else
										{
											UDebug.Log($">>> 警告: playerNumber {player.playerState?.playerNumber} 超出 swallowedItems 数组范围");
										}
									}
									else
									{
										UDebug.Log($">>> 警告: story.saveState.swallowedItems 为 null");
									}
								}

								foreach (var str in strStomach)
								{
									UDebug.Log($">>> [38] 处理物品: {str}");
									if (string.IsNullOrWhiteSpace(str))
									{
										UDebug.Log(">>>[39] 物品为空：跳过");
										continue;
									}

									if (!string.IsNullOrWhiteSpace(strings))
									{
										if (str == strings)
										{
											found = true;
											UDebug.Log($">>> 找到匹配的 str: {str}");
										}
									}

									AbstractPhysicalObject? obj = null;
									try
									{
										if (str.Contains("<oA>"))
										{
											UDebug.Log(">>> [40] 解析物品对象");
											UDebug.Log($">>> 使用 GlobalVar._game?.world: {(GlobalVar._game?.world != null ? "有效" : "为 null")}");
											obj = SaveState.AbstractPhysicalObjectFromString(GlobalVar._game?.world, str);
										}
										else if (str.Contains("<cA>"))
										{
											UDebug.Log(">>> [41] 解析生物对象");
											UDebug.Log($">>> 使用 GlobalVar._game?.world: {(GlobalVar._game?.world != null ? "有效" : "为 null")}");
											obj = SaveState.AbstractCreatureFromString(GlobalVar._game?.world, str, false, default(WorldCoordinate));
										}
										else
										{
											UDebug.Log($">>> [42] 未知物品类型: {str}");
										}
									}
									catch (Exception ex)
									{
										UDebug.LogWarning($">>> [WARN] 解析物品失败: {ex.Message}\nStackTrace: {ex.StackTrace}");
										continue;
									}

									if (obj != null)
									{
										UDebug.Log(">>> [43] 添加物品到列表");
										obj.pos = abstractCreature.pos;
										stomachItems.Add(obj);
									}
									else
									{
										UDebug.LogWarning($">>> [WARN] 物品解析结果为 null");
									}
								}

								if (!found && strings != null)
								{
									UDebug.Log(">>> 加载 objectInStomach");
									if (player.objectInStomach != null)
									{
										stomachItems.Add(player.objectInStomach);
										UDebug.Log($">>> 添加 objectInStomach: {player.objectInStomach}");
									}
									else
									{
										UDebug.Log(">>> 警告: player.objectInStomach 为 null");
									}
								}
							}
							else
							{
								UDebug.Log(">>> strStomach.Count == 0，跳过处理");
							}
						}
						catch (Exception ex)
						{
							UDebug.LogError($">>> [严重错误] 处理 strStomach 时发生异常: {ex.Message}\nStackTrace: {ex.StackTrace}");
							throw; // 重新抛出异常以便外部 catch 捕获
						}
					}
				}
				else if (abstractCreature.world?.game.session is StoryGameSession story_ && story_.saveState.malnourished && stomachItems.Count > 0)
				{
					UDebug.Log($">>> 进入挨饿轮回处理，stomachItems.Count = {stomachItems.Count}");
					for (int i = 0; i < stomachItems.Count; i++)
					{
						UDebug.Log($">>> 挨饿轮回，重新解析 stomachItems[{i}] = {stomachItems[i]?.ToString() ?? "null"}");

						if (stomachItems[i] == null)
						{
							UDebug.Log($">>> 警告: stomachItems[{i}] 为 null，跳过");
							continue;
						}

						string? strings;
						if (stomachItems[i] is AbstractCreature abstractCreature3)
						{
							abstractCreature3.pos = player.coord;
							strings = SaveState.AbstractCreatureToStringStoryWorld(abstractCreature3);
							UDebug.Log($">>> 转换生物为字符串: {strings}");
						}
						else
						{
							strings = stomachItems[i].ToString();
							UDebug.Log($">>> 使用 ToString(): {strings}");
						}

						if (!string.IsNullOrWhiteSpace(strings))
						{
							AbstractPhysicalObject? obj = null;
							try
							{
								if (strings.Contains("<oA>"))
								{
									UDebug.Log(">>> [40] 解析物品对象");
									obj = SaveState.AbstractPhysicalObjectFromString(abstractCreature.world, strings);
								}
								else if (strings.Contains("<cA>"))
								{
									UDebug.Log(">>> [41] 解析生物对象");
									obj = SaveState.AbstractCreatureFromString(abstractCreature.world, strings, false, default(WorldCoordinate));
								}

								if (obj != null)
								{
									stomachItems[i] = obj;
									UDebug.Log($">>> 重新解析成功，更新 stomachItems[{i}]");
								}
								else
								{
									UDebug.Log($">>> 重新解析失败，obj 为 null");
								}
							}
							catch (Exception ex)
							{
								UDebug.LogError($">>> 重新解析时发生异常: {ex.Message}");
							}
						}
					}
				}

				UDebug.Log($">>> objectInStomach = {player.objectInStomach?.ToString() ?? "null"}, stomachItems.Count = {stomachItems.Count}");
				if (player.objectInStomach != null && stomachItems.Count == 0)
				{
					UDebug.Log(">>> 使用原版 objectInStomach");
					stomachItems.Add(player.objectInStomach);
				}

				UDebug.Log($">>> 当前轮回数: {GlobalVar._game?.GetStorySession?.saveState?.cycleNumber}_Player_ctor_cycleNum");

				//最后总是移除
				UDebug.Log($">>> 从 playersStrStomachs 移除玩家 {N}");
				GlobalVar.playersStrStomachs.Remove(N);

#if MYDEBUG
			}
			catch (Exception e)
			{
				StackTrace st = new StackTrace(new StackFrame(true));
				StackFrame sf = st.GetFrame(0);
				var sr = sf.GetFileName().Split('\\');
				MyDebug.outStr = sr[sr.Length - 1] + "\n";
				MyDebug.outStr += sf.GetMethod() + "\n";
				MyDebug.outStr += e;
				UDebug.Log(e);
			}
#endif
		}

		// 管理扩展的胃部存储
		public class ESS//ExtendedStomachStorage
		{
			public static readonly Dictionary<int, List<AbstractPhysicalObject>> stomachItems = new();//胃部存储列表

			public static List<AbstractPhysicalObject> GetstomachItems(Player player)
			{
				int N = player.playerState.playerNumber;
				if (!stomachItems.TryGetValue(N, out var contents))
				{
					contents = new List<AbstractPhysicalObject>();
					stomachItems.Add(N, contents);
				}
				return contents;
			}
			/*public static List<AbstractPhysicalObject> GetstomachItems(Player player)
			{
				return GlobalVar.GetPlayerVar(player).stomachItems;
			}*/

			// 获取第一个物品
			public static AbstractPhysicalObject? GetFirstStomachItem(Player player)
			{
				var contents = GetstomachItems(player);
				return contents.Count > 0 ? contents[0] : null;
			}

			// 获取最后一个物品
			public static AbstractPhysicalObject? GetLastStomachItem(Player player)
			{
				var contents = GetstomachItems(player);
				return contents.Count > 0 ? contents[contents.Count - 1] : null;
			}

			// 移除指定位置的物品
			public static AbstractPhysicalObject? RemoveStomachItem(Player player, int index)
			{
				var contents = GetstomachItems(player);
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

				if (MyOptions.Instance?.StomachCapacity != null)
				{
					Capacity = MyOptions.Instance.StomachCapacity.Value;
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
				return GetstomachItems(player).Count < GetStomachCapacity(player);
			}
		}

		private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
		{
			orig(player, eu);

			if (player == null) return;
		}

		#region MyOptions
		private static Player.ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player player, PhysicalObject testObj)
		{
			var opt = MyOptions.Instance;
			Player.ObjectGrabability original = orig(player, testObj);

			if (testObj == player) return original;

			// 全局抓取设置
			if (opt.GetGrabSpecial("OneHandGrabAll"))
				return Player.ObjectGrabability.OneHand;

			if (opt.GetGrabSpecial("DragGrabAll") && original == Player.ObjectGrabability.CantGrab)
				return Player.ObjectGrabability.Drag;

			bool isCreature = testObj is Creature;

			
			if (isCreature)
			{
				string mode = opt.GetGrabMode("Creature");
				if (mode != MyOptions.NotSelected)
				{
					return opt.GetPlayerGrab(mode);
				}
			}
			else
			{
				string mode = opt.GetGrabMode("Item");
				if (mode != MyOptions.NotSelected)
				{
					return opt.GetPlayerGrab(mode);
				}
			}
			if (testObj is Player)
			{
				string mode = opt.GetGrabMode("Slugcat");
				if (mode != MyOptions.NotSelected)
				{
					return opt.GetPlayerGrab(mode);
				}
			}

			
			HashSet<string> chain = GetInheritanceChain(testObj);

			foreach (string type in chain)
			{
				string mode = opt.GetGrabMode(type);
				if (mode != MyOptions.NotSelected)
				{
					return opt.GetPlayerGrab(mode);
				}
			}

			return original;
		}

		private static bool Player_CanBeSwallowed(On.Player.orig_CanBeSwallowed orig, Player player, PhysicalObject testObj)
		{
			//bool a = testObj is Player;//Slugcat

			var opt = MyOptions.Instance;

			// 矛大师特殊处理
			if (ModManager.MSC && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear && !(opt.SpearmasterStoreItems?.Value == true))
				return false;

			// 全局吞咽
			if (opt.CanSwallow("All"))
				return true;

			bool isCreature = testObj is Creature;

			if (isCreature && opt.CanSwallow("Creature"))
			{
				return true;
			}
			if (!isCreature && opt.CanSwallow("Item"))
			{
				return true;
			}
			if (testObj is Player && opt.CanSwallow("Slugcat"))
			{
				return true;
			}

			// 获取继承链
			HashSet<string> chain = GetInheritanceChain(testObj);
			// 遍历所有启用的吞咽类型
			foreach (string inherit in chain)
			{
				if (opt.CanSwallow(inherit))
				{
					return true;
				}
			}

			/*foreach (var kvp in opt.SwallowTypes)
			{
				if (!kvp.Value.Value) continue;
				if (kvp.Key == "All") continue;
				if (!chain.Contains(kvp.Key)) continue;

				// 特殊处理 Creature/Item 大类
				if (kvp.Key == "Creature" && !isCreature) continue;
				if (kvp.Key == "Item" && isCreature) continue;

				return true;
			}*/

			return orig(player, testObj);
		}
		#endregion

		// 修改SwallowObject方法
		private static void Player_SwallowObject(On.Player.orig_SwallowObject orig, Player player, int grasp)
		{
			UDebug.Log("SwallowObject_B");
			if (!ESS.HasSpace(player))
			{
				UDebug.Log("胃部已满，无法吞咽！");
				return;
			}
			if (MyOptions.Instance?.DebugMode?.Value == true)
			{
				var stomachItems = ESS.GetstomachItems(player);
				for (int i = 0; i < stomachItems.Count; i++)
				{
					UDebug.Log(stomachItems[i].ToString());
				}
			}

			player.objectInStomach = null;

			orig(player, grasp);

			if (player.objectInStomach != null)
			{
				var stomachItems = ESS.GetstomachItems(player);
				stomachItems.Add(player.objectInStomach);

				//player.objectInStomach = null;

				UDebug.Log($"吞咽成功！胃部物品数量: {stomachItems.Count}");
				for (int i = 0; i < stomachItems.Count; i++)
				{
					UDebug.Log(stomachItems[i].ToString());
				}
			}
			else
			{
				UDebug.Log("原版吞咽函数没有处理物品");
			}
			UDebug.Log("SwallowObject_A");
		}

		// 修正Regurgitate方法
		private static void Player_Regurgitate(On.Player.orig_Regurgitate orig, Player player)
		{
			UDebug.Log("Regurgitate_B");
			var stomachItems = ESS.GetstomachItems(player);

			for (int i = 0; i < stomachItems.Count; i++)
			{
				UDebug.Log(stomachItems[i].ToString());
			}

			if (stomachItems.Count == 0)
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
				if (MyOptions.Instance.SpearmasterStoreItems?.Value == true)
				{ }
				else
				{
					UDebug.Log("矛大师珍珠");

					orig(player);
					return;
				}
			}

			if (stomachItems.Count > 0)
			{
				player.objectInStomach = stomachItems[stomachItems.Count - 1];
			}

			if (player.objectInStomach.world.GetAbstractRoom(player.objectInStomach.pos)?.realizedRoom != null)
			{ }
			else
			{
				UDebug.Log($">>> 物品原房间已卸载，重新解析 {player.objectInStomach}");

				string? strings;
				if (player.objectInStomach is AbstractCreature abstractCreature3)
				{
					abstractCreature3.pos = player.coord;
					strings = SaveState.AbstractCreatureToStringStoryWorld(abstractCreature3);
				}
				else
				{
					strings = player.objectInStomach.ToString();
				}
				if (strings != null)
				{
					AbstractPhysicalObject? obj = null;

					if (strings.Contains("<oA>"))
					{
						UDebug.Log(">>> [40] 解析物品对象");
						obj = SaveState.AbstractPhysicalObjectFromString(player.room.world, strings);
					}
					else if (strings.Contains("<cA>"))
					{
						UDebug.Log(">>> [41] 解析生物对象");
						obj = SaveState.AbstractCreatureFromString(player.room.world, strings, false, default(WorldCoordinate));
					}
					if (obj != null)
					{
						player.objectInStomach = obj;
					}
				}
			}

			if (ModManager.MSC && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear && MyOptions.Instance.SpearmasterStoreItems?.Value == true)
			{
				NewRegurgitate(player);
			}
			else
			{
				orig(player);
			}

			stomachItems.RemoveAt(stomachItems.Count - 1);
			if (stomachItems.Count > 0)
			{
				player.objectInStomach = stomachItems[stomachItems.Count - 1];
			}
			else
			{
				player.objectInStomach = null;
			}
			//player.objectInStomach = null;

			UDebug.Log("Regurgitate_A");
		}

		public static void Player_GrabUpdate(On.Player.orig_GrabUpdate orig, Player player, bool eu)
		{
			//
			var stomachItems = ESS.GetstomachItems(player);
			if (player.objectInStomach == null)
			{
				//UDebug.Log($">>> [2] {stomachItems.Count}_GrabUpdate");
				if (stomachItems.Count > 0)
				{
					//UDebug.Log($">>> [3] {stomachItems[stomachItems.Count - 1]}_GrabUpdate");
					player.objectInStomach = stomachItems[stomachItems.Count - 1];
				}
			}
			else
			{
				//bool found = false;

/*                string objectInStrings = "";
				if (player.objectInStomach is AbstractCreature abstractCreature4)
				{
					if (player.room?.world?.GetAbstractRoom(abstractCreature4.pos.room) == null)
					{
						abstractCreature4.pos = player.coord;
					}
					objectInStrings = SaveState.AbstractCreatureToStringStoryWorld(abstractCreature4);
				}
				else
				{
					objectInStrings = player.objectInStomach.ToString();
				}*/

				/*if (!string.IsNullOrWhiteSpace(objectInStrings))
				{
					foreach (var item in stomachItems)
					{
						string strings = "";
						if (item is AbstractCreature abstractCreature3)
						{
							if (player.room?.world?.GetAbstractRoom(abstractCreature3.pos.room) == null)
							{
								abstractCreature3.pos = player.coord;
							}
							strings = SaveState.AbstractCreatureToStringStoryWorld(abstractCreature3);
						}
						else
						{
							strings = item.ToString();
						}
						if (!string.IsNullOrWhiteSpace(strings))
						{
							if (strings == objectInStrings)
							{
								found = true;
								break;
							}
						}
					}
				}*/
				/*for (int i = 0; i < stomachItems.Count; i++)
				{
								if (stomachItems[i] == player.objectInStomach) { found = true; break; }
				}*/



				if (Find(stomachItems, player.objectInStomach, player.coord) && stomachItems.Count > 0)
				{
					player.objectInStomach = stomachItems[stomachItems.Count - 1];
				}
				else
				{
					for (int i = 0; i < stomachItems.Count; i++)
					{
						UDebug.Log($"GrabUpdate_{stomachItems[i].ToString()}");
					}
					stomachItems.Add(player.objectInStomach);
					UDebug.Log($"GrabUpdate_{player.objectInStomach.ToString()}");
				}
			}
			//

			if (GlobalVar.IsPressedSwallow(player) && ESS.HasSpace(player))
			{
				int num13 = 0;
				while (num13 < 2)
				{
					if (player.grasps[num13] != null && player.CanBeSwallowed(player.grasps[num13].grabbed))
					{
						player.objectInStomach = null;
						player.swallowAndRegurgitateCounter++;
						break;
					}
					else
					{
						num13++;
					}
				}

				if (player.swallowAndRegurgitateCounter > 90)
				{
					num13 = 0;
					while (num13 < 2)
					{
						if (player.grasps[num13] != null && player.CanBeSwallowed(player.grasps[num13].grabbed))
						{
							player.bodyChunks[0].pos += Custom.DirVec(player.grasps[num13].grabbed.firstChunk.pos, player.bodyChunks[0].pos) * 2f;
							player.SwallowObject(num13);
							if (player.spearOnBack != null)
							{
								player.spearOnBack.interactionLocked = true;
							}
							if ((ModManager.MSC || ModManager.CoopAvailable) && player.slugOnBack != null)
							{
								player.slugOnBack.interactionLocked = true;
							}
							player.swallowAndRegurgitateCounter = 0;
							if (player.graphicsModule != null && player.graphicsModule is PlayerGraphics playerGraphics)
							{
								playerGraphics.swallowing = 20;
								break;
							}
							break;
						}
						else
						{
							num13++;
						}
					}
				}

				return;
			}
			else if (GlobalVar.IsPressedRegurgitate(player) && ((player.objectInStomach != null && stomachItems.Count > 0) || player.isGourmand))
			{
				if (player.isGourmand)
				{
					if (stomachItems.Count > 0)
					{
						player.objectInStomach = stomachItems[stomachItems.Count - 1];
					}
					else
					{
						player.objectInStomach = null;
					}
				}

				player.swallowAndRegurgitateCounter++;

				if (player.swallowAndRegurgitateCounter > 110)
				{
					bool flag6 = false;
					if (player.isGourmand && player.objectInStomach == null)
					{
						flag6 = true;
					}
					if (!flag6 || (flag6 && player.FoodInStomach >= 1))
					{
						if (flag6)
						{
							player.SubtractFood(1);
						}
						player.Regurgitate();
					}
					else
					{
						player.firstChunk.vel += new Vector2(UnityEngine.Random.Range(-1f, 1f), 0f);
						player.Stun(30);
					}
					if (player.spearOnBack != null)
					{
						player.spearOnBack.interactionLocked = true;
					}
					if ((ModManager.MSC || ModManager.CoopAvailable) && player.slugOnBack != null)
					{
						player.slugOnBack.interactionLocked = true;
					}
					player.swallowAndRegurgitateCounter = 0;
				}

				return;
			}



			//
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
								//UDebug.Log($">>> [4] {player.objectInStomach}_GrabUpdate");
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
					if ((player.objectInStomach != null && stomachItems.Count > 0) || player.isGourmand || (ModManager.MSC && player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Spear))
					{
						bool flag6 = false;
						if (player.isGourmand && stomachItems.Count == 0)
						{
							flag6 = true;
						}
						if (!flag6 || (flag6 && player.FoodInStomach >= 1))
						{
							//UDebug.Log($">>> [5] {stomachItems.Count > 0}_GrabUpdate");
							if (stomachItems.Count > 0)
							{
								player.objectInStomach = stomachItems[stomachItems.Count - 1];
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
				//UDebug.Log($">>> [5] {player.objectInStomach}_GrabUpdate");
			}

			// 调用原始方法
			orig(player, eu);
		}

		private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics playerGra, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
			orig(playerGra, sLeaser, rCam);

			GlobalVar.playerVar.TryGetValue(playerGra.player, out var pv);

			if (pv.myDebug != null)
			{
				pv.myDebug.InitiateSprites(sLeaser, rCam);
			}
		}

		public static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics playerGra, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
			orig(playerGra, sLeaser, rCam, timeStacker, camPos);

			GlobalVar.playerVar.TryGetValue(playerGra.player, out var pv);

			if (pv.myDebug != null)
			{
				pv.myDebug.DrawSprites(sLeaser, rCam, timeStacker, camPos);
			}
		}

		private static void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics playerGra)
		{
			orig(playerGra);
		}



		public static void NewRegurgitate(Player player)
		{
			if (player.objectInStomach == null)
			{
				if (!player.isGourmand)
				{
					return;
				}
				player.objectInStomach = GourmandCombos.RandomStomachItem(player);
			}
			player.room.abstractRoom.AddEntity(player.objectInStomach);
			player.objectInStomach.pos = player.abstractCreature.pos;
			player.objectInStomach.RealizeInRoom();
			if (ModManager.MMF && MMF.cfgKeyItemTracking.Value && AbstractPhysicalObject.UsesAPersistantTracker(player.objectInStomach) && player.room.game.IsStorySession && player.room.game.session is StoryGameSession storyGameSession)
			{
				storyGameSession.AddNewPersistentTracker(player.objectInStomach, player.room.world);
				if (player.room.abstractRoom.NOTRACKERS)
				{
					WorldCoordinate worldCoordinate = player.lastGoodTrackerSpawnCoord;
					player.objectInStomach.tracker.lastSeenRegion = player.lastGoodTrackerSpawnRegion;
					player.objectInStomach.tracker.lastSeenRoom = player.lastGoodTrackerSpawnRoom;
					player.objectInStomach.tracker.ChangeDesiredSpawnLocation(player.lastGoodTrackerSpawnCoord);
				}
			}
			Vector2 vector = player.bodyChunks[0].pos;
			Vector2 a = Custom.DirVec(player.bodyChunks[1].pos, player.bodyChunks[0].pos);
			bool flag = false;
			if (Mathf.Abs(player.bodyChunks[0].pos.y - player.bodyChunks[1].pos.y) > Mathf.Abs(player.bodyChunks[0].pos.x - player.bodyChunks[1].pos.x) && player.bodyChunks[0].pos.y > player.bodyChunks[1].pos.y)
			{
				vector += Custom.DirVec(player.bodyChunks[1].pos, player.bodyChunks[0].pos) * 5f;
				a *= -1f;
				a.x += 0.4f * (float)player.flipDirection;
				a.Normalize();
				flag = true;
			}
			player.objectInStomach.realizedObject.firstChunk.HardSetPosition(vector);
			player.objectInStomach.realizedObject.firstChunk.vel = Vector2.ClampMagnitude((a * 2f + Custom.RNV() * UnityEngine.Random.value) / player.objectInStomach.realizedObject.firstChunk.mass, 6f);
			player.bodyChunks[0].pos -= a * 2f;
			player.bodyChunks[0].vel -= a * 2f;
			if (player.graphicsModule != null && player.graphicsModule is PlayerGraphics playerGraphics)
			{
				playerGraphics.head.vel += Custom.RNV() * (UnityEngine.Random.value * 3f);
			}
			for (int i = 0; i < 3; i++)
			{
				player.room.AddObject(new WaterDrip(vector + Custom.RNV() * (UnityEngine.Random.value * 1.5f), Custom.RNV() * (3f * UnityEngine.Random.value) + a * Mathf.Lerp(2f, 6f, UnityEngine.Random.value), false));
			}
			player.room.PlaySound(SoundID.Slugcat_Regurgitate_Item, player.mainBodyChunk);
			if (player.objectInStomach.realizedObject is Hazer hazer && player.graphicsModule != null)
			{
				hazer.SpitOutByPlayer(PlayerGraphics.SlugcatColor(player.playerState.slugcatCharacter));
			}
			if (flag && player.FreeHand() > -1)
			{
				if (ModManager.MMF && (player.grasps[0] != null ^ player.grasps[1] != null) && player.Grabability(player.objectInStomach.realizedObject) == Player.ObjectGrabability.BigOneHand)
				{
					int num = 0;
					if (player.FreeHand() == 0)
					{
						num = 1;
					}
					if (player.Grabability(player.grasps[num].grabbed) != Player.ObjectGrabability.BigOneHand)
					{
						player.SlugcatGrab(player.objectInStomach.realizedObject, player.FreeHand());
					}
				}
				else
				{
					player.SlugcatGrab(player.objectInStomach.realizedObject, player.FreeHand());
				}
			}
			player.objectInStomach = null;
		}



	}
}
