using System.Text;
using FoodDelivery.Commands;
using FoodDelivery.Decorator;
using FoodDelivery.Models;
using FoodDelivery.States;
using FoodDelivery.Strategies;

namespace FoodDelivery
{
	public class Order : IOrderDecorator
	{
		private IOrderState _state;
		private Stack<IOrderCommand> _executedCommands = new Stack<IOrderCommand>();
		private string _specialInstructions = "";
		private decimal _specialInstructionsCost = 0;
		private bool _isUrgent = false;

		public string Id { get; private set; }
		public string CustomerName { get; set; }
		public string CustomerAddress { get; set; }
		public string CustomerPhone { get; set; }
		public DateTime OrderTime { get; private set; }
		public List<OrderItem> Items { get; private set; }
		public IOrderTypeStrategy OrderTypeStrategy { get; private set; }

		public OrderStatus Status => _state.Status;
		public string OrderType => OrderTypeStrategy.GetOrderType();

		public Order(IOrderTypeStrategy orderTypeStrategy, string customerName, string address, string phone)
		{
			Id = Guid.NewGuid().ToString();
			OrderTime = DateTime.Now;
			Items = new List<OrderItem>();
			OrderTypeStrategy = orderTypeStrategy;
			_state = new PreparingState();
			CustomerName = customerName;
			CustomerAddress = address;
			CustomerPhone = phone;

		}

		public void SetState(IOrderState state) => _state = state;
		public void ProcessOrder() => _state.ProcessOrder(this);
		public void CancelOrder() => _state.CancelOrder(this);
		public void DeliverOrder() => _state.DeliverOrder(this);
		public IEnumerable<string> GetCommandHistory() => _executedCommands.Select(cmd => cmd.Description);
		public void SetUrgentDelivery(bool isUrgent) => _isUrgent = isUrgent;
		public decimal CalculateSubtotal() => Items.Sum(item => item.GetTotalPrice());
		public decimal CalculateTotal() => GetCost();
		public string GetOrderSummary()
			=> new StringBuilder()
				.AppendLine($"Order Summary:")
				.AppendLine($"ID: {Id}")
				.AppendLine($"Customer: {CustomerName}")
				.AppendLine($"Address: {CustomerAddress}")
				.AppendLine($"Phone: {CustomerPhone}")
				.AppendLine($"Type: {OrderType}")
				.AppendLine($"Status: {Status}")
				.AppendLine($"Items: {Items.Count}")
				.AppendLine($"Subtotal: ${CalculateSubtotal():F2}")
				.AppendLine($"Total: ${CalculateTotal():F2}")
				.AppendLine($"Estimated Time: {GetPreparationTime()} minutes")
				.ToString();

		public void ExecuteCommand(IOrderCommand command)
		{
			command.Execute();
			_executedCommands.Push(command);
		}

		public void UndoLastCommand()
		{
			if (_executedCommands.Count > 0)
			{
				var command = _executedCommands.Pop();
				command.Undo();
			}
		}

		public void AddSpecialInstructions(string instructions, decimal additionalCost = 0)
		{
			_specialInstructions = instructions;
			_specialInstructionsCost = additionalCost;
		}

		public string GetDescription()
		{
			var itemsDescription = string.Join(", ", Items.Select(item => item.GetDescription()));
			var baseDescription = $"{OrderType} Order #{Id}: {itemsDescription}";

			if (!string.IsNullOrEmpty(_specialInstructions))
				baseDescription += $" [Instructions: {_specialInstructions}]";
			if (_isUrgent)
				baseDescription += " [URGENT]";

			return baseDescription;
		}

		public decimal GetCost()
		{
			var itemsCost = Items.Sum(item => item.GetTotalPrice());
			var total = OrderTypeStrategy.CalculateTotal(itemsCost);
			total += _specialInstructionsCost;

			if (_isUrgent)
				total += 7.0m;

			return total;
		}

		public int GetPreparationTime()
		{
			var maxItemTime = Items.Any() ? Items.Max(item => item.GetPreparationTime()) : 0;
			var totalTime = OrderTypeStrategy.GetPreparationTime(maxItemTime);
			totalTime += !string.IsNullOrEmpty(_specialInstructions) ? 5 : 0;

			if (_isUrgent)
				totalTime = Math.Max(15, totalTime / 2);

			return totalTime;
		}

		public void AddStandardItem(MenuItem menuItem, int quantity = 1)
		{
			if (!menuItem.IsCustomizable && OrderTypeStrategy.CanAddCustomItems())
				throw new Exception("Cannot add non-customizable item to custom order");

			var orderItem = new OrderItem
			{
				MenuItem = menuItem,
				Quantity = quantity,
				IsCustomized = false,
				CustomizationDescription = string.Empty,
				CustomizationPrice = 0
			};

			Items.Add(orderItem);
		}

		public void AddCustomItem(MenuItem menuItem, int quantity, string customization, decimal customizationPrice = 0)
		{
			if (!OrderTypeStrategy.CanAddCustomItems())
				throw new Exception("Cannot add custom items to standard order");
			if (!menuItem.IsCustomizable)
				throw new Exception("This item cannot be customized");

			var orderItem = new OrderItem
			{
				MenuItem = menuItem,
				Quantity = quantity,
				IsCustomized = true,
				CustomizationDescription = customization,
				CustomizationPrice = customizationPrice
			};

			Items.Add(orderItem);
		}
	}
}
