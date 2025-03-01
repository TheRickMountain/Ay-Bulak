using Microsoft.Xna.Framework;

namespace palmesneo_village
{
    public class Player : Creature
    {
        public Player(CreatureTemplate creatureTemplate) : base(creatureTemplate)
        {
            
        }

        public override void Update()
        {
            base.Update();

            Vector2 movement = new Vector2(InputBindings.MoveHorizontally.Value, InputBindings.MoveVertically.Value);

            if(movement != Vector2.Zero)
            {
                movement.Normalize();

                LocalPosition += movement * Speed * Engine.GameDeltaTime;
            }
        }
    }
}
