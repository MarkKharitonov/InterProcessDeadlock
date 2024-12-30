using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace InterProcessDeadlock;

public record ProcessRunnerResult(int ExitCode, string? StdErr, ExceptionDispatchInfo? ExceptionDispatchInfo = null);

public class ProcessRunnerException(string ExecutablePath, string? Args, int ExitCode, Exception? innerException = null) :
    Exception($"Running << {ExecutablePath} {Args} >> failed with exit code {ExitCode}", innerException);

public interface IProcessRunner
{
    public enum Mode
    {
        NeverThrow,
        RethrowReturnExitCode,
        RethrowAndIfNonZeroExitCode
    }

    Task<ProcessRunnerResult> Run(string executablePath,
        Mode mode,
        string? args = null,
        string? workingDirectory = null,
        Action<ProcessStartInfo>? beforeInvoke = null,
        Action<string>? notifyStdOutLine = null,
        CancellationToken cancellationToken = default);
}

public class ProcessRunner : IProcessRunner
{
    public async Task<ProcessRunnerResult> Run(string executablePath,
        IProcessRunner.Mode mode,
        string? args = null,
        string? workingDirectory = null,
        Action<ProcessStartInfo>? beforeInvoke = null,
        Action<string>? notifyStdOutLine = null,
        CancellationToken cancellationToken = default)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = workingDirectory
            }
        };

        string? stdErr = null;
        try
        {
            beforeInvoke?.Invoke(process.StartInfo);

            process.Start();

            Task[] tasks = [
                ReadStdOutAsync(notifyStdOutLine, process, cancellationToken),
                process.StandardError.ReadToEndAsync(cancellationToken)
            ];
            await Task.WhenAll(tasks);
            stdErr = ((Task<string>)tasks[1]).Result;
        }
        catch (Exception ex)
        {
            if (mode == IProcessRunner.Mode.NeverThrow)
            {
                return new ProcessRunnerResult(-1, null, ExceptionDispatchInfo.Capture(ex));
            }
            throw new ProcessRunnerException(executablePath, args, -1, ex);
        }

        if (mode == IProcessRunner.Mode.RethrowAndIfNonZeroExitCode && process.ExitCode != 0)
        {
            throw new ProcessRunnerException(executablePath, args, process.ExitCode);
        }

        return new ProcessRunnerResult(process.ExitCode, stdErr);
    }

    private static async Task ReadStdOutAsync(Action<string>? notifyStdOutLine, Process process, CancellationToken cancellationToken)
    {
        string? line;
        while ((line = await process.StandardOutput.ReadLineAsync(cancellationToken)) != null)
        {
            notifyStdOutLine?.Invoke(line);
        }
    }
}
