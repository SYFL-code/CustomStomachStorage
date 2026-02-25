#region using
using BepInEx;
using SlugBase.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
#endregion

namespace CustomStomachStorage
{
	internal class Zname//Scrap 废案
	{
		#region Items
		#endregion
		#region Creatures
		#endregion




		//mklink /j "D:\Steam\steamapps\common\Rain World\RainWorld_Data\StreamingAssets\mods\EnderPearl" "D:\Other\EnderPearl\mod"

		//<DefineConstants>MYDEBUG</DefineConstants>


		#region More
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

			#region room
			//room
			//player.room.game.Players
			//player.room.game.GetStorySession.Players
			//player.room.game.warpDeferPlayerSpawnRoomName
			//player.room.abstractRoom.name
			#endregion
			#region Debug
			//Debug
			//Debug.Log("普通消息");
			//Debug.LogWarning("警告消息");
			//Debug.LogError("错误消息");
			#endregion
		}
		#endregion

		#region Save
		private void Save()
		{
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
		#endregion

		#region 按键
		/*public static readonly PlayerKeybind Explode = PlayerKeybind.Register(
				"example:explode",      // 唯一ID（格式：作者:功能）
				"Example Mod",          // 模组显示名称
				"Explode",              // 按键显示名称
				KeyCode.C,              // 键盘默认键（C键）
				KeyCode.JoystickButton3 // 手柄默认键（通常是RB或R1）
			);*/
		#endregion

		#region Items
		string[] baseItemTypes = {
				"Item",
				"Rock",
				"Spear",
				"VultureMask",
				"NeedleEgg",
				"OracleSwarmer",
				"SeedCob",
				"SporePlant",
				"FlareBomb",
				"PuffBall",
				"FirecrackerPlant",
				"KarmaFlower",
			};
		string[] mscItemTypes = {
				"LillyPuck",
				"FireEgg",
				"JokeRifle",
				"EnergyCell",
				"MoonCloak",
			};
		string[] watcherItemTypes = {
				"Boomerang",
				"GraffitiBomb",
			};
		#endregion
		#region Creatures
			string[] baseCreatureTypes = {
				"Creature",
				"Slugcat",
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


	}
}

#region 胖世界 BPOptions
/*public class BPOptions : OptionInterface
{


	public BPOptions()
	{
		//BPOptions.hardMode = this.config.Bind<bool>("hardMode", true, new ConfigurableInfo("(This is just info I guess)", null, "", new object[] { "something idk" }));

		BPOptions.hardMode = this.config.Bind<bool>("hardMode", false);
		BPOptions.holdShelterDoor = this.config.Bind<bool>("holdShelterDoor", false);
		BPOptions.backFoodStorage = this.config.Bind<bool>("backFoodStorage", false);
		BPOptions.easilyWinded = this.config.Bind<bool>("easilyWinded", false);
		BPOptions.extraTime = this.config.Bind<bool>("extraTime", true);
		BPOptions.hudHints = this.config.Bind<bool>("hudHints", true);
		BPOptions.fatArmor = this.config.Bind<bool>("fatArmor", true);
		BPOptions.slugSlams = this.config.Bind<bool>("slugSlams", true);
		//BPOptions.dietNeedles = this.config.Bind<bool>("dietNeedles", false);
		BPOptions.detachNeedles = this.config.Bind<bool>("detachNeedles", false);
		BPOptions.detachablePopcorn = this.config.Bind<bool>("detachablePopcorn", true);
		BPOptions.foodLoverPerk = this.config.Bind<bool>("foodLoverPerk", false);
		BPOptions.visualsOnly = this.config.Bind<bool>("visualsOnly", false);

		BPOptions.debugTools = this.config.Bind<bool>("debugTools", false);
		BPOptions.debugLogs = this.config.Bind<bool>("debugLogs", false);
		BPOptions.blushEnabled = this.config.Bind<bool>("blushEnabled", false);
		BPOptions.bpDifficulty = this.config.Bind<float>("bpDifficulty", -2f, new ConfigAcceptableRange<float>(-5f, 5f));
		BPOptions.sfxVol = this.config.Bind<float>("sfxVol", 0.1f, new ConfigAcceptableRange<float>(-0.1f, 0.4f));
		BPOptions.startThresh = this.config.Bind<int>("startThresh", 4, new ConfigAcceptableRange<int>(-4, 8));//(0, 4)
		BPOptions.gapVariance = this.config.Bind<float>("gapVariance", 1.0f, new ConfigAcceptableRange<float>(0.5f, 1.75f));
		BPOptions.jokeContent1 = this.config.Bind<bool>("jokeContent1", true);
		BPOptions.foodMult = this.config.Bind<int>("foodMult", 1, new ConfigAcceptableRange<int>(1, 4));
		BPOptions.meadowFoodStart = this.config.Bind<int>("meadowFoodStart", 4, new ConfigAcceptableRange<int>(0, 25));
		BPOptions.visualFatScale = this.config.Bind<float>("visualFatScale", 1.0f, new ConfigAcceptableRange<float>(0.33f, 1.00f));

		BPOptions.fatP1 = this.config.Bind<bool>("fatP1", true);
		BPOptions.fatP2 = this.config.Bind<bool>("fatP2", true);
		BPOptions.fatP3 = this.config.Bind<bool>("fatP3", true);
		BPOptions.fatP4 = this.config.Bind<bool>("fatP4", true);
		BPOptions.fatLiz = this.config.Bind<bool>("fatLiz", true);
		BPOptions.fatMice = this.config.Bind<bool>("fatMice", true);
		BPOptions.fatScavs = this.config.Bind<bool>("fatScavs", true);
		BPOptions.fatSquids = this.config.Bind<bool>("fatSquids", true);
		BPOptions.fatNoots = this.config.Bind<bool>("fatNoots", true);
		BPOptions.fatCentis = this.config.Bind<bool>("fatCentis", true);
		BPOptions.fatDll = this.config.Bind<bool>("fatDll", true);
		BPOptions.fatVults = this.config.Bind<bool>("fatVults", true);
		BPOptions.fatMiros = this.config.Bind<bool>("fatMiros", true);
		BPOptions.fatWigs = this.config.Bind<bool>("fatWigs", true);
		BPOptions.fatEels = this.config.Bind<bool>("fatEels", true);
		BPOptions.fatPups = this.config.Bind<bool>("fatPups", true);

		BPOptions.fatJets = this.config.Bind<bool>("fatJets", true);
		BPOptions.fatDeer = this.config.Bind<bool>("fatDeer", true);
		BPOptions.fatYeeks = this.config.Bind<bool>("fatYeeks", true);
		BPOptions.fatLeechs = this.config.Bind<bool>("fatLeechs", true);
		BPOptions.fatMoths = this.config.Bind<bool>("fatMoths", true);
		BPOptions.fatTards = this.config.Bind<bool>("fatTards", true);
		BPOptions.fatLoachs = this.config.Bind<bool>("fatLoachs", true);
	}



	public static Configurable<bool> blushEnabled;
	public static Configurable<bool> debugTools;
	public static Configurable<bool> holdShelterDoor;
	public static Configurable<bool> backFoodStorage;
	public static Configurable<bool> hardMode;
	public static Configurable<bool> easilyWinded;
	public static Configurable<bool> extraTime;
	public static Configurable<bool> hudHints;
	public static Configurable<bool> fatArmor;
	public static Configurable<bool> slugSlams;
	public static Configurable<bool> debugLogs;
	public static Configurable<float> bpDifficulty;
	public static Configurable<float> sfxVol;
	public static Configurable<int> startThresh;
	public static Configurable<float> gapVariance;
	public static Configurable<bool> detachNeedles;
	public static Configurable<bool> visualsOnly;
	public static Configurable<bool> jokeContent1;
	public static Configurable<bool> detachablePopcorn;
	public static Configurable<bool> foodLoverPerk;
	public static Configurable<int> foodMult;
	public static Configurable<int> meadowFoodStart;
	public static Configurable<float> visualFatScale;

	public static Configurable<bool> fatP1;
	public static Configurable<bool> fatP2;
	public static Configurable<bool> fatP3;
	public static Configurable<bool> fatP4;
	public static Configurable<bool> fatLiz;
	public static Configurable<bool> fatMice;
	public static Configurable<bool> fatScavs;
	public static Configurable<bool> fatSquids;
	public static Configurable<bool> fatNoots;
	public static Configurable<bool> fatCentis;
	public static Configurable<bool> fatDll;
	public static Configurable<bool> fatVults;
	public static Configurable<bool> fatMiros;
	public static Configurable<bool> fatWigs;
	public static Configurable<bool> fatEels;
	public static Configurable<bool> fatPups;
	public static Configurable<bool> fatJets;
	public static Configurable<bool> fatDeer;
	public static Configurable<bool> fatYeeks;
	public static Configurable<bool> fatLeechs;
	public static Configurable<bool> fatMoths;
	public static Configurable<bool> fatTards;
	public static Configurable<bool> fatLoachs;
	//Lizard
	//Lantern Mice
	//Scavengers

	//Squidcada
	//Noodle Flies
	//Centipedes

	//DLL
	//Vultures
	//Miros Birds

	//Dropwigs
	//Leviathan


	private OpSimpleButton presetSilly;
	private OpSimpleButton presetBalanced;
	private OpSimpleButton presetPipeCleaner;


	public override void Update()
	{
		base.Update();

		//this.Tabs[0].items

		if (this.chkBoxVisOnly != null)
		{
			if (this.chkBoxVisOnly.GetValueBool() == true)
			{
				this.diffSlide.greyedOut = true;
				this.opLab1.Hidden = true;
				this.opLab2.Hidden = true;
				for (int i = 1; i < myBoxes.Length; i++)
				{
					if (myBoxes[i] != null)
						myBoxes[i].greyedOut = true;
				}
			}
			else
			{
				this.diffSlide.greyedOut = false;
				this.opLab1.Hidden = false;
				this.opLab2.Hidden = false;
				for (int i = 1; i < myBoxes.Length; i++)
				{
					if (myBoxes[i] != null)
						myBoxes[i].greyedOut = false;
				}
			}
		}

	}

	public static string BPTranslate(string t)
	{
		return OptionInterface.Translate(t); //this.manager.rainWorld.inGameTranslator.BPTranslate(t);
	}


	public OpFloatSlider diffSlide;
	public OpCheckBox chkBoxVisOnly;

	public OpCheckBox chkBoxslugSlams;
	public OpCheckBox chkBoxNeedles;

	public OpLabel opLab1;
	public OpLabel opLab2;

	public static OpCheckBox[] myBoxes;


	public void SillyPreset(UIfocusable trigger)
	{
		//for (int i = 0; i < MMF.boolPresets.Count; i++)
		//{
		//    if (MMF.boolPresets[i].config.BoundUIconfig != null)
		//    {
		//        //MMF.boolPresets[i].config.BoundUIconfig.value = ValueConverter.ConvertToString<bool>(MMF.boolPresets[i].remixValue);
		//    }
		//}
		this.diffSlide.SetValueFloat(-2f);
	}

	public void BalancedPreset(UIfocusable trigger)
	{
		this.diffSlide.SetValueFloat(0f);
	}

	public void PipeCleanerPreset(UIfocusable trigger)
	{
		this.diffSlide.SetValueFloat(3f);
	}


	public override void Initialize()
	{
		base.Initialize();

		// OpTab opTab = new OpTab(this, "Options");
		this.Tabs = new OpTab[]
		{
			//opTab
			new OpTab(this, BPTranslate("Options")),
			new OpTab(this, BPTranslate("Misc")),
			new OpTab(this, BPTranslate("Creatures")),
			new OpTab(this, BPTranslate("Info"))
		};

		Vector2 btnSize = new Vector2(130f, 25f);
		float btnHeight = 548f;
		Tabs[0].AddItems(
			 this.presetSilly = new OpSimpleButton(new Vector2(40f, btnHeight), btnSize, "Silly") { description = OptionInterface.Translate("A more relaxed and goofy experience with fewer downsides to becoming round (Default)") },
			 this.presetBalanced = new OpSimpleButton(new Vector2(40f + 145f, btnHeight), btnSize, "Balanced") { description = OptionInterface.Translate("The original mod experience of risk and reward where each extra pip comes with tradeoffs to consider") },
			 this.presetPipeCleaner = new OpSimpleButton(new Vector2(40f + 290f, btnHeight), btnSize, "Pipe Cleaner") { description = OptionInterface.Translate("A challenge mode for only the most stubborn food enthusiests") }
		   );
		Tabs[0].AddItems(new OpLabel(40F, 578, BPTranslate("Difficulty Presets")));
		this.presetSilly.OnClick += this.SillyPreset;
		this.presetBalanced.OnClick += this.BalancedPreset;
		this.presetPipeCleaner.OnClick += this.PipeCleanerPreset;


		float lineCount = 500;

		Tabs[0].AddItems(new OpLabel(175f, 595, BPTranslate("Hover over a setting to read more info about it")));


		//OpLabel opLabel = new OpLabel(new Vector2(100f, opRect.size.y - 25f), new Vector2(30f, 25f), ":(", FLabelAlignment.Left, true, null)
		//OpCheckBox opCheckBox = new OpCheckBox(this.config as Configurable<bool>, posX, posY)
		//this.numberPlayersSlider = new OpSliderTick(menu.oi.config.Bind<int>("_cosmetic", Custom.rainWorld.options.JollyPlayerCount, new ConfigAcceptableRange<int>(1, 4)), this.playerSelector[0].pos + new Vector2((float)num / 2f, 130f), (int)(this.playerSelector[3].pos - this.playerSelector[0].pos).x, false);
		// Tabs[0].AddItems(new OpLabel(50f, lineCount - 20f, "Makes squeezing through pipes even more difficult for fatter creatures") { description = "This is My Text" });

		//Tabs[0].AddItems(new OpLabel(50f, lineCount - 20f, "Makes squeezing through pipes even more difficult for fatter creatures") { description = "This is My Text" });
		// Tabs[0].AddItems(new OpSlider(BPOptions.bpDifficulty, new Vector2(50f, lineCount), 50, false));
		this.diffSlide = new OpFloatSlider(BPOptions.bpDifficulty, new Vector2(55f, lineCount - 0), 250, 0, false);
		string discDiff = BPTranslate("Sets the average difficulty for squeezing through pipes, and the impact weight has on your agility");
		Tabs[0].AddItems(this.diffSlide, new OpLabel(50f, lineCount - 15, BPTranslate("Pipe Size Difficulty")) { bumpBehav = this.diffSlide.bumpBehav, description = discDiff });
		this.diffSlide.description = discDiff;
		Tabs[0].AddItems(this.opLab1 = new OpLabel(15f, lineCount + 5, BPTranslate("Wide")) { description = BPTranslate("Easy") });
		Tabs[0].AddItems(this.opLab2 = new OpLabel(320f, lineCount + 5, BPTranslate("Snug")) { description = BPTranslate("Hard") });

		//OpCheckBox chkBox5 = new OpCheckBox(BPOptions.hardMode, new Vector2(15f, lineCount));
		//Tabs[0].AddItems(chkBox5, new OpLabel(45f, lineCount, "Snug Pipes") { bumpBehav = chkBox5.bumpBehav });



		*//*
		OpFloatSlider agilSlide = new OpFloatSlider(BPOptions.agilityDiff, new Vector2(55f, lineCount - 0), 250, 0, false);
		Tabs[0].AddItems(agilSlide, new OpLabel(50f, lineCount - 15, BPTranslate("Agility Penalty")) { bumpBehav = agilSlide.bumpBehav, description = BPTranslate("Your weight has a more noticeable impact on your ability to run, climb and jump") });

		Tabs[0].AddItems(new OpLabel(15f, lineCount + 5, BPTranslate("Easy")) );
		Tabs[0].AddItems(new OpLabel(320f, lineCount +5, BPTranslate("Hard")) );
		*//*


		string dscVisuals = BPTranslate("Removes all gameplay changes except visual ones");
		this.chkBoxVisOnly = new OpCheckBox(BPOptions.visualsOnly, new Vector2(15f + 425, lineCount));
		Tabs[0].AddItems(this.chkBoxVisOnly, new OpLabel(45f + 425, lineCount, BPTranslate("Visuals only")) { bumpBehav = this.chkBoxVisOnly.bumpBehav, description = dscVisuals });
		this.chkBoxVisOnly.description = dscVisuals;
		//chkBoxVisOnly.Hidden = true;



		float indenting = 250f;

		lineCount -= 70;
		OpSlider threshSlide = new OpSlider(BPOptions.startThresh, new Vector2(55f, lineCount - 0), 150, false);
		string dscThresh = BPTranslate("Offset how much food you need to eat to get fat. Lower values will make you fat earlier.");
		// Tabs[0].AddItems(threshSlide, new OpLabel(50f, lineCount - 15, BPTranslate("Starting threshold")) { bumpBehav = threshSlide.bumpBehav, description = BPTranslate("Sets how close to full you must be before eating food will add weight. Lower values will make you fat earlier.") });
		Tabs[0].AddItems(threshSlide, new OpLabel(50f, lineCount - 15, BPTranslate("Starting threshold")) { bumpBehav = threshSlide.bumpBehav, description = dscThresh });
		Tabs[0].AddItems(new OpLabel(15f, lineCount + 5, BPTranslate("Early")) { description = BPTranslate("You will start getting fat before your belly is full.") });
		Tabs[0].AddItems(new OpLabel(220f, lineCount + 5, BPTranslate("Late")) { description = BPTranslate("You won't start getting fat until your belly is full") });
		threshSlide.description = dscThresh;


		OpFloatSlider varianceSlide = new OpFloatSlider(BPOptions.gapVariance, new Vector2(350f, lineCount - 0), 150, 1, false);
		dscThresh = BPTranslate("Determines how wide the range of gap sizes can be. Wider variety makes easy gaps easier and harder gaps harder");
		Tabs[0].AddItems(varianceSlide, new OpLabel(varianceSlide.pos.x + 25f, lineCount - 15, BPTranslate("Pipe size variety")) { bumpBehav = varianceSlide.bumpBehav, description = dscThresh });
		Tabs[0].AddItems(new OpLabel(varianceSlide.pos.x - 45f, lineCount + 5, BPTranslate("Similar")) { description = BPTranslate("Gap sizes will be similar to each other") });
		Tabs[0].AddItems(new OpLabel(varianceSlide.pos.x + 160f, lineCount + 5, BPTranslate("Diverse")) { description = BPTranslate("Gap sizes will vary widely") });
		varianceSlide.description = dscThresh;





		//Pipes are less snug and easier to wiggle through, even when very fat
		//Makes squeezing through pipes even more difficult for fatter creatures
		//Snug Pipes
		lineCount -= 60;
		BPOptions.hardMode.Value = false;

		//OKAY MAKE IT BUT DON'T SHOW IT
		*//*-------------------------------------
		string dsc5 = BPTranslate("Outgrowing pipes is more punishing on how long it takes to wiggle through");
		//Tabs[0].AddItems(new OpLabel(50f, lineCount - 20f, BPTranslate("Outgrowing pipes is more punishing on how long it takes to wiggle through")) );
		OpCheckBox chkBox5 = new OpCheckBox(BPOptions.hardMode, new Vector2(15f + 800f, lineCount - 120f));
		Tabs[0].AddItems(chkBox5, new OpLabel(45f + 800f, lineCount - 120, BPTranslate("Unforgiving Gap Sizes")) { bumpBehav = chkBox5.bumpBehav, description = dsc5 });
		chkBox5.description = dsc5;
		*//*

		string dscHints = BPTranslate("Occasionally show in-game hints related to controls and mechanics of the mod");
		//Tabs[0].AddItems(new OpLabel(50f, lineCount - 20f, "Occasionally show in-game hints related to controls and mechanics of the mod") ); //{ description = "This is My Text" }
		OpCheckBox chkBoxHints = new OpCheckBox(BPOptions.hudHints, new Vector2(15f, lineCount));
		Tabs[0].AddItems(chkBoxHints, new OpLabel(45f, lineCount, BPTranslate("Hud Hints")) { bumpBehav = chkBoxHints.bumpBehav, description = dscHints });
		chkBoxHints.description = dscHints;


		string dscArmor = BPTranslate("Increase resistance to bites based on how fat you are") + ", as well as spears, stuns, and coldness";
		OpCheckBox chkBoxArmor = new OpCheckBox(BPOptions.fatArmor, new Vector2(15f + indenting, lineCount));
		Tabs[0].AddItems(chkBoxArmor, new OpLabel(45f + indenting, lineCount, BPTranslate("Fat Armor")) { bumpBehav = chkBoxArmor.bumpBehav, description = dscArmor });
		chkBoxArmor.description = dscArmor;





		lineCount -= 40;
		string dsc6 = BPTranslate("Your weight has a more noticeable impact on your ability to run, climb and jump");
		//Tabs[0].AddItems(new OpLabel(50f, lineCount - 20f, BPTranslate("Your weight has a more noticeable impact on your ability to run, climb and jump")) );
		OpCheckBox chkBox6 = new OpCheckBox(BPOptions.easilyWinded, new Vector2(15f, lineCount));
		Tabs[0].AddItems(chkBox6, new OpLabel(45f, lineCount, BPTranslate("Easily Winded")) { bumpBehav = chkBox6.bumpBehav, description = dsc6 });
		chkBox6.description = dsc6;


		string dscCorn = BPTranslate("Popcorn plants can be torn from their stems");
		OpCheckBox mpBox1 = new OpCheckBox(BPOptions.detachablePopcorn, new Vector2(15f + indenting, lineCount));
		Tabs[0].AddItems(mpBox1, new OpLabel(45f + indenting, lineCount, BPTranslate("Detachable Popcorn Plants")) { bumpBehav = mpBox1.bumpBehav, description = dscCorn });
		mpBox1.description = dscCorn;





		lineCount -= 40;
		string dsc4 = BPTranslate("Double-tap the Grab button to store an edible item on your back like a spear");
		//Tabs[0].AddItems(new OpLabel(50f, lineCount - 20f, BPTranslate("Double-tap the Grab button to store an edible item on your back like a spear")) );
		OpCheckBox chkBox4 = new OpCheckBox(BPOptions.backFoodStorage, new Vector2(15f, lineCount));
		Tabs[0].AddItems(chkBox4, new OpLabel(45f, lineCount, BPTranslate("Back Food Storage")) { bumpBehav = chkBox4.bumpBehav, description = dsc4 });
		chkBox4.description = dsc4;




		string dscFood = BPTranslate("Allows the player to eat all food types for their full value");
		OpCheckBox mpBox2 = new OpCheckBox(BPOptions.foodLoverPerk, new Vector2(15f + indenting, lineCount));
		Tabs[0].AddItems(mpBox2, new OpLabel(45f + indenting, lineCount, BPTranslate("Food Lover")) { bumpBehav = mpBox2.bumpBehav, description = dscFood });
		mpBox2.description = dscFood;


		lineCount -= 40;
		string dsc7 = BPTranslate("Slightly increase the cycle timer to account for the slowdowns");
		//Tabs[0].AddItems(new OpLabel(50f, lineCount - 20f, BPTranslate("Slightly increase the cycle timer to account for the slowdowns")) );
		OpCheckBox chkBox7 = new OpCheckBox(BPOptions.extraTime, new Vector2(15f, lineCount));
		Tabs[0].AddItems(chkBox7, new OpLabel(45f, lineCount, BPTranslate("Extra Cycle Time")) { bumpBehav = chkBox7.bumpBehav, description = dsc7 });
		chkBox7.description = dsc7;


		if (ModManager.MSC)
		{
			string dscSlams = BPTranslate("All slugcats can do Gourmand's body slam, if fat enough") + " (" + BPTranslate("an audio queue will play" + ")");
			this.chkBoxslugSlams = new OpCheckBox(BPOptions.slugSlams, new Vector2(15f + indenting, lineCount));
			Tabs[0].AddItems(this.chkBoxslugSlams, new OpLabel(45f + indenting, lineCount, BPTranslate("Slug Slams")) { bumpBehav = this.chkBoxslugSlams.bumpBehav, description = dscSlams });
			this.chkBoxslugSlams.description = dscSlams;
		}
		else
		{
			BPOptions.slugSlams.Value = false;
		}


		if (ModManager.MSC)
		{
			lineCount -= 40;
			//string dscNeedles = BPTranslate("Spearmaster's needles will gain less food when your belly is full");
			//Diet Needles
			string dscNeedles = BPTranslate("When Spearmaster is full, switching hands (double tap grab) will detatch your needles");
			this.chkBoxNeedles = new OpCheckBox(BPOptions.detachNeedles, new Vector2(15f + 0, lineCount));
			Tabs[0].AddItems(this.chkBoxNeedles, new OpLabel(45f + 0, lineCount, BPTranslate("Detachable Needles")) { bumpBehav = this.chkBoxNeedles.bumpBehav, description = dscNeedles });
			this.chkBoxNeedles.description = dscNeedles;
		}


		//lineCount -= 40;
		OpSlider meadowSizeSlider = new OpSlider(BPOptions.meadowFoodStart, new Vector2(15f + indenting, lineCount - 10), 200, false);
		string dscMeadowSizeSlider = BPTranslate("Set how rotund your character will be when joining Meadow-Mode lobbies online. Requires the Rain Meadow mod");
		Tabs[0].AddItems(meadowSizeSlider, new OpLabel(15f + indenting, lineCount - 25, BPTranslate("Meadow Mode character fatness")) { bumpBehav = meadowSizeSlider.bumpBehav, description = dscMeadowSizeSlider });
		meadowSizeSlider.description = dscMeadowSizeSlider;



		myBoxes = new OpCheckBox[10];
		myBoxes[0] = null; // chkBox5;
		myBoxes[1] = chkBoxArmor;
		myBoxes[2] = chkBox6;
		myBoxes[3] = chkBox4;
		myBoxes[4] = chkBox7;
		myBoxes[5] = chkBoxHints;
		myBoxes[6] = mpBox1;
		myBoxes[7] = mpBox2;
		if (ModManager.MSC)
		{
			myBoxes[8] = this.chkBoxslugSlams;
			myBoxes[9] = this.chkBoxNeedles;
		}



		//EHH, WE CAN DO WTHIS WITH SANDBOX MODE
		//OpCheckBox chkBox3 = new OpCheckBox(50f, 250f, "noRain", false);
		//      Tabs[0].AddItems(chkBox3,
		//          new OpLabel(50f, 280f, "No Rain") { bumpBehav = chkBox2.bumpBehav });

		//lineCount -= 65;
		//Tabs[0].AddItems(new OpLabel(50f, lineCount - 20f, "If enabled; shelter doors won't automatically close unless a player holds down to sleep") { description = "This is My Text" });
		//OpCheckBox chkBox3 = new OpCheckBox(BPOptions.holdShelterDoor, new Vector2(15f, lineCount));
		//Tabs[0].AddItems(chkBox3, new OpLabel(45f, lineCount, "Hold Shelter Doors") { bumpBehav = chkBox3.bumpBehav });







		//------------------ NEW TAB FOR OTHER STUFF



		lineCount = 550;
		Tabs[1].AddItems(new OpLabel(50f, lineCount - 20f, BPTranslate("Press Throw + Jump to add food pips. Press Crouch + Throw + Jump to subtract")));
		OpCheckBox chkBox2 = new OpCheckBox(BPOptions.debugTools, new Vector2(15f, lineCount));
		Tabs[1].AddItems(chkBox2, new OpLabel(45f, lineCount, BPTranslate("Debug Tools")) { bumpBehav = chkBox2.bumpBehav });

		lineCount -= 65;
		Tabs[1].AddItems(new OpLabel(50f, lineCount - 20f, BPTranslate("Development logs. For development things")));
		OpCheckBox chkLogs = new OpCheckBox(BPOptions.debugLogs, new Vector2(15f, lineCount));
		Tabs[1].AddItems(chkLogs, new OpLabel(45f, lineCount, BPTranslate("Debug Logs")) { bumpBehav = chkLogs.bumpBehav });


		lineCount -= 65;
		Tabs[1].AddItems(new OpLabel(50f, lineCount - 20f, BPTranslate("Adds a panting and red-faced visual effect when struggling for long periods of time")));
		OpCheckBox chkExample = new OpCheckBox(BPOptions.blushEnabled, new Vector2(15f, lineCount));
		Tabs[1].AddItems(chkExample, new OpLabel(45f, lineCount, BPTranslate("Exhaustion FX")) { bumpBehav = chkExample.bumpBehav });
		Tabs[1].AddItems(new OpLabel(50f, lineCount - 35f, BPTranslate("(can cause some visual glitches with held items)")));


		lineCount -= 95;
		OpFloatSlider sfxSlide = new OpFloatSlider(BPOptions.sfxVol, new Vector2(30f, lineCount - 25), 250, 2, false);
		string dscSFXvol = BPTranslate("Volume of the squeeze sound effect when slugcats are stuck");
		Tabs[1].AddItems(sfxSlide, new OpLabel(30f, lineCount + 15, BPTranslate("Squeeze SFX Volume")) { bumpBehav = sfxSlide.bumpBehav, description = dscSFXvol });
		Tabs[1].AddItems(new OpLabel(10f, lineCount - 40f, BPTranslate("(If the sfx is too soft or played without headphones)")));
		sfxSlide.description = dscSFXvol;

		OpSlider foodMultSlide = new OpSlider(BPOptions.foodMult, new Vector2(325f, lineCount - 25), 125, false);
		string dscFoodMult = BPTranslate("Multiplies how much food you get per-food");
		Tabs[1].AddItems(foodMultSlide, new OpLabel(325f, lineCount + 15, BPTranslate("Food Multiplier")) { bumpBehav = foodMultSlide.bumpBehav, description = dscFoodMult });
		foodMultSlide.description = dscFoodMult;

		OpFloatSlider fatScaleSlider = new OpFloatSlider(BPOptions.visualFatScale, new Vector2(475f, lineCount - 25), 125, 1, false); //WHY DOES THIS NOT WORK IF ITS 2 DECIMALS?????? HELLO???
		string dscFatScale = BPTranslate("The scale at which food points affect your visual size. Lower numbers makes fat look less pronounced.");
		Tabs[1].AddItems(fatScaleSlider, new OpLabel(475f, lineCount + 15, BPTranslate("Fat Visual Scale")) { bumpBehav = fatScaleSlider.bumpBehav, description = dscFatScale });
		fatScaleSlider.description = dscFatScale;

		lineCount -= 80;
		Tabs[1].AddItems(new OpLabel(50f, lineCount, BPTranslate("Tip: The squeeze sfx pitch hints how close you are to popping free")));
		lineCount -= 20;
		Tabs[1].AddItems(new OpLabel(50f, lineCount, BPTranslate("Spending lots of stamina when you are close to freedom can help you pop through early!")));


		for (int j = 0; j < 2; j++)
		{
			int descLine = 155;
			Tabs[j].AddItems(new OpLabel(25f, descLine + 25f, "--- MOD FEATURES ---"));
			// Tabs[0].AddItems(new OpLabel(25f, descLine, "Press up against stuck creatures to push them. Grab them to pull"));
			// descLine -= 20;
			Tabs[j].AddItems(new OpLabel(25f, descLine, BPTranslate("Press against stuck creatures to push them. Grab them and move backwards to pull")));
			descLine -= 20;
			Tabs[j].AddItems(new OpLabel(25f, descLine, BPTranslate("Press Jump while pushing or pulling to strain harder and spend stamina")));
			descLine -= 20;
			Tabs[j].AddItems(new OpLabel(25f, descLine, BPTranslate("Pivot dash, belly slide, or charge-jump into stuck creatures to ram them")));
			descLine -= 20;
			Tabs[j].AddItems(new OpLabel(25f, descLine, BPTranslate("Spending stamina too quickly can make you exhausted and slow down progress")));
			descLine -= 30;
			//descLine -= 20;
			// Tabs[0].AddItems(new OpLabel(25f, 140f, "Certain fruits can be used to slicken up stuck creatures. Push against them with fruit in hand to apply"));
			Tabs[j].AddItems(new OpLabel(25f, descLine, BPTranslate("Certain fruits can be used to slicken up stuck creatures. (blue fruit, slime mold, mushrooms, etc)")));
			descLine -= 20;
			Tabs[j].AddItems(new OpLabel(25f, descLine, BPTranslate("While stuck, tab the Grab button to smear fruit on yourself")));
			descLine -= 20;
			Tabs[j].AddItems(new OpLabel(25f, descLine, BPTranslate("Push against stuck creatures with fruit in hand and press Jump to smear it on them")));
			descLine -= 25;
			if (ModManager.JollyCoop)
				Tabs[j].AddItems(new OpLabel(25f, descLine, BPTranslate("Hold Grab + Throw while holding food next to a co-op partner to feed them")));
		}




		//SIIIIGH.... FIIIIINE....

		int critTab = 2;
		float xPad = 30f;
		float yPad = 3f;
		//Slugcat

		//Lizard
		//Lantern Mice
		//Scavengers

		//Squidcada
		//Noodle Flies
		//Centipedes

		//DLL
		//Vultures
		//Miros Birds

		//Dropwigs
		//Leviathan


		Tabs[critTab].AddItems(new OpLabel(125f, 575f, BPTranslate("Select which creatures can become fat"), bigText: true));

		lineCount = 515;
		int baseMargin = 65;
		int margin = baseMargin;
		string dsc = "";

		Tabs[critTab].AddItems(new UIelement[]
		{
			new OpRect(new Vector2(0, lineCount - 15), new Vector2(600, 55))
		});

		OpCheckBox pBox1;
		dsc = BPTranslate("Player") + " 1";
		Tabs[critTab].AddItems(new UIelement[]
		{
			pBox1 = new OpCheckBox(BPOptions.fatP1, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(pBox1.pos.x + xPad, pBox1.pos.y + yPad, dsc)
			{description = dsc}  //bumpBehav = chkBox5.bumpBehav, 
		});

		margin += 125;
		OpCheckBox pBox2;
		dsc = BPTranslate("Player") + " 2";
		Tabs[critTab].AddItems(new UIelement[]
		{
			pBox2 = new OpCheckBox(BPOptions.fatP2, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(pBox2.pos.x + xPad, pBox2.pos.y + yPad, dsc)
			{description = dsc}
		});

		margin += 125;
		OpCheckBox pBox3;
		dsc = BPTranslate("Player") + " 3";
		Tabs[critTab].AddItems(new UIelement[]
		{
			pBox3 = new OpCheckBox(BPOptions.fatP3, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(pBox3.pos.x + xPad, pBox3.pos.y + yPad, dsc)
			{description = dsc}
		});

		margin += 125;
		OpCheckBox pBox4;
		dsc = BPTranslate("Player") + " 4";
		Tabs[critTab].AddItems(new UIelement[]
		{
			pBox4 = new OpCheckBox(BPOptions.fatP4, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(pBox4.pos.x + xPad, pBox4.pos.y + yPad, dsc)
			{description = dsc}
		});



		//---------------- crits-------------
		margin = baseMargin;
		lineCount -= 75;
		float linePadding = 45f;

		Tabs[critTab].AddItems(new UIelement[]
		{
			new OpRect(new Vector2(0, lineCount - 285), new Vector2(600, 405))
		});

		OpCheckBox critBox1;
		dsc = BPTranslate("Lizard"); //creaturetype-GreenLizard
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox1 = new OpCheckBox(BPOptions.fatLiz, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox1.pos.x + xPad, critBox1.pos.y + yPad, dsc)
			{description = dsc}  //bumpBehav = chkBox5.bumpBehav, 
		});


		margin += 175;
		OpCheckBox critBox2;
		dsc = BPTranslate("creaturetype-LanternMouse");
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox2 = new OpCheckBox(BPOptions.fatMice, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox2.pos.x + xPad, critBox2.pos.y + yPad, dsc)
			{description = dsc}
		});



		margin += 175;
		OpCheckBox critBox3;
		dsc = BPTranslate("creaturetype-Scavenger");
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox3 = new OpCheckBox(BPOptions.fatScavs, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox3.pos.x + xPad, critBox3.pos.y + yPad, dsc)
			{description = dsc}
		});



		margin = baseMargin;
		lineCount -= linePadding;
		OpCheckBox critBox4;
		dsc = BPTranslate("creaturetype-CicadaA");
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox4 = new OpCheckBox(BPOptions.fatSquids, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox4.pos.x + xPad, critBox4.pos.y + yPad, dsc)
			{description = dsc}
		});


		margin += 175;
		OpCheckBox critBox5;
		dsc = BPTranslate("creaturetype-BigNeedleWorm");
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox5 = new OpCheckBox(BPOptions.fatNoots, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox5.pos.x + xPad, critBox5.pos.y + yPad, dsc)
			{description = dsc}
		});


		margin += 175;
		OpCheckBox critBox6;
		dsc = BPTranslate("creaturetype-Centipede");
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox6 = new OpCheckBox(BPOptions.fatCentis, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox6.pos.x + xPad, critBox6.pos.y + yPad, dsc)
			{description = dsc}
		});

		//DLL
		//Vultures
		//Miros Birds
		margin = baseMargin;
		lineCount -= linePadding;
		OpCheckBox critBox7;
		dsc = BPTranslate("creaturetype-DaddyLongLegs");
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox7 = new OpCheckBox(BPOptions.fatDll, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox7.pos.x + xPad, critBox7.pos.y + yPad, dsc)
			{description = dsc}
		});


		margin += 175;
		OpCheckBox critBox8;
		dsc = BPTranslate("creaturetype-Vulture");
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox8 = new OpCheckBox(BPOptions.fatVults, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox8.pos.x + xPad, critBox8.pos.y + yPad, dsc)
			{description = dsc}
		});


		margin += 175;
		OpCheckBox critBox9;
		dsc = BPTranslate("creaturetype-MirosBird");
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox9 = new OpCheckBox(BPOptions.fatMiros, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox9.pos.x + xPad, critBox9.pos.y + yPad, dsc)
			{description = dsc}
		});

		//Dropwigs
		//Leviathan
		margin = baseMargin;
		lineCount -= linePadding;
		OpCheckBox critBox10;
		dsc = BPTranslate("creaturetype-DropBug");
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox10 = new OpCheckBox(BPOptions.fatWigs, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox10.pos.x + xPad, critBox10.pos.y + yPad, dsc)
			{description = dsc}
		});


		margin += 175;
		OpCheckBox critBox11;
		dsc = BPTranslate("creaturetype-BigEel");
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox11 = new OpCheckBox(BPOptions.fatEels, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox11.pos.x + xPad, critBox11.pos.y + yPad, dsc)
			{description = dsc}
		});


		margin += 175;
		//OpCheckBox critBox11;
		dsc = BPTranslate("creaturetype-JetFish");
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox11 = new OpCheckBox(BPOptions.fatJets, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox11.pos.x + xPad, critBox11.pos.y + yPad, dsc)
			{description = dsc}
		});


		margin = baseMargin;
		lineCount -= linePadding;
		dsc = BPTranslate("creaturetype-Deer");
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox11 = new OpCheckBox(BPOptions.fatDeer, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox11.pos.x + xPad, critBox11.pos.y + yPad, dsc)
			{description = dsc}
		});


		margin += 175;
		//OpCheckBox critBox11;
		dsc = BPTranslate("creaturetype-Leech");
		Tabs[critTab].AddItems(new UIelement[]
		{
			critBox11 = new OpCheckBox(BPOptions.fatLeechs, new Vector2(margin, lineCount))
			{description = dsc},
			new OpLabel(critBox11.pos.x + xPad, critBox11.pos.y + yPad, dsc)
			{description = dsc}
		});


		if (ModManager.MSC)
		{
			margin += 175;
			OpCheckBox critBox13;
			dsc = BPTranslate("creaturetype-Yeek");
			Tabs[critTab].AddItems(new UIelement[]
			{
				critBox13 = new OpCheckBox(BPOptions.fatYeeks, new Vector2(margin, lineCount))
				{description = dsc},
				new OpLabel(critBox13.pos.x + xPad, critBox13.pos.y + yPad, dsc)
				{description = dsc}
			});



			margin = baseMargin;
			lineCount -= linePadding;
			OpCheckBox critBox12;
			dsc = BPTranslate("creaturetype-SlugNPC");
			Tabs[critTab].AddItems(new UIelement[]
			{
				critBox12 = new OpCheckBox(BPOptions.fatPups, new Vector2(margin, lineCount))
				{description = dsc},
				new OpLabel(critBox12.pos.x + xPad, critBox12.pos.y + yPad, dsc)
				{description = dsc}
			});
		}
		else
		{
			BPOptions.fatPups.Value = true;
		}


		if (ModManager.Watcher)
		{
			margin = baseMargin;
			lineCount -= linePadding;

			OpCheckBox critBox13;
			dsc = BPTranslate("creaturetype-BigMoth");
			Tabs[critTab].AddItems(new UIelement[]
			{
				critBox13 = new OpCheckBox(BPOptions.fatMoths, new Vector2(margin, lineCount))
				{description = dsc},
				new OpLabel(critBox13.pos.x + xPad, critBox13.pos.y + yPad, dsc)
				{description = dsc}
			});


			margin += 175;
			OpCheckBox critBox12;
			dsc = BPTranslate("creaturetype-Tardigrade");
			Tabs[critTab].AddItems(new UIelement[]
			{
				critBox12 = new OpCheckBox(BPOptions.fatTards, new Vector2(margin, lineCount))
				{description = dsc},
				new OpLabel(critBox12.pos.x + xPad, critBox12.pos.y + yPad, dsc)
				{description = dsc}
			});

			margin += 175;
			dsc = BPTranslate("creaturetype-Loach");
			Tabs[critTab].AddItems(new UIelement[]
			{
				critBox12 = new OpCheckBox(BPOptions.fatLoachs, new Vector2(margin, lineCount))
				{description = dsc},
				new OpLabel(critBox12.pos.x + xPad, critBox12.pos.y + yPad, dsc)
				{description = dsc}
			});
		}





		//------------------ ANOTHER NEW TAB FOR OTHER STUFF

		int infoTab = 3;
		lineCount = 550;

		Tabs[infoTab].AddItems(new OpLabel(35f, lineCount, BPTranslate("Enable the Improved Input Config mod to change keybinds")));

		lineCount -= 25;
		Tabs[infoTab].AddItems(new OpLabel(35, lineCount, BPTranslate("If you run into any bugs or issues, let me know so I can fix them!")));

		lineCount -= 35;
		Tabs[infoTab].AddItems(new OpLabel(35f, lineCount, BPTranslate("Message me on discord: WillowWisp#3565 ")));
		lineCount -= 25;
		Tabs[infoTab].AddItems(new OpLabel(35f, lineCount, BPTranslate("Or ping @WillowWisp on the rain world Discord Server in the modding-support channel")));
		lineCount -= 25;
		Tabs[infoTab].AddItems(new OpLabel(35f, lineCount, BPTranslate("Or leave a comment on the mod workshop page in the Bug Reporting discussion!")));
		lineCount -= 25;
		Tabs[infoTab].AddItems(new OpLabel(35f, lineCount, BPTranslate("Or anywhere you want please I'm begging and sobbing please report your bugs")));

		lineCount -= 50;
		Tabs[infoTab].AddItems(new OpLabel(35f, lineCount, BPTranslate("Super bonus points if you include your error log files (if they generated)")));
		lineCount -= 25;
		Tabs[infoTab].AddItems(new OpLabel(35f, lineCount, BPTranslate("located in: /Program Files (86)/Steam/steamapps/common/Rain World/exceptionLog.txt")));


		lineCount -= 50;
		Tabs[infoTab].AddItems(new OpLabel(35f, lineCount, BPTranslate("Leave feedback on the mod page and rate the mod if you enjoy it!")));
		lineCount -= 25;
		Tabs[infoTab].AddItems(new OpLabel(35f, lineCount, BPTranslate("If you post footage, send me a link! I'd love to see! :D")));

	}

}*/
#endregion




/*#region Creatures

namespace Menu.Remix.MixedUI
{
    // Token: 0x0200060A RID: 1546
    public class OpComboBox__ : UIconfig, ICanBeTyped
    {
        // Token: 0x06004092 RID: 16530 RVA: 0x004AFC84 File Offset: 0x004ADE84
        public OpComboBox__(Configurable<string> config, Vector2 pos, float width, List<ListItem> list) : this(config, pos, width, list)
        {
        }

        // Token: 0x06004093 RID: 16531 RVA: 0x004AFC91 File Offset: 0x004ADE91
        public OpComboBox__(Configurable<string> config, Vector2 pos, float width, string[] array) : this(config, pos, width, OpComboBox._ArrayToList(array))
        {
        }

        // Token: 0x06004094 RID: 16532 RVA: 0x004AFCA4 File Offset: 0x004ADEA4
        protected static List<ListItem> _ArrayToList(string[] array)
        {
            List<ListItem> list = new List<ListItem>();
            for (int i = 0; i < array.Length; i++)
            {
                list.Add(new ListItem(array[i], i));
            }
            return list;
        }

        // Token: 0x06004095 RID: 16533 RVA: 0x004AFCD5 File Offset: 0x004ADED5
        public OpComboBox__(Configurable<string> config, Vector2 pos, float width) : this(config, pos, width, OpComboBox._InfoToList(config))
        {
        }

        // Token: 0x06004096 RID: 16534 RVA: 0x004AFCE8 File Offset: 0x004ADEE8
        protected static List<ListItem> _InfoToList(Configurable<string> config)
        {
            if (config == null || config.info == null || config.info.acceptable == null || !(config.info.acceptable is ConfigAcceptableList<string>))
            {
                throw new ElementFormatException("To use this constructor, Configurable<string> must have ConfigurableInfo with ConfigAccpetableList.");
            }
            ConfigAcceptableList<string> configAcceptableList = config.info.acceptable as ConfigAcceptableList<string>;
            List<ListItem> list = new List<ListItem>();
            for (int i = 0; i < configAcceptableList.AcceptableValues.Length; i++)
            {
                list.Add(new ListItem(configAcceptableList.AcceptableValues[i], i));
            }
            return list;
        }

        // Token: 0x06004097 RID: 16535 RVA: 0x004AFD6C File Offset: 0x004ADF6C
        internal OpComboBox__(ConfigurableBase configBase, Vector2 pos, float width, List<ListItem> list) : base(configBase, pos, new Vector2(width, 24f))
        {
            this._size = new Vector2(Mathf.Max(30f, base.size.x), 24f);
            this.fixedSize = new Vector2?(new Vector2(-1f, 24f));
            if (this._IsResourceSelector)
            {
                return;
            }
            if (list == null || list.Count < 1)
            {
                throw new ElementFormatException(this, "The list must contain at least one ListItem", base.Key);
            }
            list.Sort(new Comparison<ListItem>(ListItem.Comparer));
            this._itemList = list.ToArray();
            this._ResetIndex();
            this._Initialize(base.defaultValue);
        }

        // Token: 0x06004098 RID: 16536 RVA: 0x004AFE70 File Offset: 0x004AE070
        protected internal override string DisplayDescription()
        {
            if (base.MenuMouseMode)
            {
                if (!this.held)
                {
                    if (!string.IsNullOrEmpty(this.description))
                    {
                        return this.description;
                    }
                    return OptionalText.GetText(OptionalText.ID.OpComboBox_MouseOpenTuto);
                }
                else
                {
                    if (this._listHover >= 0)
                    {
                        string text = "";
                        if (this._searchMode)
                        {
                            if (this._listTop + this._listHover < this._searchList.Count)
                            {
                                text = this._searchList[this._listTop + this._listHover].desc;
                            }
                        }
                        else if (this._listTop + this._listHover < this._itemList.Length)
                        {
                            text = this._itemList[this._listTop + this._listHover].desc;
                        }
                        if (!string.IsNullOrEmpty(text))
                        {
                            return text;
                        }
                    }
                    if (!string.IsNullOrEmpty(this.description))
                    {
                        return this.description;
                    }
                    if (this._searchMode)
                    {
                        return OptionalText.GetText(OptionalText.ID.OpComboBox_MouseSearchTuto);
                    }
                    return OptionalText.GetText(OptionalText.ID.OpComboBox_MouseUseTuto);
                }
            }
            else if (!this.held)
            {
                if (!string.IsNullOrEmpty(this.description))
                {
                    return this.description;
                }
                return OptionalText.GetText(OptionalText.ID.OpComboBox_NonMouseOpenTuto);
            }
            else
            {
                if (this._listHover >= 0)
                {
                    string text2 = "";
                    if (this._searchMode)
                    {
                        if (this._listTop + this._listHover < this._searchList.Count)
                        {
                            text2 = this._searchList[this._listTop + this._listHover].desc;
                        }
                    }
                    else if (this._listTop + this._listHover < this._itemList.Length)
                    {
                        text2 = this._itemList[this._listTop + this._listHover].desc;
                    }
                    if (!string.IsNullOrEmpty(text2))
                    {
                        return text2;
                    }
                }
                if (!string.IsNullOrEmpty(this.description))
                {
                    return this.description;
                }
                return OptionalText.GetText(OptionalText.ID.OpComboBox_NonMouseUseTuto);
            }
        }

        // Token: 0x06004099 RID: 16537 RVA: 0x004B0038 File Offset: 0x004AE238
        protected void _ResetIndex()
        {
            for (int i = 0; i < this._itemList.Length; i++)
            {
                this._itemList[i].index = i;
            }
            this._searchDelay = Mathf.FloorToInt(Custom.LerpMap((float)Mathf.Clamp(this._itemList.Length, 10, 90), 10f, 90f, 10f, 50f));
            this._searchDelay = UIelement.FrameMultiply(this._searchDelay);
            if (ModManager.MMF)
            {
                if (Custom.rainWorld.options.quality == Options.Quality.HIGH)
                {
                    this._searchDelay = 10;
                    return;
                }
                if (Custom.rainWorld.options.quality == Options.Quality.MEDIUM)
                {
                    this._searchDelay = Math.Max(10, this._searchDelay / 2);
                }
            }
        }

        // Token: 0x0600409A RID: 16538 RVA: 0x004B010C File Offset: 0x004AE30C
        protected virtual void _Initialize(string defaultName)
        {
            this.OnKeyDown = (Action<char>)Delegate.Combine(this.OnKeyDown, new Action<char>(this.KeyboardAccept));
            this.Assign();
            this.mouseOverStopsScrollwheel = true;
            if (string.IsNullOrEmpty(defaultName))
            {
                this.allowEmpty = true;
                base.defaultValue = "";
                this._value = "";
            }
            else
            {
                bool flag = false;
                for (int i = 0; i < this._itemList.Length; i++)
                {
                    if (this._itemList[i].name == defaultName)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    base.defaultValue = defaultName;
                    this._value = base.defaultValue;
                }
                else
                {
                    base.defaultValue = this._itemList[0].name;
                    this._value = this._itemList[0].name;
                }
            }
            this._rect = new DyeableRect(this.myContainer, Vector2.zero, base.size, true);
            this._lblText = UIelement.FLabelCreate("", false);
            this._lblText.text = LabelTest.TrimText(string.IsNullOrEmpty(this.value) ? this.DASHES : this._GetDisplayValue(), base.size.x - 32f, true, false);
            this._lblText.alignment = FLabelAlignment.Left;
            this._lblText.x = 12f;
            this._lblText.y = base.size.y / 2f;
            this.myContainer.AddChild(this._lblText);
            if (this._IsListBox)
            {
                this._neverOpened = false;
                this._rectList = new DyeableRect(this.myContainer, Vector2.zero, base.size, true);
                this._rectScroll = new DyeableRect(this.myContainer, Vector2.zero, Vector2.one * 15f, true);
                this._glowFocus = new GlowGradient(this.myContainer, Vector2.zero, new Vector2((base.size.x - 25f) / 2f, 15f), 0.5f)
                {
                    color = this.colorEdge
                };
                this._glowFocus.Hide();
            }
            else
            {
                this._neverOpened = true;
                this._sprArrow = new FSprite("Big_Menu_Arrow", true)
                {
                    scale = 0.5f,
                    rotation = 180f,
                    anchorX = 0.5f,
                    anchorY = 0.5f
                };
                this.myContainer.AddChild(this._sprArrow);
                this._sprArrow.SetPosition(base.size.x - 12f, base.size.y / 2f);
            }
            this._lblList = new FLabel[0];
            this._searchCursor = new FSprite("modInputCursor", true);
            this.myContainer.AddChild(this._searchCursor);
            this._searchCursor.isVisible = false;
            this._bumpList = new BumpBehaviour(this)
            {
                held = false,
                Focused = false
            };
            this._bumpScroll = new BumpBehaviour(this)
            {
                held = false,
                Focused = false
            };
            this.GrafUpdate(0f);
        }

        // Token: 0x0600409B RID: 16539 RVA: 0x004B0444 File Offset: 0x004AE644
        public ListItem[] GetItemList()
        {
            return this._itemList;
        }

        // Token: 0x17000A82 RID: 2690
        // (get) Token: 0x0600409C RID: 16540 RVA: 0x004B044C File Offset: 0x004AE64C
        protected bool _IsResourceSelector
        {
            get
            {
                return this is OpResourceSelector || this is OpResourceList;
            }
        }

        // Token: 0x17000A83 RID: 2691
        // (get) Token: 0x0600409D RID: 16541 RVA: 0x004B0461 File Offset: 0x004AE661
        protected bool _IsListBox
        {
            get
            {
                return this is OpListBox;
            }
        }

        // Token: 0x0600409E RID: 16542 RVA: 0x004B046C File Offset: 0x004AE66C
        public override void Reset()
        {
            if (this.held)
            {
                this._CloseList();
            }
            base.Reset();
        }

        // Token: 0x17000A84 RID: 2692
        // (get) Token: 0x0600409F RID: 16543 RVA: 0x004B0482 File Offset: 0x004AE682
        // (set) Token: 0x060040A0 RID: 16544 RVA: 0x004B048C File Offset: 0x004AE68C
        public override string value
        {
            get
            {
                return base.value;
            }
            set
            {
                if (base.value != value)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        base.value = (this.allowEmpty ? "" : this._itemList[0].name);
                        return;
                    }
                    ListItem[] itemList = this._itemList;
                    for (int i = 0; i < itemList.Length; i++)
                    {
                        if (itemList[i].name == value)
                        {
                            base.value = value;
                            return;
                        }
                    }
                }
            }
        }

        // Token: 0x17000A85 RID: 2693
        // (get) Token: 0x060040A1 RID: 16545 RVA: 0x004B0507 File Offset: 0x004AE707
        // (set) Token: 0x060040A2 RID: 16546 RVA: 0x004B050F File Offset: 0x004AE70F
        public bool allowEmpty { get; private set; }

        // Token: 0x060040A3 RID: 16547 RVA: 0x004B0518 File Offset: 0x004AE718
        public void SetAllowEmpty()
        {
            if (string.IsNullOrEmpty(this.cfgEntry.defaultValue))
            {
                if (!this.allowEmpty && this.value == this._itemList[0].name)
                {
                    this._value = "";
                }
                base.defaultValue = "";
            }
            this.allowEmpty = true;
        }

        // Token: 0x060040A4 RID: 16548 RVA: 0x004B057C File Offset: 0x004AE77C
        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
            base.bumpBehav.greyedOut = this.greyedOut;
            this._rect.colorEdge = base.bumpBehav.GetColor(this.colorEdge);
            this._rect.fillAlpha = base.bumpBehav.FillAlpha;
            this._rect.addSize = new Vector2(4f, 4f) * base.bumpBehav.AddSize;
            this._rect.colorFill = (this.greyedOut ? base.bumpBehav.GetColor(this.colorFill) : this.colorFill);
            this._rect.GrafUpdate(timeStacker);
            Color color = this.held ? MenuColorEffect.MidToDark(this._rect.colorEdge) : this._rect.colorEdge;
            if (!this._IsListBox)
            {
                this._sprArrow.color = color;
                this._lblText.color = (this._searchMode ? this._rect.colorEdge : color);
            }
            this._lblText.color = (this._searchMode ? this._rect.colorEdge : color);
            if (!this._IsListBox && !this.held)
            {
                this._lblText.text = LabelTest.TrimText(string.IsNullOrEmpty(this.value) ? this.DASHES : this._GetDisplayValue(), base.size.x - 32f, true, false);
                return;
            }
            this._rectList.size.x = base.size.x;
            this._rectList.pos = new Vector2(0f, this._downward ? (-this._rectList.size.y) : base.size.y);
            if (this._IsListBox && this._downward)
            {
                DyeableRect rectList = this._rectList;
                rectList.pos.y = rectList.pos.y + this._rectList.size.y;
            }
            this._rectList.addSize = new Vector2(4f, 4f) * this._bumpList.AddSize;
            this._rectList.colorEdge = this._bumpList.GetColor(this.colorEdge);
            this._rectList.colorFill = this.colorFill;
            this._rectList.fillAlpha = (this._IsListBox ? this._bumpList.FillAlpha : Mathf.Lerp(0.5f, 0.7f, this._bumpList.col));
            this._rectList.GrafUpdate(timeStacker);
            for (int i = 0; i < this._lblList.Length; i++)
            {
                string text = this._searchMode ? ((this._searchList.Count > this._listTop + i) ? this._searchList[this._listTop + i].displayName : "") : this._itemList[this._listTop + i].displayName;
                this._lblList[i].text = LabelTest.TrimText(text, base.size.x - 36f, true, false);
                this._lblList[i].color = ((text == this.value) ? MenuColorEffect.MidToDark(this._rect.colorEdge) : this._rectList.colorEdge);
                if (i == this._listHover)
                {
                    this._lblList[i].color = Color.Lerp(this._lblList[i].color, (this._mouseDown || text == this.value) ? MenuColorEffect.MidToDark(this._lblList[i].color) : Color.white, this._bumpList.Sin((text == this._GetDisplayValue()) ? 60f : 10f));
                }
                this._lblList[i].x = 12f;
                this._lblList[i].y = -15f - 20f * (float)i + (this._downward ? 0f : (base.size.y + this._rectList.size.y));
                if (this._IsListBox && this._downward)
                {
                    this._lblList[i].y += this._rectList.size.y;
                }
            }
            if (this._glowFocus != null)
            {
                if (this._listHover >= 0 && (this._MouseOverList() || (!base.MenuMouseMode && this.held)))
                {
                    this._glowFocus.Show();
                    this._glowFocus.pos = this._lblList[Math.Min(this._listHover, this._lblList.Length - 1)].GetPosition() - new Vector2(0f, this._glowFocus.radV);
                    this._glowFocus.color = Color.Lerp(this._rect.colorEdge, Color.white, 0.6f);
                    this._glowFocus.alpha = Mathf.Lerp(0.2f, 0.6f, this._bumpList.Sin(10f));
                }
                else
                {
                    this._glowFocus.Hide();
                }
            }
            this._lblText.text = LabelTest.TrimText(this._searchMode ? this._searchQuery : (string.IsNullOrEmpty(this.value) ? this.DASHES : this._GetDisplayValue()), base.size.x - 32f, true, false);
            if (this._searchMode)
            {
                this._searchCursor.x = Mathf.Min(base.size.x - 24f, 12f + LabelTest.GetWidth(this._searchQuery, false) + LabelTest.CharMean(false));
                this._searchCursor.y = base.size.y * 0.5f + ((this._IsListBox && this._downward) ? this._rectList.size.y : 0f);
                this._searchCursor.color = Color.Lerp(this.colorEdge, MenuColorEffect.MidToDark(this.colorEdge), base.bumpBehav.Sin((this._searchList.Count > 0) ? 10f : 30f));
                this._searchCursor.alpha = 0.4f + 0.6f * Mathf.Clamp01((float)this._searchIdle / (float)this._searchDelay);
            }
            int num = this._searchMode ? this._searchList.Count : this._itemList.Length;
            if (num > this._lblList.Length)
            {
                this._rectScroll.Show();
                this._rectScroll.pos.x = this._rectList.pos.x + this._rectList.size.x - 20f;
                if (!Mathf.Approximately(this._rectScroll.size.y, this._ScrollLen(num)))
                {
                    this._rectScroll.size.y = this._ScrollLen(num);
                    this._rectScroll.pos.y = this._ScrollPos(num);
                }
                else
                {
                    this._rectScroll.pos.y = Custom.LerpAndTick(this._rectScroll.pos.y, this._ScrollPos(num), this._bumpScroll.held ? 0.6f : 0.2f, (this._bumpScroll.held ? 0.6f : 0.2f) / UIelement.frameMulti);
                }
                this._rectScroll.addSize = new Vector2(2f, 2f) * this._bumpScroll.AddSize;
                this._rectScroll.colorEdge = this._bumpScroll.GetColor(this.colorEdge);
                this._rectScroll.colorFill = (this._bumpScroll.held ? this._rectScroll.colorEdge : this.colorFill);
                this._rectScroll.fillAlpha = (this._bumpScroll.held ? 1f : this._bumpScroll.FillAlpha);
                this._rectScroll.GrafUpdate(timeStacker);
                return;
            }
            this._rectScroll.Hide();
        }

        // Token: 0x17000A86 RID: 2694
        // (get) Token: 0x060040A5 RID: 16549 RVA: 0x004B0DFB File Offset: 0x004AEFFB
        protected int _listHeight
        {
            get
            {
                return Custom.IntClamp((int)this.listHeight, 1, this._itemList.Length);
            }
        }

        // Token: 0x060040A6 RID: 16550 RVA: 0x004B0E14 File Offset: 0x004AF014
        protected float _ScrollPos(int listSize)
        {
            return this._rectList.pos.y + 10f + (this._rectList.size.y - 20f - this._rectScroll.size.y) * (float)(listSize - this._lblList.Length - this._listTop) / (float)(listSize - this._lblList.Length);
        }

        // Token: 0x060040A7 RID: 16551 RVA: 0x004B0E7E File Offset: 0x004AF07E
        protected float _ScrollLen(int listSize)
        {
            return (this._rectList.size.y - 40f) * Mathf.Clamp01((float)this._lblList.Length / (float)listSize) + 20f;
        }

        // Token: 0x060040A8 RID: 16552 RVA: 0x004B0EB0 File Offset: 0x004AF0B0
        public int GetIndex(string checkName = "")
        {
            if (string.IsNullOrEmpty(checkName))
            {
                checkName = this.value;
            }
            for (int i = 0; i < this._itemList.Length; i++)
            {
                if (this._itemList[i].name == checkName)
                {
                    return this._itemList[i].index;
                }
            }
            return -1;
        }

        // Token: 0x17000A87 RID: 2695
        // (get) Token: 0x060040A9 RID: 16553 RVA: 0x004B0F0C File Offset: 0x004AF10C
        // (set) Token: 0x060040AA RID: 16554 RVA: 0x004B0F14 File Offset: 0x004AF114
        public Action<char> OnKeyDown { get; set; }

        // Token: 0x060040AB RID: 16555 RVA: 0x004B0F20 File Offset: 0x004AF120
        public void KeyboardAccept(char input)
        {
            if (!this.held || !this._searchMode || this._bumpScroll.held)
            {
                return;
            }
            if (input == '\b')
            {
                if (this._searchQuery.Length > 0)
                {
                    this._searchQuery = this._searchQuery.Substring(0, this._searchQuery.Length - 1);
                    this._searchIdle = -1;
                    base.PlaySound(SoundID.MENY_Already_Selected_MultipleChoice_Clicked);
                    return;
                }
            }
            else if (char.IsLetterOrDigit(input) || input == ' ')
            {
                base.bumpBehav.flash = 2.5f;
                this._searchQuery += input.ToString();
                this._searchIdle = -1;
                base.PlaySound(SoundID.MENU_Checkbox_Uncheck);
            }
        }

        // Token: 0x060040AC RID: 16556 RVA: 0x004B0FD8 File Offset: 0x004AF1D8
        protected void _SearchModeUpdate()
        {
            base.ForceMenuMouseMode(new bool?(true));
            if (this._inputDelay == 0 && this._searchIdle < UIelement.FrameMultiply(this._searchDelay))
            {
                this._searchIdle++;
                if (this._searchIdle == this._searchDelay)
                {
                    this._RefreshSearchList();
                }
            }
        }

        // Token: 0x060040AD RID: 16557 RVA: 0x004B1030 File Offset: 0x004AF230
        public override void Update()
        {
            base.Update();
            if (this._dTimer > 0)
            {
                this._dTimer--;
            }
            this._rect.Update();
            DyeableRect rectList = this._rectList;
            if (rectList != null)
            {
                rectList.Update();
            }
            DyeableRect rectScroll = this._rectScroll;
            if (rectScroll != null)
            {
                rectScroll.Update();
            }
            if (this.greyedOut)
            {
                DyeableRect rectList2 = this._rectList;
                if (rectList2 != null && !rectList2.isHidden)
                {
                    this._mouseDown = false;
                    this.held = false;
                    if (!this._IsListBox)
                    {
                        this._CloseList();
                    }
                }
                return;
            }
            if (base.MenuMouseMode)
            {
                this._MouseModeUpdate();
                return;
            }
            this._NonMouseModeUpdate();
        }

        // Token: 0x060040AE RID: 16558 RVA: 0x004B10D8 File Offset: 0x004AF2D8
        protected virtual void _MouseModeUpdate()
        {
            base.bumpBehav.Focused = (base.MouseOver && !this._MouseOverList());
            if (this.held)
            {
                this._bumpList.Focused = this._MouseOverList();
                this._bumpScroll.Focused = (base.MousePos.x >= this._rectScroll.pos.x && base.MousePos.x <= this._rectScroll.pos.x + this._rectScroll.size.x);
                this._bumpScroll.Focused = (this._bumpScroll.Focused && base.MousePos.y >= this._rectScroll.pos.y && base.MousePos.y <= this._rectScroll.pos.y + this._rectScroll.size.y);
                if (this._searchMode && !this._bumpScroll.held)
                {
                    this._SearchModeUpdate();
                }
                int num = this._searchMode ? this._searchList.Count : this._itemList.Length;
                if (!this._bumpScroll.held)
                {
                    if (this.MouseOver)
                    {
                        if (Input.GetMouseButton(0) && !this._mouseDown)
                        {
                            if (this._bumpScroll.Focused && num > this._lblList.Length)
                            {
                                this._scrollHeldPos = base.MousePos.y - this._rectScroll.pos.y + base.pos.y;
                                this._bumpScroll.held = true;
                                base.PlaySound(SoundID.MENU_First_Scroll_Tick);
                            }
                            else
                            {
                                this._mouseDown = true;
                            }
                        }
                        if (!this._MouseOverList())
                        {
                            if (Input.GetMouseButton(0) || !this._mouseDown)
                            {
                                goto IL_60F;
                            }
                            this._mouseDown = false;
                            if (this._dTimer > 0)
                            {
                                this._dTimer = 0;
                                this._searchMode = true;
                                this._EnterSearchMode();
                                return;
                            }
                            this._dTimer = UIelement.FrameMultiply(15);
                            if (this.allowEmpty)
                            {
                                this.value = "";
                            }
                            base.PlaySound(SoundID.MENU_Checkbox_Uncheck);
                        }
                        else
                        {
                            if (base.MousePos.x >= 10f && base.MousePos.x <= this._rectList.size.x - 30f)
                            {
                                if (this._downward)
                                {
                                    this._listHover = Mathf.FloorToInt((base.MousePos.y + this._rectList.size.y + 10f) / 20f);
                                }
                                else
                                {
                                    this._listHover = Mathf.FloorToInt((base.MousePos.y - base.size.y + 10f) / 20f);
                                }
                                if (this._listHover > this._lblList.Length || this._listHover <= 0)
                                {
                                    this._listHover = -1;
                                }
                                else
                                {
                                    this._listHover = this._lblList.Length - this._listHover;
                                }
                            }
                            else
                            {
                                this._listHover = -1;
                            }
                            if (base.Menu.mouseScrollWheelMovement != 0)
                            {
                                int num2 = this._listTop + (int)Mathf.Sign((float)base.Menu.mouseScrollWheelMovement) * Mathf.CeilToInt((float)this._lblList.Length / 2f);
                                num2 = Custom.IntClamp(num2, 0, num - this._lblList.Length);
                                if (this._listTop != num2)
                                {
                                    base.PlaySound(SoundID.MENU_Scroll_Tick);
                                    this._listTop = num2;
                                    this._bumpScroll.flash = Mathf.Min(1f, this._bumpScroll.flash + 0.2f);
                                    this._bumpScroll.sizeBump = Mathf.Min(2.5f, this._bumpScroll.sizeBump + 0.3f);
                                    goto IL_60F;
                                }
                                goto IL_60F;
                            }
                            else
                            {
                                if (Input.GetMouseButton(0) || !this._mouseDown)
                                {
                                    goto IL_60F;
                                }
                                this._mouseDown = false;
                                if (this._listHover < 0)
                                {
                                    goto IL_60F;
                                }
                                string text = this.value;
                                if (this._searchMode)
                                {
                                    if (this._listTop + this._listHover < this._searchList.Count)
                                    {
                                        text = this._searchList[this._listTop + this._listHover].name;
                                    }
                                }
                                else if (this._listTop + this._listHover < this._itemList.Length)
                                {
                                    text = this._itemList[this._listTop + this._listHover].name;
                                }
                                if (!(text != this.value))
                                {
                                    base.PlaySound(SoundID.MENY_Already_Selected_MultipleChoice_Clicked);
                                    goto IL_60F;
                                }
                                this.value = text;
                                base.PlaySound(SoundID.MENU_MultipleChoice_Clicked);
                            }
                        }
                    }
                    else
                    {
                        if ((!Input.GetMouseButton(0) || this._mouseDown) && (!this._mouseDown || Input.GetMouseButton(0)))
                        {
                            goto IL_60F;
                        }
                        base.PlaySound(SoundID.MENU_Checkbox_Uncheck);
                    }
                    this._mouseDown = false;
                    this.held = false;
                    this._CloseList();
                    return;
                }
                if (Input.GetMouseButton(0))
                {
                    int num3 = Mathf.RoundToInt((base.MousePos.y - this._rectList.pos.y + base.pos.y - 10f - this._scrollHeldPos) * (float)(num - this._lblList.Length) / (this._rectList.size.y - 20f - this._rectScroll.size.y));
                    num3 = Custom.IntClamp(num - this._lblList.Length - num3, 0, num - this._lblList.Length);
                    if (this._listTop != num3)
                    {
                        base.PlaySound(SoundID.MENU_Scroll_Tick);
                        this._listTop = num3;
                        this._bumpScroll.flash = Mathf.Min(1f, this._bumpScroll.flash + 0.2f);
                        this._bumpScroll.sizeBump = Mathf.Min(2.5f, this._bumpScroll.sizeBump + 0.3f);
                    }
                }
                else
                {
                    this._bumpScroll.held = false;
                    this._mouseDown = false;
                    base.PlaySound(SoundID.MENU_Scroll_Tick);
                }
            IL_60F:
                this._bumpList.Update();
                this._bumpScroll.Update();
                return;
            }
            if (this.greyedOut)
            {
                return;
            }
            if (base.MouseOver)
            {
                if (Input.GetMouseButton(0))
                {
                    this._mouseDown = true;
                    return;
                }
                if (this._mouseDown)
                {
                    this._mouseDown = false;
                    if (this._dTimer > 0)
                    {
                        this._dTimer = 0;
                        this._searchMode = true;
                        this._EnterSearchMode();
                    }
                    else
                    {
                        this._dTimer = UIelement.FrameMultiply(15);
                    }
                    this.held = true;
                    this.fixedSize = new Vector2?(base.size);
                    this._OpenList();
                    base.PlaySound(SoundID.MENU_Checkbox_Check);
                    return;
                }
                if (base.Menu.mouseScrollWheelMovement != 0)
                {
                    int index = this.GetIndex("");
                    int num4 = index + (int)Mathf.Sign((float)base.Menu.mouseScrollWheelMovement);
                    num4 = Custom.IntClamp(num4, 0, this._itemList.Length - 1);
                    if (num4 != index)
                    {
                        base.bumpBehav.flash = 1f;
                        base.PlaySound(SoundID.MENU_Scroll_Tick);
                        base.bumpBehav.sizeBump = Mathf.Min(2.5f, base.bumpBehav.sizeBump + 1f);
                        this.value = this._itemList[num4].name;
                        return;
                    }
                }
            }
            else if (!Input.GetMouseButton(0))
            {
                this._mouseDown = false;
            }
        }

        // Token: 0x060040AF RID: 16559 RVA: 0x004B1860 File Offset: 0x004AFA60
        protected virtual void _NonMouseModeUpdate()
        {
            base.bumpBehav.Focused = (base.Focused && !this.held);
            if (this.held)
            {
                this._bumpList.Focused = true;
                if (!base.bumpBehav.ButtonPress(BumpBehaviour.ButtonType.Throw))
                {
                    OpComboBox.<> c__DisplayClass66_0 CS$<> 8__locals1;
                    CS$<> 8__locals1.listSize = (this._searchMode ? this._searchList.Count : this._itemList.Length);
                    if (base.CtlrInput.y != 0)
                    {
                        OpComboBox.<> c__DisplayClass66_1 CS$<> 8__locals2;
                        CS$<> 8__locals2.dir = base.bumpBehav.JoystickPressAxis(true);
                        if (CS$<> 8__locals2.dir != 0)
						{
                            this.< _NonMouseModeUpdate > g___ScrollTick | 66_0(true, ref CS$<> 8__locals1, ref CS$<> 8__locals2);
                        }

                        else
                        {
                            CS$<> 8__locals2.dir = base.bumpBehav.JoystickHeldAxis(true, 2f);
                            if (CS$<> 8__locals2.dir != 0)
							{
                                this.< _NonMouseModeUpdate > g___ScrollTick | 66_0(false, ref CS$<> 8__locals1, ref CS$<> 8__locals2);
                            }
                        }
                    }
                    if (base.bumpBehav.ButtonPress(BumpBehaviour.ButtonType.Jump))
                    {
                        string text = this.value;
                        if (this._searchMode)
                        {
                            if (this._listTop + this._listHover < this._searchList.Count)
                            {
                                text = this._searchList[this._listTop + this._listHover].name;
                            }
                        }
                        else if (this._listTop + this._listHover < this._itemList.Length)
                        {
                            text = this._itemList[this._listTop + this._listHover].name;
                        }
                        if (text != this.value)
                        {
                            this.value = text;
                            base.PlaySound(SoundID.MENU_MultipleChoice_Clicked);
                            goto IL_19E;
                        }
                        base.PlaySound(SoundID.MENY_Already_Selected_MultipleChoice_Clicked);
                    }
                    this._bumpList.Update();
                    this._bumpScroll.Update();
                    return;
                }
            IL_19E:
                this.held = false;
                if (!this._IsListBox)
                {
                    this._CloseList();
                }
                return;
            }
        }

        // Token: 0x060040B0 RID: 16560 RVA: 0x004B1A24 File Offset: 0x004AFC24
        protected internal override void NonMouseSetHeld(bool newHeld)
        {
            base.NonMouseSetHeld(newHeld);
            if (newHeld)
            {
                if (!this._IsListBox)
                {
                    this._OpenList();
                    base.PlaySound(SoundID.MENU_Checkbox_Check);
                }
                this._listHover = Custom.IntClamp(this.GetIndex("") - this._listTop, 0, this._lblList.Length - 1);
                return;
            }
            if (!this._IsListBox)
            {
                this._CloseList();
            }
        }

        // Token: 0x060040B1 RID: 16561 RVA: 0x004B1A8C File Offset: 0x004AFC8C
        protected internal override void Change()
        {
            base.Change();
            this._size.x = Mathf.Max(30f, this._size.x);
            this._rect.size = base.size;
            this._lblText.x = 12f;
            this._lblText.y = base.size.y / 2f;
            if (this._glowFocus != null)
            {
                this._glowFocus.radH = (this._size.x - 25f) / 2f;
            }
            if (this._IsListBox && this._downward)
            {
                this._rect.pos.y = this._rectList.size.y;
                this._lblText.y += this._rectList.size.y;
            }
            else
            {
                this._rect.pos.y = 0f;
            }
            if (!this._IsListBox)
            {
                this._sprArrow.SetPosition(base.size.x - 12f, base.size.y / 2f);
            }
        }

        // Token: 0x060040B2 RID: 16562 RVA: 0x004B1BC4 File Offset: 0x004AFDC4
        protected internal override void Deactivate()
        {
            base.Deactivate();
            if (this._IsListBox)
            {
                this._searchMode = false;
                this._bumpScroll.held = false;
                return;
            }
            this._CloseList();
        }

        // Token: 0x14000016 RID: 22
        // (add) Token: 0x060040B3 RID: 16563 RVA: 0x004B1BF0 File Offset: 0x004AFDF0
        // (remove) Token: 0x060040B4 RID: 16564 RVA: 0x004B1C28 File Offset: 0x004AFE28
        public event OnSignalHandler OnListOpen;

        // Token: 0x14000017 RID: 23
        // (add) Token: 0x060040B5 RID: 16565 RVA: 0x004B1C60 File Offset: 0x004AFE60
        // (remove) Token: 0x060040B6 RID: 16566 RVA: 0x004B1C98 File Offset: 0x004AFE98
        public event OnSignalHandler OnListClose;

        // Token: 0x060040B7 RID: 16567 RVA: 0x004B1CD0 File Offset: 0x004AFED0
        protected void _OpenList()
        {
            float num = 20f * (float)Mathf.Clamp(this._itemList.Length, 1, this._listHeight) + 10f;
            if (!this._IsListBox)
            {
                OnSignalHandler onListOpen = this.OnListOpen;
                if (onListOpen != null)
                {
                    onListOpen(this);
                }
                if (num < base.GetPos().y)
                {
                    this._downward = true;
                }
                else if (100f < base.GetPos().y)
                {
                    this._downward = true;
                    num = 100f;
                }
                else
                {
                    float num2 = 600f;
                    if (base.InScrollBox)
                    {
                        num2 = (base.scrollBox.horizontal ? base.scrollBox.size.y : base.scrollBox.contentSize);
                    }
                    num2 -= base.GetPos().y + base.size.y;
                    if (num2 < base.GetPos().y)
                    {
                        this._downward = true;
                        num = Mathf.Floor(base.GetPos().y / 20f) * 20f - 10f;
                    }
                    else
                    {
                        this._downward = false;
                        num = Mathf.Min(num, Mathf.Clamp(Mathf.Floor(num2 / 20f), 1f, (float)this._listHeight) * 20f + 10f);
                    }
                }
                if (this._neverOpened)
                {
                    this._rectList = new DyeableRect(this.myContainer, Vector2.zero, base.size, true)
                    {
                        fillAlpha = 0.95f
                    };
                    this._rectScroll = new DyeableRect(this.myContainer, Vector2.zero, 15f * Vector2.one, true);
                    this._glowFocus = new GlowGradient(this.myContainer, Vector2.zero, new Vector2((base.size.x - 25f) / 2f, 15f), 0.5f)
                    {
                        color = this.colorEdge
                    };
                    this._neverOpened = false;
                }
                this._sprArrow.rotation = (this._downward ? 180f : 0f);
            }
            this._rectList.Show();
            this._rectList.size = new Vector2(base.size.x, num);
            this._rectList.pos = new Vector2(0f, this._downward ? (-this._rectList.size.y) : base.size.y);
            if (this._IsListBox && this._downward)
            {
                DyeableRect rectList = this._rectList;
                rectList.pos.y = rectList.pos.y + this._rectList.size.y;
                this._rect.pos.y = this._rectList.size.y;
                this._lblText.y += this._rectList.size.y;
            }
            this._glowFocus.Show();
            this._glowFocus.radH = (base.size.x - 25f) / 2f;
            this._lblList = new FLabel[Mathf.FloorToInt(num / 20f)];
            if (this._downward)
            {
                this._listTop = this.GetIndex("") + 1;
                if (this._listTop > this._itemList.Length - this._lblList.Length)
                {
                    this._listTop = this._itemList.Length - this._lblList.Length;
                }
            }
            else
            {
                this._listTop = this.GetIndex("") - this._lblList.Length;
            }
            if (this._listTop < 0)
            {
                this._listTop = 0;
            }
            for (int i = 0; i < this._lblList.Length; i++)
            {
                this._lblList[i] = UIelement.FLabelCreate("", false);
                this._lblList[i].text = this._itemList[this._listTop + i].EffectiveDisplayName;
                this._lblList[i].alignment = FLabelAlignment.Left;
                this.myContainer.AddChild(this._lblList[i]);
            }
            if (this._lblList.Length < this._itemList.Length)
            {
                this._rectScroll.Show();
                this._rectScroll.size = new Vector2(15f, this._ScrollLen(this._itemList.Length));
                this._rectScroll.pos = new Vector2(this._rectList.pos.x + this._rectList.size.x - 20f, this._ScrollPos(this._itemList.Length));
            }
            else
            {
                this._rectScroll.Hide();
            }
            base.bumpBehav.flash = 1f;
            this._bumpList.flash = 1f;
            this._bumpList.held = false;
            this._bumpScroll.held = false;
            if (base.InScrollBox && !this._IsListBox)
            {
                base.scrollBox.ScrollToRect(new Rect(base.PosX, base.PosY - (this._downward ? this._rectList.size.y : 0f), base.size.x, base.size.y + this._rectList.size.y), false);
            }
        }

        // Token: 0x060040B8 RID: 16568 RVA: 0x004B2224 File Offset: 0x004B0424
        private void _CloseList()
        {
            OnSignalHandler onListClose = this.OnListClose;
            if (onListClose != null)
            {
                onListClose(this);
            }
            this._searchMode = false;
            this._searchCursor.isVisible = false;
            this.fixedSize = null;
            if (!this._neverOpened)
            {
                this._rectList.Hide();
                this._rectScroll.Hide();
                this._glowFocus.Hide();
            }
            for (int i = 0; i < this._lblList.Length; i++)
            {
                this._lblList[i].isVisible = false;
                this._lblList[i].RemoveFromContainer();
            }
            this._lblList = new FLabel[0];
            this._bumpScroll.held = false;
        }

        // Token: 0x060040B9 RID: 16569 RVA: 0x004B22D4 File Offset: 0x004B04D4
        protected bool _MouseOverList()
        {
            if (!base.MenuMouseMode)
            {
                return false;
            }
            if (!this.held && !this._IsListBox)
            {
                return false;
            }
            if (base.MousePos.x < 0f || base.MousePos.x > base.size.x)
            {
                return false;
            }
            if (!this._downward)
            {
                return base.MousePos.y >= base.size.y && base.MousePos.y <= base.size.y + this._rectList.size.y;
            }
            if (this._IsListBox)
            {
                return base.MousePos.y >= 0f && base.MousePos.y <= this._rectList.size.y;
            }
            return base.MousePos.y >= -this._rectList.size.y && base.MousePos.y <= 0f;
        }

        // Token: 0x17000A88 RID: 2696
        // (get) Token: 0x060040BA RID: 16570 RVA: 0x004B23EA File Offset: 0x004B05EA
        protected internal override bool MouseOver
        {
            get
            {
                return base.MouseOver || this._MouseOverList();
            }
        }

        // Token: 0x060040BB RID: 16571 RVA: 0x004B23FC File Offset: 0x004B05FC
        public void AddItems(bool sort = true, params ListItem[] newItems)
        {
            if (this._IsResourceSelector)
            {
                throw new InvalidActionException(this, "You cannot use AddItems for OpResourceSelector", base.Key);
            }
            List<ListItem> list = new List<ListItem>(this._itemList);
            list.AddRange(newItems);
            if (sort)
            {
                list.Sort(new Comparison<ListItem>(ListItem.Comparer));
            }
            this._itemList = list.ToArray();
            this._ResetIndex();
            this.Change();
        }

        // Token: 0x060040BC RID: 16572 RVA: 0x004B2464 File Offset: 0x004B0664
        public void RemoveItems(bool selectNext = true, params string[] names)
        {
            if (this._IsResourceSelector)
            {
                throw new InvalidActionException(this, "You cannot use RemoveItems for OpResourceSelector", base.Key);
            }
            List<ListItem> list = new List<ListItem>(this._itemList);
            foreach (string text in names)
            {
                int j = 0;
                while (j < list.Count)
                {
                    if (list[j].name == text)
                    {
                        if (list.Count == 1)
                        {
                            throw new InvalidActionException(this, "You cannot remove every items in OpComboBox", base.Key);
                        }
                        if (text == this.value)
                        {
                            this._value = (selectNext ? list[(j == 0) ? 1 : (j - 1)].name : "");
                        }
                        list.RemoveAt(j);
                        break;
                    }
                    else
                    {
                        j++;
                    }
                }
            }
            this._itemList = list.ToArray();
            this._ResetIndex();
            this.Change();
        }

        // Token: 0x060040BD RID: 16573 RVA: 0x004B254C File Offset: 0x004B074C
        protected internal override string CopyToClipboard()
        {
            if (!this._IsListBox)
            {
                this._CloseList();
            }
            return base.CopyToClipboard();
        }

        // Token: 0x060040BE RID: 16574 RVA: 0x004B2562 File Offset: 0x004B0762
        protected internal override bool CopyFromClipboard(string value)
        {
            if (!this._searchMode)
            {
                this._searchMode = true;
                this._EnterSearchMode();
            }
            this._searchQuery = value;
            this._RefreshSearchList();
            return true;
        }

        // Token: 0x060040BF RID: 16575 RVA: 0x004B2588 File Offset: 0x004B0788
        protected void _EnterSearchMode()
        {
            this._searchQuery = "";
            this._searchList = new List<ListItem>(this._itemList);
            this._searchList.Sort(new Comparison<ListItem>(ListItem.Comparer));
            this._searchCursor.isVisible = true;
            this._searchCursor.SetPosition(LabelTest.CharMean(false) * 1.5f, base.size.y * 0.5f + ((this._IsListBox && this._downward) ? this._rectList.size.y : 0f));
            this._searchIdle = 1000;
        }

        // Token: 0x060040C0 RID: 16576 RVA: 0x004B2630 File Offset: 0x004B0830
        protected void _RefreshSearchList()
        {
            int num = (this._searchList.Count > 0) ? this.GetIndex(this._searchList[this._listTop].name) : 0;
            this._searchList.Clear();
            for (int i = 0; i < this._itemList.Length; i++)
            {
                if (ListItem.SearchMatch(this._searchQuery, this._itemList[i].displayName) || ListItem.SearchMatch(this._searchQuery, this._itemList[i].name))
                {
                    this._searchList.Add(this._itemList[i]);
                }
            }
            this._searchList.Sort(new Comparison<ListItem>(ListItem.Comparer));
            for (int j = 1; j < this._searchList.Count; j++)
            {
                if (num > this.GetIndex(this._searchList[j].name))
                {
                    this._listTop = Math.Max(0, j - 1);
                    return;
                }
            }
            this._listTop = 0;
        }

        // Token: 0x060040C1 RID: 16577 RVA: 0x004B273C File Offset: 0x004B093C
        protected string _GetDisplayValue()
        {
            foreach (ListItem listItem in this._itemList)
            {
                if (listItem.name == this.value)
                {
                    return listItem.EffectiveDisplayName;
                }
            }
            return this.value;
        }

        // Token: 0x060040C2 RID: 16578 RVA: 0x004B2788 File Offset: 0x004B0988
        [CompilerGenerated]
        private void <_NonMouseModeUpdate>g___ScrollTick|66_0(bool first, ref OpComboBox.<>c__DisplayClass66_0 A_2, ref OpComboBox.<>c__DisplayClass66_1 A_3)
		{

            bool flag = true;
			this._listHover -= Math.Sign(A_3.dir);
			if (this._listHover< 0)

            {
            if (this._listTop > 0)
            {
                this._listTop--;
                this._listHover = 0;
                this._bumpScroll.flash = Mathf.Min(1f, this._bumpScroll.flash + 0.2f);
                this._bumpScroll.sizeBump = Mathf.Min(2.5f, this._bumpScroll.sizeBump + 0.3f);
            }
            else
            {
                flag = false;
                this._listHover = 0;
            }
        }
			if (this._listHover >= this._lblList.Length)

            {
            if (this._listTop < A_2.listSize - this._lblList.Length)
            {
                this._listTop++;
                this._listHover--;
                this._bumpScroll.flash = Mathf.Min(1f, this._bumpScroll.flash + 0.2f);
                this._bumpScroll.sizeBump = Mathf.Min(2.5f, this._bumpScroll.sizeBump + 0.3f);
            }
            else
            {
                flag = false;
                this._listHover--;
            }
        }
			if (flag)
			{
				base.PlaySound(first? SoundID.MENU_First_Scroll_Tick : SoundID.MENU_Scroll_Tick);
    }
}

// Token: 0x04003BC0 RID: 15296
private const float LBLTEXTTRIM = 32f;

// Token: 0x04003BC1 RID: 15297
private readonly string DASHES = "------";

// Token: 0x04003BC2 RID: 15298
protected ListItem[] _itemList;

// Token: 0x04003BC3 RID: 15299
protected List<ListItem> _searchList;

// Token: 0x04003BC4 RID: 15300
protected bool _neverOpened;

// Token: 0x04003BC5 RID: 15301
protected DyeableRect _rect;

// Token: 0x04003BC6 RID: 15302
protected DyeableRect _rectList;

// Token: 0x04003BC7 RID: 15303
protected DyeableRect _rectScroll;

// Token: 0x04003BC8 RID: 15304
protected FLabel _lblText;

// Token: 0x04003BC9 RID: 15305
protected FLabel[] _lblList;

// Token: 0x04003BCA RID: 15306
protected FSprite _sprArrow;

// Token: 0x04003BCB RID: 15307
public Color colorEdge = MenuColorEffect.rgbMediumGrey;

// Token: 0x04003BCC RID: 15308
public Color colorFill = MenuColorEffect.rgbBlack;

// Token: 0x04003BCE RID: 15310
public ushort listHeight = 5;

// Token: 0x04003BCF RID: 15311
protected bool _mouseDown;

// Token: 0x04003BD0 RID: 15312
protected bool _searchMode;

// Token: 0x04003BD1 RID: 15313
protected bool _downward = true;

// Token: 0x04003BD2 RID: 15314
protected int _dTimer;

// Token: 0x04003BD3 RID: 15315
protected int _searchDelay;

// Token: 0x04003BD4 RID: 15316
protected int _searchIdle;

// Token: 0x04003BD5 RID: 15317
protected int _listTop;

// Token: 0x04003BD6 RID: 15318
protected int _listHover = -1;

// Token: 0x04003BD7 RID: 15319
protected float _scrollHeldPos;

// Token: 0x04003BD8 RID: 15320
protected BumpBehaviour _bumpList;

// Token: 0x04003BD9 RID: 15321
protected BumpBehaviour _bumpScroll;

// Token: 0x04003BDA RID: 15322
protected string _searchQuery = "";

// Token: 0x04003BDB RID: 15323
protected FSprite _searchCursor;

// Token: 0x04003BDC RID: 15324
protected int _inputDelay;

// Token: 0x04003BDD RID: 15325
private char _lastChar = '\r';

// Token: 0x04003BDF RID: 15327
protected GlowGradient _glowFocus;

// Token: 0x02000C81 RID: 3201
public class Queue : UIconfig.ConfigQueue
{
    // Token: 0x06005E79 RID: 24185 RVA: 0x00610186 File Offset: 0x0060E386
    public Queue(Configurable<string> config, object sign = null) : base(config, sign)
    {
        if (config == null || config.info == null || config.info.acceptable == null || !(config.info.acceptable is ConfigAcceptableList<string>))
        {
            throw new ArgumentNullException("To use this constructor, Configurable<string> must have ConfigurableInfo with ConfigAccpetableList.");
        }
    }

    // Token: 0x17000E7A RID: 3706
    // (get) Token: 0x06005E7A RID: 24186 RVA: 0x006101C5 File Offset: 0x0060E3C5
    protected override float sizeY
    {
        get
        {
            return 30f;
        }
    }

    // Token: 0x06005E7B RID: 24187 RVA: 0x006101CC File Offset: 0x0060E3CC
    protected override List<UIelement> _InitializeThisQueue(IHoldUIelements holder, float posX, ref float offsetY)
    {
        offsetY += this.sizeY;
        float posY = base.GetPosY(holder, offsetY);
        float width = base.GetWidth(holder, posX, 150f, 50f);
        List<UIelement> list = new List<UIelement>();
        OpComboBox opComboBox = new OpComboBox(this.config as Configurable<string>, new Vector2(posX, posY), width)
        {
            sign = this.sign
        };
        if (this.onChange != null)
        {
            opComboBox.OnChange += this.onChange;
        }
        if (this.onHeld != null)
        {
            opComboBox.OnHeld += this.onHeld;
        }
        if (this.onValueUpdate != null)
        {
            opComboBox.OnValueUpdate += this.onValueUpdate;
        }
        if (this.onValueChanged != null)
        {
            opComboBox.OnValueChanged += this.onValueChanged;
        }
        if (this.onListOpen != null)
        {
            opComboBox.OnListOpen += this.onListOpen;
        }
        if (this.onListClose != null)
        {
            opComboBox.OnListClose += this.onListClose;
        }
        this.mainFocusable = opComboBox;
        list.Add(opComboBox);
        ConfigurableInfo info = this.config.info;
        if (!string.IsNullOrEmpty((info != null) ? info.description : null))
        {
            opComboBox.description = UIQueue.GetFirstSentence(UIQueue.Translate(this.config.info.description));
        }
        if (!string.IsNullOrEmpty(this.config.key))
        {
            OpLabel item = new OpLabel(new Vector2(posX + width + 10f, posY), new Vector2(holder.CanvasSize.x - posX - width - 10f, 30f), UIQueue.Translate(this.config.key), FLabelAlignment.Left, false, null)
            {
                autoWrap = true,
                bumpBehav = opComboBox.bumpBehav,
                description = opComboBox.description
            };
            list.Add(item);
        }
        opComboBox.ShowConfig();
        this.hasInitialized = true;
        return list;
    }

    // Token: 0x04006446 RID: 25670
    public OnSignalHandler onListOpen;

    // Token: 0x04006447 RID: 25671
    public OnSignalHandler onListClose;
}
	}
}
using System;
using UnityEngine;

namespace Menu.Remix.MixedUI
{
    // Token: 0x02000633 RID: 1587
    public abstract class UIconfig : UIfocusable
    {
        // Token: 0x060042AB RID: 17067 RVA: 0x004C34E4 File Offset: 0x004C16E4
        public UIconfig(ConfigurableBase config, Vector2 pos, Vector2 size) : base(pos, size)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config cannot be null. If you want a cosmetic UIconfig, generate cosmetic Configurable with OptionInterface.config.Bind.");
            }
            if (config.BoundUIconfig != null)
            {
                throw new MultiuseConfigurableException(config);
            }
            config.BoundUIconfig = this;
            this.cfgEntry = config;
            this.cosmetic = this.cfgEntry.IsCosmetic;
            this.defaultValue = this.cfgEntry.defaultValue;
            this._value = this.defaultValue;
            this.lastValue = this._value;
        }

        // Token: 0x060042AC RID: 17068 RVA: 0x004C3560 File Offset: 0x004C1760
        public UIconfig(ConfigurableBase config, Vector2 pos, float rad) : base(pos, rad)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config cannot be null. If you want a cosmetic UIconfig, generate cosmetic Configurable with OptionInterface.config.Bind.");
            }
            if (config.BoundUIconfig != null)
            {
                throw new MultiuseConfigurableException(config);
            }
            config.BoundUIconfig = this;
            this.cfgEntry = config;
            this.cosmetic = this.cfgEntry.IsCosmetic;
            this.defaultValue = this.cfgEntry.defaultValue;
            this._value = this.defaultValue;
            this.lastValue = this._value;
        }

        // Token: 0x17000AE1 RID: 2785
        // (get) Token: 0x060042AD RID: 17069 RVA: 0x004C35DA File Offset: 0x004C17DA
        // (set) Token: 0x060042AE RID: 17070 RVA: 0x004C35E2 File Offset: 0x004C17E2
        public string defaultValue { get; protected set; }

        // Token: 0x060042AF RID: 17071 RVA: 0x004C35EB File Offset: 0x004C17EB
        public override void Reset()
        {
            base.Reset();
            this.value = this.defaultValue;
            this.held = false;
        }

        // Token: 0x17000AE2 RID: 2786
        // (get) Token: 0x060042B0 RID: 17072 RVA: 0x004C3606 File Offset: 0x004C1806
        public string Key
        {
            get
            {
                return this.cfgEntry.key;
            }
        }

        // Token: 0x060042B1 RID: 17073 RVA: 0x004C3613 File Offset: 0x004C1813
        public void ForceValue(string newValue)
        {
            this._value = newValue;
        }

        // Token: 0x17000AE3 RID: 2787
        // (get) Token: 0x060042B2 RID: 17074 RVA: 0x004C361C File Offset: 0x004C181C
        // (set) Token: 0x060042B3 RID: 17075 RVA: 0x004C3624 File Offset: 0x004C1824
        public virtual string value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (this._value != value)
                {
                    base.FocusMoveDisallow(null);
                    string value2 = this._value;
                    this._value = value;
                    if (!UIelement.ContextWrapped && ConfigContainer.instance != null)
                    {
                        ConfigContainer.instance.NotifyConfigChange(this, value2, value);
                    }
                    OnValueChangeHandler onValueUpdate = this.OnValueUpdate;
                    if (onValueUpdate != null)
                    {
                        onValueUpdate(this, value, value2);
                    }
                    this.Change();
                    if (!this.held)
                    {
                        OnValueChangeHandler onValueChanged = this.OnValueChanged;
                        if (onValueChanged != null)
                        {
                            onValueChanged(this, this._value, this.lastValue);
                        }
                        this.lastValue = this._value;
                    }
                }
            }
        }

        // Token: 0x14000028 RID: 40
        // (add) Token: 0x060042B4 RID: 17076 RVA: 0x004C36BC File Offset: 0x004C18BC
        // (remove) Token: 0x060042B5 RID: 17077 RVA: 0x004C36F4 File Offset: 0x004C18F4
        public event OnValueChangeHandler OnValueUpdate;

        // Token: 0x14000029 RID: 41
        // (add) Token: 0x060042B6 RID: 17078 RVA: 0x004C372C File Offset: 0x004C192C
        // (remove) Token: 0x060042B7 RID: 17079 RVA: 0x004C3764 File Offset: 0x004C1964
        public event OnValueChangeHandler OnValueChanged;

        // Token: 0x060042B8 RID: 17080 RVA: 0x004C379C File Offset: 0x004C199C
        public void ShowConfig()
        {
            this.value = (this.cfgEntry.OI.config.pendingReset ? this.cfgEntry.defaultValue : ValueConverter.ConvertToString(this.cfgEntry.BoxedValue, this.cfgEntry.settingType));
        }

        // Token: 0x060042B9 RID: 17081 RVA: 0x004C37F0 File Offset: 0x004C19F0
        protected internal virtual bool CopyFromClipboard(string value)
        {
            bool result;
            try
            {
                this.value = value;
                this.held = false;
                result = (this.value == value);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        // Token: 0x060042BA RID: 17082 RVA: 0x004C3830 File Offset: 0x004C1A30
        protected internal virtual string CopyToClipboard()
        {
            this.held = false;
            return this.value;
        }

        // Token: 0x060042BB RID: 17083 RVA: 0x004C383F File Offset: 0x004C1A3F
        protected internal override void Deactivate()
        {
            this.held = false;
            base.Deactivate();
        }

        // Token: 0x060042BC RID: 17084 RVA: 0x004C384E File Offset: 0x004C1A4E
        protected internal override void Unload()
        {
            base.Unload();
            this.cfgEntry.BoundUIconfig = null;
        }

        // Token: 0x17000AE4 RID: 2788
        // (get) Token: 0x060042BD RID: 17085 RVA: 0x004C3862 File Offset: 0x004C1A62
        // (set) Token: 0x060042BE RID: 17086 RVA: 0x004C386C File Offset: 0x004C1A6C
        protected internal override bool held
        {
            get
            {
                return base.held;
            }
            set
            {
                base.held = value;
                if (!value && base.Focused && this.lastValue != this._value)
                {
                    OnValueChangeHandler onValueChanged = this.OnValueChanged;
                    if (onValueChanged != null)
                    {
                        onValueChanged(this, this._value, this.lastValue);
                    }
                }
                this.lastValue = this._value;
            }
        }

        // Token: 0x060042BF RID: 17087 RVA: 0x004C38C8 File Offset: 0x004C1AC8
        internal void _UndoCallChanges()
        {
            OnValueChangeHandler onValueUpdate = this.OnValueUpdate;
            if (onValueUpdate != null)
            {
                onValueUpdate(this, this._value, this.lastValue);
            }
            OnValueChangeHandler onValueChanged = this.OnValueChanged;
            if (onValueChanged != null)
            {
                onValueChanged(this, this._value, this.lastValue);
            }
            this.lastValue = this._value;
        }

        // Token: 0x04003D08 RID: 15624
        internal const string errorNull = "config cannot be null. If you want a cosmetic UIconfig, generate cosmetic Configurable with OptionInterface.config.Bind.";

        // Token: 0x04003D09 RID: 15625
        public readonly ConfigurableBase cfgEntry;

        // Token: 0x04003D0B RID: 15627
        public readonly bool cosmetic;

        // Token: 0x04003D0E RID: 15630
        protected string lastValue;

        // Token: 0x04003D0F RID: 15631
        protected string _value;

        // Token: 0x02000CA5 RID: 3237
        public abstract class ConfigQueue : UIfocusable.FocusableQueue
        {
            // Token: 0x06005EC1 RID: 24257 RVA: 0x00612A05 File Offset: 0x00610C05
            protected ConfigQueue(ConfigurableBase config, object sign = null) : base(sign)
            {
                this.config = config;
            }

            // Token: 0x040064B7 RID: 25783
            public readonly ConfigurableBase config;

            // Token: 0x040064B8 RID: 25784
            public OnValueChangeHandler onValueUpdate;

            // Token: 0x040064B9 RID: 25785
            public OnValueChangeHandler onValueChanged;
        }
    }
}
#endregion*/

