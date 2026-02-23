using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CustomStomachStorage.GlobalVar;
using static CustomStomachStorage.MyPlayer;
using static CustomStomachStorage.Plugin;


namespace CustomStomachStorage
{
	public class State
	{

		public static void HookAdd()
		{
			//保存数据
			On.SaveState.SaveToString += SaveState_SaveToString;
			//读取数据
			On.SaveState.LoadGame += SaveState_LoadGame;
		}
		public static void HookSubtract()
		{
			//保存数据
			On.SaveState.SaveToString -= SaveState_SaveToString;
			//读取数据
			On.SaveState.LoadGame -= SaveState_LoadGame;
		}

		public const string svA = "<svA>";
		public const string svB = "<svB>";
		public const string svC = "<svC>";
		public const string svD = "<svD>";
		public const string PlayerStr = "Player";

		private static void SaveState_LoadGame(On.SaveState.orig_LoadGame orig, SaveState saveState, string str, RainWorldGame game_)
		{
#if MYDEBUG
			try
			{
#endif

			#region B_LoadGame
			UDebug.Log($">>> [0] 方法入口_{GlobalVar.playersStrStomachs.Count}_LoadGame");

			// 检查字典
			UDebug.Log(">>> [10] 检查字典");
			if (playersStrStomachs == null)
			{
				UDebug.LogWarning(">>> [WARN] 字典为 null，重新创建");
				playersStrStomachs = new Dictionary<int, List<string>>();
			}
			else
			{
				UDebug.Log(">>> [11] 清空字典");
				playersStrStomachs.Clear();
			}

			ESS.stomachItems.Clear();

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
			#endregion

				UDebug.Log(">>> [12] 检查 ESS_savefield_name");

				// 查找字段名在文本中的首次出现位置
				int index_start = str.IndexOf(ESS_savefield_name);

				if (index_start == -1)
				{
					UDebug.Log(">>> [13] 未找到数据index_start，返回");
					return;
				}

				// 查找字段值结束标签"<svA>"的位置
				// 从字段名位置开始搜索，确保找到的是当前字段对应的结束标签
				int endTagIndex = str.IndexOf(svA, index_start);

				if (endTagIndex == -1)
				{
					UDebug.Log(">>> [13] 未找到数据endTagIndex，返回");
					return;
				}

				int index_end = endTagIndex;
				//int index_end = endTagIndex + svA.Length;
				// 注意：+5是为了包含"<svA>"标签本身（标签长度为5个字符）

				// 计算要移除的文本长度（从字段名开始到结束标签之后）
				int removeLength = index_end - index_start;

				string removedContent = str.Substring(index_start, removeLength);

				string[] dictionary = Regex.Split(removedContent, svB);

				if (dictionary == null || dictionary.Length < 2)
				{
					UDebug.Log(">>> [20] dictionary 长度不足，跳过");
					return;
				}

				UDebug.Log($">>> [21] dictionary[0] = {dictionary[0]}");

				if (dictionary[0] != ESS_savefield_name)
				{
					UDebug.Log(">>> [22] 不是数据，跳过");
					return;
				}

				string savedData = dictionary[1];

				if (string.IsNullOrEmpty(savedData))
				{
					UDebug.LogWarning(">>> [WARN] savedData 为空");
					return;
				}

				string[] savePlayers = Regex.Split(savedData, svC);

				if (savePlayers == null || savePlayers.Length == 0)
				{
					UDebug.LogError(">>> [ERR] savePlayers 为空");
					return;
				}

				foreach (var savePlayer in savePlayers)
				{
					UDebug.Log($">>> [26] 处理 savePlayer 块");

					if (string.IsNullOrEmpty(savePlayer))
					{
						UDebug.Log(">>> [27] savePlayer 为空，跳过");
						continue;
					}

					UDebug.Log(">>> [28] 分割 PlayerAndData");
					string[] PlayerAndData = Regex.Split(savePlayer, svD);

					if (PlayerAndData == null || PlayerAndData.Length < 2)
					{
						UDebug.Log(">>> [30] PlayerAndData 长度不足，跳过");
						continue;
					}

					UDebug.Log($">>> [31] PlayerAndData[0] = {PlayerAndData[0]}");
					if (!PlayerAndData[0].Contains(PlayerStr))
					{
						UDebug.Log(">>> [32] 不包含 Player，跳过");
						continue;
					}

					UDebug.Log(">>> [34] 解析玩家编号");
					string playerNumStr = PlayerAndData[0].Replace(PlayerStr, "");

					if (!int.TryParse(playerNumStr, out int N))
					{
						UDebug.LogWarning($">>> [WARN] 无法解析玩家编号: {PlayerAndData[0]}");
						continue;
					}
					UDebug.Log($">>> [35] 玩家编号 N = {N}");

					string? Data = PlayerAndData[1];

					if (string.IsNullOrEmpty(Data) || Data == null)
					{
						UDebug.LogWarning($">>> [WARN] Player{N} 数据为空");
						continue;
					}

					UDebug.Log(">>> [36] 分割物品列表");
					List<string> result = Data.Split(',').ToList();
					UDebug.Log($">>> [37] 物品数量: {result?.Count ?? -1}");

					if (result != null && result.Count > 0 && N >= 0)
					{
						UDebug.Log($">>> [44] 保存 Player{N} 的 {result.Count} 个物品");
						playersStrStomachs[N] = result;
					}
					else
					{
						UDebug.Log($">>> [45] Player{N} 无有效物品");
					}


				}

				// 分割字符串
				/*UDebug.Log(">>> [14] 分割 sA");
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

						string? savedDataOther = sD[1];
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

						if (string.IsNullOrEmpty(savedDataOther) || savedDataOther == null)
						{
							UDebug.LogWarning($">>> [WARN] Player{N} 数据为空");
							continue;
						}

						UDebug.Log(">>> [36] 分割物品列表");
						List<string> result = savedDataOther.Split(',').ToList();
						UDebug.Log($">>> [37] 物品数量: {result?.Count ?? -1}");

						if (result != null && result.Count > 0 && N >= 0)
						{
							UDebug.Log($">>> [44] 保存 Player{N} 的 {result.Count} 个物品");
							playersStrStomachs[N] = result;
						}
						else
						{
							UDebug.Log($">>> [45] Player{N} 无有效物品");
						}


					}
				}*/
				//

				UDebug.Log(">>> [47] 方法结束");

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

		private static string SaveState_SaveToString(On.SaveState.orig_SaveToString orig, SaveState saveState)
		{
			UDebug.Log(">>> [1] 方法入口_SaveState");
			string text = orig(saveState);
			UDebug.Log($"{text}_SaveToString_B");

#if MYDEBUG
			try
			{
#endif

				if (_game == null || _game?.world == null || _game.Players == null)
				{
					return text;
				}

				if (string.IsNullOrEmpty(ESS_savefield_name))
				{
					return text;
				}

				string extendedText = "";

				// 查找字段名在文本中的首次出现位置
				int index_start = text.IndexOf(ESS_savefield_name);

				if (index_start != -1)
				{
					// 从字段名位置开始搜索，确保找到的是当前字段对应的结束标签
					int endTagIndex = text.IndexOf(svA, index_start);

					// 找到 svB 的位置（在 ESS_savefield_name 之后）
					int svBPos = text.IndexOf(svB, index_start);

					if (endTagIndex != -1 && svBPos != -1)
					{
						int svBIndex = svBPos + svB.Length;

						//int index_end = endTagIndex + svA.Length;
						// 注意：+5是为了包含"<svA>"标签本身（标签长度为5个字符）

						// 计算文本长度（从字段名开始到结束标签之后）
						//int removeLength = index_end - index_start;

						// 字段名及其对应的值（包括结束标签）
						string removedContent = text.Substring(svBIndex, endTagIndex - svBIndex);

						extendedText = removedContent;
					}
				}

				text = RemoveField(text, ESS_savefield_name);

				//string extendedText = "";
				foreach (var absc in _game.Players)
				{
					if (absc == null || absc.realizedCreature == null)
					{
						continue;
					}
					if (absc.realizedCreature is Player player)
					{
						int N = player.playerState.playerNumber;
						List<AbstractPhysicalObject> stomach = ESS.GetstomachItems(player);
						if (stomach != null)
						{
							string strings = "";
							foreach (var item in stomach)
							{
								if (item is AbstractCreature abstractCreature3)
								{
									if (_game.world.GetAbstractRoom(abstractCreature3.pos.room) == null)
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
							if (strings.Length > 0)
							{
								strings = strings.Substring(0, strings.Length - 1);
							}

							int index_start_ = extendedText.IndexOf($"{PlayerStr}{N}");

							if (index_start_ != -1)
							{
								int endTagIndex = extendedText.IndexOf(svC, index_start_);

								if (endTagIndex != -1)
								{
									int index_end = endTagIndex + svC.Length;
									// 注意：+5是为了包含"<svC>"标签本身（标签长度为5个字符）

									// 计算文本长度（从字段名开始到结束标签之后）
									int removeLength = index_end - index_start_;

									// 字段名及其对应的值（包括结束标签）
									//string removedContent = extendedText.Substring(index_start_, removeLength);
									extendedText = extendedText.Remove(index_start_, removeLength);
									//extendedText = removedContent;
								}
							}

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

			UDebug.Log($"{text}_SaveToString_A");
			return text;
		}


		public static string RemoveField(string dataText, string fieldName)
		{
#if MYDEBUG
			try
			{
#endif

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

				int index_end = endTagIndex + svA.Length;
				// 注意：+5是为了包含"<svA>"标签本身（标签长度为5个字符）

				// 计算要移除的文本长度（从字段名开始到结束标签之后）
				int removeLength = index_end - index_start;

				// 移除字段名及其对应的值（包括结束标签）
				dataText = dataText.Remove(index_start, removeLength);

				// 查找下一个匹配的字段名位置
				index_start = dataText.IndexOf(fieldName);
			}

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

			// 返回处理后的文本
			return dataText;
		}




	}
}
