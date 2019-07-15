using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHeatingApi.Models
{
    public class DeviceEvent
    {
        public int DeviceEventID { get; set; }
        public Guid DeviceID { get; set; }
        public DateTime TSCreated { get; set; }
        public string Event { get; set; }
    }
}

/*
    CREATE TABLE [dbo].[DeviceEvents](
	[DeviceEventID] [int] IDENTITY(1,1) NOT NULL,
	[DeviceID] [uniqueidentifier] NOT NULL,
	[TSCreated] [datetime] NOT NULL,
	[Event] [nvarchar](4000) NULL,
*/