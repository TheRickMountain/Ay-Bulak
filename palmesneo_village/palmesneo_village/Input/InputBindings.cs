using Microsoft.Xna.Framework.Input;

namespace palmesneo_village
{
    public static class InputBindings
    {

        public static VirtualIntegerAxis MoveHorizontally { get; private set; }
        public static VirtualIntegerAxis MoveVertically{ get; private set; }

        public static VirtualButton Rotate { get; private set; }

        public static VirtualButton Pause { get; private set; }
        public static VirtualButton Exit { get; private set; }
        public static VirtualButton Accept { get; private set; }
        public static VirtualButton Next { get; private set; }
        public static VirtualButton Previous { get; private set; }

        public static void Initialize()
        {
            MoveHorizontally = new VirtualIntegerAxis();
            MoveHorizontally.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeOlder, Keys.A, Keys.D));
            MoveHorizontally.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeOlder, Keys.Left, Keys.Right));
            MoveHorizontally.Nodes.Add(new VirtualAxis.PadDpadLeftRight(0));
            MoveHorizontally.Nodes.Add(new VirtualAxis.PadLeftStickX(0, 0.0f));

            MoveVertically = new VirtualIntegerAxis();
            MoveVertically.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeOlder, Keys.W, Keys.S));
            MoveVertically.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeOlder, Keys.Up, Keys.Down));
            MoveVertically.Nodes.Add(new VirtualAxis.PadDpadUpDown(0));
            MoveVertically.Nodes.Add(new VirtualAxis.PadLeftStickY(0, 0.0f));

            Rotate = new VirtualButton();
            Rotate.Nodes.Add(new VirtualButton.KeyboardKey(Keys.R));

            Pause = new VirtualButton();
            Pause.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Space));

            Exit = new VirtualButton();
            Exit.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Escape));

            Accept = new VirtualButton();
            Accept.Nodes.Add(new VirtualButton.KeyboardKey(Keys.E));
            Accept.Nodes.Add(new VirtualButton.MouseLeftButton());
            Accept.Nodes.Add(new VirtualButton.PadButton(0, Buttons.A));

            Previous = new VirtualButton();
            Previous.Nodes.Add(new VirtualButton.PadButton(0, Buttons.LeftShoulder));

            Next = new VirtualButton();
            Next.Nodes.Add(new VirtualButton.PadButton(0, Buttons.RightShoulder));
        }

    }
}
