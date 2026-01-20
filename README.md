
# DevTrack API

DevTrack API é uma API REST para gerenciamento de projetos e tarefas, desenvolvida em .NET (C#) como projeto de portfólio.  
A aplicação fornece autenticação segura com JWT e endpoints protegidos para controle completo de projetos e tarefas.

--- 

## Stack

- .NET (C#)
- ASP.NET Core
- Entity Framework Core
- JWT (Bearer)
- BCrypt
- Swagger / OpenAPI

---

## Visão Geral

- API REST desenvolvida em ASP.NET Core
- Autenticação stateless utilizando JWT
- Persistência de dados com Entity Framework Core
- Endpoints protegidos por autenticação
- Estrutura organizada, preparada para evolução

---

## Funcionalidades

### Autenticação
- Cadastro de usuários
- Login com geração de token JWT
- Validação de token em rotas protegidas

### Projetos
- Listagem de projetos
- Criação de projetos
- Atualização de projetos
- Exclusão de projetos
- Paginação, busca e ordenação

### Tarefas
- Criação de tarefas vinculadas a projetos
- Listagem de tarefas por projeto
- Atualização do título da tarefa
- Exclusão de tarefas
- Atualização de status da tarefa:
  - Pendente
  - Em andamento
  - Concluída

---

## Autenticação

A API utiliza autenticação JWT no formato Bearer Token.

```

Authorization: Bearer {token}

````

Todas as rotas de projetos e tarefas exigem autenticação.

---

## Endpoints Principais

### Auth
- POST /api/auth/register
- POST /api/auth/login

### Projetos
- GET /api/projects
- POST /api/projects
- GET /api/projects/{id}
- PUT /api/projects/{id}
- DELETE /api/projects/{id}

### Tarefas
- POST /api/tasks
- GET /api/tasks/by-project/{projectId}
- PUT /api/tasks/{id}
- PATCH /api/tasks/{id}/status
- DELETE /api/tasks/{id}

---

## Executar o Projeto

```bash
dotnet restore
dotnet run
````

A API estará disponível em:

```
http://localhost:5273
```

---

## Documentação

A documentação interativa da API está disponível via Swagger:

```
http://localhost:5273/swagger
```

---

## Estrutura de Pastas

```
DevTrack.API/
├── Controllers/
├── Data/
├── DTOs/
├── Models/
├── Services/
├── Program.cs
└── appsettings.json
```

---

## Boas Práticas Aplicadas

* Separação clara de responsabilidades
* Uso de DTOs para isolamento do domínio
* Hash seguro de senhas com BCrypt
* Autenticação stateless
* Padrões REST
* Código organizado e legível
* Validações no backend

```
```
