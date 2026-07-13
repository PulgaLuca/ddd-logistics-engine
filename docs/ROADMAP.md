# ROADMAP - PascalLogistics
## Progetto Scolastico - Classe 4^I

---

## Overview

Questo documento definisce la roadmap di sviluppo del progetto **PascalLogistics**, un sistema di gestione logistica costruito con architettura **DDD (Domain Driven Design)** e **Clean Architecture**. Il progetto è diviso in **fasi di sviluppo** corrispondenti ai periodi di valutazione del quadrimestre.

**Struttura Valutazioni:**
- **2-3 valutazioni per quadrimestre** (tramite Code Review)
- Ogni valutazione corrisponde a una fase completata
- Focus su: Funzionalità, Architettura, Clean Code, Test

---

## Fase 0: Setup e Fondamenti Architetturali
**Durata:** 1 settimana  
**Periodo:** Inizio progetto  
**Valutazione:** Setup iniziale e comprensione dell'architettura

### Obiettivi
- [ ] Configurare la soluzione .NET con la struttura DDD/Clean Architecture
- [ ] Definire le convenzioni di nomenclatura e struttura cartelle
- [ ] Creare i layer fondamentali: Domain, Application, Infrastructure, No presentation

### Argomenti
- **DDD Fundamentals**: Entities, ValueObjects, Aggregates
- **Clean Architecture**: Dependency Inversion, Layering, Separation of Concerns
- **Project Structure**: Folder organization, NuGet references

### Classi/Progetti da creare
```
BlaisePascal.SharedKernel/
  - Entity.cs
  - ValueObject.cs
  - Error.cs
  - ErrorType.cs
  - Result<T>.cs
  - IDomainEvent.cs

BlaisePascal.PascalLogistics.Domain/
  - Users/ (Aggregate Root)
    - User.cs
    - UserId.cs (ValueObject)
    - Email.cs (ValueObject)
  - Shared/ (Shared stuff)
```

### Deliverables
- Soluzione compilabile
- Struttura cartelle definita
- Base classes implementate in SharedKernel

---

## CODE REVIEW 1^ - Fondamenti DDD e CQRS
**Durata:** 3-4 settimane  
**Periodo:** Prime settimane del quadrimestre  
**Tipo:** Code Review + Presentazione Architettura

### 🎯 Scopo Principale
Implementare la **prima modellazione del dominio** con focus su **Users Aggregate** come vertical slice completa attraverso Domain, Application e Infrastructure (JSON).

### Argomenti Principali
1. **Domain Model**
   - Definizione Entities e ValueObjects
   - Aggregate pattern
   - Domain Events
   - Business Rules nel dominio

2. **Application Layer - CQRS + MediatR**
   - Command/Query Separation
   - Handlers con MediatR
   - DTOs (Data Transfer Objects)
   - Validation behaviors

3. **Infrastructure Layer - JSON Persistence**
   - Repository pattern
   - File-based storage
   - Dependency Injection setup

### Classi da Implementare

#### Domain Layer
```csharp
// Users Aggregate
BlaisePascal.PascalLogistics.Domain/Users/
├── User.cs                    // Aggregate Root
├── UserId.cs                  // ValueObject
├── Email.cs                   // ValueObject
├── PhoneNumber.cs             // ValueObject
├── Address.cs                 // ValueObject
├── UserRole.cs                // Enum/ValueObject
└── Specifications/
    └── UserSpecifications.cs   // Queries specifiche
```

#### Application Layer
```csharp
// Users Use Cases
BlaisePascal.PascalLogistics.Application/Users/
├── Commands/
│   ├── CreateUserCommand.cs
│   └── CreateUserCommandHandler.cs
├── Queries/
│   ├── GetUserByIdQuery.cs
│   ├── GetUserByIdQueryHandler.cs
│   └── GetAllUsersQuery.cs
├── DTOs/
│   └── UserDto.cs
└── Validators/
    └── CreateUserCommandValidator.cs
```

#### Infrastructure Layer
```csharp
// JSON Persistence
BlaisePascal.PascalLogistics.Infrastructure/
├── Persistence/
│   ├── JsonFileContext.cs      // Context per JSON
│   ├── Repositories/
│   │   └── UserRepository.cs
│   └── Data/
│       └── users.json          // Storage file
└── DependencyInjection.cs
```

#### API Layer
```csharp
// Controllers (verrà aggiunto dopo)
// Ancora NON introdotto in questa fase
```

### Tecnologie Utilizzate
- ✅ C# / .NET 8+
- ✅ MediatR (CQRS pattern)
- ✅ FluentValidation (basic)
- ✅ JSON file storage
- ✅ Dependency Injection (built-in)

### Criteri di Valutazione
- ✔️ Entities e ValueObjects correttamente modellate
- ✔️ Commands e Queries implementati
- ✔️ Handlers con logica di business
- ✔️ Repository pattern implementato
- ✔️ Persistenza JSON funzionante
- ✔️ Code review: Clean Code, SOLID principles
- ✔️ Test unitari sui domain models (optional ma consigliato)

### Deliverables
- ✅ Vertical Slice completa per Users (domain→app→infrastructure)
- ✅ Test unitari su User Aggregate
- ✅ File JSON con dati di esempio
- ✅ Documentazione della struttura

---

## 📍 CODE REVIEW 2^ - ASP.NET Web API Entry Point
**Durata:** 2-3 settimane  
**Periodo:** Dopo valutazione 1^  
**Tipo:** Code Review + Demo funzionale

### 🎯 Scopo Principale
Introdurre **ASP.NET Web API** come entry point per esporre i casi d'uso dell'Application layer.

### Argomenti Principali
1. **RESTful API Principles**
   - HTTP Verbs (GET, POST, PUT, DELETE)
   - Status Codes
   - URL conventions

2. **ASP.NET Core Web API**
   - Controllers pattern
   - Routing
   - Dependency Injection
   - Middleware

3. **API Documentation**
   - Swagger/OpenAPI
   - Endpoint documentation

### Classi da Implementare

#### API Layer
```csharp
BlaisePascal.PascalLogistics.Api/
├── Controllers/
│   └── Users/
│       └── UsersController.cs    // Endpoints per Users
├── Program.cs                     // Startup config
└── appsettings.json
```

#### Endpoints Definiti
```http
POST   /api/users                 // CreateUserCommand
GET    /api/users/{id}            // GetUserByIdQuery
GET    /api/users                 // GetAllUsersQuery
```

### Tecnologie Utilizzate
- ✅ ASP.NET Core Web API
- ✅ Swagger/Swashbuckle per API documentation
- ✅ MediatR (integrate con API)
- ✅ JSON Persistence (still)

### Criteri di Valutazione
- ✔️ Controllers implementati correttamente
- ✔️ Routing funzionante
- ✔️ Error handling appropriato
- ✔️ HTTP Status Codes corretti
- ✔️ Swagger documentation completa
- ✔️ API testabile con Postman/VS Code REST Client

### Deliverables
- ✅ Controller completo per Users
- ✅ Swagger UI funzionante
- ✅ File .http con richieste di test
- ✅ Integrazione MediatR→Controller

---

## 📍 CODE REVIEW 3^ - MongoDB Persistence Layer
**Durata:** 3-4 settimane  
**Periodo:** Post valutazione 2^  
**Tipo:** Code Review + Performance analysis

### 🎯 Scopo Principale
Sostituire la **persistenza JSON con MongoDB**, mantenendo l'interfaccia dell'Application layer invariata.

### Argomenti Principali
1. **NoSQL Databases**
   - MongoDB fundamentals
   - Collections vs Tables
   - Document structure

2. **MongoDB Driver per .NET**
   - MongoClient setup
   - CRUD operations
   - Querying

3. **Repository Pattern Refactoring**
   - Mantenere interface consistency
   - Implementazione MongoDB vs JSON

### Classi da Modificare/Creare

#### Infrastructure Layer Refactoring
```csharp
BlaisePascal.PascalLogistics.Infrastructure/
├── Persistence/
│   ├── IContext.cs              // Interface
│   ├── JsonFileContext.cs        // Keep as alternative
│   ├── MongoDbContext.cs         // NEW MongoDB context
│   ├── Repositories/
│   │   ├── IUserRepository.cs    // Interface
│   │   ├── JsonUserRepository.cs // Keep as alternative
│   │   └── MongoDbUserRepository.cs  // NEW
│   └── Data/
│       └── users.json            // Keep as fallback
├── MongoDbSettings.cs             // Config
└── DependencyInjection.cs         // Update DI setup
```

#### MongoDB Connection String
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "PascalLogistics"
  }
}
```

### Tecnologie Utilizzate
- ✅ MongoDB
- ✅ MongoDB.Driver NuGet
- ✅ Docker (optional per MongoDB)

### Criteri di Valutazione
- ✔️ Migrazione dati completata
- ✔️ Repository MongoDB implementato
- ✔️ Query performance ottimizzate
- ✔️ Indici MongoDB creati
- ✔️ Application layer invariato (Dependency Inversion)
- ✔️ Test di integrazione con MongoDB

### Deliverables
- ✅ MongoDbContext implementato
- ✅ MongoDb Repository per Users
- ✅ Connection string configurato
- ✅ Migration script (opzionale)
- ✅ Test di integrazione

---

## 📍 CODE REVIEW 4^ - Authentication & Authorization
**Durata:** 3-4 settimane  
**Periodo:** Post valutazione 3^  
**Tipo:** Code Review + Security audit

### 🎯 Scopo Principale
Implementare **autenticazione (JWT)** e **autorizzazione (role-based)** per proteggere gli endpoint.

### Argomenti Principali
1. **Authentication**
   - JWT (JSON Web Tokens)
   - Password hashing (bcrypt/argon2)
   - Token refresh mechanism

2. **Authorization**
   - Role-based Access Control (RBAC)
   - Policy-based authorization
   - Claims

3. **Identity Management**
   - User roles
   - Permissions
   - Policies

### Classi da Implementare/Modificare

#### Domain Layer Enhancements
```csharp
BlaisePascal.PascalLogistics.Domain/Users/
├── User.cs                      // Add Password, Roles
├── UserRole.cs                  // Enum o Aggregate
├── Permission.cs                // ValueObject
└── Auth/
    └── IAuthenticationService.cs // Domain service
```

#### Application Layer
```csharp
BlaisePascal.PascalLogistics.Application/Auth/
├── Commands/
│   ├── RegisterUserCommand.cs
│   ├── LoginCommand.cs
│   └── RefreshTokenCommand.cs
├── Handlers/
│   ├── RegisterUserCommandHandler.cs
│   ├── LoginCommandHandler.cs
│   └── RefreshTokenCommandHandler.cs
└── DTOs/
    ├── LoginRequestDto.cs
    ├── TokenResponseDto.cs
    └── RegisterRequestDto.cs
```

#### Infrastructure Layer
```csharp
BlaisePascal.PascalLogistics.Infrastructure/Identity/
├── JwtTokenProvider.cs
├── PasswordHasher.cs
└── AuthenticationService.cs
```

#### API Layer
```csharp
BlaisePascal.PascalLogistics.Api/Controllers/
├── Auth/
│   └── AuthController.cs        // Login, Register
└── Users/
    └── UsersController.cs       // Add [Authorize] attributes
```

### Tecnologie Utilizzate
- ✅ JWT Bearer authentication
- ✅ BCrypt per password hashing
- ✅ ASP.NET Core Identity (optional)
- ✅ Authorization policies

### Criteri di Valutazione
- ✔️ JWT tokens generati correttamente
- ✔️ Password hashate sicuramente
- ✔️ Endpoints protetti con [Authorize]
- ✔️ Roles e claims implementati
- ✔️ Token refresh funzionante
- ✔️ Nessun hardcoded credentials
- ✔️ Security best practices rispettate

### Deliverables
- ✅ AuthController con login/register
- ✅ JwtTokenProvider implementato
- ✅ Password hashing implementato
- ✅ Endpoints protetti
- ✅ Test di security

---

## 📍 CODE REVIEW 5^ - Logging e Validation Avanzata
**Durata:** 2-3 settimane  
**Periodo:** Post valutazione 4^  
**Tipo:** Code Review + Observability analysis

### 🎯 Scopo Principale
Implementare **Serilog** per il logging strutturato e **FluentValidation** avanzata per la validazione.

### Argomenti Principali
1. **Structured Logging con Serilog**
   - Log levels
   - Structured data
   - Sinks (console, file, etc)
   - Correlation IDs

2. **FluentValidation**
   - Rule definitions
   - Custom validators
   - Cross-property validation
   - Async validation

3. **Middleware**
   - Exception handling
   - Request logging
   - Response logging

### Classi da Implementare/Modificare

#### Application Layer
```csharp
BlaisePascal.PascalLogistics.Application/
├── Behaviours/
│   ├── ValidationBehaviour.cs   // MediatR behavior
│   ├── LoggingBehaviour.cs      // NEW
│   └── PerformanceBehaviour.cs  // NEW
└── Users/
    └── Validators/
        ├── CreateUserCommandValidator.cs  // Enhanced
        └── LoginCommandValidator.cs        // NEW
```

#### Infrastructure Layer
```csharp
BlaisePascal.PascalLogistics.Infrastructure/
├── Logging/
│   ├── LoggerConfiguration.cs
│   └── StructuredLogging.cs
└── DependencyInjection.cs       // Update Serilog setup
```

#### API Layer
```csharp
BlaisePascal.PascalLogistics.Api/
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs
│   ├── RequestLoggingMiddleware.cs
│   └── ErrorResponse.cs
└── Program.cs                   // Add middleware pipeline
```

### Tecnologie Utilizzate
- ✅ Serilog
- ✅ FluentValidation
- ✅ MediatR Behaviors
- ✅ Custom middleware

### Criteri di Valutazione
- ✔️ Serilog configurato con multiple sinks
- ✔️ Structured logging implementato
- ✔️ FluentValidation per tutti i commands/queries
- ✔️ Custom validation rules
- ✔️ Exception handling middleware
- ✔️ Performance logging
- ✔️ Correlation IDs per tracing

### Deliverables
- ✅ Serilog configuration completa
- ✅ ValidationBehaviors per MediatR
- ✅ Exception handling middleware
- ✅ Log files generati e consultabili
- ✅ Validation messages localizzati

---

## 📍 CODE REVIEW 6^ - LINQ e Lambda Expressions
**Durata:** 2-3 settimane  
**Periodo:** Post valutazione 5^  
**Tipo:** Code Review + Query optimization analysis

### 🎯 Scopo Principale
Approfondire **LINQ** e **lambda expressions** con query ottimizzate su MongoDB e implementare **Specifications** pattern.

### Argomenti Principali
1. **LINQ Fundamentals**
   - Query syntax vs Method syntax
   - Deferred execution
   - Lazy evaluation

2. **Lambda Expressions**
   - Arrow functions
   - Expression trees
   - Func<T> e Predicate<T>

3. **Advanced Queries**
   - Specifications pattern
   - Complex filtering
   - Grouping e aggregations
   - Projections

4. **Query Performance**
   - Index usage
   - Query profiling
   - N+1 problem avoidance

### Classi da Implementare/Modificare

#### Domain Layer
```csharp
BlaisePascal.PascalLogistics.Domain/
├── Shared/
│   ├── ISpecification.cs        // NEW
│   └── BaseSpecification.cs     // NEW
├── Users/
│   └── Specifications/
│       ├── GetUserByIdSpec.cs
│       ├── GetUsersByRoleSpec.cs
│       └── GetActiveUsersSpec.cs
└── Orders/  // Se aggiunto
    └── Specifications/
        ├── GetOrdersByUserSpec.cs
        └── GetPendingOrdersSpec.cs
```

#### Application Layer
```csharp
BlaisePascal.PascalLogistics.Application/Users/
├── Queries/
│   ├── GetUsersByRoleQuery.cs   // NEW
│   ├── GetUsersByRoleQueryHandler.cs
│   ├── SearchUsersQuery.cs      // NEW
│   └── SearchUsersQueryHandler.cs
└── Validators/
```

#### Infrastructure Layer
```csharp
BlaisePascal.PascalLogistics.Infrastructure/
├── Persistence/
│   ├── SpecificationEvaluator.cs  // NEW
│   └── Repositories/
│       └── UserRepository.cs      // Refactor con Specifications
```

### Query Examples

```csharp
// Specifications Pattern
public class GetActiveUsersByRoleSpec : BaseSpecification<User>
{
    public GetActiveUsersByRoleSpec(UserRole role)
    {
        AddCriteria(u => u.IsActive && u.Role == role);
        AddOrderByDescending(u => u.CreatedAt);
    }
}

// LINQ Method Syntax
var activeUsers = await _userRepository
    .GetAsync(new GetActiveUsersByRoleSpec(UserRole.Admin))
    .ToListAsync();

// Lambda & Func
Func<User, bool> isAdmin = u => u.Role == UserRole.Admin;
var admins = users.Where(isAdmin).ToList();
```

### Tecnologie Utilizzate
- ✅ LINQ to MongoDB
- ✅ Lambda expressions
- ✅ Expression trees
- ✅ Specifications pattern

### Criteri di Valutazione
- ✔️ Specifications pattern implementato correttamente
- ✔️ Query LINQ ottimizzate
- ✔️ Lambda expressions usate appropriatamente
- ✔️ Nessun N+1 problem
- ✔️ Query performance misurato
- ✔️ Projection con Select usate
- ✔️ Complex queries sulla base di dati reali

### Deliverables
- ✅ Specifications implementate
- ✅ SpecificationEvaluator
- ✅ LINQ queries ottimizzate
- ✅ Test di performance su queries
- ✅ Documentazione delle query usate

---

## 📍 CODE REVIEW 7^ e seguenti - Data Structures & Algorithms Integration
**Durata:** 4-6 settimane (per ogni modulo)  
**Periodo:** Dal 2^ quadrimestre in poi  
**Tipo:** Code Review + Integration testing

### 🎯 Scopo Principale
Integrare **Data Structures** e **Algorithms** nel contesto del dominio logistico, affrontando problemi reali del progetto.

### 📊 Strutture Dati da Implementare

#### 1. **Stack - Undo/Redo Mechanism**
**Modulo 1 - Durata: 1-2 settimane**

```csharp
// Domain Layer
BlaisePascal.PascalLogistics.Domain/
├── Shared/
│   ├── Stack<T>.cs              // Implementazione
│   └── Command.cs               // Pattern Command per Undo/Redo
├── Orders/
│   ├── OrderCommand.cs          // Interfaccia
│   └── OrderCommandHistory.cs   // Aggregate
└── Users/
    └── UserActionHistory.cs     // Track user actions

// Caso d'uso: Undo/Redo per operazioni su ordini
// Undo: Pop dalla stack e eseguire azione inversa
// Redo: Stack separata per azioni disfatte
```

**Argomenti:**
- Stack data structure (LIFO)
- Command pattern
- Memento pattern per undo/redo
- State restoration

**Query del progetto:**
- Permettere all'utente di annullare/ripetere operazioni su ordini
- Mantenere cronologia delle azioni

---

#### 2. **Queue - Order Processing System**
**Modulo 2 - Durata: 2-3 settimane**

```csharp
// Domain Layer
BlaisePascal.PascalLogistics.Domain/
├── Shared/
│   ├── Queue<T>.cs              // Implementazione
│   └── PriorityQueue<T>.cs      // Implementazione
├── Orders/
│   ├── OrderQueue.cs            // Aggregate
│   ├── OrderPriority.cs         // Enum
│   └── OrderProcessor.cs        // Service
└── Shifts/
    └── ShiftOrderAssignment.cs  // Service

// Caso d'uso: Processare ordini in coda
// FIFO: Ordini semplici processati in ordine
// Priority Queue: Ordini urgenti/VIP vanno prima
```

**Argomenti:**
- Queue data structure (FIFO)
- Priority Queue
- Dequeue/Enqueue operations
- Circular buffer optimization

**Query del progetto:**
- Coda di ordini in attesa di processamento
- Ordini prioritari (urgenti, VIP customer)
- Assegnazione automatica ai delivery agents
- Load balancing tra i driver

**Implementazione Esempio:**
```csharp
public class OrderQueue
{
    private PriorityQueue<Order, int> _queue;
    
    public void EnqueueOrder(Order order)
    {
        var priority = CalculatePriority(order);
        _queue.Enqueue(order, priority);
    }
    
    public Order DequeueNextOrder()
        => _queue.Dequeue();
    
    private int CalculatePriority(Order order)
    {
        // Bassa priorità = numero basso = processa prima
        int basePriority = 100;
        if (order.IsUrgent) basePriority -= 50;
        if (order.CustomerIsVip) basePriority -= 30;
        return basePriority;
    }
}
```

---

#### 3. **Binary Search Tree (BST) - Route Optimization**
**Modulo 3 - Durata: 2-3 settimane**

```csharp
// Domain Layer
BlaisePascal.PascalLogistics.Domain/
├── Shared/
│   ├── BinarySearchTree<T>.cs    // Implementazione
│   └── TreeNode<T>.cs
├── Vehicles/
│   ├── RouteNode.cs              // Nodo per posizione
│   ├── DeliveryRoute.cs          // Albero di consegne
│   └── RouteOptimizer.cs         // Service
└── Shifts/
    └── DriverRouteAssignment.cs  // Service

// Caso d'uso: Trovare la rotta ottimale per consegne
// BST ordinato per ZIP code o distanza
// Ricerca veloce di punti di consegna
```

**Argomenti:**
- Binary Search Tree insertion/deletion/search
- In-order, pre-order, post-order traversal
- Tree balancing (AVL basics)
- Spatial indexing concepts

**Query del progetto:**
- Cercare consegne per zona geografica
- Ottimizzare rotte per efficienza
- Bilanciamento carico tra driver
- Minimizzare distanza totale

---

#### 4. **Hash Table / Dictionary - Fast Lookup**
**Modulo 4 - Durata: 1-2 settimane**

```csharp
// Infrastructure Layer
BlaisePascal.PascalLogistics.Infrastructure/
├── Caching/
│   ├── UserCache.cs             // HashMap per users
│   ├── OrderCache.cs            // HashMap per ordini
│   └── CacheManager.cs
└── Persistence/
    └── InMemoryRepository.cs     // Cache layer

// Caso d'uso: Caching veloce dei dati frequenti
// Hash per accesso O(1)
```

**Argomenti:**
- Hash functions
- Collision handling
- Load factor
- Caching strategies

**Query del progetto:**
- Cache di users/drivers frequenti
- Cache di ordini recenti
- Cache di zone di consegna
- Invalidazione di cache

---

#### 5. **Linked List - Event Log / Audit Trail**
**Modulo 5 - Durata: 1-2 settimane**

```csharp
// Domain Layer
BlaisePascal.PascalLogistics.Domain/
├── Shared/
│   └── LinkedList<T>.cs          // Implementazione
├── Shared/
│   └── AuditTrail.cs             // Aggregate
└── Users/
    └── UserActionLog.cs          // Linked list di azioni

// Caso d'uso: Tracciare cronologia azioni
// Ogni azione linked alla precedente
```

**Argomenti:**
- Singly/Doubly Linked Lists
- Insertion/Deletion
- Traversal
- Reverse operations

**Query del progetto:**
- Cronologia completa di un ordine
- Log delle azioni di un driver
- Audit trail per compliance
- Reverse order traversal

---

### 🔄 Algoritmi da Implementare

#### 6. **Graph Algorithms - Delivery Network Optimization**
**Modulo 6 - Durata: 3-4 settimane**

```csharp
// Domain Layer
BlaisePascal.PascalLogistics.Domain/
├── Shared/
│   ├── Graph<T>.cs               // Implementazione
│   ├── WeightedEdge.cs
│   └── GraphAlgorithms.cs        // Algoritmi
├── Geography/
│   ├── DeliveryZone.cs           // Nodo del grafo
│   ├── Zone.cs
│   ├── DeliveryNetwork.cs        // Grafo di zone
│   └── DistrictOptimizer.cs      // Service
└── Vehicles/
    └── RouteOptimizer.cs         // Service

// Algoritmi da implementare:
// 1. Dijkstra - Shortest path tra hub e destinazioni
// 2. BFS/DFS - Visita tutte le consegne
// 3. Bellman-Ford - Per negativi (sconto se ritorno)
// 4. Traveling Salesman (TSP) - Route ottimale
```

**Caso d'uso: Ottimizzazione della rete di consegna**

```csharp
public class DeliveryNetwork
{
    private Graph<DeliveryZone> _zones;
    
    // Dijkstra: Trovare il percorso più breve
    public List<DeliveryZone> FindShortestPath(
        DeliveryZone start, 
        DeliveryZone end)
    {
        return DijkstraAlgorithm.Execute(_zones, start, end);
    }
    
    // BFS: Visitare tutte le zone
    public List<DeliveryZone> VisitAllZones(DeliveryZone start)
    {
        return BreadthFirstSearch.Execute(_zones, start);
    }
    
    // TSP: Rotta ottimale per più consegne
    public List<DeliveryZone> OptimizeDeliveryRoute(
        List<Order> orders)
    {
        return TravelingSalesmanProblem.Execute(_zones, orders);
    }
}
```

**Argomenti:**
- Graph representation (adjacency matrix, adjacency list)
- Dijkstra's algorithm
- BFS/DFS algorithms
- TSP (Traveling Salesman Problem)
- Minimum Spanning Tree (Kruskal, Prim)

**Query del progetto:**
- Trovare il percorso più veloce tra hub
- Visitare tutte le zone di consegna
- Ottimizzare rotta per minimizzare distanza
- Coordinare consegne tra più driver

---

#### 7. **Sorting & Searching Algorithms - Data Organization**
**Modulo 7 - Durata: 2-3 settimane**

```csharp
// Application Layer
BlaisePascal.PascalLogistics.Application/
├── Orders/
│   ├── Queries/
│   │   ├── SearchOrdersQuery.cs
│   │   └── SortOrdersQuery.cs
│   └── Handlers/
│       ├── SearchOrdersHandler.cs
│       └── SortOrdersHandler.cs
└── Infrastructure/
    ├── Sorting/
    │   ├── QuickSort.cs
    │   ├── MergeSort.cs
    │   └── HeapSort.cs
    └── Searching/
        ├── BinarySearch.cs
        └── LinearSearch.cs

// Caso d'uso: Ordinare e cercare ordini
// Quick Sort per ordini per data
// Binary search su ordini ordinati
```

**Algoritmi:**
- QuickSort - Average O(n log n)
- MergeSort - Guaranteed O(n log n)
- HeapSort - O(n log n), in-place
- Binary Search - O(log n) su dati ordinati

**Query del progetto:**
- Cercare ordini per ID velocemente
- Ordinare consegne per data
- Ordinare driver per numero consegne
- Report mensili con dati ordinati

---

#### 8. **Dynamic Programming - Cost Optimization**
**Modulo 8 - Durata: 2-3 settimane**

```csharp
// Domain Layer
BlaisePascal.PascalLogistics.Domain/
├── Orders/
│   ├── CostCalculation.cs        // DP problem
│   └── PricingStrategy.cs
└── Vehicles/
    ├── FuelConsumption.cs        // DP problem
    └── MaintenanceSchedule.cs

// Caso d'uso: Minimizzare costi
// DP per combinazioni ottimali di ordini
// Memoization per evitare ricalcoli
```

**Argomenti:**
- Memoization
- Tabulation
- Optimal substructure
- Overlapping subproblems

**Query del progetto:**
- Minimizzare costi totali di consegna
- Ottimizzare assegnazione ordini a driver
- Programmazione manutenzione veicoli
- Allocazione budget per zone

---

### 📅 Timeline di Integrazione

| Settimana | Modulo | Argomento | Focus |
|-----------|--------|-----------|-------|
| 1-2 | Stack | Undo/Redo | Basic implementation |
| 3-4 | Queue | Order Processing | Priority queue |
| 5-7 | BST | Route Optimization | Tree operations |
| 8-9 | Hash Table | Caching | Performance tuning |
| 10-11 | Linked List | Audit Trail | Sequential access |
| 12-15 | Graph | Network Optimization | Dijkstra, TSP |
| 16-18 | Sorting/Searching | Data Organization | Algorithm comparison |
| 19-21 | Dynamic Programming | Cost Optimization | Advanced DP problems |

### 📊 Integration Points nel Progetto

```csharp
// Esempio di integrazione: Order Processing Pipeline

public class ProcessOrderUseCase
{
    private readonly OrderQueue _queue;              // Queue<Order>
    private readonly OrderCommandHistory _history;  // Stack<Command>
    private readonly DeliveryNetwork _network;      // Graph
    private readonly UserCache _cache;              // HashMap
    private readonly AuditTrail _auditTrail;        // LinkedList
    
    public async Task ProcessOrder(Order order)
    {
        // 1. Enqueue l'ordine
        _queue.EnqueueOrder(order);  // Priority Queue
        
        // 2. Dequeue il prossimo ordine
        var nextOrder = _queue.DequeueNextOrder();
        
        // 3. Trovare il driver (cached lookup)
        var driver = _cache.GetDriver(nextOrder.AssignedDriverId);
        
        // 4. Ottimizzare la rotta
        var route = _network.FindShortestPath(
            driver.CurrentLocation, 
            nextOrder.DeliveryAddress);
        
        // 5. Registrare azione nell'audit trail
        _auditTrail.LogAction(
            new DeliveryAction(nextOrder, driver, route));
        
        // 6. Aggiungere comando allo stack per undo
        var command = new AssignOrderCommand(nextOrder, driver);
        _history.Push(command);
    }
}
```

---

## 🎯 Riepilogo Valutazioni

| N. | Valutazione | Durata | Periodo | Focus | Max Score |
|:--:|-------------|--------|---------|-------|:---------:|
| 0 | Setup | 1 sett | Inizio | Architettura | 10 |
| 1⭐ | DDD + CQRS | 3-4 sett | Prime sett | Domain + App (JSON) | 20 |
| 2⭐ | Web API | 2-3 sett | Post-1 | REST Endpoints | 20 |
| 3⭐ | MongoDB | 3-4 sett | Post-2 | Persistence layer | 15 |
| 4⭐ | Auth/Authz | 3-4 sett | Post-3 | Security | 15 |
| 5⭐ | Logging+Validation | 2-3 sett | Post-4 | Observability | 10 |
| 6⭐ | LINQ + Lambda | 2-3 sett | Post-5 | Query optimization | 10 |
| 7⭐ | Stack | 1-2 sett | Q2 inizio | Undo/Redo | 15 |
| 8⭐ | Queue | 2-3 sett | Q2 | Order processing | 15 |
| 9⭐ | BST | 2-3 sett | Q2 | Route optimization | 15 |
| 10⭐ | Hash Table | 1-2 sett | Q2 | Caching | 10 |
| 11⭐ | Linked List | 1-2 sett | Q2 | Audit trail | 10 |
| 12⭐ | Graphs | 3-4 sett | Q2 fine | Network optimization | 20 |
| 13⭐ | Sorting/Search | 2-3 sett | Q2 fine | Data organization | 15 |
| 14⭐ | Dynamic Programming | 2-3 sett | Fine anno | Cost optimization | 20 |

---

## ✅ Criteria di Valutazione Generale

Ogni valutazione sarà basata su:

### 🏗️ Architettura (25%)
- [ ] Clean Code principles rispettati
- [ ] SOLID principles implementati
- [ ] Separation of Concerns
- [ ] Design patterns appropriati
- [ ] Dependency Injection

### 🔨 Implementazione (35%)
- [ ] Funcionalità completate
- [ ] Edge cases gestiti
- [ ] Error handling robusto
- [ ] Performance accettabile
- [ ] Database queries ottimizzate

### 📝 Code Quality (20%)
- [ ] Naming conventions consistent
- [ ] Code style uniforme
- [ ] Documentation presente
- [ ] No code duplication
- [ ] Comments significativi

### 🧪 Testing (15%)
- [ ] Unit tests scritti
- [ ] Integration tests
- [ ] Test coverage > 70%
- [ ] Tests passano tutti
- [ ] Edge cases testati

### 📚 Documentation (5%)
- [ ] README aggiornato
- [ ] API documentation (Swagger)
- [ ] Code comments
- [ ] Commit messages chiare
- [ ] Architecture decision records

---

## 📖 Risorse di Apprendimento

### Phase 1-6 Resources
- **DDD**: "Domain-Driven Design" - Eric Evans
- **Clean Architecture**: Robert C. Martin blogs
- **CQRS/MediatR**: Jimmy Bogard tutorials
- **MongoDB**: Official docs + Pluralsight courses
- **Authentication**: Auth0 docs, JWT.io

### Phase 7+ Resources
- **Data Structures**: "Introduction to Algorithms" - CLRS
- **Algorithms**: GeeksforGeeks, LeetCode
- **Graph Algorithms**: YouTube - Abdul Bari, Kunal Kushwaha
- **Dynamic Programming**: Striver's DP series on YouTube

---

## 🚀 Getting Started

### Setup Environment
```bash
# 1. Clone repository
git clone <repo-url>
cd PascalLogistics

# 2. Install .NET 8+
dotnet --version

# 3. Restore packages
dotnet restore

# 4. Build solution
dotnet build

# 5. Run API
cd backend/src/BlaisePascal.PascalLogistics.Api
dotnet run
```

### First Code Review Checklist
- [ ] Solution compila senza errori
- [ ] Struttura cartelle rispettata
- [ ] User Aggregate modellato
- [ ] Commands/Queries implementati
- [ ] Repository pattern applicato
- [ ] JSON persistence funziona
- [ ] Test unitari presenti

---

## 📞 Code Review Process

Ogni valutazione avrà:

1. **Pre-Review**
   - Commit su branch dedicato
   - Pull request con descrizione
   - Self-review completato

2. **Review**
   - Code review con mentor
   - Discussione su decisioni architetturali
   - Feedback su SOLID principles

3. **Post-Review**
   - Fix issues richiesti
   - Rebase e merge
   - Document learnings

---

## 📝 Note Finali

- **Flessibilità**: Timeline può essere adattata in base al progresso
- **Integrazione**: Ogni fase integra le precedenti
- **Pratica**: Ogni modulo include esercizi pratici
- **Real-world**: Tutti i problemi riflettono real-world scenarios
- **Scalabilità**: L'architettura supporta crescita futura

**Buono sviluppo! 🚀**
