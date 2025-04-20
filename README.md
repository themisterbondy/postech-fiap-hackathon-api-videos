# 🚀 Postech.Fiap.Hackathon.VideoProcessing.WebApi

> API REST que integra com o sistema de processamento de vídeos do Hackathon FIAP, permitindo o upload de arquivos, consulta de status e interação com o fluxo de processamento.

---

## 🌐 Visão Geral

A `WebApi` é o ponto de entrada para os usuários interagirem com o sistema. Através dela, é possível:

- Realizar upload de vídeos
- Autenticar usuários (ASP.NET Identity)
- Consultar status de processamento
- Disparar o envio para a fila

---

## 🧱 Tecnologias Utilizadas

- **.NET 9**
- **Minimal APIs**
- **Entity Framework Core**
- **ASP.NET Identity**
- **Azure Blob Storage**
- **Azure Queue Storage**
- **FluentValidation**
- **Serilog**
- **Swagger/OpenAPI**

---

## 🚀 Como Executar

```bash
dotnet build
dotnet run --project src/Postech.Fiap.Hackathon.VideoProcessing.WebApi
```

Ou via Docker:

```bash
docker build -t fiap-hackathon-webapi .
docker run --env-file .env -p 8080:8080 fiap-hackathon-webapi
```

---

## 📃 Swagger

Disponível em:

```
http://localhost:8080/swagger
```

---

## ⚖️ Funcionalidades

- `POST /videos` - Upload de vídeo
- `GET /videos/{id}` - Obter status de um vídeo
- `POST /auth/login` - Login de usuário
- `POST /auth/register` - Registro de usuário

---

## 📆 CI/CD

Pipeline GitHub Actions:
- Build da aplicação
- Execução de testes
- Publicação de imagem Docker
- Deploy com Helm no AKS

---

## 📍 Observabilidade

- Logs estruturados com Serilog
- HealthChecks configurados para Blob e Database

---