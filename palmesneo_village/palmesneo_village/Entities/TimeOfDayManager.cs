using System;

namespace palmesneo_village
{
    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }

    public enum Weather
    {
        Sun,
        Rain,
        Snow
    }

    public class TimeOfDayManager : Entity
    {
        public const int MINUTES_PER_HOUR = 30;
        public const int HOURS_PER_CYCLE = 24;
        public const int MINUTES_PER_DAY = MINUTES_PER_HOUR * HOURS_PER_CYCLE;
        public const int INITIAL_HOUR = 5;
        public const int DAYS_IN_THE_SEASON = 28;

        public Season CurrentSeason { get; private set; } = Season.Spring;
        public Weather CurrentWeather { get; private set; } = Weather.Sun;

        public int CurrentDay { get => day; }
        public int CurrentHour { get => hour; }
     
        private GradientTexture1D currentDayGradient;

        public Action<int> DayChanged { get; set; }
        public Action<Season> SeasonChanged { get; set; }

        private TimeOfDayGradients timeOfDayGradients;

        private float totalMinutes = INITIAL_HOUR * MINUTES_PER_HOUR;

        private float currentDayMinutes = 0;
        private float cycleProgress = 0;

        private int day = 1;
        private int hour = 0;
        private int minute = 0;

        public TimeOfDayManager()
        {
            timeOfDayGradients = new TimeOfDayGradients();

            currentDayGradient = timeOfDayGradients.GetGradientFor(CurrentWeather);
        }

        public override void Update()
        {
            base.Update();

            totalMinutes += Engine.GameDeltaTime;

            currentDayMinutes = totalMinutes % MINUTES_PER_DAY;

            hour = (int)(currentDayMinutes / MINUTES_PER_HOUR);

            minute = (int)(currentDayMinutes % MINUTES_PER_HOUR);

            cycleProgress = currentDayMinutes / MINUTES_PER_DAY;

            Engine.Penumbra.AmbientColor = currentDayGradient.GetColor(cycleProgress);
        }

        public void StartNextDay()
        {
            totalMinutes = INITIAL_HOUR * MINUTES_PER_HOUR;

            day++;

            if (day == DAYS_IN_THE_SEASON + 1)
            {
                day = 1;

                CurrentSeason = CurrentSeason.Next();

                SeasonChanged?.Invoke(CurrentSeason);
            }

            if (CurrentSeason == Season.Winter)
            {
                if (day == 1)
                {
                    CurrentWeather = Weather.Snow;
                }
                else
                {
                    CurrentWeather = Calc.Random.Chance(0.2f) ? Weather.Snow : Weather.Sun;
                }
            }
            else
            {
                if (day == 1)
                {
                    CurrentWeather = Weather.Sun;
                }
                else
                {
                    CurrentWeather = Calc.Random.Chance(0.2f) ? Weather.Rain : Weather.Sun;
                }
            }

            currentDayGradient = timeOfDayGradients.GetGradientFor(CurrentWeather);

            DayChanged?.Invoke(day);
        }

        public string GetTimeString()
        {
            int changedMinute = (60 * minute) / MINUTES_PER_HOUR;

            return $"{hour:00}:{changedMinute:00}";
        }
    }
}
