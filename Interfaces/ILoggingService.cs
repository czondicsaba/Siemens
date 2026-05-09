namespace Siemens.Internship2026.GradeBook.Interfaces
{
    public interface ILoggingService
    {
        void Log(string message, DateTime _lastUpdated);
        void Log(string v);
    }
}
