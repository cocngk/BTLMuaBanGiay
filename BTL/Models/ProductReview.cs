using System;

namespace ShoeStore.Models
{
    public class ProductReview
    {
        public int ProductId { get; set; }
        public string UserKey { get; set; }
        public string CustomerName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Stars
        {
            get
            {
                return new string('★', Rating) +
                       new string('☆', 5 - Rating);
            }
        }
    }
}
