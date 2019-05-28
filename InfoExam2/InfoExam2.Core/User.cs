namespace InfoExam2.Core
{
	public class User
	{
		public int Id { get; set; }
		public string Login { get; set; }
		public string Password
		{
			get; set;
		}
		
		public bool Admin { get; set; } = false;

		public int CurrentOrder { get; set; }
	}
	
}