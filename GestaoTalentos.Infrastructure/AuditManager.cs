namespace GestaoTalentos.Infrastructure;

using GestaoTalentos.Domain;
    
public class AuditManager
{
    private readonly IEnumerable<ILogObserver> _observers;

    public AuditManager(IEnumerable<ILogObserver> observers)
    {
        _observers = observers;
    }

    public async Task NotifyAsync(Log log)
    {
        foreach (var obs in _observers)
            await obs.UpdateAsync(log);
    }
}