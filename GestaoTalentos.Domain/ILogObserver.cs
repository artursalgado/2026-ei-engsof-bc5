namespace GestaoTalentos.Domain;

public interface ILogObserver
{
    Task UpdateAsync(Log log);
}