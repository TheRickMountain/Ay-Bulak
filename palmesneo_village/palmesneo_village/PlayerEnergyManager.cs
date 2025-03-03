using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    using System;

    public class PlayerEnergyManager
    {
        public Action<int> EnergyAmountChanged { get; set; }
        public Action<int> MaxEnergyAmountChanged { get; set; }

        private int energyAmount;
        private int maxEnergyAmount;

        public int EnergyAmount
        {
            get => energyAmount;
            private set
            {
                int newValue = Math.Clamp(value, 0, maxEnergyAmount);
                if (energyAmount == newValue) return;

                energyAmount = newValue;
                EnergyAmountChanged?.Invoke(energyAmount);
            }
        }

        public int MaxEnergyAmount
        {
            get => maxEnergyAmount;
            private set
            {
                if (maxEnergyAmount == value) return;

                maxEnergyAmount = Math.Max(1, value);
                MaxEnergyAmountChanged?.Invoke(maxEnergyAmount);

                if (EnergyAmount > maxEnergyAmount)
                    EnergyAmount = maxEnergyAmount;
            }
        }

        public PlayerEnergyManager(int initialEnergy = 0, int maxEnergy = 100)
        {
            maxEnergyAmount = Math.Max(1, maxEnergy);
            energyAmount = Math.Clamp(initialEnergy, 0, maxEnergyAmount);
        }

        public void AddEnergy(int amount)
        {
            if (amount <= 0) return;
            EnergyAmount = energyAmount + amount;
        }

        public void ConsumeEnergy(int amount)
        {
            if (amount <= 0) return;
            EnergyAmount = energyAmount - amount;
        }

        public void SetMaxEnergy(int maxEnergy)
        {
            MaxEnergyAmount = maxEnergy;
        }

        public bool IncreaseMaxEnergy(int amount)
        {
            if (amount <= 0) return false;
            MaxEnergyAmount = maxEnergyAmount + amount;
            return true;
        }

        public void RefillEnergy()
        {
            EnergyAmount = maxEnergyAmount;
        }

        public bool HasEnoughEnergy(int requiredAmount)
        {
            return energyAmount >= requiredAmount;
        }
    }
}
