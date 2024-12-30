using InterProcessDeadlock;
using System.Reflection;

string s_exePath = Assembly.GetExecutingAssembly().Location[..^3] + "exe";

if (!File.Exists(s_exePath))
{
    Console.Error.WriteLine($"Code Bug - incorrect exe path '{s_exePath}'");
    return 1;
}

if (args.Length >= 1)
{
    if (args[0] == "parent")
    {
        var processRunner = new ProcessRunner();
        List<string> stdOut = [];
        args[0] = "child";
        var cts = new CancellationTokenSource(5000);
        try
        {
            var res = await processRunner.Run(s_exePath, IProcessRunner.Mode.RethrowAndIfNonZeroExitCode, string.Join(' ', args), notifyStdOutLine: stdOut.Add, cancellationToken: cts.Token);
            foreach (string line in stdOut)
            {
                Console.WriteLine("StdOut: " + line);
            }
            if (res.StdErr != null)
            {
                string[] stdErrLines = res.StdErr.Split(Environment.NewLine);
                var count = stdErrLines.Length;
                if (stdErrLines[^1] == "")
                {
                    --count;
                }
                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine("StdErr: " + stdErrLines[i]);
                }
            }
        }
        catch (ProcessRunnerException exc) when (exc.InnerException is OperationCanceledException)
        {
            Console.WriteLine("Deadlock");
            return 1;
        }
        return 0;
    }
    if (args[0] == "child")
    {
        int lineCount = 1;
        string lineContent = "Line";
        if (args.Length >= 2)
        {
            lineCount = int.Parse(args[1]);
        }
        if (args.Length >= 3)
        {
            lineContent = string.Join(' ', args.Skip(2));
        }
        for (int i = 0; i < lineCount; ++i)
        {
            Console.Error.WriteLine($"{lineContent} #{i + 1}");
        }
        Console.WriteLine("Done.");
        return 0;
    }
}

Console.Error.WriteLine("Did not recognize the command line arguments");
return 1;
