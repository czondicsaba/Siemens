using Siemens.Internship2026.GradeBook.Interfaces;

namespace Siemens.Internship2026.GradeBook.Services
{
    public class LoggingService:ILoggingService
    {
        public void Log(string message)
        {
            Console.WriteLine($"[LOG] {DateTime.UtcNow}: {message}");
        }
    }
}
