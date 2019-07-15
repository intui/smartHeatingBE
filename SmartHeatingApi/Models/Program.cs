using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHeatingApi.Models
{
    public class Program
    {
        public int ProgramID { get; set; }
        public string ProgramText { get; set; }
        public int ProgramType { get; set; }
        public bool Active { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string ProgramContent { get; set; }
        public Guid DeviceID { get; set; }
        public DateTime TSCreated { get; set; }
        public DateTime TSUpdated { get; set; }

    }
}

/*
 * CREATE TABLE [dbo].[Programs](
	[ProgramID] [int] IDENTITY(1,1) NOT NULL,
	[ProgramText] [nvarchar](255) NULL,
	[ProgramType] [int] NULL,
	[Active] [bit] NULL,
	[ValidFrom] [date] NULL,
	[ValidTo] [date] NULL,
	[Program] [nvarchar](MAX) NULL,
	[DeviceID] [uniqueidentifier] NOT NULL,
	[TSCreated] [datetime] NOT NULL,
	[TSUpdated] [datetime] NOT NULL,
*/