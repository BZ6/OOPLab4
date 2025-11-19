namespace FoodDelivery.Commands
{
	public class UpdateOrderCommand : IOrderCommand
	{
		private Order _order;
		private string _oldAddress;
		private string _newAddress;

		public string Description => $"Update delivery address from '{_oldAddress}' to '{_newAddress}'";

		public UpdateOrderCommand(Order order, string newAddress)
		{
			_order = order;
			_oldAddress = order.CustomerAddress;
			_newAddress = newAddress;
		}

		public void Execute() => _order.CustomerAddress = _newAddress;
		public void Undo() => _order.CustomerAddress = _oldAddress;
	}
}
