namespace palmesneo_village
{
    public class CreatureTemplate
    {
        public string Name { get; set; }

        public MTexture Texture { get; set; }

        public float MovementSpeed { get; set; }

        public CreatureTemplate(string name, MTexture texture, float movementSpeed)
        {
            Name = name;
            Texture = texture;
            MovementSpeed = movementSpeed;
        }
    }
}
