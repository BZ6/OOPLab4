namespace FoodDelivery.Decorator
{
	public class UrgentDeliveryDecorator : IOrderDecorator
	{
		private IOrderDecorator _order;
		private decimal _urgentFee;

		public UrgentDeliveryDecorator(IOrderDecorator order, decimal urgentFee = 7.0m)
		{
			_order = order;
			_urgentFee = urgentFee;
		}

		public string GetDescription() => $"{_order.GetDescription()} [Urgent Delivery]";
		public decimal GetCost() => _order.GetCost() + _urgentFee;
		public int GetPreparationTime() => Math.Max(15, _order.GetPreparationTime() / 2);
	}
}
