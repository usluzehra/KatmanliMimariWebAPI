using kutuphaneServis.Interfaces;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.Response
{
    public class ResponseGeneric<T> : Response, IResponse<T>
    {
        public T Data { get; set; }
        public ResponseGeneric( T data ,bool issuccess, string message) : base(issuccess, message)
        {
            Data = data;
        }

        public static ResponseGeneric<T> Success(T data, string message= "")
        {
            return new  ResponseGeneric<T>(data ,true, message); 
        }

        public static ResponseGeneric<T> Error(string message = "")
        {
            return  new ResponseGeneric<T>(default(T), false, message);
        }
    }
}
