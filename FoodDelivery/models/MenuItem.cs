namespace FoodDelivery.Models
{
	public class MenuItem
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public string Category { get; set; }
		public int PreparationTime { get; set; }
		public bool IsCustomizable { get; set; }

		public MenuItem(string id, string name, decimal price, string category, int prepTime, bool customizable = true)
		{
			Id = id;
			Name = name;
			Price = price;
			Category = category;
			PreparationTime = prepTime;
			IsCustomizable = customizable;
			Description = "";
		}
	}
}
