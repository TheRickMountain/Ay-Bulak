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

        [JsonIgnore]
        public Dictionary<Season, MTexture> TrunkTextures { get; private set; }

        [JsonIgnore]
        public Dictionary<Season, MTexture[]> GrowthStagesTextures { get; private set; }

        public override void Initialize(MTileset sourceTileset)
        {
            base.Initialize(sourceTileset);

            MTexture texture = ResourcesManager.GetTexture("Items", Name);

            int growthStages = GrowthStagesData.Length;

            // Нужно брать на +1 больше, так как последний спрайт в текстуре - это ствол дерева
            int textureSpritesAmount = growthStages + 1;

            int seasonsAmount = 4;

            int textureRegionWidth = texture.Width / textureSpritesAmount;
            int textureRegionHeight = texture.Height / seasonsAmount;

            MTileset tileset = new MTileset(texture, textureRegionWidth, textureRegionHeight);

            GrowthStagesTextures = new Dictionary<Season, MTexture[]>
            {
                { Season.Spring, new MTexture[growthStages] },
                { Season.Summer, new MTexture[growthStages] },
                { Season.Autumn, new MTexture[growthStages] },
                { Season.Winter, new MTexture[growthStages] }
            };

            for (int i = 0; i < growthStages; i++)
            {
                GrowthStagesTextures[Season.Spring][i] = tileset[i, 0];
                GrowthStagesTextures[Season.Summer][i] = tileset[i, 1];
                GrowthStagesTextures[Season.Autumn][i] = tileset[i, 2];
                GrowthStagesTextures[Season.Winter][i] = tileset[i, 3];
            }

            TrunkTextures = new Dictionary<Season, MTexture>()
            {
                {Season.Spring,  tileset[textureSpritesAmount - 1, 0]},
                {Season.Summer,  tileset[textureSpritesAmount - 1, 1]},
                {Season.Autumn,  tileset[textureSpritesAmount - 1, 2]},
                {Season.Winter,  tileset[textureSpritesAmount - 1, 3]}
            };

            DirectionTexture[Direction.Down] = GrowthStagesTextures[Season.Spring][0];

            Icon = DirectionTexture[Direction.Down];
        }
    }
}
