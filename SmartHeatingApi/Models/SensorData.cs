using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHeatingApi.Models
{
    public class SensorData
    {
        public int SensorDataID { get; set; }
        public int SensorID { get; set; }
        public DateTime TSCreated { get; set; }
        public int SensorDataType { get; set; } // ToDo: make Enum
        public string Unit { get; set; }
        public decimal Value { get; set; }
        public string ValueText { get; set; }
    }
}

/*
 * create table SensorData (
	SensorDataID  INTEGER PRIMARY KEY, --Autoincrement in SQLLite
	SensorID INT not null,
	TSCreated datetime not null DEFAULT CURRENT_TIMESTAMP,
	SensorDataType int,
	Unit nvarchar(50),
	Value real,
	ValueText nvarchar(255)
);
*/