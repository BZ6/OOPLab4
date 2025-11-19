using FoodDelivery.Strategies;

namespace FoodDelivery.Factories
{
	public class OrderFactory : IOrderFactory
	{
		public OrderFactory() {}

		public Order CreateStandardOrder(string customerName, string address, string phone)
			=> new Order(new StandardOrderStrategy(), customerName, address, phone);

		public Order CreateCustomOrder(string customerName, string address, string phone, decimal customizationFee = 3.0m)
			=> new Order(new CustomOrderStrategy(customizationFee), customerName, address, phone);
	}
}
