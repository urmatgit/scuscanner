using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCUScanner.Models
{
   public  class Cart
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public Part Part { get; set; }
    }
}
