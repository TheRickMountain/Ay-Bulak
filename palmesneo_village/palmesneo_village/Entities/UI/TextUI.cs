using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;

namespace palmesneo_village
{
    public class TextUI : EntityUI
    {
        public Action<string> TextChanged { get; set; }
        
        private string text = "";
        public string Text 
        { 
            get { return text; }
            set
            {
                if (text == value) return;

                text = value;

                UpdateSize();

                TextChanged?.Invoke(text);
            }
        }

        public TextUI()
        {
            UpdateSize();
        }

        private void UpdateSize()
        {
            Size = RenderManager.StashDefaultFont.MeasureString(Text);
        }

        public override void Render()
        {
            RenderManager.StashDefaultFont.DrawText(RenderManager.SpriteBatch, Text, GlobalPosition, SelfColor,
                GlobalRotation, Origin, GlobalScale);

            base.Render();
        }

    }
}
