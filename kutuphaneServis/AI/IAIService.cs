using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.AI
{
    public interface IAIService
    {
        Task<string> GenerateTextAsync(string prompt , CancellationToken ct = default); 
    }
}
