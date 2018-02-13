using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCUScanner.Models
{
    public class SCUItem
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string ID { get; set; }
        public string MacAddress { get; set; }
        public DateTime DateWithTime { get; set; }
        public int Speed { get; set; }
        [MaxLength(32)]
        public string Location { get; set; }
        [MaxLength(255)]
        public string Comment { get; set; }
        [MaxLength(32)]
        public string Operator { get; set; }
    }
}
