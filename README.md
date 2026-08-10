# Desafio Técnico: Sistema de Fluxo de Caixa

Este projeto consiste em uma aplicação de controle de fluxo de caixa para comerciantes, permitindo o registro de lançamentos diários (créditos e débitos) e a consulta de relatórios consolidados de saldo diário. A solução foi desenvolvida utilizando as melhores práticas do ecossistema .NET, focando em alta performance, resiliência e testabilidade.

## 🚀 Diferenciais Técnicos e Resiliência
*   **Alta Performance Concorrente:** Para atender ao requisito de alto volume de acessos (ex: picos de 50 chamadas/segundo), a persistência em memória foi implementada utilizando a coleção thread-safe `ConcurrentBag`. Isso evita gargalos de travamento (`lock`) que ocorreriam com listas tradicionais em cenários de concorrência.
*   **Isolamento de Regras:** Validações de entrada desacopladas usando a biblioteca profissional **FluentValidation**.
*   **Testabilidade:** Suíte de testes unitários robusta cobrindo as regras de negócio de fluxo e a matemática crítica por trás do saldo consolidado.

---

## 🏛️ Arquitetura do Sistema

O ecossistema foi desenvolvido separando rigidamente as responsabilidades. O backend foi estruturado seguindo os princípios de **Clean Architecture** (Arquitetura Limpa) e os conceitos do **SOLID**, garantindo o desacoplamento evidente entre as camadas e facilitando a manutenção e evolução do software. O frontend foi acoplado na raiz de forma totalmente independente.

*   📂 **CaixaFluxoDesafio** (Pasta Raiz)
    *   📂 **CaixaFluxo.Domain:** Entidades de negócio e regras puras (Isolada)
    *   📂 **CaixaFluxo.Application:** Casos de uso (Services), DTOs, Validadores e Interfaces
    *   📂 **CaixaFluxo.Infrastructure:** Persistência em memória (ConcurrentBag) e Repositórios
    *   📂 **CaixaFluxo.API:** Controladores HTTP, Rotas REST e configuração de DI
    *   📂 **CaixaFluxo.Tests:** Testes Unitários automatizados com xUnit e Moq
    *   📂 **CaixaFluxo.Dashboard:** Frontend minimalista em Angular para execução do teste de carga reativo

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
A API ficará disponível nos endereços padrão informados no console (ex: `http://localhost:5295`).

---

## 🐳 Executando a Aplicação via Docker Container

Caso você possua o Docker instalado em sua máquina local, a aplicação já está totalmente configurada e otimizada com suporte a **Multi-stage builds**. O arquivo `Dockerfile` na raiz orquestra a compilação de todas as camadas em um ambiente isolado do Linux e gera uma imagem final ultra leve contendo apenas o runtime necessário.

### 1. Construir a Imagem do Container
Abra o terminal na pasta raiz da solução (onde se encontra o arquivo `Dockerfile`) e execute o comando abaixo para compilar o ecossistema:
```bash
docker build -t desafio-caixa-fluxo:latest .
```

### 2. Iniciar o Container Mapeando as Portas
Após a compilação da imagem com sucesso, execute o comando a seguir para subir o servidor da API em segundo plano:
```bash
docker run -d -p 8080:8080 --name api-fluxo-caixa desafio-caixa-fluxo:latest
```

### 3. Validar a Execução e Testar
A API estará disponível para receber requisições através do endereço local na porta `8080`:

*   **Endpoint de Verificação (Healthcheck):** Faça um `GET` para `http://localhost:8080/health`
*   **Criar Lançamento:** Faça um `POST` para `http://localhost:8080/api/lancamentos`
*   **Consultar Relatório:** Faça um `GET` para `http://localhost:8080/api/lancamentos/consolidado`

*(Nota: Como o ecossistema de Web API do .NET não possui páginas ou visualizações padrão mapeadas na URL raiz, o acesso direto a `http://localhost:8080/` retornará um status HTTP 404 de rota não encontrada por design. Utilize os caminhos completos listados acima para testes).*

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

---

## 🖥️ Painel Frontend de Teste de Carga (Angular)

Para validar o requisito não-funcional de **resiliência e suporte a alta carga concorrente**, a solução conta com um painel de controle moderno desenvolvido em **Angular**. Esta interface reativa (baseada em RxJS) simula picos de acessos simultâneos de **0 a 100 requisições paralela**, auditando o tempo de resposta individual e a saúde do servidor.

### 🚀 Como Executar o Frontend Localmente

#### 1. Pré-requisitos
Certifique-se de possuir o **Node.js (versão 20 LTS ou superior)** instalado em sua máquina.

#### 2. Instalar as Dependências e Inicializar
Abra um novo terminal na pasta raiz da solução e execute os seguintes comandos para navegar até o diretório do frontend, instalar o módulo reativo necessário e subir o servidor de desenvolvimento:
```bash
# Entrar na pasta do projeto Angular
cd CaixaFluxo.Dashboard

# Instalar o pacote de sincronização reativa de microtarefas (Zone.js)
npm install zone.js

# Inicializar o servidor local do Angular via NPX
npx ng serve
```

#### 3. Realizar o Teste de Carga
*   Abra o navegador no endereço indicado pelo console: `http://localhost:4200`
*   Verifique se a porta da **URL da API Alvo** condiz com a porta ativa do seu backend .NET local (ex: `http://localhost:5295/api/lancamentos`).
*   Utilize o controle deslizante (*slider*) para escolher o volume de requisições concorrentes (de 0 a 100).
*   Clique em **"Disparar Carga Concorrente ⚡"**.

O painel disparará o lote de requisições simultâneas em segundo plano e, graças ao monitoramento do `ChangeDetectorRef` injetado, atualizará a tela dinamicamente exibindo o **tempo total do lote (em ms)**, a **taxa de sucesso percentual** e um **log detalhado de auditoria** para cada envio recebido com status HTTP 201 pela API.