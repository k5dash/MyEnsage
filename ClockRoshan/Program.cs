using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;

namespace SlarkAnnihilation
{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    internal static class Program
    {
        private static bool _loaded;
        private static readonly Menu Menu = new Menu("Clock Roshan", "roshan", true);

        private static void Main()
        {
            Game.OnUpdate+=Game_OnUpdate;
            Drawing.OnDraw+=Drawing_OnDraw;

            Menu.AddItem(new MenuItem("Enabled", "Enabled").SetValue(true));
            Menu.AddItem(new MenuItem("uphill", "Up Hill Hotkey").SetValue(new KeyBind('X', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("downhill", "Down Hill Hotkey").SetValue(new KeyBind('C', KeyBindType.Press)));

            Menu.AddToMainMenu();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!_loaded) return;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            var me = ObjectMgr.LocalHero;

            if (!_loaded)
            {
                if (!Game.IsInGame || me == null)
                {
                    return;
                }
                _loaded = true;
                Game.PrintMessage(
                    "<font face='Comic Sans MS, cursive'><font color='#00aaff'>" + Menu.DisplayName + " By k5dash" +
                    " loaded!</font> <font color='#aa0000'>v" + Assembly.GetExecutingAssembly().GetName().Version, MessageType.LogMessage);
            }

            if (!Game.IsInGame || me == null)
            {
                _loaded = false;
                return;
            }
            if (!Menu.Item("Enabled").GetValue<bool>()||!me.IsAlive || Game.IsPaused) return;

            if (Menu.Item("uphill").GetValue<KeyBind>().Active)
            {
                pushRoshanUpHill(me);
                return;
            }

            if (Menu.Item("downhill").GetValue<KeyBind>().Active)
            {
                pushRoshanDownHill(me);
                return;
            }
        }

        private static void pushRoshanUpHill(Hero me){
            Game.PrintMessage("X:"+me.Position.X+",Y:" +me.Position.Y+",Z:"+me.Position.Z, MessageType.LogMessage);

            var darkPact = me.Spellbook.SpellQ;
            var pounce = me.Spellbook.SpellW;
            var rocket = me.Spellbook.SpellE;
            var shadowDance = me.Spellbook.SpellR;

            me.Move(new Vector3(3872,-2336,384));
            DelayAction.Add(
            1500, 
            () =>
                {
                    rocket.UseAbility(new Vector3(3884,-1997,0));
                });

            DelayAction.Add(
            5000, 
            () =>
                {
                    shadowDance.UseAbility(new Vector3(4038,-1766,0));
            });
            DelayAction.Add(
            5200, 
            () =>
                {
                    me.Move(new Vector3(3981.969f,-1904.844f,159.9688f));
            });
            DelayAction.Add(
            6200, 
            () =>
                {
                    pounce.UseAbility();
            });
        }

        private static void pushRoshanDownHill(Hero me){

            var darkPact = me.Spellbook.SpellQ;
            var pounce = me.Spellbook.SpellW;
            var rocket = me.Spellbook.SpellE;
            var shadowDance = me.Spellbook.SpellR;
            me.Move(new Vector3(4064f,-2272,384));
            DelayAction.Add(
            2000, 
            () =>
                {
                    me.Move(new Vector3(4290.717f,-2336,384));
                });

            
            DelayAction.Add(
            3500, 
            () =>
                {
                    pounce.UseAbility();
                });
        }
    }
}
