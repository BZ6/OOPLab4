using FoodDelivery.Commands;
using FoodDelivery.Data;
using FoodDelivery.Factories;
using FoodDelivery.Models;
using FoodDelivery.Services;
using FoodDelivery.States;
using FoodDelivery.Strategies;

namespace FoodDelivery.Tests
{
	public class OrderManagementTests
	{
		private readonly Menu _menu;
		private readonly IOrderFactory _orderFactory;
		private readonly IOrderService _orderService;

		public OrderManagementTests()
		{
			_menu = new Menu();
			_orderFactory = new OrderFactory();
			_orderService = new OrderService(_orderFactory, _menu);
		}

		[Fact]
		public void MenuItem_Constructor_SetsPropertiesCorrectly()
		{
			// Arrange & Act
			var menuItem = new MenuItem("test1", "Test Pizza", 10.99m, "Pizza", 15, true);

			// Assert
			Assert.Equal("test1", menuItem.Id);
			Assert.Equal("Test Pizza", menuItem.Name);
			Assert.Equal(10.99m, menuItem.Price);
			Assert.Equal("Pizza", menuItem.Category);
			Assert.Equal(15, menuItem.PreparationTime);
			Assert.True(menuItem.IsCustomizable);
		}

		[Fact]
		public void OrderItem_GetTotalPrice_CalculatesCorrectly()
		{
			// Arrange
			var menuItem = new MenuItem("1", "Pizza", 12.99m, "Pizza", 20);
			var orderItem = new OrderItem
			{
				MenuItem = menuItem,
				Quantity = 2,
				CustomizationPrice = 1.5m,
				CustomizationDescription = "Test"
			};

			// Act
			var total = orderItem.GetTotalPrice();

			// Assert
			Assert.Equal((12.99m + 1.5m) * 2, total);
		}

		[Fact]
		public void OrderItem_GetDescription_StandardItem_ReturnsCorrectFormat()
		{
			// Arrange
			var menuItem = new MenuItem("1", "Pizza", 12.99m, "Pizza", 20);
			var orderItem = new OrderItem
			{
				MenuItem = menuItem,
				Quantity = 2,
				IsCustomized = false,
				CustomizationDescription = "Test"
			};

			// Act
			var description = orderItem.GetDescription();

			// Assert
			Assert.Equal("2x Pizza", description);
		}

		[Fact]
		public void OrderItem_GetDescription_CustomItem_ReturnsCorrectFormat()
		{
			// Arrange
			var menuItem = new MenuItem("1", "Pizza", 12.99m, "Pizza", 20);
			var orderItem = new OrderItem
			{
				MenuItem = menuItem,
				Quantity = 1,
				IsCustomized = true,
				CustomizationDescription = "Extra cheese"
			};

			// Act
			var description = orderItem.GetDescription();

			// Assert
			Assert.Equal("1x Pizza [Custom: Extra cheese]", description);
		}

		[Fact]
		public void OrderItem_GetPreparationTime_CustomItem_AddsExtraTime()
		{
			// Arrange
			var menuItem = new MenuItem("1", "Pizza", 12.99m, "Pizza", 20);
			var orderItem = new OrderItem
			{
				MenuItem = menuItem,
				Quantity = 1,
				IsCustomized = true,
				CustomizationDescription = "Test"
			};

			// Act
			var prepTime = orderItem.GetPreparationTime();

			// Assert
			Assert.Equal(30, prepTime);
		}

		[Fact]
		public void PreparingState_ProcessOrder_DoesNothing()
		{
			// Arrange
			var state = new PreparingState();
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");

			// Act & Assert
			var exception = Record.Exception(() => state.ProcessOrder(order));
			Assert.Null(exception);
		}

		[Fact]
		public void PreparingState_CancelOrder_ChangesToCancelledState()
		{
			// Arrange
			var state = new PreparingState();
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");

			// Act
			state.CancelOrder(order);

			// Assert
			Assert.Equal(OrderStatus.Cancelled, order.Status);
		}

		[Fact]
		public void PreparingState_DeliverOrder_ChangesToOutForDeliveryState()
		{
			// Arrange
			var state = new PreparingState();
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");

			// Act
			state.DeliverOrder(order);

			// Assert
			Assert.Equal(OrderStatus.OutForDelivery, order.Status);
		}

		[Fact]
		public void OutForDeliveryState_CancelOrder_ThrowsException()
		{
			// Arrange
			var state = new OutForDeliveryState();
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");

			// Act & Assert
			Assert.Throws<Exception>(() => state.CancelOrder(order));
		}

		[Fact]
		public void OutForDeliveryState_DeliverOrder_ChangesToDeliveredState()
		{
			// Arrange
			var state = new OutForDeliveryState();
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");

			// Act
			state.DeliverOrder(order);

			// Assert
			Assert.Equal(OrderStatus.Delivered, order.Status);
		}

		[Fact]
		public void DeliveredState_ProcessOrder_ThrowsException()
		{
			// Arrange
			var state = new DeliveredState();
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");

			// Act & Assert
			Assert.Throws<Exception>(() => state.ProcessOrder(order));
		}

		[Fact]
		public void CancelledState_ProcessOrder_ThrowsException()
		{
			// Arrange
			var state = new CancelledState();
			var order = new Order(new StandardOrderStrategy(), "Test", "Test Address", "555-1234");

			// Act & Assert
			var exception = Assert.Throws<Exception>(() => state.ProcessOrder(order));
			Assert.Equal("Cannot process cancelled order", exception.Message);
		}

		[Fact]
		public void StandardOrderStrategy_CalculateTotal_IncludesDeliveryAndTax()
		{
			// Arrange
			var strategy = new StandardOrderStrategy();
			var subtotal = 50.0m;

			// Act
			var total = strategy.CalculateTotal(subtotal);

			// Assert
			var expectedTax = 50.0m * 0.1m;
			var expectedDelivery = 5.0m;
			var expectedTotal = 50.0m + expectedTax + expectedDelivery;
			Assert.Equal(expectedTotal, total);
		}

		[Fact]
		public void StandardOrderStrategy_GetOrderType_ReturnsStandard()
		{
			// Arrange
			var strategy = new StandardOrderStrategy();

			// Act
			var orderType = strategy.GetOrderType();

			// Assert
			Assert.Equal("Standard", orderType);
		}

		[Fact]
		public void StandardOrderStrategy_CanAddCustomItems_ReturnsFalse()
		{
			// Arrange
			var strategy = new StandardOrderStrategy();

			// Act
			var canAddCustom = strategy.CanAddCustomItems();

			// Assert
			Assert.False(canAddCustom);
		}

		[Fact]
		public void CustomOrderStrategy_CalculateTotal_IncludesCustomizationFee()
		{
			// Arrange
			var strategy = new CustomOrderStrategy(3.0m);
			var subtotal = 50.0m;

			// Act
			var total = strategy.CalculateTotal(subtotal);

			// Assert
			var expectedTax = 50.0m * 0.1m;
			var expectedTotal = 50.0m + 5.0m + expectedTax + 3.0m;
			Assert.Equal(expectedTotal, total);
		}

		[Fact]
		public void CustomOrderStrategy_CanAddCustomItems_ReturnsTrue()
		{
			// Arrange
			var strategy = new CustomOrderStrategy();

			// Act
			var canAddCustom = strategy.CanAddCustomItems();

			// Assert
			Assert.True(canAddCustom);
		}

		[Fact]
		public void AddStandardItemCommand_Execute_AddsItemToOrder()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test", "Test Address", "555-1234");
			var menuItem = _menu.GetItem("1");
			Assert.NotNull(menuItem);
			var command = new AddStandardItemCommand(order, menuItem, 2);

			// Act
			command.Execute();

			// Assert
			Assert.Single(order.Items);
			Assert.Equal(2, order.Items[0].Quantity);
			Assert.Equal(menuItem, order.Items[0].MenuItem);
		}

		[Fact]
		public void AddStandardItemCommand_Undo_RemovesAddedItem()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");
			var menuItem = _menu.GetItem("1");
			Assert.NotNull(menuItem);
			var command = new AddStandardItemCommand(order, menuItem, 2);

			// Act
			command.Execute();
			command.Undo();

			// Assert
			Assert.Empty(order.Items);
		}

		[Fact]
		public void AddStandardItemCommand_Description_ReturnsCorrectFormat()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");
			var menuItem = _menu.GetItem("1");
			Assert.NotNull(menuItem);
			var command = new AddStandardItemCommand(order, menuItem, 2);

			// Act
			var description = command.Description;

			// Assert
			Assert.Equal("Add 2x Margherita Pizza to order", description);
		}

		[Fact]
		public void AddCustomItemCommand_Execute_AddsCustomItemToOrder()
		{
			// Arrange
			var order = new Order(new CustomOrderStrategy(), "Test Customer", "Test Address", "555-1234");
			var menuItem = _menu.GetItem("1");
			Assert.NotNull(menuItem);
			var command = new AddCustomItemCommand(order, menuItem, 1, "Extra cheese", 1.5m);

			// Act
			command.Execute();

			// Assert
			Assert.Single(order.Items);
			Assert.True(order.Items[0].IsCustomized);
			Assert.Equal("Extra cheese", order.Items[0].CustomizationDescription);
			Assert.Equal(1.5m, order.Items[0].CustomizationPrice);
		}

		[Fact]
		public void AddCustomItemCommand_Execute_OnStandardOrder_ThrowsException()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test", "Test Address", "555-1234");
			var menuItem = _menu.GetItem("1");
			Assert.NotNull(menuItem);
			var command = new AddCustomItemCommand(order, menuItem, 1, "Extra cheese", 1.5m);

			// Act & Assert
			var exception = Assert.Throws<Exception>(() => command.Execute());
			Assert.Equal("Cannot add custom items to standard order", exception.Message);
		}

		[Fact]
		public void AddCustomItemCommand_Execute_NonCustomizableItem_ThrowsException()
		{
			// Arrange
			var order = new Order(new CustomOrderStrategy(), "Test", "Test Address", "555-1234");
			var menuItem = _menu.GetItem("7");
			Assert.NotNull(menuItem);
			var command = new AddCustomItemCommand(order, menuItem, 1, "Extra chocolate", 1.0m);

			// Act & Assert
			var exception = Assert.Throws<Exception>(() => command.Execute());
			Assert.Equal("This item cannot be customized", exception.Message);
		}

		[Fact]
		public void UpdateOrderCommand_Execute_UpdatesAddress()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");
			var command = new UpdateOrderCommand(order, "New Address");

			// Act
			command.Execute();

			// Assert
			Assert.Equal("New Address", order.CustomerAddress);
		}

		[Fact]
		public void UpdateOrderCommand_Undo_RestoresOldAddress()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test", "Old Address", "555-1234");
			var command = new UpdateOrderCommand(order, "New Address");

			// Act
			command.Execute();
			command.Undo();

			// Assert
			Assert.Equal("Old Address", order.CustomerAddress);
		}

		[Fact]
		public void OrderFactory_CreateStandardOrder_ReturnsStandardOrder()
		{
			// Arrange
			var factory = new OrderFactory();

			// Act
			var order = factory.CreateStandardOrder("John", "123 St", "555-1234");

			// Assert
			Assert.NotNull(order);
			Assert.Equal("Standard", order.OrderType);
			Assert.IsType<StandardOrderStrategy>(order.OrderTypeStrategy);
		}

		[Fact]
		public void OrderFactory_CreateCustomOrder_ReturnsCustomOrder()
		{
			// Arrange
			var factory = new OrderFactory();

			// Act
			var order = factory.CreateCustomOrder("Jane", "456 St", "555-5678", 5.0m);

			// Assert
			Assert.NotNull(order);
			Assert.Equal("Custom", order.OrderType);
			Assert.IsType<CustomOrderStrategy>(order.OrderTypeStrategy);
		}

		[Fact]
		public void Order_Constructor_SetsPropertiesCorrectly()
		{
			// Arrange & Act
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");

			// Assert
			Assert.NotNull(order.Id);
			Assert.NotEmpty(order.Id);
			Assert.Equal("Test Customer", order.CustomerName);
			Assert.Equal("Test Address", order.CustomerAddress);
			Assert.Equal("555-1234", order.CustomerPhone);
			Assert.Equal(OrderStatus.Preparing, order.Status);
			Assert.Empty(order.Items);
		}

		[Fact]
		public void Order_AddStandardItem_AddsItemToOrder()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");
			var menuItem = _menu.GetItem("1");
			Assert.NotNull(menuItem);

			// Act
			order.AddStandardItem(menuItem, 2);

			// Assert
			Assert.Single(order.Items);
			Assert.Equal(2, order.Items[0].Quantity);
		}

		[Fact]
		public void Order_AddStandardItem_NonCustomizableToCustomOrder_ThrowsException()
		{
			// Arrange
			var order = new Order(new CustomOrderStrategy(), "Test", "Test Address", "555-1234");
			var menuItem = _menu.GetItem("7");
			Assert.NotNull(menuItem);

			// Act & Assert
			var exception = Assert.Throws<Exception>(() => order.AddStandardItem(menuItem, 1));
			Assert.Equal("Cannot add non-customizable item to custom order", exception.Message);
		}

		[Fact]
		public void Order_AddCustomItem_ToStandardOrder_ThrowsException()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test", "Test Address", "555-1234");
			var menuItem = _menu.GetItem("1");
			Assert.NotNull(menuItem);

			// Act & Assert
			var exception = Assert.Throws<Exception>(() => order.AddCustomItem(menuItem, 1, "Extra cheese", 1.5m));
			Assert.Equal("Cannot add custom items to standard order", exception.Message);
		}

		[Fact]
		public void Order_CalculateSubtotal_ReturnsCorrectSum()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");
			order.AddStandardItem(_menu.GetItem("1")!, 1);
			order.AddStandardItem(_menu.GetItem("3")!, 2);

			// Act
			var subtotal = order.CalculateSubtotal();

			// Assert
			Assert.Equal(12.99m + 17.98m, subtotal);
		}

		[Fact]
		public void Order_GetPreparationTime_CalculatesCorrectly()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");
			order.AddStandardItem(_menu.GetItem("1")!, 1);

			// Act
			var prepTime = order.GetPreparationTime();

			// Assert
			Assert.Equal(20 + 30, prepTime);
		}

		[Fact]
		public void Order_AddSpecialInstructions_AffectsCostAndTime()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");
			order.AddStandardItem(_menu.GetItem("1")!, 1);

			// Act
			order.AddSpecialInstructions("Test instructions", 2.0m);

			// Assert
			var cost = order.GetCost();
			var time = order.GetPreparationTime();
			Assert.True(cost > 0);
			Assert.True(time > 0);
		}

		[Fact]
		public void Order_SetUrgentDelivery_AffectsCostAndTime()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");
			order.AddStandardItem(_menu.GetItem("1")!, 1);

			// Act
			order.SetUrgentDelivery(true);

			// Assert
			var cost = order.GetCost();
			var time = order.GetPreparationTime();
			Assert.True(cost > 0);
			Assert.True(time > 0);
		}

		[Fact]
		public void Order_ExecuteCommand_AddsToHistory()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");
			var menuItem = _menu.GetItem("1");
			Assert.NotNull(menuItem);
			var command = new AddStandardItemCommand(order, menuItem, 1);

			// Act
			order.ExecuteCommand(command);

			// Assert
			var history = order.GetCommandHistory();
			Assert.Single(history);
		}

		[Fact]
		public void Order_UndoLastCommand_RemovesFromHistory()
		{
			// Arrange
			var order = new Order(new StandardOrderStrategy(), "Test Customer", "Test Address", "555-1234");
			var menuItem = _menu.GetItem("1");
			Assert.NotNull(menuItem);
			var command = new AddStandardItemCommand(order, menuItem, 1);
			order.ExecuteCommand(command);

			// Act
			order.UndoLastCommand();

			// Assert
			Assert.Empty(order.Items);
		}

		[Fact]
		public void OrderService_CreateStandardOrder_Success()
		{
			// Act
			var order = _orderService.CreateStandardOrder("John Doe", "123 Main St", "555-1234");

			// Assert
			Assert.NotNull(order);
			Assert.Equal("Standard", order.OrderType);
			Assert.Equal(OrderStatus.Preparing, order.Status);
		}

		[Fact]
		public void OrderService_CreateCustomOrder_Success()
		{
			// Act
			var order = _orderService.CreateCustomOrder("Jane Smith", "456 Oak Ave", "555-5678", 5.0m);

			// Assert
			Assert.NotNull(order);
			Assert.Equal("Custom", order.OrderType);
			Assert.Equal(OrderStatus.Preparing, order.Status);
		}

		[Fact]
		public void OrderService_AddStandardItemToOrder_Success()
		{
			// Arrange
			var order = _orderService.CreateStandardOrder("John Doe", "123 Main St", "555-1234");
			var pizza = _menu.GetItem("1");
			Assert.NotNull(pizza);

			// Act
			_orderService.AddStandardItemToOrder(order.Id, pizza, 2);

			// Assert
			var retrievedOrder = _orderService.GetOrder(order.Id);
			Assert.Single(retrievedOrder.Items);
			Assert.Equal(25.98m, retrievedOrder.CalculateSubtotal());
		}

		[Fact]
		public void OrderService_AddCustomItemToOrder_Success()
		{
			// Arrange
			var order = _orderService.CreateCustomOrder("Jane Smith", "456 Oak Ave", "555-5678");
			var burger = _menu.GetItem("5");
			Assert.NotNull(burger);

			// Act
			_orderService.AddCustomItemToOrder(order.Id, burger, 1, "Extra cheese", 1.5m);

			// Assert
			var retrievedOrder = _orderService.GetOrder(order.Id);
			Assert.Single(retrievedOrder.Items);
			Assert.Equal(11.49m, retrievedOrder.CalculateSubtotal());
		}

		[Fact]
		public void OrderService_AddCustomItemToStandardOrder_ThrowsException()
		{
			// Arrange
			var order = _orderService.CreateStandardOrder("John Doe", "123 Main St", "555-1234");
			var pizza = _menu.GetItem("1");
			Assert.NotNull(pizza);

			// Act & Assert
			Assert.Throws<Exception>(() =>
				_orderService.AddCustomItemToOrder(order.Id, pizza, 1, "Extra sauce", 1.0m));
		}

		[Fact]
		public void OrderService_UndoLastOperation_Success()
		{
			// Arrange
			var order = _orderService.CreateStandardOrder("John Doe", "123 Main St", "555-1234");
			var pizza = _menu.GetItem("1");
			var salad = _menu.GetItem("3");
			Assert.NotNull(pizza);
			Assert.NotNull(salad);

			// Act
			_orderService.AddStandardItemToOrder(order.Id, pizza, 1);
			_orderService.AddStandardItemToOrder(order.Id, salad, 1);
			_orderService.UndoLastOperation(order.Id);

			// Assert
			Assert.Single(_orderService.GetOrder(order.Id).Items);
		}

		[Fact]
		public void OrderService_UpdateOrderAddress_Success()
		{
			// Arrange
			var order = _orderService.CreateStandardOrder("John Doe", "123 Main St", "555-1234");
			var newAddress = "789 Pine Rd";

			// Act
			_orderService.UpdateOrderAddress(order.Id, newAddress);

			// Assert
			var updatedOrder = _orderService.GetOrder(order.Id);
			Assert.Equal(newAddress, updatedOrder.CustomerAddress);
		}

		[Fact]
		public void OrderService_ProcessOrder_ChangesState()
		{
			// Arrange
			var order = _orderService.CreateStandardOrder("John Doe", "123 Main St", "555-1234");

			// Act
			_orderService.ProcessOrder(order.Id);
			_orderService.DeliverOrder(order.Id);

			// Assert
			var updatedOrder = _orderService.GetOrder(order.Id);
			Assert.Equal(OrderStatus.OutForDelivery, updatedOrder.Status);
		}

		[Fact]
		public void OrderService_CancelOrder_ChangesToCancelled()
		{
			// Arrange
			var order = _orderService.CreateStandardOrder("John Doe", "123 Main St", "555-1234");

			// Act
			_orderService.CancelOrder(order.Id);

			// Assert
			Assert.Equal(OrderStatus.Cancelled, _orderService.GetOrder(order.Id).Status);
		}

		[Fact]
		public void OrderService_GetOrder_NonExistentOrder_ThrowsException()
		{
			// Act & Assert
			Assert.Throws<Exception>(() => _orderService.GetOrder("non-existent-id"));
		}

		[Fact]
		public void OrderService_GetAllOrders_ReturnsAllOrders()
		{
			// Arrange
			_orderService.CreateStandardOrder("John", "Addr1", "555-1111");
			_orderService.CreateCustomOrder("Jane", "Addr2", "555-2222");

			// Act
			var orders = _orderService.GetAllOrders();

			// Assert
			Assert.Equal(2, orders.Count());
		}

		[Fact]
		public void OrderService_CalculateOrderTotal_ReturnsCorrectAmount()
		{
			// Arrange
			var order = _orderService.CreateStandardOrder("John Doe", "123 Main St", "555-1234");
			_orderService.AddStandardItemToOrder(order.Id, _menu.GetItem("1")!, 1);

			// Act
			var total = _orderService.CalculateOrderTotal(order.Id);

			// Assert
			Assert.True(total > 0);
		}

		[Fact]
		public void OrderService_GetOrderHistory_ReturnsCommandHistory()
		{
			// Arrange
			var order = _orderService.CreateStandardOrder("John Doe", "123 Main St", "555-1234");
			_orderService.AddStandardItemToOrder(order.Id, _menu.GetItem("1")!, 1);

			// Act
			var history = _orderService.GetOrderHistory(order.Id);

			// Assert
			Assert.Single(history);
		}

		[Fact]
		public void CompleteOrderFlow_StandardOrder_Success()
		{
			// Arrange
			var order = _orderService.CreateStandardOrder("John Doe", "123 Main St", "555-1234");
			_orderService.AddStandardItemToOrder(order.Id, _menu.GetItem("1")!, 1);
			_orderService.AddStandardItemToOrder(order.Id, _menu.GetItem("9")!, 2);

			// Act
			_orderService.ProcessOrder(order.Id);
			_orderService.DeliverOrder(order.Id);

			// Assert
			var finalOrder = _orderService.GetOrder(order.Id);
			Assert.Equal(OrderStatus.OutForDelivery, finalOrder.Status);
			Assert.Equal(2, finalOrder.Items.Count);
			Assert.True(finalOrder.CalculateTotal() > 0);
		}

		[Fact]
		public void CompleteOrderFlow_CustomOrder_Success()
		{
			// Arrange
			var order = _orderService.CreateCustomOrder("Jane Smith", "456 Oak Ave", "555-5678", 3.0m);
			_orderService.AddCustomItemToOrder(order.Id, _menu.GetItem("5")!, 1, "No onions", 0.5m);

			// Act
			_orderService.ProcessOrder(order.Id);

			// Assert
			var finalOrder = _orderService.GetOrder(order.Id);
			Assert.Equal(OrderStatus.Preparing, finalOrder.Status);
			Assert.Single(finalOrder.Items);
			Assert.True(finalOrder.CalculateTotal() > 0);
		}

		[Fact]
		public void Order_WithSpecialInstructionsAndUrgent_CalculatesCorrectly()
		{
			// Arrange
			var order = _orderService.CreateStandardOrder("John Doe", "123 Main St", "555-1234");
			_orderService.AddStandardItemToOrder(order.Id, _menu.GetItem("1")!, 1);

			var retrievedOrder = _orderService.GetOrder(order.Id);
			retrievedOrder.AddSpecialInstructions("Ring bell twice", 1.0m);
			retrievedOrder.SetUrgentDelivery(true);

			// Act
			var total = retrievedOrder.CalculateTotal();
			var prepTime = retrievedOrder.GetPreparationTime();

			// Assert
			Assert.True(total > 0);
			Assert.True(prepTime > 0);
		}

		[Fact]
		public void Menu_GetItemsByCategory_ReturnsCorrectItems()
		{
			// Arrange
			var menu = new Menu();

			// Act
			var pizzaItems = menu.GetItemsByCategory("Pizza");

			// Assert
			Assert.All(pizzaItems, item => Assert.Equal("Pizza", item.Category));
		}

		[Fact]
		public void Menu_GetCustomizableItems_ReturnsOnlyCustomizable()
		{
			// Arrange
			var menu = new Menu();

			// Act
			var customizableItems = menu.GetCustomizableItems();

			// Assert
			Assert.All(customizableItems, item => Assert.True(item.IsCustomizable));
		}
	}
}
