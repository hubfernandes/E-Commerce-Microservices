# **E-Commerce Microservices Architecture**

## **Overview**
This project is a **highly scalable** and **maintainable** e-commerce system built using **ASP.NET Core 8, Clean Architecture, and Microservices**. Each service follows **domain-driven design (DDD)** principles and is deployed as an independent unit, ensuring flexibility and resilience.

## **Technologies Used**
- **.NET 8** - Modern, high-performance framework
- **C#** - Core development language
- **Entity Framework Core** - Database ORM
- **Ocelot API Gateway** - API aggregation and routing
- **RabbitMQ** - Event-driven messaging
- **Redis** - Caching and data synchronization
- **Docker** - Containerization and deployment
- **SQL Server** - Relational database management

## **Architecture**
The system follows **Clean Architecture**, separating concerns into distinct layers:
- **Api Layer**: Handles API requests
- **Application Layer**: Business logic and use cases
- **Domain Layer**: Core entities, aggregates, and domain services
- **Infrastructure Layer**: Data persistence, external services, and message brokers

## **Microservices**
Each service operates independently and communicates through **RabbitMQ (event-driven)** or **HTTP APIs (via Ocelot Gateway)**.

### **1. Authentication Service (🔑 AuthService)**
- Manages user authentication and authorization
- Implements JWT-based security
- Handles user registration and login

### **2. Product Service (🛒 ProductService)**
- Manages product catalog (CRUD operations)
- Stores product details, pricing, and availability
- Publishes events when product stock changes

### **3. Order Service (📦 OrderService)**
- Processes customer orders
- Ensures payment validation before order confirmation
- Implements retry mechanisms for failed transactions

### **4. Inventory Service (📦 InventoryService)**
- Tracks stock levels and updates product availability
- Subscribes to product stock updates from RabbitMQ
- Notifies Order Service of stock shortages

### **5. Cart Service (🛍️ CartService)**
- Manages user shopping carts
- Supports session-based and persistent cart storage
- Syncs with Inventory Service to prevent stock issues

### **6. Payment Service (💳 PaymentService)**
- Handles transactions securely
- Integrates with third-party payment gateways
- Uses RabbitMQ to notify Order Service upon successful payment

### **7. Wishlist Service (❤️ WishListService)**
- Allows users to save favorite products
- Syncs with Product Service for availability updates
- Uses Redis for fast retrieval

### **8. Shared Services (🔄 SharedServices)**
- Provides utilities such as logging, notifications, and email services
- Ensures cross-cutting concerns are handled consistently

## **API Gateway (🐆 Ocelot)**
- Routes all requests to the appropriate microservices
- Implements authentication, caching, and rate-limiting
- Enhances security by preventing direct access to services

## **Message Broker (📩 RabbitMQ)**
- Facilitates asynchronous communication between services
- Ensures system resilience by handling event-based updates
- Used for order processing, inventory updates, and payment confirmation

## **Deployment (🐳 Docker)**
- Each service runs inside a **Docker container**
- **Docker Compose** is used to manage multi-container setup
- Scalable and deployable in **Kubernetes** or cloud platforms

## **How to Run Locally**
1. Clone the repository:  
   ```bash
   git clone https://github.com/your-repo/ecommerce-microservices.git
   cd ecommerce-microservices
   ```
2. Build and run with Docker:  
   ```bash
   docker-compose up --build
   ```
3. Access services via API Gateway:  
   ```
   http://localhost:5000/api/products
   http://localhost:5000/api/orders
   ```

## **Contributing**
We welcome contributions! Follow these steps:
1. Fork the repository
2. Create a new branch (`feature-branch`)
3. Commit your changes
4. Submit a pull request

## **Author**
**Mostafa Sharaby**

---
This README provides a clear, structured explanation of the project. Let me know if you need refinements! 🚀

