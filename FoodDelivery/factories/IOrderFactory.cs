namespace FoodDelivery.Factories
{
	public interface IOrderFactory
	{
		Order CreateStandardOrder(string customerName, string address, string phone);
		Order CreateCustomOrder(string customerName, string address, string phone, decimal customizationFee = 3.0m);
	}
}
