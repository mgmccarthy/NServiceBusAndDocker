param(
	[string]$resourceGroupName, 
	[string]$storageAccountName,
	[string]$location
)

az group create --name $resourceGroupName --location $location
az storage account create --location eastus --name $storageAccountName --resource-group $resourceGroupName --sku "Standard_LRS"
az storage account show-connection-string --name $storageAccountName --resource-group $resourceGroupName