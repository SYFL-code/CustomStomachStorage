using Menu.Remix.MixedUI;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static CustomStomachStorage.Plugin;


namespace CustomStomachStorage
{
	public class MyOptions : OptionInterface
	{
		public static readonly MyOptions Instance = new MyOptions();

		public readonly Configurable<int> StomachCapacity;
		public readonly Configurable<bool> DebugMode;
		public readonly Configurable<bool> SpearmasterStoreItems;

		public readonly Dictionary<string, Configurable<bool>> SwallowTypes = new Dictionary<string, Configurable<bool>>();
		public readonly Dictionary<string, Configurable<bool>> GrabTypes = new Dictionary<string, Configurable<bool>>();

        public HashSet<string> ItemTypeNames { get; private set; } = new();
        public HashSet<string> CreatureTypeNames { get; private set; } = new();

        string[] otherTypeNames = {//45
				"All",

				"OneHandGrabAll",
				"DragGrabAll",
			};


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


		MyOptions()
		{
			//设置默认值
			StomachCapacity = config.Bind<int>($"StomachCapacity_conf_{MOD_name}", 3, new ConfigAcceptableRange<int>(0, 500));

			DebugMode = config.Bind<bool>($"DebugMode_conf_{MOD_name}", false);

			SpearmasterStoreItems = config.Bind<bool>($"SpearmasterStoreItems_conf_{MOD_name}", false);

            // 基础类型
            foreach (string typeName in otherTypeNames)
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
            }
        }

		private void InitializeSwallowType(string typeName, bool defaultValue = false)
		{
			SwallowTypes[typeName] = config.Bind<bool>($"{typeName}_conf_{MOD_name}", defaultValue);
			//SwallowTypes[typeName] = config.Bind<bool>($"{typeName}_Swallow_conf_{MOD_name}", defaultValue);
			GrabTypes[typeName] = config.Bind<bool>($"{typeName}_Grab_conf_{MOD_name}", defaultValue);

			/*if (!ItemTypeNames.Contains(typeName))
			{
				GrabTypes[typeName] = config.Bind<bool>($"{typeName}_Grab_conf_{MOD_name}", defaultValue);
			}*/
		}


		public override void Initialize()
		{
			// 选项卡
			OpTab opTab = new OpTab(this, "Options");
			OpTab SwallowTypeTab = new OpTab(this, "Swallowing Type");
			OpTab GrabTypeTab = new OpTab(this, "Grab Type");
			InGameTranslator inGameTranslator = Custom.rainWorld.inGameTranslator;
			this.Tabs = new OpTab[]
			{
				opTab,
				SwallowTypeTab,
				GrabTypeTab,
			};

			const float Title_X = 10f;
			const float Title_Y = 560;

			const float Creature_X = 230f;

			const float OpBox_Y = 450f;
			const float OpLabel_X = 75f;

			const float spacing = 30f; // 元素间距

			// 标题
			opTab.AddItems(new UIelement[]
			{
				new OpLabel(Title_X, Title_Y, inGameTranslator.Translate("Custom Stomach Storage"), true)
				{
					alignment = FLabelAlignment.Left
				}
			});

			//选项
			opTab.AddItems(new UIelement[]
			{
				new OpTextBox(StomachCapacity, new Vector2(Title_X, OpBox_Y - 0f), 50f),
				new OpLabel(new Vector2(OpLabel_X, OpBox_Y - 0f), new Vector2(200f, 24f), inGameTranslator.Translate("Stomach capacity"), FLabelAlignment.Left, false, null),

				new OpCheckBox(SpearmasterStoreItems, new Vector2(Title_X, OpBox_Y - spacing)),
				new OpLabel(new Vector2(OpLabel_X, OpBox_Y - spacing), new Vector2(200f, 24f), inGameTranslator.Translate("Allow Spearmaster to store items"), FLabelAlignment.Left, false, null),

				new OpCheckBox(DebugMode, new Vector2(Title_X, 40f)),
				new OpLabel(new Vector2(OpLabel_X, 40f), new Vector2(200f, 24f), inGameTranslator.Translate("DebugMode"), FLabelAlignment.Left, false, null)
			});




			// 吞咽类型标题
			SwallowTypeTab.AddItems(new UIelement[]
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
			GrabTypeTab.AddItems(G_ui.ToArray());

		}


	}
}
