using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public enum ToolType
    {
        None,
        Axe,
        WateringCan,
        Showel,
        Scythe,
        FishingRod
    }

    public class ToolItem : Item
    {
        public ToolType ToolType { get; init; }
        public int Efficiency { get; init; }
        public int Capacity { get; init; }
        public string[] SoundEffects { get; init; }

        public void PlaySoundEffect()
        {
            if(SoundEffects == null || SoundEffects.Length == 0)
            {
                return;
            }

            string soundEffect = Calc.Random.Choose(SoundEffects);
            ResourcesManager.GetSoundEffect(soundEffect).Play();
        }
    }
}
