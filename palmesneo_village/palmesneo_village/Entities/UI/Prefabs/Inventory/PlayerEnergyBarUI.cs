using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace palmesneo_village
{
    public class PlayerEnergyBarUI : ProgressBarUI
    {
        private PlayerEnergyManager energyManager;

        public PlayerEnergyBarUI(PlayerEnergyManager energyManager)
        {
            this.energyManager = energyManager ?? throw new ArgumentNullException(nameof(energyManager));

            BackColor = Color.Black;
            FrontColor = Color.LightGreen;

            MinValue = 0;
            MaxValue = energyManager.MaxEnergyAmount;
            CurrentValue = energyManager.EnergyAmount;

            energyManager.EnergyAmountChanged += OnEnergyChanged;
            energyManager.MaxEnergyAmountChanged += OnMaxEnergyChanged;

            UpdateProgressBarSize();
        }

        private void OnEnergyChanged(int newEnergy)
        {
            CurrentValue = newEnergy;
        }

        private void OnMaxEnergyChanged(int newMaxEnergy)
        {
            MaxValue = newMaxEnergy;

            UpdateProgressBarSize();
        }

        private void UpdateProgressBarSize()
        {
            Size = new Vector2(MaxValue, 16);
        }
    }
}