using Microsoft.Xna.Framework;
using System;

namespace palmesneo_village
{
    public class WindowBuilding : Building
    {
        // Менять спрайт окна в зависимости от времени суток
        private TimeOfDayManager timeOfDayManager;

        private WindowItem windowItem;

        public WindowBuilding(GameLocation gameLocation, WindowItem item, Direction direction, Vector2[,] occupiedTiles) 
            : base(gameLocation, item, direction, occupiedTiles)
        {
            windowItem = item;
        }

        public override void Update()
        {
            if(timeOfDayManager == null)
            {
                timeOfDayManager = Engine.CurrentScene.MasterEntity.GetChildByName<TimeOfDayManager>("time_of_day_manager");
            }
            else
            {
                if (timeOfDayManager.CurrentHour >= 18)
                {
                    Sprite.Texture = windowItem.NightTexture;
                }
                else
                {
                    Sprite.Texture = windowItem.DayTexture;
                }
            }

            base.Update();
        }

    }
}
