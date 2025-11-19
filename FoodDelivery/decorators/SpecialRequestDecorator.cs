namespace FoodDelivery.Decorator
{
	public class SpecialRequestDecorator : IOrderDecorator
	{
		private IOrderDecorator _order;
		private string _specialRequest;
		private decimal _additionalCost;
		private int _additionalTime;

		public SpecialRequestDecorator(IOrderDecorator order, string specialRequest, decimal additionalCost = 0, int additionalTime = 5)
		{
			_order = order;
			_specialRequest = specialRequest;
			_additionalCost = additionalCost;
			_additionalTime = additionalTime;
		}

		public string GetDescription() => $"{_order.GetDescription()} [Special: {_specialRequest}]";
		public decimal GetCost() => _order.GetCost() + _additionalCost;
		public int GetPreparationTime() => _order.GetPreparationTime() + _additionalTime;
	}
}
