#!/bin/bash

RESOURCE_GROUP="aca-grpc-service1-rg"
LOCATION="canadacentral"
ENVIRONMENT="env-grpc-service1-containerapps"
API_NAME="grpc-service"
GITHUB_USERNAME="thevman"
ACR_NAME="acagrpc1"$GITHUB_USERNAME


az login -t "ff87514a-dc3e-4e9a-a805-fa5dd9b76e28"

az group create --name $RESOURCE_GROUP --location "$LOCATION"

az acr create \
  --resource-group $RESOURCE_GROUP \
  --name $ACR_NAME \
  --sku Basic \
  --admin-enabled true

az acr build --registry $ACR_NAME --image $API_NAME ./grpcService

az containerapp env create \
  --name $ENVIRONMENT \
  --resource-group $RESOURCE_GROUP \
  --location "$LOCATION"

az containerapp create \
  --name $API_NAME \
  --resource-group $RESOURCE_GROUP \
  --environment $ENVIRONMENT \
  --image $ACR_NAME.azurecr.io/$API_NAME \
  --target-port 8000 \
  --ingress 'external' \
  --registry-server $ACR_NAME.azurecr.io \
  --transport http2 \
  --min-replicas 1 \
  --max-replicas 1 \
  --cpu 0.25 --memory 0.5Gi \
  --revision-suffix v20230630 \
  --query properties.configuration.ingress.fqdn



#  az containerapp auth microsoft update --debug --verbose \
# --name album-api \
# --resource-group album-containerapps \
# --client-id 738d3dbd-5c4b-4d6d-8e5f-7765280795e0 \
# --client-secret TgE8Q~dssICYDE0rvJ1EFMi33zlSQLFwfEpzcaz0 \
# --tenant-id ff87514a-dc3e-4e9a-a805-fa5dd9b76e28