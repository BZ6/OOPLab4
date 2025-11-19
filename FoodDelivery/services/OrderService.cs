using FoodDelivery.Commands;
using FoodDelivery.Data;
using FoodDelivery.Factories;
using FoodDelivery.Models;

namespace FoodDelivery.Services
{
	public class OrderService : IOrderService
	{
		private Dictionary<string, Order> _orders;
		private IOrderFactory _orderFactory;
		private Menu _menu;

		public OrderService(IOrderFactory orderFactory, Menu menu)
		{
			_orders = new Dictionary<string, Order>();
			_orderFactory = orderFactory;
			_menu = menu;
		}

		public IEnumerable<Order> GetAllOrders() => _orders.Values;

		public decimal CalculateOrderTotal(string orderId)
			=> _orders.ContainsKey(orderId)
				? _orders[orderId].CalculateTotal()
				: 0;
		public IEnumerable<string> GetOrderHistory(string orderId)
			=> _orders.ContainsKey(orderId)
				? _orders[orderId].GetCommandHistory()
				: Enumerable.Empty<string>();

		public Order CreateStandardOrder(string customerName, string address, string phone)
		{
			var order = _orderFactory.CreateStandardOrder(customerName, address, phone);
			_orders[order.Id] = order;
			return order;
		}

		public Order CreateCustomOrder(string customerName, string address, string phone, decimal customizationFee = 3.0m)
		{
			var order = _orderFactory.CreateCustomOrder(customerName, address, phone, customizationFee);
			_orders[order.Id] = order;
			return order;
		}

		public Order GetOrder(string orderId)
		{
			if (!_orders.ContainsKey(orderId))
				throw new Exception($"Order with ID {orderId} not found");
			return _orders[orderId];
		}

		public void AddStandardItemToOrder(string orderId, MenuItem item, int quantity = 1)
		{
			if (!_orders.ContainsKey(orderId))
				throw new Exception($"Order with ID {orderId} not found");

			var command = new AddStandardItemCommand(_orders[orderId], item, quantity);
			_orders[orderId].ExecuteCommand(command);
		}

		public void AddCustomItemToOrder(string orderId, MenuItem item, int quantity, string customization, decimal customizationPrice = 0)
		{
			if (!_orders.ContainsKey(orderId))
				throw new Exception($"Order with ID {orderId} not found");

			var command = new AddCustomItemCommand(_orders[orderId], item, quantity, customization, customizationPrice);
			_orders[orderId].ExecuteCommand(command);
		}

		public void UpdateOrderAddress(string orderId, string newAddress)
		{
			if (!_orders.ContainsKey(orderId))
				throw new Exception($"Order with ID {orderId} not found");

			var command = new UpdateOrderCommand(_orders[orderId], newAddress);
			_orders[orderId].ExecuteCommand(command);
		}

		public void UndoLastOperation(string orderId)
		{
			if (_orders.ContainsKey(orderId))
				_orders[orderId].UndoLastCommand();
		}

		public void ProcessOrder(string orderId)
		{
			if (_orders.ContainsKey(orderId))
				_orders[orderId].ProcessOrder();
		}

		public void CancelOrder(string orderId)
		{
			if (_orders.ContainsKey(orderId))
				_orders[orderId].CancelOrder();
		}

		public void DeliverOrder(string orderId)
		{
			if (_orders.ContainsKey(orderId))
				_orders[orderId].DeliverOrder();
		}
	}
}
