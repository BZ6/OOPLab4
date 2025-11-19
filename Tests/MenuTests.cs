using FoodDelivery.Data;
using FoodDelivery.Models;

namespace FoodDelivery.Tests
{
	public class MenuTests
	{
		[Fact(DisplayName = "Menu InitializeSampleMenu should create predefined menu items")]
		public void Menu_InitializeSampleMenu_CreatesItems()
		{
			// Arrange & Act
			var menu = new Menu();

			// Assert
			var items = menu.GetAllItems();
			Assert.NotEmpty(items);
		}

		[Fact(DisplayName = "Menu GetItem with existing ID should return correct menu item")]
		public void Menu_GetItem_ExistingId_ReturnsItem()
		{
			// Arrange
			var menu = new Menu();

			// Act
			var item = menu.GetItem("1");

			// Assert
			Assert.NotNull(item);
			Assert.Equal("1", item.Id);
		}

		[Fact(DisplayName = "Menu GetItem with non-existent ID should return null")]
		public void Menu_GetItem_NonExistentId_ReturnsNull()
		{
			// Arrange
			var menu = new Menu();

			// Act
			var item = menu.GetItem("non-existent");

			// Assert
			Assert.Null(item);
		}

		[Fact(DisplayName = "Menu AddItem should add new item to menu")]
		public void Menu_AddItem_AddsToMenu()
		{
			// Arrange
			var menu = new Menu();
			var newItem = new MenuItem("test", "Test Item", 9.99m, "Test", 10);

			// Act
			menu.AddItem(newItem);

			// Assert
			var retrievedItem = menu.GetItem("test");
			Assert.NotNull(retrievedItem);
			Assert.Equal("Test Item", retrievedItem.Name);
		}

		[Fact(DisplayName = "Menu GetAllItems should return all menu items")]
		public void Menu_GetAllItems_ReturnsAllItems()
		{
			// Arrange
			var menu = new Menu();

			// Act
			var items = menu.GetAllItems();

			// Assert
			Assert.True(items.Count() >= 10);
		}
	}
}
