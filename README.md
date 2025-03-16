# Azure Key Vault + Event Grid PoC

## ✨ Visão Geral
Este repositório contém uma Prova de Conceito (PoC) demonstrando a integração entre **Azure Key Vault**, **Azure Event Grid** e **Azure Functions** para monitorar a expiração de secrets e acionar notificações automáticas.

## ⚡ Arquitetura da Solução
1. **Azure Key Vault**: Armazena secrets sensíveis e define datas de expiração.
2. **Azure Event Grid**: Monitora o Key Vault e gera eventos quando uma secret está próxima da expiração.
3. **Azure Function (C#)**: Processa os eventos do Event Grid e envia alertas via e-mail (usando SendGrid ou Microsoft Graph API).

## 🛠 Configuração do Ambiente

### 1. **Criando os Recursos no Azure**
Execute o seguinte comando no **Azure CLI** para criar os recursos necessários:

```bash
az deployment group create --resource-group MeuGrupo --template-file deploy.bicep --parameters sendGridApiKey='SUA_SENDGRID_API_KEY'
```

Isso cria:
- ✨ **Azure Key Vault**
- ✨ **Azure Function App**
- ✨ **Azure Event Grid Topic**
- ✨ **Subscription do Event Grid para a Function**

### 2. **Publicando a Azure Function**
1. Instale as ferramentas de desenvolvimento da Azure Function:
   ```bash
   npm install -g azure-functions-core-tools@4 --unsafe-perm true
   ```
2. Crie e publique a Azure Function:
   ```bash
   func azure functionapp publish KeyVaultAlertFunction
   ```

## 📝 Como Testar

1. **Criar uma secret com expiração curta no Key Vault**:
   ```bash
   az keyvault secret set --vault-name "MyKeyVaultAlert" --name "MinhaSecret" --value "meuValor" --expires "2024-03-15T00:00:00Z"
   ```
2. O **Event Grid** dispara um evento quando a secret estiver próxima da expiração.
3. A **Azure Function** recebe o evento e envia um alerta para o e-mail configurado.

## 🎯 Benefícios
- ✅ **Automatiza o monitoramento de secrets**
- ✅ **Escalável e baseado em eventos**
- ✅ **Pode ser integrado com Teams, Webhooks, SMS, etc.**

## 🔧 Tecnologias Utilizadas
- **Azure Key Vault**
- **Azure Event Grid**
- **Azure Functions (.NET Core / C#)**
- **SendGrid API (para envio de e-mails)**
- **Bicep (Infraestrutura como Código - IaC)**

## 👥 Contribuição
Sinta-se à vontade para abrir issues e enviar PRs para melhorias nesta PoC.

---
Criado por Jailton S. Sales Jr - Microsoft

