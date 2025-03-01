using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class TooltipUI : ImageUI
    {

        private TextUI tooltipText;

        public TooltipUI()
        {
            Texture = RenderManager.Pixel;
            SelfColor = Color.Black * 0.75f;

            tooltipText = new TextUI();
            AddChild(tooltipText);
        }

        public void ShowTooltip(string text)
        {
            tooltipText.Text = text;

            Size = tooltipText.Size;
        }

    }
}
