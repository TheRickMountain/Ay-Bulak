using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class GrassItem : BuildingItem
    {
        public int Variations { get; init; }

        private MTexture[] variationsTextures;

        public override void Initialize(MTileset sourceTileset)
        {
            MTexture texture = ResourcesManager.GetTexture("Items", Name);

            variationsTextures = new MTexture[Variations];

            int textureWidth = texture.Width / Variations;
            int textureHeight = texture.Height;

            for (int i = 0; i < Variations; i++)
            {
                variationsTextures[i] = new MTexture(texture, new Rectangle(textureWidth * i, 0, textureWidth, textureHeight));
            }

            DirectionTexture[Direction.Down] = variationsTextures[0];

            Icon = variationsTextures[0];
        }

        public MTexture GetRandomVariationTexture()
        {
            return Calc.Random.Choose(variationsTextures);
        }

    }
}
