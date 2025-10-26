using Newtonsoft.Json;
using System;

namespace palmesneo_village
{
    public class AnimalItem : Item
    {
        public float MovementSpeed { get; init; }
        public string[] SoundEffects { get; init; } = Array.Empty<string>();

        [JsonIgnore]
        public MTexture BodySprite = RenderManager.Pixel;

        [JsonIgnore]
        public MTexture ShadowSprite = RenderManager.Pixel;

        public override void Initialize(MTileset sourceTileset)
        {
            MTexture spriteSheet = ResourcesManager.GetTexture("Items", Name);

            int singleSpriteWidth = spriteSheet.Width / 2;
            int singleSpriteHeight = spriteSheet.Height;

            BodySprite = new MTexture(spriteSheet, 0, 0, singleSpriteWidth, singleSpriteHeight);

            ShadowSprite = new MTexture(spriteSheet, singleSpriteWidth, 0, singleSpriteWidth, singleSpriteHeight);

            Icon = BodySprite;
        }
    }
}
