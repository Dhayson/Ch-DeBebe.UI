# Sobre a Aplicação
Chá de Bebê Digital - Gerenciador de Presentes

Uma aplicação web robusta e moderna para organização de listas de presentes de chá de bebê. O sistema permite que os pais gerenciem itens, quantidades e visualizem reservas em tempo real, enquanto oferece aos convidados uma interface fluida para escolher e reservar presentes.
## Tecnologias Utilizadas
### Frontend

    Vue.js 3 (Composition API): Framework core para uma interface reativa.

    PrimeVue & Aura Theme: Biblioteca de componentes de UI de última geração com design limpo e responsivo.

    Axios: Cliente HTTP para comunicação com a API.

    Vite: Tooling rápido para build e desenvolvimento.

### Backend

    ASP.NET Core 8 (C#): API RESTful de alta performance.

    Entity Framework Core: ORM para persistência de dados.

    Postgres SQL Server: Banco de dados relacional para armazenamento de informações críticas.

### Infraestrutura

    Docker & Docker Compose: Containerização de toda a stack (App, DB e Client).

    Nginx: Servidor de alta performance para entrega do conteúdo estático do frontend.

## Arquitetura e Funcionalidades
### Para os Administradores (Pais)

    Painel de Gerenciamento: Interface protegida para criar, editar e excluir presentes.

    Upload de Imagens: Suporte para fotos dos produtos via FormData, com armazenamento persistente em volume Docker.

    Rastreamento de Reservas: Visualização detalhada de quem reservou cada item e a quantidade selecionada através de tabelas expansíveis (Row Expansion).

### Para os Convidados

    Catálogo de Presentes: Visualização em Grid com cards modernos e status de disponibilidade.

    Reserva Simplificada: Processo de reserva sem necessidade de login complexo, focado na facilidade de uso.

## Como Executar (Docker)

A aplicação está totalmente em container docker, o que garante que ela rode de maneira independente do ambiente.

    Clone o repositório:

    ```
    $ git clone https://github.com/Dhayson/Ch-DeBebe.UI
    ```

    Suba os containers:

    ```
    $ docker-compose up --build
    ```
    Acesse as interfaces:

        Frontend: http://localhost:5173

        Backend (API) - swagger: http://localhost:5000/swagger/index.html

Configurações de Ambiente

A aplicação utiliza Variáveis de Ambiente no Docker para separar configurações de desenvolvimento e produção:

    Armazenamento: As imagens são persistidas no host através de volumes mapeados.

    CORS: A API está configurada para permitir requisições originadas do domínio do frontend.

    Database: Configuração automática de conexão entre os containers via rede interna do Docker.

Notas de Implementação:

    Static Files: O backend utiliza um PhysicalFileProvider customizado para servir imagens dinâmicas com segurança e performance.