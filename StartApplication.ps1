docker run -d -p 1433:1433 --network nat --name sql -e sa_password=Password123! -e ACCEPT_EULA=Y microsoft/mssql-server-windows-developer

docker-compose up -d

$sqlContainerIp = docker inspect --format="{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}" sql
$mvcContainerIp = docker inspect --format="{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}" mvc

Write-Host –NoNewLine "sql container ip:" $sqlContainerIp
Write-Host ""
Write-Host –NoNewLine "mvc container ip:" $mvcContainerIp