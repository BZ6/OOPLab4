namespace FoodDelivery.Decorator
{
	public interface IOrderDecorator
	{
		string GetDescription();
		decimal GetCost();
		int GetPreparationTime();
	}
}
