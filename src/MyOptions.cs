using Menu;
using Menu.Remix.MixedUI;
using MonoMod.RuntimeDetour;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static CustomStomachStorage.Plugin;
using static CustomStomachStorage.ObjectIcon;
using static CustomStomachStorage.Extended;


namespace CustomStomachStorage
{
	public class MyOptions : OptionInterface
	{
        #region Variable变量
        public static readonly MyOptions Instance = new MyOptions();

        /// <summary>
        /// 可配置的胃容量
        /// </summary>
        public readonly Configurable<int> StomachCapacity;
        /// <summary>
        /// 可配置的调试模式
        /// </summary>
        public readonly Configurable<bool> DebugMode;
        /// <summary>
        /// 可配置的矛大师是否可以存储物品
        /// </summary>
        public readonly Configurable<bool> SpearmasterStoreItems;

        /// <summary>
        /// 可配置的吞咽类型字典
        /// </summary>
        public readonly Dictionary<string, Configurable<bool>> SwallowTypes = new();
        /// <summary>
        /// 可配置的抓取类型字典
        /// </summary>
        public readonly Dictionary<string, Configurable<string>> GrabTypes = new();
        /// <summary>
        /// 可配置的特殊抓取类型字典
        /// </summary>
        public readonly Dictionary<string, Configurable<bool>> GrabSpecialTypes = new();

        /// <summary>
        /// 可配置的全局可见性模式
        /// </summary>
        public readonly Configurable<string> GlobalVisibility;
        /// <summary>
        /// 可配置的背景可见性模式
        /// </summary>
        public readonly Configurable<string> BackgroundVisibility;
        /// <summary>
        /// 可配置的侧边线可见性模式
        /// </summary>
        public readonly Configurable<string> SideLineVisibility;
        /// <summary>
        /// 可配置的侧边线位置
        /// </summary>
        public readonly Configurable<string> SideLinePosition;
        /// <summary>
        /// 可配置的背景透明度
        /// </summary>
        public readonly Configurable<float> BackgroundOpacity;
        /// <summary>
        /// 可配置的侧边线透明度
        /// </summary>
        public readonly Configurable<float> SideLineOpacity;
        /// <summary>
        /// 可配置的图标颜色模式
        /// </summary>
        //public readonly Configurable<string> IconColorMode;
        /// <summary>
        /// 可配置的图标大小
        /// </summary>
        public readonly Configurable<int> IconSize;

        /// <summary>
        /// 物品类型名称集合
        /// </summary>
        public HashSet<string> ItemTypeNames { get; private set; } = new();
        /// <summary>
        /// 生物类型名称集合
        /// </summary>
        public HashSet<string> CreatureTypeNames { get; private set; } = new();
        #endregion

        #region Special
        string[] specialTypeNames = {//45
				"All",

				"OneHandGrabAll",
				"DragGrabAll",
			};
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

        public const string NotSelected = "Not selected";
        #region Modes
        /// <summary>
        ///抓取模式
        /// </summary>
        public enum GrabMode
		{
			NotSelected,
			OneHand,
			BigOneHand,
			TwoHands,
			Drag,
			CantGrab
		}

        /// <summary>
        /// 全局可见性模式
        /// </summary>
        public enum GlobalVisibilityMode
		{
			WhenMapButtonIsHeld,  // 按住地图键时显示
			Always,               // 始终显示
            Never,                // 从不显示
        }

        /// <summary>
        /// 部件可见性模式（背景/侧边线）
        /// </summary>
        public enum PartVisibilityMode
		{
			Never,                 // 从不显示
			WhenAnObjectIsHeld,    // 持有物品时显示
			Always                 // 始终显示
		}
        /// <summary>
        /// 定义侧线位置
        /// </summary>
        /*public enum SideLinePositionMode
        {
            Top,
            Bottom,
            Left,
            Right
        }*/
        /// <summary>
        /// 图标颜色模式
        /// </summary>
        public enum IconColorModes
		{
			ColorCoded,    // 彩色编码（左右手不同色）
			DefaultColors  // 默认颜色
		}
		#endregion

		#region Transform转换
        private static string GrabModeToString(GrabMode mode)
		{
			return mode switch
			{
				GrabMode.NotSelected => "Not selected",
				GrabMode.OneHand => "One hand",
				GrabMode.BigOneHand => "Big one hand",
				GrabMode.TwoHands => "Two hands",
				GrabMode.Drag => "Drag",
				GrabMode.CantGrab => "Cannot grab",
				_ => "Not selected"
			};
		}
		private static GrabMode GrabModeFromString(string value)
		{
			foreach (GrabMode mode in Enum.GetValues(typeof(GrabMode)))
				if (GrabModeToString(mode) == value)
					return mode;
			return GrabMode.NotSelected;
		}
		public Player.ObjectGrabability GetPlayerGrab(string mode)
		{
			// 转换显示文本到实际值
			return mode switch
			{
				"One hand" => Player.ObjectGrabability.OneHand,
				"Big one hand" => Player.ObjectGrabability.BigOneHand,
				"Two hands" => Player.ObjectGrabability.TwoHands,
				"Drag" => Player.ObjectGrabability.Drag,
				"Cannot grab" => Player.ObjectGrabability.CantGrab,
				"Not selected" => Player.ObjectGrabability.CantGrab,
                "单手" => Player.ObjectGrabability.OneHand,
                "大单手" => Player.ObjectGrabability.BigOneHand,
                "双手" => Player.ObjectGrabability.TwoHands,
                "拖拽" => Player.ObjectGrabability.Drag,
                "无法抓取" => Player.ObjectGrabability.CantGrab,
                "未选择" => Player.ObjectGrabability.CantGrab,
                _ => Player.ObjectGrabability.CantGrab
			};
		}

		private static string GlobalVisibilityModeToString(GlobalVisibilityMode value)
		{
			return value switch
			{
				GlobalVisibilityMode.WhenMapButtonIsHeld => "When Map button is held",
				GlobalVisibilityMode.Always => "Always",
                GlobalVisibilityMode.Never => "Never",
                _ => "Never"
            };
		}
		private static GlobalVisibilityMode GlobalVisibilityModeFromString(string value)
		{
			foreach (GlobalVisibilityMode mode in Enum.GetValues(typeof(GlobalVisibilityMode)))
				if (GlobalVisibilityModeToString(mode) == value)
					return mode;
			return GlobalVisibilityMode.Always;
		}

		private static string PartVisibilityModeToString(PartVisibilityMode value)
		{
			return value switch
			{
				PartVisibilityMode.Never => "Never",
				PartVisibilityMode.WhenAnObjectIsHeld => "When an object is held",
				PartVisibilityMode.Always => "Always",
				_ => "Never"
			};
		}
		private static PartVisibilityMode PartVisibilityModeFromString(string value)
		{
			foreach (PartVisibilityMode mode in Enum.GetValues(typeof(PartVisibilityMode)))
				if (PartVisibilityModeToString(mode) == value)
					return mode;
			return PartVisibilityMode.Never;
		}

        private static string SideLinePositionModeToString(SideLinePositionMode value)
        {
            return value switch
            {
                SideLinePositionMode.Top => "Top",
                SideLinePositionMode.Bottom => "Bottom",
                SideLinePositionMode.Left => "Left",
                SideLinePositionMode.Right => "Right",
                _ => "Left"
            };
        }
        private static SideLinePositionMode SideLinePositionModeFromString(string value)
        {
            foreach (SideLinePositionMode mode in Enum.GetValues(typeof(SideLinePositionMode)))
                if (SideLinePositionModeToString(mode) == value)
                    return mode;
            return SideLinePositionMode.Left;
        }

        private static string IconColorModeToString(IconColorModes value)
		{
			return value switch
			{
				IconColorModes.ColorCoded => "Color-coded",
				IconColorModes.DefaultColors => "Default colors",
				_ => "Color-coded"
			};
		}
		private static IconColorModes IconColorModeFromString(string value)
		{
			foreach (IconColorModes mode in Enum.GetValues(typeof(IconColorModes)))
				if (IconColorModeToString(mode) == value)
					return mode;
			return IconColorModes.ColorCoded;
		}
        #endregion

        #region 访问器
        // ============ 访问器 ============
        public bool CanSwallow(string type) =>
			SwallowTypes.TryGetValue(type, out var config) && config.Value == true;
		public bool GetGrabSpecial(string typeName) =>
			GrabSpecialTypes.TryGetValue(typeName, out var config) && config.Value == true;
		public string GetGrabMode(string typeName) =>
			GrabTypes.TryGetValue(typeName, out var config) ? config.Value : NotSelected;

		public int GetIconSize()
		{
			return IconSize.Value;
		}
		public GlobalVisibilityMode GetGlobalVisibility()
		{
			return GlobalVisibilityModeFromString(GlobalVisibility.Value);
		}
		public PartVisibilityMode GetBackgroundVisibility()
		{
			return PartVisibilityModeFromString(BackgroundVisibility.Value);
		}
		public PartVisibilityMode GetSideLineVisibility()
		{
			return PartVisibilityModeFromString(SideLineVisibility.Value);
		}
        public SideLinePositionMode GetSideLinePosition()
        {
            return SideLinePositionModeFromString(SideLinePosition.Value);
        }
        public float GetBackgroundOpacity()
		{
			return BackgroundOpacity.Value;
		}
		public float GetSideLineOpacity()
		{
			return SideLineOpacity.Value;
		}
		/*public IconColorModes GetIconColorMode()
		{
			return IconColorModeFromString(IconColorMode.Value);
		}*/
        #endregion

        #region MyOptions
        MyOptions()
		{
			//设置默认值
			StomachCapacity = config.Bind<int>($"StomachCapacity_conf_{MOD_name}", 3, new ConfigAcceptableRange<int>(0, 500));

			DebugMode = config.Bind<bool>($"DebugMode_conf_{MOD_name}", false);

			SpearmasterStoreItems = config.Bind<bool>($"SpearmasterStoreItems_conf_{MOD_name}", false);

			// 添加 HUD 相关配置
			GlobalVisibility = config.Bind<string>(
				$"GlobalVisibility_conf_{MOD_name}",
				GlobalVisibilityModeToString(GlobalVisibilityMode.Never)
			);

			BackgroundVisibility = config.Bind<string>(
				$"BackgroundVisibility_conf_{MOD_name}",
				PartVisibilityModeToString(PartVisibilityMode.Never)
			);

			SideLineVisibility = config.Bind<string>(
				$"SideLineVisibility_conf_{MOD_name}",
				PartVisibilityModeToString(PartVisibilityMode.Always)
			);

            SideLinePosition = config.Bind<string>(
                $"SideLinePosition_conf_{MOD_name}",
                SideLinePositionModeToString(SideLinePositionMode.Left)
            );

            BackgroundOpacity = config.Bind<float>(
				$"BackgroundOpacity_conf_{MOD_name}",
				0.5f,
				new ConfigAcceptableRange<float>(0f, 1f)
			);

			SideLineOpacity = config.Bind<float>(
				$"SideLineOpacity_conf_{MOD_name}",
				1f,
				new ConfigAcceptableRange<float>(0f, 1f)
			);

			/*IconColorMode = config.Bind<string>(
				$"IconColorMode_conf_{MOD_name}",
				IconColorModeToString(IconColorModes.ColorCoded)
			);*/

			// 图标大小（默认38，范围15-64）
			IconSize = config.Bind<int>(
				$"IconSize_conf_{MOD_name}",
				38,
				new ConfigAcceptableRange<int>(15, 64)
			);

			// 基础类型
			SwallowTypes["All"] = config.Bind<bool>($"{"All"}_GrabSpecial_conf_{MOD_name}", false);
			GrabSpecialTypes["OneHandGrabAll"] = config.Bind<bool>($"{"OneHandGrabAll"}_GrabSpecial_conf_{MOD_name}", false);
			GrabSpecialTypes["DragGrabAll"] = config.Bind<bool>($"{"DragGrabAll"}_GrabSpecial_conf_{MOD_name}", false);

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
			SwallowTypes[typeName] = config.Bind<bool>($"{typeName}_Swallow_conf_{MOD_name}", defaultValue);
			GrabTypes[typeName] = config.Bind<string>(
				$"{typeName}_Grab_conf_{MOD_name}",
				GrabModeToString(GrabMode.NotSelected)
			);
		}
        #endregion

        #region Add
        /// <summary>
        /// 添加带有标签的选择框
        /// </summary>
        private void AddLabeledCheckbox(OpTab tab, Vector2 pos, string label,
			Configurable<bool> config, string description = "", Color? warningColor = null)
		{
            InGameTranslator translator = Custom.rainWorld.inGameTranslator;
            var checkbox = new OpCheckBox(config, pos)
			{
				description = translator.Translate(description)
			};
			//float labelWidth = Custom.rainWorld.inGameTranslator.Translate(label).Length * 10f;

			var labelElement = new OpLabel(
				pos.x + checkbox.size.x + 5f,
				pos.y + checkbox.size.y / 2f - 10f,
                translator.Translate(label), false)
			{
				description = translator.Translate(description)
			};

			if (warningColor.HasValue)
			{
				labelElement.color = warningColor.Value;
				checkbox.colorEdge = warningColor.Value;
			}

			tab.AddItems(new UIelement[] { checkbox, labelElement });
			//_valueItems.Add(checkbox);
			//_checkBoxItems.Add(checkbox);
		}

        /// <summary>
        /// 添加带有标签的下拉框
        /// </summary>
        private void AddLabeledComboBox(OpTab tab, Vector2 pos, string label,
			Configurable<string> config, string[] items, string description = "", Color? warningColor = null)
		{
            InGameTranslator translator = Custom.rainWorld.inGameTranslator;
			// 计算合适的宽度（基于最长的选项文本）
            float maxItemWidth = CalculateMaxItemWidth(items);
			float comboBoxWidth = Mathf.Max(100f, maxItemWidth + 20f);

			/*for (int i = 0; i < items.Length; i++)
			{
				items[i] = translator.Translate(items[i]);
			}*/

			// 创建下拉框
			var comboBox = new OpaqueComboBox(config, pos, comboBoxWidth, items)
			{
                description = translator.Translate(description),
				colorFill = new Color(0f, 0f, 0f, 1f),
			};

			// 创建标签
			var labelElement = new OpLabel(
				pos.x + comboBox.size.x + 5f,
				pos.y + comboBox.size.y / 2f - 10f,
                translator.Translate(label), false)
			{
                description = translator.Translate(description)
            };

			// 警告颜色（如果需要）
			if (warningColor.HasValue)
			{
				labelElement.color = warningColor.Value;
				comboBox.colorEdge = warningColor.Value;
			}

			// 添加到选项卡
			tab.AddItems(new UIelement[] { comboBox, labelElement });
		}

        /// <summary>
        /// 添加选择框的类型列
        /// </summary>
        private void AddCheckboxType(OpTab tab, ref Vector2 pos, string title,
			IEnumerable<string> types, Dictionary<string, Configurable<bool>> configs,
			string note = "")
		{
			if (!types.Any()) return;

            InGameTranslator translator = Custom.rainWorld.inGameTranslator;

            float yTop = TITLE_Y;
            if (pos.y < -1f)
            {
                pos.y = yTop;
                pos.x += COLUMN_WIDTH;
            }
            // 分类标题
            if (!string.IsNullOrEmpty(title))
			{
				var titleLabel = new OpLabel(pos.x, pos.y, translator.Translate(title), true)
				{
					alignment = FLabelAlignment.Left
				};
				tab.AddItems(new UIelement[] { titleLabel });
				pos.y -= SPACING * 1.5f;
			}

            // 类型选择框
            foreach (var type in types)
			{
				if (!configs.ContainsKey(type)) continue;

                string displayName = translator.Translate(type);
				if (!string.IsNullOrEmpty(note))
					displayName += $" {translator.Translate(note)}";

                displayName = translator.Translate(displayName);

                if (pos.y < -1f)
                {
                    pos.y = yTop;
                    pos.x += COLUMN_WIDTH;
                }
                AddLabeledCheckbox(tab,
					new Vector2(pos.x, pos.y),
                    displayName,
					configs[type]);

                pos.y -= SPACING;
			}

            pos.y -= SPACING * 0.5f;
		}

        /// <summary>
        /// 添加下拉框的类型列
        /// </summary>
        private void AddComboBoxType(OpTab tab, ref Vector2 pos, string title,
			IEnumerable<string> types, Dictionary<string, Configurable<string>> configs, string[] items, 
			string note = "")
		{
            if (!types.Any()) return;

            InGameTranslator translator = Custom.rainWorld.inGameTranslator;

            float yTop = TITLE_Y;
            if (pos.y < -1f)
            {
                pos.y = yTop;
                pos.x += COLUMN_WIDTH;
            }
            // 分类标题
            if (!string.IsNullOrEmpty(title))
			{
				var titleLabel = new OpLabel(pos.x, pos.y, translator.Translate(title), true)
				{
					alignment = FLabelAlignment.Left
				};
				tab.AddItems(new UIelement[] { titleLabel });
				pos.y -= SPACING * 1.5f;
			}

			// 类型下拉框
			foreach (var type in types)
			{
				if (!configs.ContainsKey(type)) continue;

                string displayName = translator.Translate(type);
                if (!string.IsNullOrEmpty(note))
                    displayName += $" {translator.Translate(note)}";

                displayName = translator.Translate(displayName);

                if (pos.y < -1f)
                {
                    pos.y = yTop;
                    pos.x += COLUMN_WIDTH;
                }
                AddLabeledComboBox(tab,
					new Vector2(pos.x, pos.y),
					displayName,
					configs[type],
					items);

                pos.y -= SPACING;
			}
			
            pos.y -= SPACING * 0.5f;
		}
        #endregion

        #region 常量
        // 常量
        /// <summary>
        /// 标题X坐标
        /// </summary>
        private const float TITLE_X = 10f;
        /// <summary>
        /// 标题Y坐标
        /// </summary>
        private const float TITLE_Y = 560f;
        /// <summary>
        /// 列宽度
        /// </summary>
        private const float COLUMN_WIDTH = 230f;
        /// <summary>
        /// 基础Y坐标
        /// </summary>
        private const float BASE_Y = 450f;
        /// <summary>
        /// 标签X坐标
        /// </summary>
        private const float LABEL_X = 75f;
        /// <summary>
        /// 元素间距
        /// </summary>
        private const float SPACING = 30f;
        /// <summary>
        /// 下拉框宽度
        /// </summary>
        private const float COMBOBOX_WIDTH = 120f;
        /// <summary>
        /// 警告颜色
        /// </summary>
        private readonly Color WARNING_COLOR = new(0.85f, 0.35f, 0.4f);
        #endregion

        public override void Initialize()
		{
            // 创建选项卡
            InGameTranslator translator = Custom.rainWorld.inGameTranslator;
            var optionsTab = new OpTab(this, translator.Translate("Options"));
            var hudTab = new OpTab(this, translator.Translate("HUD"));
            var swallowTab = new OpTab(this, translator.Translate("Swallowing"));
			var grabTab = new OpTab(this, translator.Translate("Grasping"));
			this.Tabs = new OpTab[]
			{
				optionsTab, 
				hudTab,
				swallowTab,
				grabTab,
			};

			#region optionsTab
			// ============ Options 选项卡 ============
			float yPos = TITLE_Y;

			// 标题
			optionsTab.AddItems(new UIelement[]
			{
				new OpLabel(TITLE_X, yPos, translator.Translate("Custom Stomach Storage"), true)
				{
					alignment = FLabelAlignment.Left
				}
			});
			yPos -= SPACING * 2;

			//选项
			// 胃容量
			var capacityBox = new OpTextBox(StomachCapacity,
				new Vector2(TITLE_X, yPos - 10f), 50f);
			var capacityLabel = new OpLabel(TITLE_X + capacityBox.size.x + 10f, yPos - 5f, translator.Translate("Stomach capacity"), false);

			optionsTab.AddItems(new UIelement[] { capacityBox, capacityLabel });

			yPos -= SPACING * 1.5f;

			// 矛大师选项
			string spearText = translator.Translate("Allow Spearmaster to store items");
			Color? spearColor = !ModManager.MSC ? WARNING_COLOR : null;
			if (!ModManager.MSC)
			{
                spearText = translator.Translate("Allow Spearmaster to store items (needs MSC)");
            }

			AddLabeledCheckbox(optionsTab, new Vector2(TITLE_X, yPos),
				spearText, SpearmasterStoreItems,
				"If enabled, Spearmaster can store items in stomach",
				spearColor);

			yPos -= SPACING;

			// 调试模式
			AddLabeledCheckbox(optionsTab, new Vector2(TITLE_X, 40f),
				translator.Translate("Debug Mode"), DebugMode,
				"Enable debug logging");
			#endregion

			#region hudTab
			yPos = TITLE_Y;

			// 标题
			hudTab.AddItems(new OpLabel(TITLE_X, yPos,
				translator.Translate("HUD Settings"), true)
			{
				alignment = FLabelAlignment.Left
			});
			yPos -= SPACING * 2;

			// 全局可见性
			string[] globalVisModes = new[]
			{
                GlobalVisibilityModeToString(GlobalVisibilityMode.Never),
                GlobalVisibilityModeToString(GlobalVisibilityMode.WhenMapButtonIsHeld),
				GlobalVisibilityModeToString(GlobalVisibilityMode.Always)
			};
			AddLabeledComboBox(hudTab, new Vector2(TITLE_X, yPos),
				translator.Translate("Global visibility"),
				GlobalVisibility, globalVisModes,
				"Choose whether the icons are always visible, or only visible when the map is shown");
			yPos -= SPACING * 1.5f;

			// 图标颜色模式
			/*string[] colorModes = new[]
			{
				IconColorModeToString(IconColorModes.ColorCoded),
				IconColorModeToString(IconColorModes.DefaultColors)
			};
			AddLabeledComboBox(hudTab, new Vector2(TITLE_X, yPos),
				translator.Translate("Icon color mode"),
				IconColorMode, colorModes,
				"Choose whether icons are color-coded or use default colors");
			yPos -= SPACING * 1.5f;*/

            // 背景可见性
            string[] partVisModes = new[]
			{
				PartVisibilityModeToString(PartVisibilityMode.Never),
				//PartVisibilityModeToString(PartVisibilityMode.WhenAnObjectIsHeld),
				PartVisibilityModeToString(PartVisibilityMode.Always)
			};
			AddLabeledComboBox(hudTab, new Vector2(TITLE_X, yPos),
				translator.Translate("Background visibility"),
				BackgroundVisibility, partVisModes,
				"Choose when the icon background is visible");
			yPos -= SPACING * 1.5f;

            // 背景透明度
            var bgOpacitySlider = new OpFloatSlider(BackgroundOpacity,
				new Vector2(TITLE_X, yPos - 10f), 200);
			var bgOpacityLabel = new OpLabel(TITLE_X + 210f, yPos - 5f,
				translator.Translate("Background opacity"), false);
			hudTab.AddItems(new UIelement[] { bgOpacitySlider, bgOpacityLabel });
			yPos -= SPACING * 1.5f;

            // 侧边线可见性
            AddLabeledComboBox(hudTab, new Vector2(TITLE_X, yPos),
				translator.Translate("Side line visibility"),
				SideLineVisibility, partVisModes,
				"Choose when the color-coded side lines are visible");
			yPos -= SPACING * 1.5f;

            // 侧边线位置
            string[] positionModes = new[]
			{
                SideLinePositionModeToString(SideLinePositionMode.Top),
                SideLinePositionModeToString(SideLinePositionMode.Bottom),
				SideLinePositionModeToString(SideLinePositionMode.Left),
                SideLinePositionModeToString(SideLinePositionMode.Right),
            };
            AddLabeledComboBox(hudTab, new Vector2(TITLE_X, yPos),
                translator.Translate("Side line position"),
                SideLinePosition, positionModes,
                "Choose the position of the color-coded side lines");
            yPos -= SPACING * 1.5f;

            // 侧边线透明度
            var slOpacitySlider = new OpFloatSlider(SideLineOpacity,
				new Vector2(TITLE_X, yPos - 10f), 200);
			var slOpacityLabel = new OpLabel(TITLE_X + 210f, yPos - 5f,
				translator.Translate("Side line opacity"), false);
			hudTab.AddItems(new UIelement[] { slOpacitySlider, slOpacityLabel });
            yPos -= SPACING * 1.5f;

            // 图标大小滑块
            var iconSizeSlider = new OpSlider(IconSize,
				new Vector2(TITLE_X, yPos - 10f), 200)
			{
				min = 15,
				max = 64
			};
			var iconSizeLabel = new OpLabel(TITLE_X + 210f, yPos - 5f,
				translator.Translate("Icon size"), false);
			/*var iconSizeDescription = new OpLabel(TITLE_X + 210f, yPos - 25f,
				translator.Translate("Size of item icons in HUD"), true)
			{
				alpha = 0.5f
			};*/

			hudTab.AddItems(new UIelement[] {
				iconSizeSlider,
				iconSizeLabel,
			 });
			yPos -= SPACING * 1.5f;
            #endregion

            #region swallowTab
            // ============ Swallowing 选项卡 ============
            yPos = TITLE_Y;

			// 标题
			swallowTab.AddItems(new OpLabel(TITLE_X, yPos,
				translator.Translate("Swallowing Settings"), true)
			{
				alignment = FLabelAlignment.Left
			});
			yPos -= SPACING * 2;

			// 吞咽全部
			AddLabeledCheckbox(swallowTab, new Vector2(TITLE_X, yPos),
				translator.Translate("Swallow All"), SwallowTypes["All"],
				"Allow swallowing all objects and creatures");
            yPos -= SPACING;

            yPos -= SPACING * 2;

			Vector2 pos = new Vector2(TITLE_X, yPos);

			// 物品分类 - 左列
			AddCheckboxType(swallowTab, ref pos,
				translator.Translate("ITEMS"),
				ItemTypeNames, SwallowTypes);

			// 生物分类 - 右列
			//float rightColY = TITLE_Y - SPACING * 2;
			AddCheckboxType(swallowTab, ref pos,
				translator.Translate("CREATURES"),
				CreatureTypeNames, SwallowTypes);
			#endregion

			#region grabTab
			// ============ Grasping 选项卡 ============
			yPos = TITLE_Y;

			// 标题
			grabTab.AddItems(new OpLabel(TITLE_X, yPos,
				translator.Translate("Grasping Settings"), true)
			{
				alignment = FLabelAlignment.Left
			});
			yPos -= SPACING * 2;

			AddLabeledCheckbox(grabTab, new Vector2(TITLE_X, yPos),
				translator.Translate("One hand can grab all"),
				GrabSpecialTypes["OneHandGrabAll"],
				"Allows grabbing any object with one hand");
			yPos -= SPACING;

			AddLabeledCheckbox(grabTab, new Vector2(TITLE_X, yPos),
				translator.Translate("At least able to drag all"),
				GrabSpecialTypes["DragGrabAll"],
				"Objects that cannot be grabbed can at least be dragged");
			yPos -= SPACING * 2;

			string[] grabModeNames = Enum.GetValues(typeof(GrabMode))
				.Cast<GrabMode>()
				.Select(m => GrabModeToString(m))
				.ToArray();

            pos = new Vector2(TITLE_X, yPos);

            // 物品分类 - 左列
            float grabYPos = yPos;
			AddComboBoxType(grabTab, ref pos,
				translator.Translate("ITEMS"),
				ItemTypeNames, GrabTypes, grabModeNames);

			// 生物分类 - 右列
			//float grabRightY = TITLE_Y - SPACING * 2;
			AddComboBoxType(grabTab, ref pos,
				translator.Translate("CREATURES"),
				CreatureTypeNames, GrabTypes, grabModeNames);
			#endregion

		}


	}


	// 自定义不透明下拉框类
	public class OpaqueComboBox : OpComboBox
	{
		public OpaqueComboBox(Configurable<string> config, Vector2 pos, float width, string[] list)
			: base(config, pos, width, list)
		{
			/*// 1. 先调用父类构造函数（此时已经排序了）

			// 2. 立即用反射替换为原始顺序
			var itemListField = typeof(OpComboBox).GetField("_itemList",
				BindingFlags.NonPublic | BindingFlags.Instance);

			// 保存原始顺序的副本
			var originalArray = originalList.ToArray();
			itemListField?.SetValue(this, originalArray);

			// 3. 重置索引但不排序
			var resetMethod = typeof(OpComboBox).GetMethod("_ResetIndex",
				BindingFlags.NonPublic | BindingFlags.Instance);
			resetMethod?.Invoke(this, null);

			// 4. 保存原始列表供后续使用
			this._originalList = originalArray;*/
		}

		//private ListItem[] _originalList;

		public override void Update()
		{
			//bool wasHeld = this.held;
			bool wasListVisible = this._rectList != null && !this._rectList.isHidden;

			base.Update();

			/*// 每次更新时确保_itemList是原始顺序（防止父类其他地方修改）
			var itemListField = typeof(OpComboBox).GetField("_itemList",
				BindingFlags.NonPublic | BindingFlags.Instance);
			var currentList = itemListField?.GetValue(this) as ListItem[];

			if (currentList != this._originalList)
			{
				itemListField?.SetValue(this, this._originalList);
			}*/

			bool isListVisible = this._rectList != null && !this._rectList.isHidden;

			// 列表刚刚打开时自定义
			if (!wasListVisible && isListVisible)
			{
				if (this.pos.y < 501f)
				{
                    // 强制向上展开
                    var downwardField = typeof(OpComboBox).GetField("_downward",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    downwardField?.SetValue(this, false);
                }

				// 设置不透明背景和位置
				if (this._rectList != null)
				{
					float listHeight = 20f * (float)Mathf.Clamp(this._itemList.Length, 1, this.listHeight) + 10f;
					this._rectList.size = new Vector2(base.size.x, listHeight);
					this._rectList.pos = new Vector2(0f, base.size.y);
					this._rectList.fillAlpha = 1f;
					this._rectList.colorFill = MenuColorEffect.rgbBlack;
				}

				// 更新列表项文本（确保使用原始顺序）
				if (this._lblList != null)
				{
					for (int i = 0; i < this._lblList.Length && i < this._itemList.Length; i++)
					{
						this._lblList[i].text = this._itemList[i].EffectiveDisplayName;
						this._lblList[i].y = base.size.y + 10f - 20f * i;
					}
				}

				// 更新滚动条
				if (this._rectScroll != null && this._rectList != null && this._lblList != null && this._itemList.Length > this._lblList.Length && this.pos.y < 501f)
				{
					this._rectScroll.size = new Vector2(15f, this._ScrollLen(this._itemList.Length));
					this._rectScroll.pos = new Vector2(this._rectList.pos.x + this._rectList.size.x - 20f,
						this._rectList.pos.y + 10f + (this._rectList.size.y - 20f - this._rectScroll.size.y) *
						(float)(this._itemList.Length - this._lblList.Length - this._listTop) /
						(float)(this._itemList.Length - this._lblList.Length));
					this._rectScroll.fillAlpha = 1f;
					this._rectScroll.colorFill = MenuColorEffect.rgbBlack;
				}
			}
		}

	}

}