$ServiceName = Read-Host -Prompt 'Name of Service to uninstall'

$ServiceName = "Dtf.Service." + $ServiceName

sc.exe delete $ServiceName