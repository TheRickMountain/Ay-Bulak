using Newtonsoft.Json;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class GrowthStageData
    {
        public int Strength { get; set; }
        public Dictionary<string, int> Loot { get; set; }
    }

    public class TreeItem : BuildingItem
    {
        public int GrowthRateInDays { get; init; }
        public GrowthStageData[] GrowthStagesData { get; init; }

        [JsonIgnore]
        public MTexture[] GrowthStagesTextures { get; private set; }

        public override void Initialize(MTileset sourceTileset)
        {
            base.Initialize(sourceTileset);

            MTexture texture = ResourcesManager.GetTexture("Items", Name);

            int growthStages = GrowthStagesData.Length;

            int textureWidth = texture.Width / growthStages;
            int textureHeight = texture.Height;

            GrowthStagesTextures = new MTexture[growthStages];

            for (int i = 0; i < growthStages; i++)
            {
                GrowthStagesTextures[i] = new MTexture(texture, i * textureWidth, 0, textureWidth, textureHeight);
            }

            DirectionIcon[Direction.Down] = GrowthStagesTextures[0];

            Icon = DirectionIcon[Direction.Down];
        }
    }
}
