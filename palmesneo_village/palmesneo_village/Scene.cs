using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace palmesneo_village
{
    public class Scene
    {
        public Entity MasterEntity { get; private set; } = new Entity();

        public EntityUI MasterUIEntity { get; private set; } = new EntityUI();

        private TooltipUI tooltip;
        private ImageUI cursor;

        public virtual void Begin()
        {
            tooltip = new TooltipUI();
            tooltip.Depth = 99;
            MasterUIEntity.AddChild(tooltip);

            cursor = new ImageUI();
            cursor.Depth = 100;
            cursor.Texture = ResourcesManager.GetTexture("Sprites", "cursor");
            cursor.Size = new Vector2(16, 16);
            cursor.Name = "Cursor";
            MasterUIEntity.AddChild(cursor);

            MasterEntity.Begin();
            MasterUIEntity.Begin();
            MasterUIEntity.IsDepthSortEnabled = true;
        }

        public virtual void Update()
        {
            MasterUIEntity.LocalScale = new Vector2(Engine.GlobalUIScale);
            MasterUIEntity.Size = new Vector2(Engine.ViewportWidth, Engine.ViewportHeight) / Engine.GlobalUIScale;

            tooltip.IsVisible = false;

            Vector2 cursorPosition = MInput.Mouse.UIScaledPosition;

            Vector2 tooltipPosition = cursorPosition + new Vector2(5, -20);

            tooltip.LocalPosition = Vector2.Clamp(tooltipPosition, Vector2.Zero, MasterUIEntity.Size - tooltip.Size);

            cursor.LocalPosition = cursorPosition;

            MasterUIEntity.Update();
            MasterEntity.Update();
        }

        public virtual void Render()
        {
            MasterEntity.Render();
        }

        public virtual void DebugRender()
        {
            MasterEntity.DebugRender();
        }

        public virtual void RenderUI()
        {
            MasterUIEntity.Render();
        }

        public virtual void DebugRenderUI()
        {
            MasterUIEntity.DebugRender();
        }

        public virtual void End()
        {

        }

        public void ShowTooltip(string text)
        {
            tooltip.IsVisible = true;

            tooltip.ShowTooltip(text);
        }

        public void ChangeCursorTexture(MTexture texture)
        {
            cursor.Texture = texture;
        }

        public void ChageCursorTextureToDefault()
        {
            cursor.Texture = ResourcesManager.GetTexture("Sprites", "cursor");
        }

    }
}
