using System;
using System.Collections.Generic;

namespace SuperLibrary.Web.Models.API
{
    public class LoanApiViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public List<LoanItemApiViewModel> LoanItems { get; set; }
    }

    public class LoanItemApiViewModel
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public int Quantity { get; set; }
    }
}
