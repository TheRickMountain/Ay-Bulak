using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class DevScene : Scene
    {

        public override void Begin()
        {
            base.Begin();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Render()
        {
            base.Render();

            // Debug lines
            float lineThickness = 1.0f / Engine.Camera.Zoom.X;
            RenderManager.Line(new Vector2(0, -10000), new Vector2(0, 10000), Color.Red * 0.5f, lineThickness);
            RenderManager.Line(new Vector2(-10000, 0), new Vector2(10000, 0), Color.Green * 0.5f, lineThickness);
        }


    }
}
