using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//amaç servisin tek tip cevap döndürmesini sağlamak.
namespace kutuphaneServis.Interfaces
{
    public interface IResponse<T>
    {
        bool IsSuccess { get; } 
        string Message { get; }
        public T Data { get; set; }
    }
}
