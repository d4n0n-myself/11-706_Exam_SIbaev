using System;

namespace InfoExam2.Core
{
	public class PromoCode
	{
		public int Id { get; set; }
		public string Code { get; set; }	
		public double Discount { get; set; }
		public int UseCount { get; set; }
		public DateTime StoreDeadLine { get; set; }
		public int UseLimit { get; set; }	
	}
}