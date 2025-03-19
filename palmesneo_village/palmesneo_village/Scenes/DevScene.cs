using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class DevScene : Scene
    {
        private TraderUI traderUI;

        public override void Begin()
        {
            traderUI = new TraderUI();
            traderUI.Anchor = Anchor.Center;
            MasterUIEntity.AddChild(traderUI);

            base.Begin();
        }

        public override void Update()
        {
            if (MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.T))
            {
                traderUI.Open(new List<Item>()
                {
                    Engine.ItemsDatabase.GetItemByName("wood"),
                    Engine.ItemsDatabase.GetItemByName("stone"),
                    Engine.ItemsDatabase.GetItemByName("tomato_seeds"),
                    Engine.ItemsDatabase.GetItemByName("eggplant_seeds"),
                    Engine.ItemsDatabase.GetItemByName("garlic_seeds"),
                    Engine.ItemsDatabase.GetItemByName("beetroot_seeds"),
                    Engine.ItemsDatabase.GetItemByName("cabbage_seeds"),
                    Engine.ItemsDatabase.GetItemByName("carrot_seeds"),
                });
            }

            base.Update();
        }


    }
}
