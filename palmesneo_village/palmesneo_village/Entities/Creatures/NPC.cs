using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class NPC : Creature
    {

        private SpriteEntity bodySprite;

        public NPC(string name, MTexture texture, float speed) 
            : base(name, texture, speed)
        {
            // TODO: перестать использовать BodyImage
            BodyImage.IsVisible = false;

            CreateAndInitializeBodySprite(texture);
        }

        private void CreateAndInitializeBodySprite(MTexture spritesheet)
        {
            int framesColumns = 4;
            int framesRows = 4;

            int frameWidth = spritesheet.Width / framesColumns;
            int frameHeight = spritesheet.Height / framesRows;

            bodySprite = new SpriteEntity();
            bodySprite.AddAnimation("idle_down", new Animation(spritesheet, 1, 0, frameWidth, frameHeight, 0, 0));
            bodySprite.AddAnimation("idle_left", new Animation(spritesheet, 1, 0, frameWidth, frameHeight, 0, frameHeight));
            bodySprite.AddAnimation("idle_up", new Animation(spritesheet, 1, 0, frameWidth, frameHeight, 0, frameHeight * 2));
            bodySprite.AddAnimation("idle_right", new Animation(spritesheet, 1, 0, frameWidth, frameHeight, 0, frameHeight * 3));

            bodySprite.AddAnimation("walk_down", new Animation(spritesheet, 4, 0, frameWidth, frameHeight, 0, 0));
            bodySprite.AddAnimation("walk_left", new Animation(spritesheet, 4, 0, frameWidth, frameHeight, 0, frameHeight));
            bodySprite.AddAnimation("walk_up", new Animation(spritesheet, 4, 0, frameWidth, frameHeight, 0, frameHeight * 2));
            bodySprite.AddAnimation("walk_right", new Animation(spritesheet, 4, 0, frameWidth, frameHeight, 0, frameHeight * 3));
            AddChild(bodySprite);

            bodySprite.LocalPosition = new Vector2(-frameWidth / 2, -(frameHeight - (Engine.TILE_SIZE / 2)));

            bodySprite.Play("idle_down");
        }

        public override IEnumerable<InteractionData> GetAvailableInteractions(Inventory inventory)
        {
            yield return new InteractionData(
                InteractionType.Talk, 
                ResourcesManager.GetTexture("Sprites", "talk_icon"), 
                LocalizationManager.GetText("talk"), 
                true);
        }

        public override void Interact(InteractionData interactionData, Inventory inventory)
        {
            switch(interactionData.InteractionType)
            {
                case InteractionType.Talk:
                    {
                        // TODO: open dialog UI
                    }
                    break;
            }
        }
    }
}
