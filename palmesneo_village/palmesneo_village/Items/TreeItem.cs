using Newtonsoft.Json;

namespace palmesneo_village
{
    public class TreeItem : BuildingItem
    {
        public int GrowthStages { get; init; }
        public int GrowthRateInDays { get; init; }

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

            DirectionIcon[Direction.Down] = GrowthStagesTextures[0];

            Icon = DirectionIcon[Direction.Down];
        }
    }
}
