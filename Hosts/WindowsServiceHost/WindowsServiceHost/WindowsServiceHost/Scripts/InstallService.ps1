$ServiceName = Read-Host -Prompt 'Input the name of this service'
$ServiceDescription = Read-Host -Prompt 'Input the description of this service'
$Path = Read-Host -Prompt 'Input path of your service'

$ServiceName = "Dtf.Service." + $ServiceName

sc.exe create $ServiceName binPath= $Path start= auto
sc.exe description $ServiceName $ServiceDescription