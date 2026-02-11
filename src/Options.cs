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


namespace CustomStomachStorage
{
	public class Options : OptionInterface
	{
		public static readonly Options Instance = new Options();

		public readonly Configurable<int> StomachCapacity;
		public readonly Configurable<bool> DebugMode;
        public readonly Configurable<bool> SpearmasterStoreItems;

        public readonly Dictionary<string, Configurable<bool>> SwallowTypes = new Dictionary<string, Configurable<bool>>();

		string[] otherTypeNames = {//37
			"All",
			};

		string[] ItemTypeNames = {
				"Item",

				"Spear",
				"VultureMask",
				"NeedleEgg",

				"JokeRifle",
				"EnergyCell",
				"MoonCloak",

				"Boomerang",
			};

		string[] CreatureTypeNames = {
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

				"Yeek",
				"Inspector",
				"StowawayBug",

				"Loach",
				"BigMoth",
				"SkyWhale",
				"BoxWorm",
				"DrillCrab",
				"Tardigrade",
				"Barnacle",
				"Frog",
			};


		Options()
		{
			//设置默认值
			StomachCapacity = config.Bind<int>($"StomachCapacity_conf_{Plugin.MOD_name}", 3, new ConfigAcceptableRange<int>(0, 500));

			DebugMode = config.Bind<bool>($"DebugMode_conf_{Plugin.MOD_name}", false);

            SpearmasterStoreItems = config.Bind<bool>($"SpearmasterStoreItems_conf_{Plugin.MOD_name}", false);

            foreach (string typeName in otherTypeNames)
			{
				InitializeSwallowType(typeName, false);
		}
			foreach (string typeName in ItemTypeNames)
			{
				InitializeSwallowType(typeName, false);
			}
			foreach (string typeName in CreatureTypeNames)
			{
				InitializeSwallowType(typeName, false);
			}
		}

		private void InitializeSwallowType(string typeName, bool defaultValue = false)
		{
			SwallowTypes[typeName] = config.Bind<bool>($"{typeName}_conf_{Plugin.MOD_name}", defaultValue);
		}


		public override void Initialize()
		{
			// 选项卡
			OpTab opTab = new OpTab(this, "Options");
			OpTab typeTab = new OpTab(this, "Type");
			InGameTranslator inGameTranslator = Custom.rainWorld.inGameTranslator;
			this.Tabs = new OpTab[]
			{
				opTab,
				typeTab
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

			// 类型标题
			typeTab.AddItems(new UIelement[]
			{
				new OpLabel(Title_X, Title_Y, inGameTranslator.Translate("Types of swallowing"), true)
				{
					alignment = FLabelAlignment.Left
				}
			});

			// 类型选项卡
			List<UIelement> ui = new List<UIelement>();

			float ItemPos_Y = OpBox_Y;
			int Itemi = 0;

			float CreaturePos_Y = OpBox_Y;
			int Creaturei = 0;

			HashSet<string> itemSet = new HashSet<string>(ItemTypeNames);
			HashSet<string> creatureSet = new HashSet<string>(CreatureTypeNames);

			foreach (var kvp in SwallowTypes)
			{
				OpCheckBox? cb = null;
				OpLabel? label = null;

				if (kvp.Key == "All")
				{
					cb = new OpCheckBox(kvp.Value, new Vector2(Title_X + Creature_X, OpBox_Y));
					label = new OpLabel(new Vector2(OpLabel_X + Creature_X, OpBox_Y), new Vector2(200f, 24f),
												inGameTranslator.Translate(kvp.Key),
												FLabelAlignment.Left, false, null);
				}

				else if (itemSet.Contains(kvp.Key))
				{
					ItemPos_Y = OpBox_Y - (spacing * (Itemi + 1));

					// 创建复选框
					cb = new OpCheckBox(kvp.Value, new Vector2(Title_X, ItemPos_Y));

					// 创建标签（使用存储的类型名称）
					label = new OpLabel(new Vector2(OpLabel_X, ItemPos_Y), new Vector2(200f, 24f),
											   inGameTranslator.Translate(kvp.Key),
											   FLabelAlignment.Left, false, null);
					Itemi += 1;
				}

				else if (creatureSet.Contains(kvp.Key))
				{
					if (Creaturei <= 14)
					{
						CreaturePos_Y = OpBox_Y - (spacing * (Creaturei + 1));
						// 创建复选框
						cb = new OpCheckBox(kvp.Value, new Vector2(Title_X + Creature_X, CreaturePos_Y));

						// 创建标签（使用存储的类型名称）
						label = new OpLabel(new Vector2(OpLabel_X + Creature_X, CreaturePos_Y), new Vector2(200f, 24f),
												   inGameTranslator.Translate(kvp.Key),
												   FLabelAlignment.Left, false, null);
					}
					else
					{
						CreaturePos_Y = OpBox_Y - (spacing * (Creaturei + 1 - 15));

						// 创建复选框
						cb = new OpCheckBox(kvp.Value, new Vector2(Title_X + (Creature_X * 2), CreaturePos_Y));

						// 创建标签（使用存储的类型名称）
						label = new OpLabel(new Vector2(OpLabel_X + (Creature_X * 2), CreaturePos_Y), new Vector2(200f, 24f),
												   inGameTranslator.Translate(kvp.Key),
												   FLabelAlignment.Left, false, null);
					}

					Creaturei += 1;
				}

				if (cb != null && label != null)
				{
					ui.Add(cb);
					ui.Add(label);
				}
			}

			// 添加到类型选项卡
			typeTab.AddItems(ui.ToArray());


		}


	}
}
