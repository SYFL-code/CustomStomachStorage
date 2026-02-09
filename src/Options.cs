using Menu.Remix.MixedUI;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
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

        Options()
        {
            //设置默认值
            StomachCapacity = config.Bind<int>($"StomachCapacity_conf_CustomStomachStorage", 3, new ConfigAcceptableRange<int>(0, 100));

            DebugMode = config.Bind<bool>($"DebugMode_conf_CustomStomachStorage", false);
        }

        public override void Initialize()
        {
            OpTab opTab = new OpTab(this, "Options");
            InGameTranslator inGameTranslator = Custom.rainWorld.inGameTranslator;
            this.Tabs = new OpTab[]
            {
                opTab
            };
            /*radioButtonGroup = new OpRadioButtonGroup(OpRadioButtonGroup_conf);
			radioButton1 = new OpRadioButton(new Vector2(10, 450));
			radioButton2 = new OpRadioButton(new Vector2(10, 420));
			radioButtonGroup.SetButtons(new OpRadioButton[] {
				radioButton1,
				radioButton2
			});*/
            //标题
            opTab.AddItems(new UIelement[]
            {
                new OpLabel(10f, 540f, inGameTranslator.Translate("Custom Stomach Storage"), true)
                {
                    alignment = FLabelAlignment.Left
                }
            });
            //选项
            opTab.AddItems(new UIelement[]
            {
                new OpTextBox(StomachCapacity, new Vector2(10, 450), 50f),
                new OpLabel(new Vector2(75f, 450f), new Vector2(200f, 24f), inGameTranslator.Translate("Stomach capacity"), FLabelAlignment.Left, false, null),

                new OpCheckBox(DebugMode, new Vector2(10, 90f)),
                new OpLabel(new Vector2(75f, 90f), new Vector2(200f, 24f), inGameTranslator.Translate("DebugMode"), FLabelAlignment.Left, false, null)

				//new OpCheckBox(OpCheckBoxStunDuration, new Vector2(10, 420)),
				/*new OpTextBox(OpCheckBoxStunDuration, new Vector2(10, 420), 50f),
                new OpLabel(new Vector2(75f, 420f), new Vector2(200f, 24f), inGameTranslator.Translate("Stun duration"), FLabelAlignment.Left, false, null),*/

				/*new OpCheckBox(OpCheckBoxSaveIceData_conf, new Vector2(10, 390)),
				new OpLabel(new Vector2(50f, 390f), new Vector2(200f, 24f), inGameTranslator.Translate("Save Ice data to the next cycle(Save bug not fixed yet)"), FLabelAlignment.Left, false, null),
				new OpCheckBox(OpCheckBoxUnlockIceShieldNum_conf, new Vector2(10, 360)),
				new OpLabel(new Vector2(50f, 360f), new Vector2(200f, 24f), inGameTranslator.Translate("Unlock the maximum number of ice shields"), FLabelAlignment.Left, false, null),*/

				//new OpLabel(new Vector2(50f, 420f), new Vector2(200f, 24f), inGameTranslator.Translate("If scavenger dies, the players continue playing"), FLabelAlignment.Left, false, null),
				/*radioButtonGroup,
				radioButton1,
				radioButton2*/
			});
        }


    }
}
