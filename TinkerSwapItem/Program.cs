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
        private static readonly Menu Menu = new Menu("TinkerSwapItem", "Tinker", true);
        private static Hero _globalTarget;
        private static readonly List<string> Items = new List<string>
        {
            "item_dagon_5",
            "item_ethereal_blade",
            "item_ultimate_scepter",
            "item_aether_lens",
            "item_cyclone",
            "item_sheepstick",
            "item_veil_of_discord",
            "item_dagon_4",
            "item_dagon_3",
            "item_dagon_2",
            "item_dagon_1"
        };
        public static Dictionary<string, int>itemsInOrder = new Dictionary<string, int>();
        private static Dictionary<Item,ItemSlot> ItemSlots = new Dictionary<Item,ItemSlot>();

        private static void Main()
        {
            Game.OnUpdate+=Game_OnUpdate;
            Drawing.OnDraw+=Drawing_OnDraw;

            var dict=new Dictionary<string,bool>
            {
                {Items[0],true},
                {Items[1],true},
                {Items[2],true},
                {Items[3],true},
                {Items[4],true},
                {Items[5],true},
                {Items[6],true},
            };

            Menu.AddItem(new MenuItem("Enabled", "Enabled").SetValue(true));
            Menu.AddItem(new MenuItem("drop", "Drop").SetValue(new KeyBind('X', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("pick", "Pick").SetValue(new KeyBind('C', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(dict)));

            Menu.AddToMainMenu();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!_loaded) return;
            if (_globalTarget == null || !_globalTarget.IsAlive) return;
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

            if (Menu.Item("drop").GetValue<KeyBind>().Active)
            {
                if (Utils.SleepCheck("stop"))
                {   me.Stop();
                    Utils.Sleep(200, "stop");  
                }

                StashItem(me);
                return;
            }

            if (Menu.Item("pick").GetValue<KeyBind>().Active)
            {
                if (Utils.SleepCheck("stop"))
                {   me.Stop();
                    Utils.Sleep(200, "stop");  
                }

                GetItem(me);
            }

        }

        public static void GetItem(Hero me){
            if (Utils.SleepCheck("pickitems"))
            {   
                var droppedItems =
                ObjectManager.GetEntities<PhysicalItem>().Where(x => x.Distance2D(me) < 250).Reverse().ToList();
                foreach (var s in droppedItems)
                {   
                    me.PickUpItem(s, true); 
                }      

                var currentItemOrder =
                    me.Inventory.Items.Where(
                        x =>
                            (Items.Contains(x.Name) || x.Name.IndexOf("dagon")!=-1)&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(x.Name) &&
                            Utils.SleepCheck(x.Name)).ToList();

                foreach (var item in currentItemOrder){
                    if(Items.Contains(item.Name) && ItemSlots.ContainsKey(item)){
                        item.MoveItem(ItemSlots[item]);
                    }
                }
                itemsInOrder = new Dictionary<string, int>();
                ItemSlots = new Dictionary<Item,ItemSlot>();
                Utils.Sleep(200, "pickitems");

            }
        }


        public static void StashItem(Hero me){
            if (Utils.SleepCheck("dropitems"))
            {   
                for (var i = 0; i < 6; i++) {
                    var currentSlot = (ItemSlot) i;
                    var currentItem = me.Inventory.GetItem(currentSlot);

                    if (currentItem == null || !Items.Contains(currentItem.Name)) continue;
                    ItemSlots.Add(currentItem,currentSlot);
                    me.DropItem(currentItem, me.NetworkPosition, true);
                }
                Utils.Sleep(200, "dropitems");
            }
        }
    }
}
