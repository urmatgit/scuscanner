using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCUScanner.Models
{
    public class SCUItem
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }
        /// <summary>
        /// String eg. SCU310 (max of 6 characters)
        /// </summary>
        [MaxLength(6)]
        public string UnitName { get; set; }
        /// <summary>
        /// Text 21 bytes wide
        /// </summary>
        [MaxLength(21)]
        public string SerialNo { get; set; }
        /// <summary>
        /// Broadcast identity	Text String  
        /// </summary>
        public string BroadCastId { get; set; }
        /// <summary>
        /// Location	Text string (32 chars max?)
        /// </summary>
        [MaxLength(32)]
        public string Location { get; set; }
        /// <summary>
        /// Notes	Text string (255 chars max?)
        /// </summary>
        [MaxLength(255)]
        public string Notes { get; set; }
        /// summary>
        /// Speed (RPM)	Integer eg 5312
        /// </summary>
        public int Speed { get; set; }
        /// <summary>
        /// Operator	Text String
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// Alarm Speed
        /// </summary>
        public int AlarmSpeed { get; set; }
        /// <summary>
        /// Date/Time
        /// </summary>
        public DateTime DateWithTime { get; set; }
        /// <summary>
        /// Hours Elapsed
        /// </summary>
        public int HoursElapsed { get; set; }
        /// <summary>
        /// Alarm Hours
        /// </summary>
        public int AlarmHours { get; set; }
        public SCUItem()
        {
            UnitName = "";
            SerialNo = "";
            BroadCastId = "";
            Location = "";
            Notes = "";
            Operator = "";
        }
        [Ignore]
        public bool IsDelete { get; set; }
    }
}
