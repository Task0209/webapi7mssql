using System;


namespace secondapp.DTOs
{
    public class FoodDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
