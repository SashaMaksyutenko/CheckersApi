using System.Diagnostics;

namespace CheckersApi.Engine;

public sealed class ChinookProcess : IDisposable
{
    private readonly Process _process;
    private readonly StreamWriter _stdin;
    private readonly StreamReader _stdout;

    public ChinookProcess(string exePath)
    {
        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        _process.Start();

        _stdin = _process.StandardInput;
        _stdout = _process.StandardOutput;

        Warmup();
    }

    private void Warmup()
    {
        // мінімальна команда, щоб engine прокинувся
        _stdin.WriteLine("xboard");
        _stdin.Flush();
    }

    public void SetPosition(string pdn)
    {
        _stdin.WriteLine($"setboard {pdn}");
        _stdin.Flush();
    }

    public string Search(int depth)
    {
        _stdin.WriteLine($"go depth {depth}");
        _stdin.Flush();

        // тут спрощено — далі будемо парсити
        return ReadUntil("bestmove");
    }

    private string ReadUntil(string marker)
    {
        string line;
        while ((line = _stdout.ReadLine()!) != null)
        {
            if (line.Contains(marker, StringComparison.OrdinalIgnoreCase))
                return line;
        }
        throw new InvalidOperationException("Engine output ended");
    }

    public void Dispose()
    {
        try { _process.Kill(); } catch { }
        _process.Dispose();
    }
}

