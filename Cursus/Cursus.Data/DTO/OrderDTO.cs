﻿namespace Cursus.Data.DTO
{
	public class OrderDTO
	{
		public int OrderId { get; set; }
		public int CartId { get; set; }
		public double Amount { get; set; }
		public double PaidAmount { get; set; }
		public DateTime DateCreated { get; set; }
		public string Status { get; set; }
	}
}