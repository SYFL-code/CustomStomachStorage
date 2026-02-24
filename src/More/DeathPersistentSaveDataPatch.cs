using CustomStomachStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;


namespace CustomStomachStorage
{
	public static class DeathPersistentSaveDataPatch//死亡持久保存数据补丁
	{
		//public static string TotalHeader => Plugin.ID.ToUpper();//总标题
		public static string TotalHeader => "AAA";//总标题

		public static bool UnitsLoaded = false;//单元是否已加载
		public static List<DeathPersistentSaveDataUnit> units = new List<DeathPersistentSaveDataUnit>();//单元列表

		public static void Patch()//call Patch() in OnModInit  在Mod Init上调用Patch()
		{
			On.SaveState.ctor += SaveState_ctor;

			On.DeathPersistentSaveData.FromString += DeathPersistentSaveData_FromString;
			On.DeathPersistentSaveData.SaveToString += DeathPersistentSaveData_SaveToString;
		}

		private static void SaveState_ctor(On.SaveState.orig_ctor orig, SaveState self, SlugcatStats.Name saveStateNumber, PlayerProgression progression)
		{
			orig.Invoke(self, saveStateNumber, progression);
			if (UnitsLoaded)
			{
				foreach(var unit in units)
				{
					unit.ClearDataForNewSaveState(saveStateNumber);//Clear data for new save state  为新的保存状态清除数据
				}
			}
			else//Load DeathPersistentSaveDataUnits here  在此加载死亡持久保存数据单元
			{
				units.Add(new TestSaveUnit(saveStateNumber));
				UnitsLoaded = true;
			}
		}

		private static string DeathPersistentSaveData_SaveToString(On.DeathPersistentSaveData.orig_SaveToString orig, DeathPersistentSaveData self, bool saveAsIfPlayerDied, bool saveAsIfPlayerQuit)
		{
			string result = orig.Invoke(self, saveAsIfPlayerDied, saveAsIfPlayerQuit);

			foreach (var unit in units)
			{
				string header = unit.header;
				string data = unit.SaveToString(saveAsIfPlayerDied, saveAsIfPlayerQuit);
				if(header != string.Empty && data != string.Empty)
				{
					result += TotalHeader + header + "<dpB>" + data + "<dpA>";
				}
				UDebug.Log($"SaveToString__{unit.ToString()}");
			}

			UDebug.Log($"SaveToString_saveAsIfPlayerDied:{saveAsIfPlayerDied.ToString()} SaveToString_saveAsIfPlayerQuit:{saveAsIfPlayerQuit.ToString()}");
			UDebug.Log($"SaveToString_{result}");

			return result;
		}

		static private void DeathPersistentSaveData_FromString(On.DeathPersistentSaveData.orig_FromString orig, DeathPersistentSaveData self, string s)
		{
			orig.Invoke(self, s);

			string[] array = Regex.Split(s, "<dpA>");
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = Regex.Split(array[i], "<dpB>");
				string header = array2[0];

				foreach(var unit in units)
				{
					if (unit.header == header)
					{
						unit.LoadDatas(array2[1]);
						UDebug.Log($"FromString__{unit.ToString()}");
					}

				}
			}

			UDebug.Log($"FromString_{s}");
		}

		public static DeathPersistentSaveDataUnit? GetUnitOfHeader(string header)//Get unit of header  获取标题的单元
		{
			foreach(var unit in units)
			{
				if (unit.header == header) return unit;
			}
			return null;
		}
	}

	public class DeathPersistentSaveDataUnit//死亡持久保存数据单元
	{
		public SlugcatStats.Name slugName;//The slugcat name this unit belongs to  这个单元所属的slugcat名字

		public string origSaveData;//The original save data loaded from the save file, used for reference when saving as if player died or quit  从存档文件加载的原始存档数据，在以玩家死亡或退出的方式保存时用作参考
		public virtual string header => "";
  
		public DeathPersistentSaveDataUnit(SlugcatStats.Name name)
		{
			slugName = name;
		}

		public virtual string SaveToString(bool saveAsIfPlayerDied, bool saveAsIfPlayerQuit)//Save data to string, called when saving  保存数据到字符串，在保存时调用
		{
			return "";
		}

		public virtual void LoadDatas(string data)//Load data from string, called when loading  从字符串加载数据，在加载时调用
		{
			origSaveData = data;
		}

		public virtual void ClearDataForNewSaveState(SlugcatStats.Name newSlugName)//Clear data for new save state, called when creating new save state  为新的保存状态清除数据，在创建新的保存状态时调用
		{
			origSaveData = "";
			slugName = newSlugName;
		}

		public override string ToString()
		{
			return base.ToString() + " SlugStateName:" + slugName.ToString() + " header:" + header;
		}
	}

	//This is a simpleTest  这是一个简单的测试
	public class TestSaveUnit : DeathPersistentSaveDataUnit//测试保存单元
	{
		public int loadThisForHowManyTimes = 0;//加载此次数多少次
		public TestSaveUnit(SlugcatStats.Name name) : base(name)
		{
		}

		public override string header => "THISISJUSTATESTLOL";// 这只是一个状态

		public override void LoadDatas(string data)
		{
			base.LoadDatas(data);

			loadThisForHowManyTimes = int.Parse(data);
		}

		public override string SaveToString(bool saveAsIfPlayerDied, bool saveAsIfPlayerQuit)//当因为玩家死亡而保存时，saveAsIfPlayerDied为true，当因为玩家退出而保存时，saveAsIfPlayerQuit为true
		{
			if (saveAsIfPlayerDied || saveAsIfPlayerQuit) return origSaveData;
			else
			{
				loadThisForHowManyTimes++;
				return loadThisForHowManyTimes.ToString();
			}
		}

		public override void ClearDataForNewSaveState(SlugcatStats.Name newSlugName)
		{
			base.ClearDataForNewSaveState(newSlugName);
			loadThisForHowManyTimes = 0;
		}

		public override string ToString()
		{
			return base.ToString() + " loadThisForHowManyTimes:" + loadThisForHowManyTimes.ToString();
		}
	}
}
