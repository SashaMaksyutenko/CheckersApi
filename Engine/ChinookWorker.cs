namespace CheckersApi.Engine;

public sealed class ChinookWorker
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly ChinookProcess _engine;

    public ChinookWorker(string exePath)
    {
        _engine = new ChinookProcess(exePath);
    }

    public async Task<string> SearchAsync(
        string pdn,
        int depth,
        CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            _engine.SetPosition(pdn);
            return _engine.Search(depth);
        }
        finally
        {
            _lock.Release();
        }
    }
}
