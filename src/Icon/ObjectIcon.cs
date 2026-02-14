using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RWCustom;
using static CustomStomachStorage.MyOptions;

namespace CustomStomachStorage
{
	/// <summary>
	/// 表示一个对象图标的类
	/// </summary>
	public class ObjectIcon
	{
		#region enum
		/// <summary>
		/// 定义侧线位置的枚举
		/// </summary>
		public enum SideLinePosition
		{
			Top,
			Bottom,
			Left,
			Right
		}

        /// <summary>
        /// 定义显示模式的枚举
        /// </summary>
        public enum DisplayMode
        {
            Holding,        // 手持状态
            PickUpCandidate // 可拾取候选状态
        }
        #endregion
        /// <summary>
        /// 存储ObjectIcon设置的结构体
        /// </summary>
        public struct Settings
		{
            /// <summary>
            /// 允许的最小尺寸
            /// </summary>
            public int? minAllowedSize;
            /// <summary>
            /// 允许的最大尺寸
            /// </summary>
            public int maxAllowedSize;

            /// <summary>
            /// 图标无着色器
            /// </summary>
            public FShader iconNoShader;
            /// <summary>
            /// 图标手持状态着色器
            /// </summary>
            public FShader iconHoldingShader;
            /// <summary>
            /// 图标可拾取候选状态着色器
            /// </summary>
            public FShader iconPickUpCandidateShader;

            /// <summary>
            /// 背景可见性模式
            /// </summary>
            public MyOptions.PartVisibilityMode backgroundVisibilityMode;
            /// <summary>
            /// 背景不透明度
            /// </summary>
            public float backgroundOpacity;

            /// <summary>
            /// 侧边线可见性模式
            /// </summary>
            public MyOptions.PartVisibilityMode sideLineVisibilityMode;
            /// <summary>
            /// 侧边线不透明度
            /// </summary>
            public float sideLineOpacity;
        }

        /// <summary>
        /// 构造函数：创建物体图标
        /// </summary>
        /// <param name="parentContainer">父级容器</param>
        /// <param name="position">位置坐标</param>
        /// <param name="forcedColor">强制颜色（可选）</param>
        /// <param name="sideLinePosition">侧边线位置</param>
        /// <param name="sideLineColor">侧边线颜色</param>
        /// <param name="settings">设置配置</param>
        public ObjectIcon(FContainer parentContainer, Vector2 position, Color? forcedColor, SideLinePosition sideLinePosition, Color sideLineColor, Settings settings)
        {
			this.container = new FContainer();
			this.position = position;
			this.forcedColor = forcedColor;
			this.sideLinePosition = sideLinePosition;
			this.sideLineColor = sideLineColor;
			this.settings = settings;

			SetupBackgroundSprite();
			SetupSideLineSprite();
			SetupIconSprite();

			parentContainer.AddChild(this.container);
		}

        /// <summary>
        /// 设置图标的精灵
        /// </summary>
        private void SetupIconSprite()
        {
            this.iconSprite = new FSprite("Futile_White");
            if (this.forcedColor != null)
            {
                this.iconSprite.color = this.forcedColor.Value;
            }
            this.iconSprite.isVisible = false;

            this.container.AddChild(this.iconSprite);
        }

        /// <summary>
        /// 根据设置初始化背景精灵
        /// </summary>
        private void SetupBackgroundSprite()
        {
            if (this.settings.backgroundVisibilityMode == MyOptions.PartVisibilityMode.Never)
            {
                return;
            }

            this.backgroundSprite = new FSprite("pixel")
            {
                scale = this.settings.maxAllowedSize,
                color = Color.black,
                alpha = this.settings.backgroundOpacity
            };

            if (this.settings.backgroundVisibilityMode == MyOptions.PartVisibilityMode.WhenAnObjectIsHeld)
            {
                this.backgroundSprite.isVisible = false;
            }

            this.container.AddChild(this.backgroundSprite);
        }

        /// <summary>
        /// 根据设置初始化侧线精灵
        /// </summary>
        private void SetupSideLineSprite()
        {
            if (this.settings.sideLineVisibilityMode == MyOptions.PartVisibilityMode.Never)
            {
                return;
            }

            this.sideLineSprite = new FSprite("pixel")
            {
                scaleX = this.settings.maxAllowedSize,
                scaleY = 2f,
                color = this.sideLineColor,
                alpha = this.settings.sideLineOpacity,
                rotation = this.sideLinePosition == SideLinePosition.Left || this.sideLinePosition == SideLinePosition.Right
                    ? 90f
                    : 0f
            };

            if (this.settings.sideLineVisibilityMode == MyOptions.PartVisibilityMode.WhenAnObjectIsHeld)
            {
                this.sideLineSprite.isVisible = false;
            }

            Vector2 posOffset;

            if (this.sideLinePosition == SideLinePosition.Left)
            {
                posOffset = new Vector2(-this.settings.maxAllowedSize / 2f + 1f, 0f);
            }
            else if (this.sideLinePosition == SideLinePosition.Right)
            {
                posOffset = new Vector2(this.settings.maxAllowedSize / 2f - 1f, 0f);
            }
            else if (this.sideLinePosition == SideLinePosition.Bottom)
            {
                posOffset = new Vector2(0f, -this.settings.maxAllowedSize / 2f + 1f);
            }
            else //if (this.sideLineSettings.position == SideLinePosition.Top)
            {
                posOffset = new Vector2(0f, this.settings.maxAllowedSize / 2f - 1f);
            }

            this.sideLineSprite.SetPosition(posOffset);

            this.container.AddChild(this.sideLineSprite);
        }

        /// <summary>
        /// 获取当前持有的对象
        /// </summary>
        public AbstractPhysicalObject? GetObject()
		{
			return this.heldObject;
		}

		/// <summary>
		/// 设置要显示的对象及其显示模式
		/// </summary>
		public void SetObject(AbstractPhysicalObject heldObject, DisplayMode displayMode)
		{
			// Object didn't change, no need to update sprite.
			// 对象没有改变，不需要更新精灵。
			if (heldObject == this.heldObject && displayMode == this.displayMode)
			{
				return;
			}

			HideSprites();
			ShowObject(heldObject, displayMode);

			this.heldObject = heldObject;
			this.displayMode = displayMode;
		}

		/// <summary>
		/// 清除所有图标和背景
		/// </summary>
		public void ClearSprites()
		{
			if (this.iconSprite != null)
			{
				this.iconSprite.RemoveFromContainer();
				this.iconSprite = null;
			}

			if (this.backgroundSprite != null)
			{
				this.backgroundSprite.RemoveFromContainer();
				this.backgroundSprite = null;
			}

			if (this.sideLineSprite != null)
			{
				this.sideLineSprite.RemoveFromContainer();
				this.sideLineSprite = null;
			}

			this.container.RemoveFromContainer();

			this.heldObject = null;
		}

		/// <summary>
		/// 获取或设置图标容器的可见性
		/// </summary>
		public bool isVisible
		{
			get { return this.container.isVisible; }
			set { this.container.isVisible = value; }
		}

		/// <summary>
		/// 获取或设置图标的位置
		/// </summary>
		public Vector2 position
		{
			get { return this.container.GetPosition() - new Vector2(0.1f, -0.1f); }

			set
			{
				// Offset position a bit to fix the sprites being 1 pixel smaller than they should be
				//偏移位置来修复精灵比实际小1像素的问题
				this.container.SetPosition(value + new Vector2(0.1f, -0.1f));
			}
		}

		/// <summary>
		/// 隐藏所有精灵
		/// </summary>
		private void HideSprites()
		{
			if (this.iconSprite != null)
			{
                this.iconSprite.isVisible = false;
            }
			if (this.backgroundSprite != null && this.settings.backgroundVisibilityMode == MyOptions.PartVisibilityMode.WhenAnObjectIsHeld)
			{
				this.backgroundSprite.isVisible = false;
			}
			if (this.sideLineSprite != null && this.settings.sideLineVisibilityMode == MyOptions.PartVisibilityMode.WhenAnObjectIsHeld)
			{
				this.sideLineSprite.isVisible = false;
			}

            if (this.iconSprite != null)
            {
                this.iconSprite.shader = this.settings.iconNoShader;
            }
		}

		/// <summary>
		/// 显示特定的对象及其模式
		/// </summary>
		private void ShowObject(AbstractPhysicalObject physObject, DisplayMode displayMode)
		{
			if (physObject == null || physObject.type == null)
			{
				return;
			}

			(var spriteName, var spriteColor) = GetSpriteNameAndColor(physObject);

			ShowSprite(spriteName, spriteColor, displayMode);
		}

		/// <summary>
		/// 根据名称和颜色显示精灵，并设置其模式
		/// </summary>
		private void ShowSprite(string spriteName, Color spriteColor, DisplayMode displayMode)
		{
			if (spriteName == null)
			{
				return;
			}

			if (this.iconSprite != null)
			{
                this.iconSprite.SetElementByName(spriteName);
                this.iconSprite.scale = 1f;
                this.iconSprite.isVisible = true;

                switch (displayMode)
                {
                    case DisplayMode.Holding:
                        this.iconSprite.shader = this.settings.iconHoldingShader;//shader 着色器
                        this.iconSprite.alpha = 1f;
                        break;
                    case DisplayMode.PickUpCandidate:
                        this.iconSprite.shader = this.settings.iconPickUpCandidateShader;
                        this.iconSprite.alpha = 0.9f;
                        break;
                }
            }

			if (this.backgroundSprite != null && this.settings.backgroundVisibilityMode == MyOptions.PartVisibilityMode.WhenAnObjectIsHeld)
			{
				this.backgroundSprite.isVisible = true;
			}

			if (this.sideLineSprite != null && this.settings.sideLineVisibilityMode == MyOptions.PartVisibilityMode.WhenAnObjectIsHeld)
			{
				this.sideLineSprite.isVisible = true;
			}

			if (this.iconSprite != null && this.forcedColor == null)
			{
				this.iconSprite.color = spriteColor;
			}

            if (this.iconSprite != null)
            {
                float? scaleFactor = CalculateSpriteScaleFactor(this.iconSprite.width, this.iconSprite.height);

                if (scaleFactor != null)
                {
                    this.iconSprite.scale = scaleFactor.Value;
                }
            }
		}

		/// <summary>
		/// 根据持有的对象获取其对应的精灵名称和颜色
		/// </summary>
		private static Tuple<string, Color>? GetSpriteNameAndColor(AbstractPhysicalObject heldObject)
		{
			if (heldObject == null)
			{
				return null;
			}

			if (heldObject.type == AbstractPhysicalObject.AbstractObjectType.Creature)
			{
				var heldCreatureIconData = CreatureSymbol.SymbolDataFromCreature(heldObject as AbstractCreature);
				return Tuple.Create(
					CreatureSymbol.SpriteNameOfCreature(heldCreatureIconData),
					CreatureSymbol.ColorOfCreature(heldCreatureIconData)
				);
			}

			if (heldObject.type == AbstractPhysicalObject.AbstractObjectType.KarmaFlower)
			{
                // Karma flower doesn't have its own icon, and needs special handling.
                // Using the small circled X sprite that shows the flower on the map.
                //业力花没有自己的图标，需要特殊处理。
                //使用在地图上显示花的小圆圈X精灵。
                return Tuple.Create("FlowerMarker", RainWorld.GoldRGB);
			}

			if (heldObject.type == AbstractPhysicalObject.AbstractObjectType.Spear && heldObject is AbstractSpear abstractSpear && abstractSpear.stuckInWall)
			{
                // ItemSymbol.SymbolDataFromItem() returns null for spears stuck in walls.
                // Handling this case the same way as ItemSymbol.SymbolDataFromItem() does for normal spears.
                // ItemSymbol.SymbolDataFromItem() 对于卡在墙上的长矛返回 null。
                // 按照 ItemSymbol.SymbolDataFromItem() 处理普通长矛的方式来处理这种情况。

                int intData = 0;
				if (ModManager.MSC && abstractSpear.hue != 0f)//颜色(Red)
                {
					intData = 3;
				}
				else if (ModManager.MSC && abstractSpear.electric)//电的
                {
					intData = 2;
				}
				else if (abstractSpear.explosive)//炸药
                {
					intData = 1;
				}

				return Tuple.Create(
					ItemSymbol.SpriteNameForItem(heldObject.type, intData),
					ItemSymbol.ColorForItem(heldObject.type, intData)
				);
			}

			var heldItemIconData = ItemSymbol.SymbolDataFromItem(heldObject);
			if (heldItemIconData != null)
			{
				return Tuple.Create(
					ItemSymbol.SpriteNameForItem(heldItemIconData.Value.itemType, heldItemIconData.Value.intData),
					ItemSymbol.ColorForItem(heldItemIconData.Value.itemType, heldItemIconData.Value.intData)
				);
			}

			Debug.Log("WhatsInMyPocket: failed to find the icon for " + heldObject.ToString());

			return Tuple.Create("Futile_White", Color.white); //Show a square if the icon wasn't found 如果找不到图标，则显示一个正方形
        }

		/// <summary>
		/// 计算精灵的缩放因子以适应给定的最小和最大尺寸
		/// </summary>
		private float? CalculateSpriteScaleFactor(float width, float height)
		{
			int? forcedSize = null;

            //If bigger side is less than min allowed size, scale up.
            //如果较长边小于允许的最小尺寸，则放大。
            if (this.settings.minAllowedSize != null && iconSprite != null && Math.Max(this.iconSprite.width, this.iconSprite.height) < this.settings.minAllowedSize.Value)
			{
				forcedSize = this.settings.minAllowedSize;
			}
            // If bigger side is greater than max allowed size, scale down.
            //如果较长边大于允许的最大尺寸，则缩小。
            else if (iconSprite != null && Math.Max(this.iconSprite.width, this.iconSprite.height) > this.settings.maxAllowedSize)
			{
				forcedSize = this.settings.maxAllowedSize;
			}

            // No need to scale.
            //无需缩放。
            if (forcedSize == null)
			{
				return null;
			}

            // The sprite needs to be scaled so its bigger side matches the forced size.
            // The bigger side is the one with smaller scale factor (in other words, the one that needs to be scaled less).
            //需要缩放精灵，使其较长边匹配强制尺寸。
            //较长边对应较小的缩放因子（换句话说，需要缩放较少的那一边）。
            var spriteScaleFactor = new Vector2(forcedSize.Value, forcedSize.Value) / new Vector2(width, height);
			var smallerScaleFactor = Math.Min(spriteScaleFactor.x, spriteScaleFactor.y);
			return smallerScaleFactor;
		}

        #region Items
        /// <summary>
        /// 容器
        /// </summary>
        private FContainer container;
        /// <summary>
        /// 图标精灵（可为空）
        /// </summary>
        private FSprite? iconSprite;
        /// <summary>
        /// 背景精灵（可为空）
        /// </summary>
        private FSprite? backgroundSprite;
        /// <summary>
        /// 侧边线精灵（可为空）
        /// </summary>
        private FSprite? sideLineSprite;
        /// <summary>
        /// 持有的物体（可为空）
        /// </summary>
        private AbstractPhysicalObject? heldObject;
        /// <summary>
        /// 显示模式
        /// </summary>
        private DisplayMode displayMode;

        /// <summary>
        /// 强制颜色（可为空，只读）
        /// </summary>
        private readonly Color? forcedColor;
        /// <summary>
        /// 侧边线位置（只读）
        /// </summary>
        private readonly SideLinePosition sideLinePosition;
        /// <summary>
        /// 侧边线颜色（只读）
        /// </summary>
        private readonly Color sideLineColor;
        /// <summary>
        /// 设置（只读）
        /// </summary>
        private readonly Settings settings;
		#endregion
    }
}