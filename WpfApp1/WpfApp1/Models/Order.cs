using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class Order
    {
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public int CustomerNumber { get; set; }
    }
}
