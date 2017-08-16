
REM EXAMPLE
REM >sc LockType C:\ProgramFiles\Daenet\dtfservice.exe "Interfaces for LockType process

sc create Dtf.Service.%1 binPath= %2 start= auto
sc description Dtf.Service.%1 %3

REM C:\tfs\daenet.vs.com\DurableTaskFramework\DurableTask\DtfService\bin\debug\dtfservice.exe







