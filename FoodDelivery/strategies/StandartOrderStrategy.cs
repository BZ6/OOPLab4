namespace FoodDelivery.Strategies
{
	public class StandardOrderStrategy : IOrderTypeStrategy
	{
		public decimal CalculateTotal(decimal subtotal)
		{
			var deliveryFee = 5.0m;
			var tax = subtotal * 0.1m;
			return subtotal + deliveryFee + tax;
		}

		public int GetPreparationTime(int basePreparationTime) => basePreparationTime + 30;
		public string GetOrderType() => "Standard";
		public bool CanAddCustomItems() => false;
	}
}
