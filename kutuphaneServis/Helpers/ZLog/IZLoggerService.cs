using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.Helpers.ZLog
{
    public interface IZLogger
    {
        void Log(string message, LogLevelEnum level, [CallerMemberName] string member = "",[CallerFilePath] string file = "");
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Error(Exception ex, string message);
        void Error(Exception ex);
        void Debug(string message);

    }
}
