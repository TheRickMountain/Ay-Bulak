using FontStashSharp.RichText;
using System;

namespace palmesneo_village
{
    public class RichTextUI : EntityUI
    {
        private RichTextLayout rtl;

        public Action<string> TextChanged { get; set; }

        public string Text
        {
            get { return rtl.Text; }
            set
            {
                if (rtl.Text == value) return;

                rtl.Text = value;

                UpdateSize();

                TextChanged?.Invoke(rtl.Text);
            }
        }

        public RichTextUI()
        {
            rtl = new RichTextLayout();
            rtl.Font = RenderManager.StashDefaultFont;

            UpdateSize();
        }

        private void UpdateSize()
        {
            Size = rtl.Size.ToVector2();
        }

        public override void Render()
        {
            rtl.Draw(RenderManager.SpriteBatch, GlobalPosition, SelfColor, GlobalRotation, Origin, GlobalScale);

            base.Render();
        }
    }
}