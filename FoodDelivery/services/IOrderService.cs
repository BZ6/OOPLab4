namespace FoodDelivery.Services
{
	public interface IOrderService
	{
		Order CreateStandardOrder(string customerName, string address, string phone);
		Order CreateCustomOrder(string customerName, string address, string phone, decimal customizationFee = 3.0m);
		void AddStandardItemToOrder(string orderId, Models.MenuItem item, int quantity = 1);
		void AddCustomItemToOrder(string orderId, Models.MenuItem item, int quantity, string customization, decimal customizationPrice = 0);
		void UpdateOrderAddress(string orderId, string newAddress);
		void UndoLastOperation(string orderId);
		void ProcessOrder(string orderId);
		void CancelOrder(string orderId);
		void DeliverOrder(string orderId);
		Order GetOrder(string orderId);
		IEnumerable<Order> GetAllOrders();
		decimal CalculateOrderTotal(string orderId);
		IEnumerable<string> GetOrderHistory(string orderId);
	}
}
