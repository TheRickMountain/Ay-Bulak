namespace palmesneo_village
{
    public class ConsumableItem : Item
    {

        public int EnergyAmount { get; init; }

        public override string GetTooltipInfo()
        {
            string tooltip = base.GetTooltipInfo();

            tooltip += $"\n/c[#00C000]{LocalizationManager.GetText("energy")}:/cd {EnergyAmount.ToString("+0;-#")}";

            return tooltip;
        }
    }
}
