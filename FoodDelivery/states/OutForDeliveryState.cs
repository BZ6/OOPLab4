namespace FoodDelivery.States
{
	public class OutForDeliveryState : IOrderState
	{
		public Models.OrderStatus Status => Models.OrderStatus.OutForDelivery;

		public void ProcessOrder(Order order) {}
		public void CancelOrder(Order order) => throw new Exception("Cannot cancel order that is out for delivery");
		public void DeliverOrder(Order order) => order.SetState(new DeliveredState());
	}
}
