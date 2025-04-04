using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class CalendarUI : ImageUI
    {

        private TimeOfDayManager timeOfDayManager;

        private TextUI dayText;
        private TextUI weekDayText;

        public CalendarUI(TimeOfDayManager timeOfDayManager)
        {
            this.timeOfDayManager = timeOfDayManager;

            Texture = ResourcesManager.GetTexture("Sprites", "UI", "calendar");
            Size = Texture.Size;

            dayText = new TextUI();
            dayText.SelfColor = Color.Black;
            dayText.Anchor = Anchor.Center;
            dayText.LocalScale = new Vector2(2, 2);
            dayText.LocalPosition = new Vector2(0, -5);
            AddChild(dayText);

            weekDayText = new TextUI();
            weekDayText.SelfColor = Color.Black;
            weekDayText.Anchor = Anchor.BottomCenter;
            weekDayText.LocalPosition = new Vector2(0, -7);
            AddChild(weekDayText);

            timeOfDayManager.DayChanged += OnDayChanged;

            OnDayChanged(timeOfDayManager.CurrentDay);
        }

        private void OnDayChanged(int day)
        {
            dayText.Text = day.ToString();
            dayText.Origin = dayText.Size / 2;

            int weekDay = day % 7;

            weekDayText.Text = LocalizationManager.GetText($"weekday_{weekDay}_short");
            weekDayText.Origin = weekDayText.Size / 2;

            // Воскресенье
            if(weekDay == 0)
            {
                dayText.SelfColor = Color.IndianRed;
                weekDayText.SelfColor = Color.IndianRed;
            }
            else
            {
                dayText.SelfColor = Color.Black;
                weekDayText.SelfColor = Color.Black;
            }
        }

    }
}
