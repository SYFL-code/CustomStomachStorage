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

        /*// Rain World 实际可用的字体名（通过反编译或文档确认）
        private const string DEFAULT_FONT = "DisplayFont";  // 或 "DisplayFont"

        public static float GetTextWidth(string text, string? fontName = null, float? fontSize = null)
        {
            if (string.IsNullOrEmpty(text)) return 0f;

            // 安全获取字体名
            string font = fontName ?? DEFAULT_FONT;
            float scale = (fontSize ?? 12f) / 12f;

            try
            {
                var tempLabel = new FLabel(font, text)
                {
                    scale = scale
                };

                float width = tempLabel.textRect.width;

                // 清理
                tempLabel.RemoveFromContainer();
                if (tempLabel.container == null)
                {
                    // 确保从 Futile 舞台移除
                    Futile.stage.RemoveChild(tempLabel);
                }

                return width;
            }
            catch (FutileException ex)
            {
                UDebug.LogError($"字体 '{font}' 不存在: {ex.Message}");
                // 回退到估算
                return EstimateWidth(text, scale);
            }
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
        /// 计算选项最大宽度（你的原始需求）
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


    }
}
