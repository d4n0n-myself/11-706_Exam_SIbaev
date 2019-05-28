namespace InfoExam2.Core
{
	public class OrderItem
	{
		public int Id { get; set; }
		public Dish Dish { get; set; }
		public int OrderId { get; set; }
	}
}