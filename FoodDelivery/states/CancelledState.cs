namespace FoodDelivery.States
{
	public class CancelledState : IOrderState
	{
		public Models.OrderStatus Status => Models.OrderStatus.Cancelled;

		public void ProcessOrder(Order order) => throw new Exception("Cannot process cancelled order");
		public void CancelOrder(Order order) {}
		public void DeliverOrder(Order order) => throw new Exception("Cannot deliver cancelled order");
	}
}
