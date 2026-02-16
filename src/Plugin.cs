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
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Watcher;
using static SlugBase.Features.FeatureTypes;
using static Player.ObjectGrabability;
using static CustomStomachStorage.Plugin;
//using UDebug =  UnityEngine.Debug;


namespace CustomStomachStorage
{
	[BepInPlugin(MOD_ID, MOD_NAME, MOD_version)]
    [BepInDependency(WHATS_IN_MY_POCKET_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    class Plugin : BaseUnityPlugin
	{
        public const string MOD_NAME = "Custom Stomach Storage";
        public const string MOD_name = "CustomStomachStorage";
        public const string MOD_ID = "CustomStomachStorage.Redlyn";
        public const string MOD_version = "0.1.1";

        public const string WHATS_IN_MY_POCKET_GUID = "Jimarad.WhatsInMyPocket";


        // Add hooks-添加钩子
        public void OnEnable()
		{
            GlobalVar.HookAdd();

			On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            // Put your custom hooks here!-在此放置你自己的钩子

            //On.SaveState.BringUpToDate += SaveState_BringUpToDate;
            //On.Player.SaveStomachObjectInPlayerState += Player_SaveStomachObjectInPlayerState;
            MyPlayer.HookAdd();
            //On.Player.Update += Player_Update;
            //On.Player.Jump += Player_Jump;    
            //在玩家触发跳跃时执行Player_Jump
            //On.Player.Die += Player_Die;
            //On.Lizard.ctor += Lizard_ctor;

            State.HookAdd();
			Fix.HookAdd();

            StomachHUD.HookAdd();
        }


		// add this to do the opposite of whatever you did in OnEnable()
		//添加此命令以执行与您在Enable()中所做的相反的操作
		// otherwise you'll wind up with two methods being called
		//否则，您将会调用两个方法
		public void OnDisable()
		{
			On.RainWorld.OnModsInit -= Extras.WrapInit(LoadResources);

			// Put your custom hooks here!-在此放置你自己的钩子
			MyPlayer.HookSubtract();
			//On.SaveState.BringUpToDate -= SaveState_BringUpToDate;
			//On.Player.SaveStomachObjectInPlayerState -= Player_SaveStomachObjectInPlayerState;

			//On.Player.Update -= Player_Update;
			//On.Player.Jump -= Player_Jump;
			//On.Player.Die -= Player_Die;
			//On.Lizard.ctor -= Lizard_ctor;

			State.HookSubtract();
			Fix.HookSubtract();

            StomachHUD.HookSubtract();

            GlobalVar.HookSubtract();
        }


		// Load any resources, such as sprites or sounds-加载任何资源 包括图像素材和音效
		private void LoadResources(RainWorld rainWorld)
		{
			MachineConnector.SetRegisteredOI(MOD_ID, MyOptions.Instance);
		}




	}
}