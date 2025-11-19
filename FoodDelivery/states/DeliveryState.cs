namespace FoodDelivery.States
{
	public class DeliveredState : IOrderState
	{
		public Models.OrderStatus Status => Models.OrderStatus.Delivered;

		public void ProcessOrder(Order order) => throw new Exception("Order already delivered");
		public void CancelOrder(Order order) => throw new Exception("Cannot cancel delivered order");
		public void DeliverOrder(Order order) => throw new Exception("Order already delivered");
	}
}
