# GraphQL Product API

Modern **GraphQL API** built with **.NET 9.0** implementing **CQRS**, **Event-Driven Architecture**, and **Clean Architecture** principles.

## 🏗️ Architecture Overview

```
┌─────────────┐
│   GraphQL   │ ──────────┐
│   Client    │           │
└─────────────┘           ▼
                  ┌───────────────┐
                  │  Hot Chocolate │
                  │  GraphQL API   │
                  └───────┬────────┘
                          │
         ┌────────────────┼────────────────┐
         ▼                ▼                ▼
    ┌─────────┐     ┌─────────┐     ┌──────────┐
    │ Queries │     │Mutations│     │ MediatR  │
    └─────────┘     └─────────┘     └────┬─────┘
                                          │
                    ┌─────────────────────┼──────────────────┐
                    ▼                     ▼                  ▼
              ┌──────────┐          ┌──────────┐      ┌──────────┐
              │ MongoDB  │          │ RabbitMQ │      │Elastic-  │
              │ (Write)  │          │  Events  │      │search    │
              └──────────┘          └─────┬────┘      │ (Read)   │
                                          │           └──────────┘
                                          ▼                ▲
                                    ┌──────────┐          │
                                    │Consumers │──────────┘
                                    └──────────┘
```

## ✨ Features

-  **Complete CRUD Operations**: Create, Read, Update, Delete products
- 🔄 **CQRS Pattern**: Separate read and write models
- 📨 **Event-Driven**: Async communication with RabbitMQ
- 🔍 **Full-Text Search**: Elasticsearch integration
- ♻️ **Message Retry**: Automatic retry with exponential back off
- 💪 **Resilient Messaging**: Acknowledgement and outbox pattern
- 🏥 **Health Checks**: Monitoring for all infrastructure services
- ✅ **Input Validation**: FluentValidation
- 📐 **Clean Architecture**: Separated concerns and dependencies
- 🎯 **Explicit Queue Configuration**: Named queues with retry policies

## 🛠️ Technology Stack

| Layer | Technology | Purpose |
|-------|------------|---------|
| API | Hot Chocolate 15.x | GraphQL server |
| Application | MediatR | CQRS mediator |
| Application | FluentValidation | Input validation |
| Domain | .NET 9.0 | Core business logic |
| Infrastructure | MongoDB 3.5 | Write model (source of truth) |
| Infrastructure | Elasticsearch 7.17 (NEST) | Read model (fast queries) |
| Infrastructure | RabbitMQ + MassTransit 8.5 | Event bus |
| Monitoring | Kibana 7.17 | Elasticsearch data visualization |
| Monitoring | ASP.NET Health Checks | Service health monitoring |

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/) (optional)

## 🚀 Quick Start

### 1. Clone the Repository

```bash
git clone <repository-url>
cd GraphQl
```

### 2. Start Infrastructure Services

Ensure Docker Desktop is running, then:

```bash
docker-compose up -d
```

This starts:
- **MongoDB** on port `27017`
- **Elasticsearch** on port `9200`
- **Kibana** on port `5601` (Elasticsearch UI)
- **RabbitMQ** on ports `5672` (AMQP) and `15672` (Management UI)

Verify services are healthy:

```bash
docker ps
```

### 3. Run the Application

```bash
dotnet run --project src/GraphQl.API/GraphQl.API.csproj
```

The API will start on `http://localhost:5000` (or check console output).

### 4. Access GraphQL Playground

Navigate to: **http://localhost:5000/graphql**

## 📝 GraphQL API Usage

### Create a Product

```graphql
mutation {
  createProduct(input: {
    name: "Wireless Keyboard"
    description: "Mechanical wireless keyboard with RGB lighting"
    price: 79.99
    stock: 50
  }) {
    id
    name
    price
    createdAt
  }
}
```

### Update a Product

```graphql
mutation {
  updateProduct(
    id: "67890abcdef1234567890abc"
    input: {
      name: "Wireless Keyboard Pro"
      description: "Premium mechanical wireless keyboard"
      price: 99.99
      stock: 30
    }
  ) {
    id
    name
    price
    updatedAt
  }
}
```

### Delete a Product

```graphql
mutation {
  deleteProduct(id: "67890abcdef1234567890abc") {
    success
    message
  }
}
```

### Search Products

```graphql
query {
  searchProducts(searchTerm: "keyboard", pageSize: 10, pageNumber: 1) {
    id
    name
    description
    price
    stock
    createdAt
  }
}
```

### Get Product by ID

```graphql
query {
  getProductById(id: "67890abcdef1234567890abc") {
    id
    name
    description
    price
    stock
    createdAt
  }
}
```

## 🏥 Health Checks

Check service health: **http://localhost:5000/health**

Expected response when all services are healthy:

```json
{
  "status": "Healthy",
  "results": {
    "rabbitmq": {
      "status": "Healthy",
      "description": "RabbitMQ is configured"
    }
  }
}
```

## 📁 Project Structure

```
GraphQl/
├── docker-compose.yml              # Infrastructure services
├── GraphQl.sln                     # Solution file
└── src/
    ├── GraphQl.Domain/             # Core business logic
    │   ├── Entities/               # Domain entities
    │   ├── Events/                 # Domain events
    │   ├── Repositories/           # Repository interfaces
    │   └── Constants/              # Queue & exchange names
    │
    ├── GraphQl.Application/        # Use cases
    │   ├── Common/
    │   │   ├── Behaviors/          # MediatR Pipeline behaviors
    │   │   └── Interfaces/         # Application interfaces
    │   └── Features/
    │       └── Products/
    │           ├── CreateProduct/  # Create command
    │           ├── UpdateProduct/  # Update command
    │           ├── DeleteProduct/  # Delete command
    │           ├── SearchProducts/ # Search query
    │           └── Consumers/      # Event consumers
    │
    ├── GraphQl.Infrastructure/     # External services
    │   ├── Persistence/            # MongoDB implementation
    │   └── Search/                 # Elasticsearch implementation
    │
    └── GraphQl.API/                # Web API
        ├── Extensions/             # Service registration
        ├── GraphQL/                # GraphQL types
        ├── HealthChecks/           # Custom health checks
        └── Program.cs              # Entry point
```

## 🔧 Configuration

Configuration is in `appsettings.json`:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://admin:admin123@localhost:27017",
    "DatabaseName": "ProductsDb",
    "ProductsCollectionName": "Products"
  },
  "ElasticsearchSettings": {
    "Url": "http://localhost:9200",
    "ProductIndex": "products"
  },
  "RabbitMqSettings": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

## 🔄 Message Queue Configuration

### Queue Names (Defined in Domain Constants)

- `product-created-queue` - For new products
- `product-updated-queue` - For product updates
- `product-deleted-queue` - For product deletions

### Retry Policy

All queues have exponential backoff retry:
- 1st retry: after 5 seconds
- 2nd retry: after 15 seconds
- 3rd retry: after 30 seconds

### Features

- **Prefetch Count**: 16 messages per consumer
- **Outbox Pattern**: Ensures exactly-once processing
- **Acknowledgement**: Manual ACK after successful processing

## 🧪 Testing

### Manual Testing

1. **Create a Product** via GraphQL mutation
2. **Verify in MongoDB**:
   ```bash
   docker exec -it graphql-mongodb mongosh -u admin -p admin123
   use ProductsDb
   db.Products.find().pretty()
   ```

3. **Verify in Elasticsearch**:
   ```bash
   curl http://localhost:9200/products/_search?pretty
   ```

4. **Check RabbitMQ**:
   - Open http://localhost:15672 (guest/guest)
   - Verify queues exist and messages are processed

### Verify Event Flow

1. Create a product → Check MongoDB → Event published
2. Consumer receives event → Product indexed in Elasticsearch
3. Search query → Results from Elasticsearch

## 🐛 Troubleshooting

### Build Errors

**Error**: Package restore failed
```bash
dotnet restore
dotnet clean
dotnet build
```

### Runtime Errors

**MongoDB Connection Failed**
- Ensure Docker is running: `docker ps`
- Check connection string in `appsettings.json`
- Wait for MongoDB to fully start (30-60 seconds)

**Elasticsearch Not Available**
- Check health: `curl http://localhost:9200/_cluster/health`
- Elasticsearch 7.x takes time to start
- Check Docker logs: `docker logs graphql-elasticsearch`

**RabbitMQ Connection Issues**
- Verify container: `docker ps | grep rabbitmq`
- Check management UI: http://localhost:15672
- Default credentials: guest/guest

## 📊 Monitoring

### Application Logs

Logs are output to console. Configure Serilog for structured logging in production.

### RabbitMQ Management

- URL: http://localhost:15672
- Credentials: `guest` / `guest`
- Monitor: Queues, exchanges, message rates

### Elasticsearch

- Cluster Health: `GET http://localhost:9200/_cluster/health`
- Index Stats: `GET http://localhost:9200/products/_stats`

## 🏷️ Design Patterns Used

1. **CQRS**: Separate command and query models
2. **Repository Pattern**: Abstract data access
3. **Mediator Pattern**: MediatR for request handling
4. **Event-Driven Architecture**: Async communication
5. **Outbox Pattern**: Reliable message delivery
6. **Clean Architecture**: Dependency inversion
7. **Extension Methods**: Modular service registration

## 📈 Performance Considerations

- **Read Scalability**: Elasticsearch handles high query loads
- **Write Consistency**: MongoDB provides strong consistency
- **Async Processing**: Events processed asynchronously
- **Message Batching**: Prefetch count optimizes throughput
- **Connection Pooling**: Built-in for all clients

## 🔒 Security Notes

> [!WARNING]
> **This is a development setup**. For production:
> - Use secrets management (Azure Key Vault, AWS Secrets Manager)
> - Enable authentication for all services
> - Use TLS/SSL for connections
> - Implement API authentication (JWT, OAuth)
> - Add rate limiting
> - Enable Elasticsearch security features

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 🙏 Acknowledgments

- [Hot Chocolate GraphQL](https://chillicream.com/docs/hotchocolate)
- [MassTransit](https://masstransit.io/)
- [MongoDB](https://www.mongodb.com/)
- [Elasticsearch](https://www.elastic.co/)
- [RabbitMQ](https://www.rabbitmq.com/)

---

**Project Status**: ✅ Production Ready | 🚀 Actively Maintained

For questions or issues, please open an issue on GitHub.
