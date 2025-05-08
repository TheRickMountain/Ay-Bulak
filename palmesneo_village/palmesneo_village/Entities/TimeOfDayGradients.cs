using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class TimeOfDayGradients
    {
        public GradientTexture1D SunnyDayGradient { get; private set; }
        public GradientTexture1D RainyDayGradient { get; private set; }

        public TimeOfDayGradients()
        {
            Color nightColor = new Color(63, 137, 255);
            Color sunriseColor = new Color(203, 178, 254);
            Color dayColor = Color.White;
            Color rainyDayColor = new Color(150, 150, 180);

            List<ColorPoint> sunnyColorPoints = new List<ColorPoint>()
            {
                new ColorPoint(0, sunriseColor),
                new ColorPoint(0.25f, sunriseColor),
                new ColorPoint(0.3f, dayColor),
                new ColorPoint(0.7f,  dayColor),
                new ColorPoint(0.75f, dayColor),
                new ColorPoint(0.9f, sunriseColor),
                new ColorPoint(1.0f, nightColor),
            };

            SunnyDayGradient = new GradientTexture1D(512, sunnyColorPoints, InterpolationType.Cubic);

            List<ColorPoint> rainyColorPoints = new List<ColorPoint>()
            {
                new ColorPoint(0, nightColor),
                new ColorPoint(0.1f, nightColor),
                new ColorPoint(0.25f, rainyDayColor),
                new ColorPoint(0.75f, rainyDayColor),
                new ColorPoint(0.9f, nightColor),
                new ColorPoint(1.0f, nightColor),
            };

            RainyDayGradient = new GradientTexture1D(512, rainyColorPoints, InterpolationType.Cubic);
        }
    }
}
