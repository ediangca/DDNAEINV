using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Entities
{
    public class Par
    {
        [Key]
        public required string parNo { get; set; }
        public required string lgu { get; set; }
        public required string fund { get; set; }
        public required string receivedBy { get; set; }
        public required string issuedBy { get; set; }
        public bool? postFlag { get; set; } = false;
        public bool? voidFlag { get; set; } = false;
        public required string createdBy { get; set; }
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }


        /*
      lgu: ['', Validators.required],
      fund: ['', Validators.required],
      parNo: ['', Validators.required],
      userID1: ['', Validators.required],
      userID2: ['', Validators.required],

    [PARNo] VARBINARY(50) NOT NULL PRIMARY KEY, 
    [LGU] NVARCHAR(MAX) NULL, 
    [Fund] NVARCHAR(MAX) NULL, 
    [ReceivedBy] NVARCHAR(50) NULL, 
    [IssuedBy] NVARCHAR(50) NULL, 
    [postFlag] BIT NULL DEFAULT ((0)), 
    [voidFlag] BIT NULL DEFAULT ((0)), 
    [CreatedBy] NVARCHAR(50) NULL , 
    [Date_Created] DATETIME NULL, 
    [Date_Updated] DATETIME NULL
         */

    }
}
