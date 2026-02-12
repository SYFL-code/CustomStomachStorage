using BepInEx;
using CoralBrain;
using Expedition;
using ImprovedInput;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Watcher;
using static CustomStomachStorage.Plugin;


namespace CustomStomachStorage
{
    public static class GlobalVar
    {
        //调试字符串
        public static string? dbgstr;
        //玩家变量
        public static ConditionalWeakTable<Player, PlayerVar> playerVar = new();
        //数据保存字段
        public const string ESS_savefield_name = "StomachStorage_ESS_SAVEFIELD";
        //全局系统变量
        public static RainWorldGame? _game = null;
        //最后一次的存档数据
        public static Dictionary<int, List<string>> playerStomachsDict = new Dictionary<int, List<string>>();
        //技能按键
        public static readonly PlayerKeybind Swallow = PlayerKeybind.Register($"{MOD_NAME}:Swallow", MOD_NAME, "Swallow", KeyCode.C, KeyCode.JoystickButton4);
		public static readonly PlayerKeybind Regurgitate = PlayerKeybind.Register($"{MOD_NAME}:Regurgitate", MOD_NAME, "Regurgitate", KeyCode.V, KeyCode.JoystickButton5);
        public static bool IsPressedSwallow(Player player)
        {
            return CustomInputExt.IsPressed(player, Swallow);
        }
        public static bool IsPressedRegurgitate(Player player)
        {
            return CustomInputExt.IsPressed(player, Regurgitate);
        }
        public static PlayerVar GetPlayerVar(Player player, out PlayerVar pv)
        {
            if (playerVar.TryGetValue(player, out PlayerVar pm_))
            {
                pv = pm_;
                return pv;
            }
            pv = new PlayerVar();
            playerVar.Add(player, pv);
            return pv;
        }
        public static PlayerVar GetPlayerVar(Player player)
        {
            if (playerVar.TryGetValue(player, out PlayerVar pv))
            {
                return pv;
            }
            pv = new PlayerVar();
            playerVar.Add(player, pv);
            return pv;
        }

        //挂钩
        public static void HookAdd()
        {
            Swallow.MapSuppressed = true;// 是否在地图中抑制
            Swallow.SleepSuppressed = true;// 是否在睡眠中抑制
            Regurgitate.MapSuppressed = true;// 是否在地图中抑制
            Regurgitate.SleepSuppressed = true;// 是否在睡眠中抑制

            playerStomachsDict = new Dictionary<int, List<string>>();
            _game = null;
        }
        public static void HookSubtract()
        {
            playerStomachsDict?.Clear();
            _game = null;
        }
    }
}
