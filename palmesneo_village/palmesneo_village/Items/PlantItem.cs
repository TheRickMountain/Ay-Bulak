using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class PlantItem : BuildingItem
    {
        public int GrowthStages { get; init; }
        public int GrowthRateInDays { get; init; }
        public bool RemoveAfterHarvest { get; init; }
        public string HarvestItem { get; init; }
        public int HarvestAmount { get; init; }

        [JsonIgnore]
        public MTexture[] GrowthStagesTextures { get; private set; }

        public override void Initialize(MTileset sourceTileset)
        {
            base.Initialize(sourceTileset);

            MTexture texture = ResourcesManager.GetTexture("Items", Name);

            int textureWidth = texture.Width / GrowthStages;
            int textureHeight = texture.Height;

            GrowthStagesTextures = new MTexture[GrowthStages];

            for (int i = 0; i < GrowthStages; i++)
            {
                GrowthStagesTextures[i] = new MTexture(texture, i * textureWidth, 0, textureWidth, textureHeight);
            }

            DirectionTexture[Direction.Down] = GrowthStagesTextures[0];

            Icon = DirectionTexture[Direction.Down];
        }

    }
}
