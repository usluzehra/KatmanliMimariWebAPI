using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;





namespace KutuphaneCore.Entities
{

    [Index(nameof(UserId), nameof(RoleId), IsUnique = true)]

    public class UserRole : BaseEntity
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }



        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = default!;

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; } = default!;
    }
}
