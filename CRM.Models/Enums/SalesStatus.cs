using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Models.Enums;

public enum SalesStatus : short
{
    New = 0,
    Pending = 1,
    Closed = 2,
    Failed = 3,
}
