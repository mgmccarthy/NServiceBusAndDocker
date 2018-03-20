# escape=`

FROM microsoft/azure-storage-emulator
EXPOSE 10000 10001 10002

COPY ChangeEmulatorConfigIpToContainerIp.ps1 .
#CMD powershell .\ChangeEmulatorConfigIpToContainerIp.ps1

COPY ChangeNginxConfigIpToContainerIp.ps1 .
#CMD powershell .\ChangeNginxConfigIpToContainerIp.ps1
