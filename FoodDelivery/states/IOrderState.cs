namespace FoodDelivery.States
{
	public interface IOrderState
	{
		Models.OrderStatus Status { get; }
		void ProcessOrder(Order order);
		void CancelOrder(Order order);
		void DeliverOrder(Order order);
	}
}
