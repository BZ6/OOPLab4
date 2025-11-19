namespace FoodDelivery.States
{
	public class PreparingState : IOrderState
	{
		public Models.OrderStatus Status => Models.OrderStatus.Preparing;

		public void ProcessOrder(Order order) {}
		public void CancelOrder(Order order) => order.SetState(new CancelledState());
		public void DeliverOrder(Order order) => order.SetState(new OutForDeliveryState());
	}
}
