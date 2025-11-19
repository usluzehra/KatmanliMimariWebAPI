using AutoMapper.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.Helpers.ZLog
{
    public class ZLoggerService : IZLogger
    {
        private const string LOG_DIR = @"C:\Users\zehra\Desktop\projeler\KatmanliMimariGenel\UdemyKutuphaneAPI\UdemyKutuphaneApi\logs\"; // C:\Users\zehra\Desktop\projeler\KatmanliMimariGenel\UdemyKutuphaneAPI\UdemyKutuphaneApi\logs\
        //_logDir de klasörün yolunu tutuyoruz
        private readonly string _logDir;
        //Yarış olmasın diye
        private  readonly object _lock = new object();

        public ZLoggerService()
        {
            // logs klasörünü proje köküne sabitle

            //string baseDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
            //_logDir = Path.Combine(baseDir, "logs");

            _logDir = LOG_DIR;
            Directory.CreateDirectory(_logDir); // yoksa oluştur



            //_logDir = ResolveLogDir();
            //Directory.CreateDirectory(_logDir);

            if (!Directory.Exists(_logDir))
                Directory.CreateDirectory(_logDir);
        }
        public void Log(string message, LogLevelEnum level, [CallerMemberName] string member = "",[CallerFilePath] string file = "")
        {

            //birden fazla thread aynı anda log yazmaya çalışırsa sorun olmasın diye lock kullanıyoruz
            lock (_lock)
            { string logFilePath = Path.Combine(_logDir, $"{DateTime.Now:yyyy-MM-dd}.log");
                Directory.CreateDirectory(_logDir);
                var source = $"{Path.GetFileNameWithoutExtension(file)}.{member}";
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
        }

        /*
        // -------- yeni: Exception içeriğini detaylı yaz --------
        private static string FormatException(Exception ex)
        {
            var sb = new StringBuilder();
            var cur = ex;
            int depth = 0;

            while (cur != null)
            {
                string prefix = depth == 0 ? "EX" : $"INNER[{depth}]";
                sb.AppendLine($"{prefix}: {cur.GetType().FullName}: {cur.Message}");
                sb.AppendLine(cur.StackTrace);
                cur = cur.InnerException;
                depth++;
            }
            return sb.ToString();
        }

        */
        public void Info(string message)
        {
            Log(message, LogLevelEnum.Info);
        }
        public void Warn(string message)
        {
            Log(message, LogLevelEnum.Warn);
        }
        public void Error(string message)
        {
            Log(message, LogLevelEnum.Error);
        }
        public void Error(Exception ex, string message)
        {
            Log($"{message}{Environment.NewLine}{ex}", LogLevelEnum.Error);
        }

        public void Error(Exception ex)
        {
            Log($"{ex}", LogLevelEnum.Error);
        }
        public void Debug(string message)
        {
            Log(message, LogLevelEnum.Debug);
        }
    }
}
