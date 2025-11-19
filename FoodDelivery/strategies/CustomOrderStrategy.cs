namespace FoodDelivery.Strategies
{
	public class CustomOrderStrategy : IOrderTypeStrategy
	{
		private decimal _customizationFee;

		public CustomOrderStrategy(decimal customizationFee = 3.0m)
			=> _customizationFee = customizationFee;

		public int GetPreparationTime(int basePreparationTime) => basePreparationTime + 45;
		public string GetOrderType() => "Custom";
		public bool CanAddCustomItems() => true;

		public decimal CalculateTotal(decimal subtotal)
		{
			var deliveryFee = 5.0m;
			var tax = subtotal * 0.1m;
			return subtotal + deliveryFee + tax + _customizationFee;
		}
	}
}
