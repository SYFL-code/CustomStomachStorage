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
using System.Drawing;
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
using UnityEngine.UI;
using Watcher;
using static CustomStomachStorage.MyOptions;
using static CustomStomachStorage.Plugin;
using static CustomStomachStorage.Extended;
using static Player.ObjectGrabability;
using static SlugBase.Features.FeatureTypes;

namespace CustomStomachStorage
{
	public static class Extended
	{

		public static bool Find(List<AbstractPhysicalObject> list, AbstractPhysicalObject obj, WorldCoordinate coord)
		{
			string strObj = "";
			if (obj is AbstractCreature abstractCreature4)
			{
                if (abstractCreature4.world.GetAbstractRoom(abstractCreature4.pos.room) == null)
                {
                    abstractCreature4.pos = coord;
                }
                strObj = SaveState.AbstractCreatureToStringStoryWorld(abstractCreature4, coord);
			}
			else
			{
                strObj = obj.ToString();
			}

			if (string.IsNullOrWhiteSpace(strObj))
			{
                UDebug.Log($"[Extended.Find] Object not found: {strObj}");
                return false;
            }

			List<string> strList = new();
            foreach (AbstractPhysicalObject Item in list)
			{
				string strItem = "";
				if (Item is AbstractCreature abstractCreature5)
				{
                    if (abstractCreature5.world.GetAbstractRoom(abstractCreature5.pos.room) == null)
                    {
                        abstractCreature5.pos = coord;
                    }
                    strItem = SaveState.AbstractCreatureToStringStoryWorld(abstractCreature5, coord);
				}
				else
				{
                    strItem = Item.ToString();
				}
                strList.Add(strItem);
                if (strObj == strItem)
				{
					return true;
				}
			}
			UDebug.Log($"[Extended.Find] Object not found: {strObj}");
			for (int i = 0; i < strList.Count; i++)
			{
				UDebug.Log($">>> strList[{i}] = {strList[i]}");
			}
            return false;
		}
        public static bool Find(List<AbstractPhysicalObject> list, AbstractPhysicalObject obj)
		{
			return Find(list, obj, obj.pos);
        }


        #region TextWidth
        /*public static float GetTextWidth(string text, float scale = 1f)
		{
			if (string.IsNullOrEmpty(text)) return 0f;

			// 创建一个临时的FLabel来测量文本
			FLabel tempLabel = new FLabel(Custom.GetDisplayFont(), text);
			//tempLabel.scale = scale;

			// 获取渲染后的宽度
			float width = tempLabel.textRect.width * scale;

			// 清理临时对象
			tempLabel.RemoveFromContainer();

			UDebug.Log($"Calculated width for '{text}' at scale {scale}: {width}");
			return width;
		}*/


        public static float GetTextWidth(string text)
		{
			float width = 0f;
			foreach (char c in text)
			{
				// 中文字符和全角字符（更宽的字符）
				if (c >= 0x4e00 && c <= 0x9fff)        // CJK 统一表意文字
					width += 20f;  // 汉字宽度
				else if (c >= 0xff01 && c <= 0xff5e)   // 全角 ASCII
					width += 16f;
				else if (c >= 0x3000 && c <= 0x303f)   // CJK 标点符号
					width += 16f;
				else if (char.IsWhiteSpace(c))
					width += 8f;   // 空格
				else
					width += 9f;   // 半角英文字母/数字（比8稍宽更准确）
			}
			return width;
		}

		/// <summary>
		/// 计算选项最大宽度
		/// </summary>
		public static float CalculateMaxItemWidth(string[] items, InGameTranslator? translator = null)
		{
			float maxWidth = 0f;

			for (int i = 0; i < items.Length; i++)
			{
				string translated = translator != null ? translator.Translate(items[i]) : items[i];
				float width = GetTextWidth(translated);
				maxWidth = Mathf.Max(maxWidth, width);
			}

			return maxWidth;
		}
		#endregion

	}
}
