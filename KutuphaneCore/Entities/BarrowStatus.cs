using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KutuphaneCore.Entities
{
    public enum BorrowStatus
    {
        Pending,
        Approved,
        Rejected,
        CheckedOut,
        Returned,
        Cancelled
    }
}
