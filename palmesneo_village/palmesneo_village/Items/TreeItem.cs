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
        public string StumpName { get; init; }

        public MTexture TrunkTexture { get; private set; }

        [JsonIgnore]
        public MTexture[] GrowthStagesTextures { get; private set; }

        public override void Initialize(MTileset sourceTileset)
        {
            base.Initialize(sourceTileset);

            MTexture texture = ResourcesManager.GetTexture("Items", Name);

            int growthStages = GrowthStagesData.Length;

            // Нужно брать на +1 больше, так как последний спрайт в текстуре - это ствол дерева
            int textureSpritesAmount = growthStages + 1;

            int textureWidth = texture.Width / textureSpritesAmount;
            int textureHeight = texture.Height;

            GrowthStagesTextures = new MTexture[growthStages];

            for (int i = 0; i < growthStages; i++)
            {
                GrowthStagesTextures[i] = new MTexture(texture, i * textureWidth, 0, textureWidth, textureHeight);
            }

            TrunkTexture = new MTexture(texture, (textureSpritesAmount - 1) * textureWidth, 0, textureWidth, textureHeight);

            DirectionTexture[Direction.Down] = GrowthStagesTextures[0];

            Icon = DirectionTexture[Direction.Down];
        }
    }
}
