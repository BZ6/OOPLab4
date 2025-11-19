using FoodDelivery.Models;

namespace FoodDelivery.Commands
{
	public class AddCustomItemCommand : IOrderCommand
	{
		private Order _order;
		private MenuItem _menuItem;
		private int _quantity;
		private string _customization;
		private decimal _customizationPrice;
		private OrderItem? _addedItem;

		public string Description => $"Add {_quantity}x {_menuItem.Name} with {_customization} to order";

		public AddCustomItemCommand(Order order, MenuItem menuItem, int quantity, string customization, decimal customizationPrice)
		{
			_order = order;
			_menuItem = menuItem;
			_quantity = quantity;
			_customization = customization;
			_customizationPrice = customizationPrice;
			_addedItem = null;
		}

		public void Execute()
		{
			if (_order.OrderType != "Custom")
				throw new Exception("Cannot add custom items to standard order");
			if (!_menuItem.IsCustomizable)
				throw new Exception("This item cannot be customized");

			_addedItem = new OrderItem
			{
				MenuItem = _menuItem,
				Quantity = _quantity,
				IsCustomized = true,
				CustomizationDescription = _customization,
				CustomizationPrice = _customizationPrice
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
