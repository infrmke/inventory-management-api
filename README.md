# [.NET] Inventory Management API

Uma API Web, **ainda em desenvolvimento**, para gerenciamento de inventário e vendas, desenvolvida em .NET 8.

Atualmente, o projeto é composto por dois contextos principais: Módulo Catalog (gerencia o ciclo de vida de Categorias e Produtos) e Módulo Sales (responsável pelo fluxo de pedidos "Orders" e seus respectivos itens "OrderItems").

Cada módulo possui seu próprio DbContext dedicado, apontando para o mesmo servidor de banco de dados.

## Visão Geral Técnica

O sistema é estruturado como um Monólito Modular, tendo módulos módulos independentes que compartilham a mesma execução, mas possuem total isolamento em suas camadas de dados.

- **Back-end**: .NET 8.0 (C# 12);
- **Database**: Microsoft SQL Server com EF Core 8;
- **Development Tools**: Docker (para conteinerização do banco) e SSMS 22 (para administração de dados).

## Arquitetura e Padrões de Design

O back-end segue o padrão de arquitetura de Monólito Modular, possuindo uma separação estrita de responsabilidades por módulos de negócio. Ainda, conta com uma estrutura de fluxo de dados CSR (Controller-Service-Repository) onde os Controllers expõem os endpoints, os Services processam as regras de negócio e o Entity Framework atua como a camada de persistência.

- **Imutabilidade com Records e Init-Only Setters:** Uso de records para DTOs e modificadores "init" nas propriedades de Chave Primária (Id) e Chave Estrangeira (FK), garantindo que a identidade dos objetos permaneça imutável após a criação;
- **Identificadores UUID/Guid Sequenciais:** Utilização de Guid em vez de "int" para as chaves do sistema, configurados nativamente no banco de dados com NEWSEQUENTIALID();
- **Lazy Loading:** Uso controlado do modificador "virtual" em propriedades de navegação para habilitar o carregamento sob demanda de relacionamentos em cenários de consultas leves.

## Tech Stack e Bibliotecas

- **Core:** `.NET 8.0` e `ASP.NET Core Web API`;
- **Documentation:** `OpenAPI` (API Explorer e Swashbuckle) para documentação e interface `SwaggerUI` para interatividade no navegador.

## Como rodar o projeto

Antes de rodar a API, você precisará acatar aos seguintes **pré-requisitos**:

- .NET 8.0 SDK;
- Docker.

Opcionalmente, o SQL Server Management Studio (SSMS 22) também pode ser instalado para auxiliar na visualização de dados do banco.

1. Clone o Repositório

```shell
	git clone https://github.com/infrmke/inventory-management-api.git
	cd inventory-management-api
```

2. Suba o Banco de Dados (Docker)

Com o Docker Desktop ativo na sua máquina, navegue até a raiz do projeto (onde está o arquivo `docker-compose.yml`) e suba o contêiner do SQL Server em segundo plano:

```shell
	docker compose up -d
```

3. Configure a String de Conexão

Certifique-se de que o arquivo `appsettings.json` está apontando para o servidor local com as mesmas credenciais configuradas no Docker Compose:

```json
	"ConnectionStrings": {
		"DefaultConnection": "Server=localhost,1433;Database=InventoryDb;User Id=sa;Password=UmaSenhaForte123;TrustServerCertificate=True;"
	}
```

4. Execute as Migrations

```shell
	# Implementa as tabelas do módulo Catalog
	dotnet ef database update --context CatalogDbContext

	# Implementa as tabelas do módulo Sales
	dotnet ef database update --context SalesDbContext
```

5. Execute a API

```shell
	dotnet run
```

## Documentação da API

É possível acessar a documentação interativa da API com o Swagger UI por meio da URL https://localhost:7275/swagger/index.html, basta rodar o servidor.