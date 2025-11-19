namespace FoodDelivery.Models
{
	public class OrderItem
	{
		public MenuItem MenuItem { get; set; }
		public int Quantity { get; set; }
		public bool IsCustomized { get; set; }
		public string CustomizationDescription { get; set; }
		public decimal CustomizationPrice { get; set; }

		public decimal GetTotalPrice() => (MenuItem.Price + CustomizationPrice) * Quantity;

		public string GetDescription()
		{
			if (IsCustomized)
				return $"{Quantity}x {MenuItem.Name} [Custom: {CustomizationDescription}]";
			return $"{Quantity}x {MenuItem.Name}";
		}

		public int GetPreparationTime()
		{
			var baseTime = MenuItem.PreparationTime;
			return IsCustomized ? baseTime + 10 : baseTime;
		}
	}
}
