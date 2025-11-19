namespace FoodDelivery.Strategies
{
	public interface IOrderTypeStrategy
	{
		decimal CalculateTotal(decimal subtotal);
		int GetPreparationTime(int basePreparationTime);
		string GetOrderType();
		bool CanAddCustomItems();
	}
}
