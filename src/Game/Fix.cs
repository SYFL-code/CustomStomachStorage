using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomStomachStorage
{
    public static class Fix
    {
        private static FieldInfo? _abstractedField;

        public static void HookAdd()
        {
            On.AbstractCreature.Abstractize += AbstractCreature_Abstractize;

            _abstractedField = typeof(AbstractCreature).GetField("abstracted",
                BindingFlags.NonPublic | BindingFlags.Instance);
        }
        public static void HookSubtract()
        {
            On.AbstractCreature.Abstractize -= AbstractCreature_Abstractize;
        }

        private static void AbstractCreature_Abstractize(
            On.AbstractCreature.orig_Abstractize orig,
            AbstractCreature abstC,
            WorldCoordinate coord)
        {
            try
            {
                // 修复null AbstractAI
                if (abstC.abstractAI == null && abstC.Room != null)
                {
                    abstC.abstractAI = new AbstractCreatureAI(abstC.Room.world, abstC);
                }

                // 修复null Room
                if (abstC.Room == null && coord.room != -1)
                {
                    abstC.Move(coord);
                }

                orig(abstC, coord);
            }
            catch (NullReferenceException)
            {
                // 只用反射标记为已抽象化

                _abstractedField?.SetValue(abstC, true);

                UDebug.Log($"[Fix] 阻止了 AbstractCreature {abstC} 的崩溃");
            }
        }
    }
}
