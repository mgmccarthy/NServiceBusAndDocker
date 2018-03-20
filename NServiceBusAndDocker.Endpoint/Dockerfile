# escape=`

FROM microsoft/windowsservercore
COPY bin/debug/ c:\\NServiceBus

#add new user, nsb, that will be the admin user we'll use install the endpoint
#add nsb to Administrators group
#add nsb to Performance Monitor Users
RUN net user /add nsb Password123 && `
net localgroup Administrators nsb /add && `
net localgroup "Performance Monitor Users" nsb /add

#install NSB endpoint
RUN powershell c:\\NServiceBus\\NServiceBus.Host.exe /install `
/serviceName:nsbendpoint `
/displayName:nsbendpoint `
/username:$env:computername\nsb `
/password:Password123 `
NServiceBus.Production
