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
using static CustomStomachStorage.Plugin;


namespace CustomStomachStorage
{
	public class MyOptions : OptionInterface
	{
		public static readonly MyOptions Instance = new MyOptions();

		public readonly Configurable<int> StomachCapacity;
		public readonly Configurable<bool> DebugMode;
		public readonly Configurable<bool> SpearmasterStoreItems;

		public readonly Dictionary<string, Configurable<bool>> SwallowTypes = new();
		public readonly Dictionary<string, Configurable<string>> GrabTypes = new();
		public readonly Dictionary<string, Configurable<bool>> GrabSpecialTypes = new();

		public HashSet<string> ItemTypeNames { get; private set; } = new();
		public HashSet<string> CreatureTypeNames { get; private set; } = new();

		string[] specialTypeNames = {//45
				"All",

				"OneHandGrabAll",
				"DragGrabAll",
			};

		#region Items
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
		#endregion
		#region Creatures
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
		#endregion

        public enum GrabMode
        {
            NotSelected,
            OneHand,
            BigOneHand,
            TwoHands,
            Drag,
            CantGrab
        }
        public const string NotSelected = "Not selected";  // 注意与数组中的文本一致
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

        // ============ 访问器 ============
        public bool CanSwallow(string type) =>
            SwallowTypes.TryGetValue(type, out var config) && config.Value == true;
        public bool GetGrabSpecial(string typeName) =>
            GrabSpecialTypes.TryGetValue(typeName, out var config) && config.Value == true;
        public string GetGrabMode(string typeName) =>
            GrabTypes.TryGetValue(typeName, out var config) ? config.Value : NotSelected;

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
                _ => Player.ObjectGrabability.CantGrab
            };
        }


        MyOptions()
		{
			//设置默认值
			StomachCapacity = config.Bind<int>($"StomachCapacity_conf_{MOD_name}", 3, new ConfigAcceptableRange<int>(0, 500));

			DebugMode = config.Bind<bool>($"DebugMode_conf_{MOD_name}", false);

			SpearmasterStoreItems = config.Bind<bool>($"SpearmasterStoreItems_conf_{MOD_name}", false);

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


        private void AddLabeledCheckbox(OpTab tab, Vector2 pos, string label,
			Configurable<bool> config, string description = "", Color? warningColor = null)
		{
			var checkbox = new OpCheckBox(config, pos)
			{
				description = description
			};
			var labelWidth = Custom.rainWorld.inGameTranslator.Translate(label).Length * 10f;

			var labelElement = new OpLabel(
				pos.x + checkbox.size.x + 5f,
				pos.y + checkbox.size.y / 2f - 10f,
				label, false)
			{
				description = description
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

        private void AddLabeledComboBox(OpTab tab, Vector2 pos, string label,
            Configurable<string> config, string[] items, string description = "", Color? warningColor = null)
        {
            // 计算合适的宽度（基于最长的选项文本）
            float maxItemWidth = items.Max(item =>
                Custom.rainWorld.inGameTranslator.Translate(item).Length * 8f);
            float comboBoxWidth = Mathf.Max(100f, maxItemWidth + 20f);

            // 创建下拉框（直接使用 string[]）
            var comboBox = new OpaqueComboBox(config, pos, comboBoxWidth, items)
            {
                description = description,
                colorFill = new Color(0f, 0f, 0f, 1f),
            };

            // 创建标签
            var labelElement = new OpLabel(
                pos.x + comboBox.size.x + 5f,
                pos.y + comboBox.size.y / 2f - 10f,
                label, false)
            {
                description = description
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

        private void AddCheckboxType(OpTab tab, ref float yPos, string title,
			IEnumerable<string> types, Dictionary<string, Configurable<bool>> configs,
			float xOffset = 0, string note = "")
		{
			if (!types.Any()) return;

			// 分类标题
			if (!string.IsNullOrEmpty(title))
			{
				var titleLabel = new OpLabel(TITLE_X + xOffset, yPos, title, true)
				{
					alignment = FLabelAlignment.Left
				};
				tab.AddItems(new UIelement[] { titleLabel });
				yPos -= SPACING * 1.5f;
			}
			float yTop = yPos;

			// 类型复选框
			foreach (var type in types)
			{
				if (!configs.ContainsKey(type)) continue;

				var displayName = type;
				if (!string.IsNullOrEmpty(note))
					displayName += $" {note}";

				if (yPos < -1f)
				{
					yPos = yTop;
					xOffset *= 2;
				}
				AddLabeledCheckbox(tab,
					new Vector2(TITLE_X + xOffset, yPos),
					displayName,
					configs[type]);

				yPos -= SPACING;
			}

			yPos -= SPACING * 0.5f;
		}

		private void AddComboBoxType(OpTab tab, ref float yPos, string title,
			IEnumerable<string> types, Dictionary<string, Configurable<string>> configs, string[] items, 
			float xOffset = 0, string note = "")
		{
			if (!types.Any()) return;

			// 分类标题
			if (!string.IsNullOrEmpty(title))
			{
				var titleLabel = new OpLabel(TITLE_X + xOffset, yPos, title, true)
				{
					alignment = FLabelAlignment.Left
				};
				tab.AddItems(new UIelement[] { titleLabel });
				yPos -= SPACING * 1.5f;
			}
			float yTop = yPos;

			// 类型复选框
			foreach (var type in types)
			{
				if (!configs.ContainsKey(type)) continue;

				var displayName = type;
				if (!string.IsNullOrEmpty(note))
					displayName += $" {note}";

				if (yPos < -1f)
				{
					yPos = yTop;
					xOffset *= 2;
				}
				AddLabeledComboBox(tab,
					new Vector2(TITLE_X + xOffset, yPos),
					displayName,
					configs[type],
					items);

				yPos -= SPACING;
			}

			yPos -= SPACING * 0.5f;
		}


		// 常量
		private const float TITLE_X = 10f;
		private const float TITLE_Y = 560f;
		private const float COLUMN_WIDTH = 230f;
		private const float BASE_Y = 450f;
		private const float LABEL_X = 75f;
		private const float SPACING = 30f; // 元素间距
		private const float COMBOBOX_WIDTH = 120f;
		private readonly Color WARNING_COLOR = new(0.85f, 0.35f, 0.4f);

		public override void Initialize()
		{
			// 创建选项卡
			var optionsTab = new OpTab(this, "Options");
			var swallowTab = new OpTab(this, "Swallowing");
			var grabTab = new OpTab(this, "Grasping");
			InGameTranslator translator = Custom.rainWorld.inGameTranslator;
			this.Tabs = new OpTab[]
			{
				optionsTab,
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
				spearText += " (needs MSC)";

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
			yPos -= SPACING * 2;

			// 物品分类 - 左列
			AddCheckboxType(swallowTab, ref yPos,
				translator.Translate("ITEMS"),
				ItemTypeNames, SwallowTypes, 0);

			// 生物分类 - 右列
			float rightColY = TITLE_Y - SPACING * 2;
			AddCheckboxType(swallowTab, ref rightColY,
				translator.Translate("CREATURES"),
				CreatureTypeNames, SwallowTypes, COLUMN_WIDTH);
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

            // 物品分类 - 左列
            float grabYPos = yPos;
			AddComboBoxType(grabTab, ref grabYPos,
				translator.Translate("ITEMS"),
				ItemTypeNames, GrabTypes, grabModeNames, 0);

			// 生物分类 - 右列
			float grabRightY = TITLE_Y - SPACING * 2;
			AddComboBoxType(grabTab, ref grabRightY,
				translator.Translate("CREATURES"),
				CreatureTypeNames, GrabTypes, grabModeNames, COLUMN_WIDTH);
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
				// 强制向上展开
				var downwardField = typeof(OpComboBox).GetField("_downward",
					BindingFlags.NonPublic | BindingFlags.Instance);
				downwardField?.SetValue(this, false);

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
				if (this._rectScroll != null && this._rectList != null && this._lblList != null && this._itemList.Length > this._lblList.Length)
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