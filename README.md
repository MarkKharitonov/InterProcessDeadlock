# Introduction
A simple program that demonstrates the deadlock between the parent and a child process 
described in this MSDN article - https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.standardoutput?view=net-9.0#remarks,
specifically the case when:
- the parent process first reads the entire stdout of the child process 
and only then proceeds to reading the stderr of that child process.
- the child process first writes to the stderr and overflows the internal buffer, thus being blocked until the buffer is read by the parent process, which is also blocked waiting for the stdout from that child process.

The program allows to control the number of lines written to the stderr by the child process as well as the content of that line. The deadlock is recognized when the child process does not return after 5 seconds.

# Examples
## Successful invocation
```
C:\work\InterProcessDeadlock [master ≡]> .\bin\Debug\net8.0\InterProcessDeadlock.exe parent 85 this is a somewhat long stderr output line
StdOut: Done.
StdErr: this is a somewhat long stderr output line #1
StdErr: this is a somewhat long stderr output line #2
StdErr: this is a somewhat long stderr output line #3
StdErr: this is a somewhat long stderr output line #4
StdErr: this is a somewhat long stderr output line #5
StdErr: this is a somewhat long stderr output line #6
StdErr: this is a somewhat long stderr output line #7
StdErr: this is a somewhat long stderr output line #8
StdErr: this is a somewhat long stderr output line #9
StdErr: this is a somewhat long stderr output line #10
StdErr: this is a somewhat long stderr output line #11
StdErr: this is a somewhat long stderr output line #12
StdErr: this is a somewhat long stderr output line #13
StdErr: this is a somewhat long stderr output line #14
StdErr: this is a somewhat long stderr output line #15
StdErr: this is a somewhat long stderr output line #16
StdErr: this is a somewhat long stderr output line #17
StdErr: this is a somewhat long stderr output line #18
StdErr: this is a somewhat long stderr output line #19
StdErr: this is a somewhat long stderr output line #20
StdErr: this is a somewhat long stderr output line #21
StdErr: this is a somewhat long stderr output line #22
StdErr: this is a somewhat long stderr output line #23
StdErr: this is a somewhat long stderr output line #24
StdErr: this is a somewhat long stderr output line #25
StdErr: this is a somewhat long stderr output line #26
StdErr: this is a somewhat long stderr output line #27
StdErr: this is a somewhat long stderr output line #28
StdErr: this is a somewhat long stderr output line #29
StdErr: this is a somewhat long stderr output line #30
StdErr: this is a somewhat long stderr output line #31
StdErr: this is a somewhat long stderr output line #32
StdErr: this is a somewhat long stderr output line #33
StdErr: this is a somewhat long stderr output line #34
StdErr: this is a somewhat long stderr output line #35
StdErr: this is a somewhat long stderr output line #36
StdErr: this is a somewhat long stderr output line #37
StdErr: this is a somewhat long stderr output line #38
StdErr: this is a somewhat long stderr output line #39
StdErr: this is a somewhat long stderr output line #40
StdErr: this is a somewhat long stderr output line #41
StdErr: this is a somewhat long stderr output line #42
StdErr: this is a somewhat long stderr output line #43
StdErr: this is a somewhat long stderr output line #44
StdErr: this is a somewhat long stderr output line #45
StdErr: this is a somewhat long stderr output line #46
StdErr: this is a somewhat long stderr output line #47
StdErr: this is a somewhat long stderr output line #48
StdErr: this is a somewhat long stderr output line #49
StdErr: this is a somewhat long stderr output line #50
StdErr: this is a somewhat long stderr output line #51
StdErr: this is a somewhat long stderr output line #52
StdErr: this is a somewhat long stderr output line #53
StdErr: this is a somewhat long stderr output line #54
StdErr: this is a somewhat long stderr output line #55
StdErr: this is a somewhat long stderr output line #56
StdErr: this is a somewhat long stderr output line #57
StdErr: this is a somewhat long stderr output line #58
StdErr: this is a somewhat long stderr output line #59
StdErr: this is a somewhat long stderr output line #60
StdErr: this is a somewhat long stderr output line #61
StdErr: this is a somewhat long stderr output line #62
StdErr: this is a somewhat long stderr output line #63
StdErr: this is a somewhat long stderr output line #64
StdErr: this is a somewhat long stderr output line #65
StdErr: this is a somewhat long stderr output line #66
StdErr: this is a somewhat long stderr output line #67
StdErr: this is a somewhat long stderr output line #68
StdErr: this is a somewhat long stderr output line #69
StdErr: this is a somewhat long stderr output line #70
StdErr: this is a somewhat long stderr output line #71
StdErr: this is a somewhat long stderr output line #72
StdErr: this is a somewhat long stderr output line #73
StdErr: this is a somewhat long stderr output line #74
StdErr: this is a somewhat long stderr output line #75
StdErr: this is a somewhat long stderr output line #76
StdErr: this is a somewhat long stderr output line #77
StdErr: this is a somewhat long stderr output line #78
StdErr: this is a somewhat long stderr output line #79
StdErr: this is a somewhat long stderr output line #80
StdErr: this is a somewhat long stderr output line #81
StdErr: this is a somewhat long stderr output line #82
StdErr: this is a somewhat long stderr output line #83
StdErr: this is a somewhat long stderr output line #84
StdErr: this is a somewhat long stderr output line #85
C:\work\InterProcessDeadlock [master ≡]>
```

## Deadlock
```
C:\work\InterProcessDeadlock [master ≡]> .\bin\Debug\net8.0\InterProcessDeadlock.exe parent 86 this is a somewhat long stderr output line
Deadlock
C:\work\InterProcessDeadlock [master ≡]>
```

# Fix
The code can easily be fixed, if the stderr and stdout of the child process are consumed simultaneously. The aforementioned MSDN article gives an example of how this can be achieved.

The fix is found on the fix branch:
```
C:\work\InterProcessDeadlock [master ≡]> git co fix
Switched to branch 'fix'
Your branch is up to date with 'origin/fix'.
C:\work\InterProcessDeadlock [fix ≡]> dotnet build
  Determining projects to restore...
  All projects are up-to-date for restore.
  InterProcessDeadlock -> C:\work\InterProcessDeadlock\bin\Debug\net8.0\InterProcessDeadlock.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:01.28

Workload updates are available. Run `dotnet workload list` for more information.
C:\work\InterProcessDeadlock [fix ≡]> .\bin\Debug\net8.0\InterProcessDeadlock.exe parent 86 this is a somewhat long stderr output line
StdOut: Done.
StdErr: this is a somewhat long stderr output line #1
StdErr: this is a somewhat long stderr output line #2
StdErr: this is a somewhat long stderr output line #3
StdErr: this is a somewhat long stderr output line #4
StdErr: this is a somewhat long stderr output line #5
StdErr: this is a somewhat long stderr output line #6
StdErr: this is a somewhat long stderr output line #7
StdErr: this is a somewhat long stderr output line #8
StdErr: this is a somewhat long stderr output line #9
StdErr: this is a somewhat long stderr output line #10
StdErr: this is a somewhat long stderr output line #11
StdErr: this is a somewhat long stderr output line #12
StdErr: this is a somewhat long stderr output line #13
StdErr: this is a somewhat long stderr output line #14
StdErr: this is a somewhat long stderr output line #15
StdErr: this is a somewhat long stderr output line #16
StdErr: this is a somewhat long stderr output line #17
StdErr: this is a somewhat long stderr output line #18
StdErr: this is a somewhat long stderr output line #19
StdErr: this is a somewhat long stderr output line #20
StdErr: this is a somewhat long stderr output line #21
StdErr: this is a somewhat long stderr output line #22
StdErr: this is a somewhat long stderr output line #23
StdErr: this is a somewhat long stderr output line #24
StdErr: this is a somewhat long stderr output line #25
StdErr: this is a somewhat long stderr output line #26
StdErr: this is a somewhat long stderr output line #27
StdErr: this is a somewhat long stderr output line #28
StdErr: this is a somewhat long stderr output line #29
StdErr: this is a somewhat long stderr output line #30
StdErr: this is a somewhat long stderr output line #31
StdErr: this is a somewhat long stderr output line #32
StdErr: this is a somewhat long stderr output line #33
StdErr: this is a somewhat long stderr output line #34
StdErr: this is a somewhat long stderr output line #35
StdErr: this is a somewhat long stderr output line #36
StdErr: this is a somewhat long stderr output line #37
StdErr: this is a somewhat long stderr output line #38
StdErr: this is a somewhat long stderr output line #39
StdErr: this is a somewhat long stderr output line #40
StdErr: this is a somewhat long stderr output line #41
StdErr: this is a somewhat long stderr output line #42
StdErr: this is a somewhat long stderr output line #43
StdErr: this is a somewhat long stderr output line #44
StdErr: this is a somewhat long stderr output line #45
StdErr: this is a somewhat long stderr output line #46
StdErr: this is a somewhat long stderr output line #47
StdErr: this is a somewhat long stderr output line #48
StdErr: this is a somewhat long stderr output line #49
StdErr: this is a somewhat long stderr output line #50
StdErr: this is a somewhat long stderr output line #51
StdErr: this is a somewhat long stderr output line #52
StdErr: this is a somewhat long stderr output line #53
StdErr: this is a somewhat long stderr output line #54
StdErr: this is a somewhat long stderr output line #55
StdErr: this is a somewhat long stderr output line #56
StdErr: this is a somewhat long stderr output line #57
StdErr: this is a somewhat long stderr output line #58
StdErr: this is a somewhat long stderr output line #59
StdErr: this is a somewhat long stderr output line #60
StdErr: this is a somewhat long stderr output line #61
StdErr: this is a somewhat long stderr output line #62
StdErr: this is a somewhat long stderr output line #63
StdErr: this is a somewhat long stderr output line #64
StdErr: this is a somewhat long stderr output line #65
StdErr: this is a somewhat long stderr output line #66
StdErr: this is a somewhat long stderr output line #67
StdErr: this is a somewhat long stderr output line #68
StdErr: this is a somewhat long stderr output line #69
StdErr: this is a somewhat long stderr output line #70
StdErr: this is a somewhat long stderr output line #71
StdErr: this is a somewhat long stderr output line #72
StdErr: this is a somewhat long stderr output line #73
StdErr: this is a somewhat long stderr output line #74
StdErr: this is a somewhat long stderr output line #75
StdErr: this is a somewhat long stderr output line #76
StdErr: this is a somewhat long stderr output line #77
StdErr: this is a somewhat long stderr output line #78
StdErr: this is a somewhat long stderr output line #79
StdErr: this is a somewhat long stderr output line #80
StdErr: this is a somewhat long stderr output line #81
StdErr: this is a somewhat long stderr output line #82
StdErr: this is a somewhat long stderr output line #83
StdErr: this is a somewhat long stderr output line #84
StdErr: this is a somewhat long stderr output line #85
StdErr: this is a somewhat long stderr output line #86
C:\work\InterProcessDeadlock [fix ≡]>
```