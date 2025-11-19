using FoodDelivery.Models;

namespace FoodDelivery.Data
{
	public class Menu
	{
		private Dictionary<string, MenuItem> _items;

		public Menu()
		{
			_items = new Dictionary<string, MenuItem>();
			InitializeSampleMenu();
		}

		private void InitializeSampleMenu()
		{
			AddItem(new MenuItem("1", "Margherita Pizza", 12.99m, "Pizza", 20, true));
			AddItem(new MenuItem("2", "Pepperoni Pizza", 14.99m, "Pizza", 22, true));
			AddItem(new MenuItem("3", "Caesar Salad", 8.99m, "Salad", 10, true));
			AddItem(new MenuItem("4", "Greek Salad", 9.99m, "Salad", 12, true));
			AddItem(new MenuItem("5", "Chicken Burger", 9.99m, "Burger", 15, true));
			AddItem(new MenuItem("6", "Veggie Burger", 8.99m, "Burger", 12, true));
			AddItem(new MenuItem("7", "Chocolate Cake", 6.99m, "Dessert", 5, false));
			AddItem(new MenuItem("8", "Cheesecake", 7.99m, "Dessert", 5, false));
			AddItem(new MenuItem("9", "Cola", 2.99m, "Drink", 2, true));
			AddItem(new MenuItem("10", "Orange Juice", 3.99m, "Drink", 2, true));
		}

		public void AddItem(MenuItem item) => _items[item.Id] = item;
		public IEnumerable<MenuItem> GetAllItems() => _items.Values;
		public MenuItem? GetItem(string id)
			=> _items.ContainsKey(id)
				? _items[id]
				: null;
		public IEnumerable<MenuItem> GetCustomizableItems()
			=> _items
				.Values
				.Where(item => item.IsCustomizable);
		public IEnumerable<MenuItem> GetItemsByCategory(string category)
			=> _items
				.Values
				.Where(item => item.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
	}
}
