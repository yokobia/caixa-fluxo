# Desafio Técnico: Sistema de Fluxo de Caixa

Este projeto consiste em uma aplicação de controle de fluxo de caixa para comerciantes, permitindo o registro de lançamentos diários (créditos e débitos) e a consulta de relatórios consolidados de saldo diário. A solução foi desenvolvida utilizando as melhores práticas do ecossistema .NET, focando em alta performance, resiliência e testabilidade.

## 🚀 Diferenciais Técnicos e Resiliência
*   **Alta Performance Concorrente:** Para atender ao requisito de alto volume de acessos (ex: picos de 50 chamadas/segundo), a persistência em memória foi implementada utilizando a coleção thread-safe `ConcurrentBag`. Isso evita gargalos de travamento (`lock`) que ocorreriam com listas tradicionais em cenários de concorrência.
*   **Isolamento de Regras:** Validações de entrada desacopladas usando a biblioteca profissional **FluentValidation**.
*   **Testabilidade:** Suíte de testes unitários robusta cobrindo as regras de negócio de fluxo e a matemática crítica por trás do saldo consolidado.

---

## 🏛️ Arquitetura do Sistema

A aplicação foi estruturada seguindo os princípios de **Clean Architecture** (Arquitetura Limpa) e os conceitos do **SOLID**, garantindo o desacoplamento evidente entre as camadas e facilitando a manutenção e evolução do software.

*   📂 **CaixaFluxoDesafio** (Pasta Raiz)
    *   📂 **CaixaFluxo.Domain:** Entidades de negócio e regras puras (Isolada)
    *   📂 **CaixaFluxo.Application:** Casos de uso (Use Cases), DTOs, Validadores e Interfaces
    *   📂 **CaixaFluxo.Infrastructure:** Persistência em memória (ConcurrentBag) e Repositórios
    *   📂 **CaixaFluxo.API:** Controladores HTTP, Rotas REST e configuração de DI
    *   📂 **CaixaFluxo.Tests:** Testes Unitários automatizados com xUnit e Moq

### 1. Diagrama de Camadas e Dependências (Clean Architecture)
Este diagrama representa o isolamento do ecossistema. A regra de ouro aqui é: as camadas externas conhecem as internas, mas as internas nunca conhecem nada do mundo externo.

    ┌─────────────────────────────────────────────────────────┐
    │                 CAIXAFLUXO.API (Apresentação)           │
    │  - Controllers HTTP                                     │
    │  - Program.cs / Configuração de DI                      │
    └────────────────────┬───────────────┬────────────────────┘
                         │               │
                         │               ▼
                         │     ┌──────────────────────────────┐
                         │     │  CAIXAFLUXO.INFRASTRUCTURE   │
                         │     │  - InMemoryDbContext         │
                         │     │  - LancamentoRepository      │
                         └─────┼───────────────┬──────────────┘
                               │               │
                               ▼               ▼
    ┌─────────────────────────────────────────────────────────┐
    │                 CAIXAFLUXO.APPLICATION                  │
    │  - Use Cases (Registrar / Consolidar)                   │
    │  - Interfaces (ILancamentoRepository)                   │
    │  - Validators (FluentValidation) / DTOs                 │
    └────────────────────────────┬────────────────────────────┘
                                 │
                                 ▼
    ┌─────────────────────────────────────────────────────────┐
    │                    CAIXAFLUXO.DOMAIN                    │
    │  - Regras de Negócio Puras / Entidades Core             │
    │  - (Nível máximo de isolamento - dependência zero)      │
    └─────────────────────────────────────────────────────────┘

### 2. Diagrama de Fluxo de Dados (Registro de Lançamento)
Este diagrama detalha o comportamento interno do sistema desde o momento em que um cliente realiza um disparo HTTP POST até o salvamento seguro e resiliente na memória do servidor.

    ┌───────────────────┐
    │ Cliente / Postman │
    └──────┬────────────┘
           │
           │ HTTP POST /api/lancamentos (JSON)
           ▼
    ┌────────────────────────────────────────────────────────┐
    │ CaixaFluxo.API -> LancamentosController                │
    └──────┬─────────────────────────────────────────────────┘
           │
           │ Injeta e executa o Caso de Uso
           ▼
    ┌────────────────────────────────────────────────────────┐
    │ CaixaFluxo.Application -> RegistrarLancamentoUseCase   │
    └───┬────────────────────────────────────────────────────┘
        │
        ├─► [ Validação ] ──► Executa CriarLancamentoRequestValidator (FluentValidation)
        │                     (Se falhar: Lança erro e o Controller retorna 400 BadRequest)
        │
        │ Se válido: Repassa dados para a abstração do repositório
        ▼
    ┌────────────────────────────────────────────────────────┐
    │ CaixaFluxo.Application -> ILancamentoRepository        │
    └───┬────────────────────────────────────────────────────┘
        │
        │ Inversão de Dependência ativa a classe concreta
        ▼
    ┌────────────────────────────────────────────────────────┐
    │ CaixaFluxo.Infrastructure -> LancamentoRepository      │
    └──────┬─────────────────────────────────────────────────┘
           │
           │ Escrita assíncrona não bloqueante
           ▼
    ┌────────────────────────────────────────────────────────┐
    │ CaixaFluxo.Infrastructure -> InMemoryDbContext         │
    │ ──► [ Persistência ] ──► Armazena no ConcurrentBag     │
    └────────────────────────────────────────────────────────┘

---

## 🛠️ Tecnologias Utilizadas

*   **.NET 8.0 (LTS)** executado sob o **SDK 10**
*   **xUnit** & **Moq** (Testes Automatizados)
*   **FluentValidation** (Validações de Contrato)
*   **ASP.NET Core Web API**

---

## 🔧 Como Executar o Projeto

### Pré-requisitos
*   SDK do .NET instalado na máquina.

### 1. Clonar e Restaurar as Dependências
Abra o terminal na pasta raiz do projeto e execute:
```bash
dotnet restore
```

### 2. Executar a Suíte de Testes Automatizados
Para rodar os testes unitários e validar as regras matemáticas de saldo:
```bash
dotnet test
```

### 3. Executar a API
Para subir o servidor localmente:
```bash
dotnet run --project CaixaFluxo.API/CaixaFluxo.API.csproj
```
A API ficará disponível nos endereços padrão informados no console (ex: `http://localhost:5000` ou `https://localhost:5001`).

---

## 🛣️ Endpoints da API (Como Testar)

### 1. Registrar um Lançamento (Crédito ou Débito)
*   **Método:** `POST`
*   **Rota:** `/api/lancamentos`
*   **Payload Exemplo (Crédito):**
    ```json
    {
      "valor": 1500.50,
      "tipo": "Credito",
      "descricao": "Venda de mercadorias"
    }
    ```
*   **Payload Exemplo (Débito):**
    ```json
    {
      "valor": 200.00,
      "tipo": "Debito",
      "descricao": "Pagamento de conta de luz"
    }
    ```

### 2. Obter Relatório de Saldo Consolidado Diário
*   **Método:** `GET`
*   **Rota:** `/api/lancamentos/consolidado`
*   **Query Parameter (Opcional):** `?data=2026-08-07` *(Se omitido, assume o dia atual)*
*   **Resposta Esperada:**
    ```json
    {
      "data": "2026-08-07T00:00:00",
      "totalCreditos": 1500.50,
      "totalDebitos": 200.00,
      "saldoConsolidado": 1300.50
    }
    ```
