using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class MoneyAmountManager
    {
        public Action<int> MoneyAmountChanged { get; set; }

        private int moneyAmount;

        public int MoneyAmount
        {
            get => moneyAmount;
            set
            {
                if (moneyAmount == value) return;

                moneyAmount = value;

                if (moneyAmount < 0)
                {
                    throw new ArgumentOutOfRangeException("Money amount can't be less than zero!");
                }

                MoneyAmountChanged?.Invoke(moneyAmount);
            }
        }
    }
}
