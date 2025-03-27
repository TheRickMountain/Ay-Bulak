using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class BirdNestItem : BuildingItem
    {
        public int Stages { get; private set; } = 4;

        private MTexture[] stagesTextures;

        public override void Initialize(MTileset sourceTileset)
        {
            MTexture texture = ResourcesManager.GetTexture("Items", Name);

            stagesTextures = new MTexture[Stages];

            int textureWidth = texture.Width / Stages;
            int textureHeight = texture.Height;

            for (int i = 0; i < Stages; i++)
            {
                stagesTextures[i] = new MTexture(texture, new Rectangle(textureWidth * i, 0, textureWidth, textureHeight));
            }

            DirectionIcon[Direction.Down] = stagesTextures[0];

            Icon = stagesTextures[0];
        }

        public MTexture GetStageTexture(int stage)
        {
            return stagesTextures[stage];
        }

    }
}
