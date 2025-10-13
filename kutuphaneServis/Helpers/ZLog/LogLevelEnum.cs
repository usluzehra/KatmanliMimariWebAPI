using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.Helpers.ZLog
{
    //enum kullanımı: sabit değerler tutmak için kullanılır
    //log seviyelerini tanımlıyoruz
    //Info: Genel bilgi mesajları
    //Warn: Uyarı mesajları, potansiyel sorunlar
    //Error: Hata mesajları, uygulama hataları
    //Debug: Hata ayıklama mesajları, geliştirme sürecinde kullanılır

    public enum LogLevelEnum
    {
        Info,
        Warn,
        Error,
        Debug
    }
}
