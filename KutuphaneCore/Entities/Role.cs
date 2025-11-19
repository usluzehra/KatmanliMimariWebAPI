using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KutuphaneCore.Entities
{
    public class Role: BaseEntity
    {
        public string Name { get; set; } = default!;
        public string NormalizedName { get; set; } = default!;
    }
}
