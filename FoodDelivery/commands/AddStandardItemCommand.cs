using FoodDelivery.Models;

namespace FoodDelivery.Commands
{
	public class AddStandardItemCommand : IOrderCommand
	{
		private Order _order;
		private MenuItem _menuItem;
		private int _quantity;
		private OrderItem? _addedItem;

		public string Description => $"Add {_quantity}x {_menuItem.Name} to order";

		public AddStandardItemCommand(Order order, MenuItem menuItem, int quantity)
		{
			_order = order;
			_menuItem = menuItem;
			_quantity = quantity;
			_addedItem = null;
		}

		public void Execute()
		{
			if (!_menuItem.IsCustomizable && _order.OrderType == "Custom")
				throw new Exception("Cannot add non-customizable item to custom order");

			_addedItem = new OrderItem
			{
				MenuItem = _menuItem,
				Quantity = _quantity,
				IsCustomized = false,
				CustomizationDescription = string.Empty,
				CustomizationPrice = 0
			};

			_order.Items.Add(_addedItem);
		}

		public void Undo()
		{
			if (_addedItem != null)
			{
				_order.Items.Remove(_addedItem);
				_addedItem = null;
			}
		}
	}
}
