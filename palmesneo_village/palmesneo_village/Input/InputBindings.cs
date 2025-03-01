using Microsoft.Xna.Framework.Input;

namespace palmesneo_village
{
    public static class InputBindings
    {

        public static VirtualIntegerAxis MoveHorizontally { get; private set; }
        public static VirtualIntegerAxis MoveVertically{ get; private set; }

        public static VirtualButton Rotate { get; private set; }

        public static VirtualButton Pause { get; private set; }

        public static void Initialize()
        {
            MoveHorizontally = new VirtualIntegerAxis();
            MoveHorizontally.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeOlder, Keys.A, Keys.D));
            MoveHorizontally.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeOlder, Keys.Left, Keys.Right));

            MoveVertically = new VirtualIntegerAxis();
            MoveVertically.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeOlder, Keys.W, Keys.S));
            MoveVertically.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeOlder, Keys.Up, Keys.Down));

            Rotate = new VirtualButton();
            Rotate.Nodes.Add(new VirtualButton.KeyboardKey(Keys.R));

            Pause = new VirtualButton();
            Pause.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Space));
        }

    }
}
