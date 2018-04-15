using System.Collections.Generic;

namespace WebAPI.Models.Orders
{
    public class OrderViewModel
    {
        public int oid { get; set; }
        public string dt { get; set; }
        public string departmentName { get; set; }
        public bool state { get; set; }
    }
}