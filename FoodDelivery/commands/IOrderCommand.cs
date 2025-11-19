namespace FoodDelivery.Commands
{
	public interface IOrderCommand
	{
		string Description { get; }
		void Execute();
		void Undo();
	}
}
