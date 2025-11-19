Реализованные паттерны:
- State - Управление состоянием заказа
	- **Классы** PreparingState, OutForDeliveryState, DeliveredState, CancelledState
	- **Интерфейс** IOrderState
	- **Методы** ProcessOrder(), CancelOrder(), DeliverOrder()

- Strategy - Разные типы заказов
	- IOrderTypeStrategy, StandardOrderStrategy, CustomOrderStrategy
	- Разные алгоритмы расчета стоимости и времени подготовки

- Command - Операции с заказом
	- IOrderCommand, AddStandardItemCommand, AddCustomItemCommand, UpdateOrderCommand
	- Методы Execute(), Undo(), история команд

- Factory - Создание заказов разных типов
	- IOrderFactory, OrderFactory
	- CreateStandardOrder(), CreateCustomOrder()

- Facade - Упрощенный интерфейс
	- OrderService скрывает сложность системы
	- Единая точка входа для клиентского кода
