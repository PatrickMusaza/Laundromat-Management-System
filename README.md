# **Laundromat Management System (LMS)**

## **📋 Project Overview (Startup Idea)**

The Laundromat Management System (LMS) is a comprehensive, enterprise-grade POS system designed for multi-location laundromat operations. The system integrates coin/card-operated machines, RRA EBM compliance, IoT device management, and remote administration capabilities.

### **✨ Key Features**
- **Multi-payment Processing**: Cash, Mobile Money, and Card payments
- **RRA EBM Compliance**: Real-time tax reporting with offline fallback
- **IoT Integration**: RS485 communication with laundry machines and payment devices
- **Remote Management**: Cloud-based control of pricing, promotions, and reporting
- **Multi-language Support**: English, Kinyarwanda, and French interfaces
- **Offline Operation**: Local transaction processing with automatic sync
- **Touchscreen POS**: WPF-based desktop application with virtual keyboard

---

## **🏗️ System Architecture**

### **High-Level Overview**
```
┌───────────────────────────────────────────────────────┐
│                    Cloud Infrastructure               │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐    │
│  │ Management  │  │   Central   │  │ External    │    │
│  │   Portal    │  │  Database   │  │   APIs      │    │
│  └─────────────┘  └─────────────┘  └─────────────┘    │
└────────────────────────────┬──────────────────────────┘
                             │ HTTPS/WebSocket
┌────────────────────────────┴──────────────────────────┐
│                    Branch Infrastructure              │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐    │
│  │  POS App    │  │ Local API   │  │ IoT Devices │    │
│  │  (WPF)      │  │  Server     │  │ (RS485)     │    │
│  └──────┬──────┘  └──────┬──────┘  └─────────────┘    │
│         │                │                            │
│  ┌──────▼──────┐  ┌──────▼──────┐                     │
│  │   Local     │  │ RRA EBM     │                     │
│  │  Database   │  │ Integration │                     │
│  └─────────────┘  └─────────────┘                     │
└───────────────────────────────────────────────────────┘
```

### **Technology Stack**

| Component | Technology | Purpose |
|-----------|------------|---------|
| **POS Application** | WPF (.NET 6+) with MVVM pattern | Customer-facing touchscreen interface |
| **Local API Server** | ASP.NET Core Web API | Local business logic and device integration |
| **Cloud Portal** | Blazor Server/React with .NET Backend | Remote management dashboard |
| **Database** | PostgreSQL (local & cloud) + Redis Cache | Data persistence and caching |
| **IoT Communication** | SerialPort with RS485 protocol | Device control and monitoring |
| **Payment Processing** | Integration with local payment devices | Cash, mobile money, and card payments |
| **EBM Integration** | HTTP/REST API with fallback | RRA tax compliance |
| **Containerization** | Docker + Docker Compose | Deployment and environment consistency |

---

## **🚀 Getting Started**

### **Prerequisites**
- **.NET 6.0 SDK** or later
- **Docker Desktop** (for containerized deployment)
- **PostgreSQL 15** (or use Docker image)
- **Redis** (for caching)
- **Visual Studio 2022** or VS Code with C# extensions

### **Quick Setup**

1. **Clone the Repository**
```bash
git clone https://github.com/PatrickMusaza/laundromat-management-system.git
cd laundromat-management-system
```

2. **Environment Configuration**
```bash
# Copy example configuration
cp .env.example .env
cp appsettings.Development.example.json src/Laundromat.API/appsettings.Development.json
```

3. **Start Development Environment**
```bash
# Using Docker Compose
docker-compose -f docker-compose.dev.yml up -d

# Or run locally
cd src/Laundromat.API
dotnet restore
dotnet run
```

4. **Initialize Database**
```bash
dotnet ef database update --project src/Laundromat.API
```

### **Development Structure**
```
laundromat-system/
├── src/
│   ├── Laundromat.POS/          # WPF desktop application
│   ├── Laundromat.API/          # Local API server
│   ├── Laundromat.CloudPortal/  # Cloud management portal
│   └── Laundromat.Shared/       # Shared models and utilities
├── tests/
│   ├── UnitTests/               # Unit tests
│   ├── IntegrationTests/        # Integration tests
│   └── E2ETests/               # End-to-end tests
├── docs/                        # Documentation
├── docker/                      # Docker configurations
└── scripts/                     # Deployment and utility scripts
```

---

## **🛠️ Development Guide**

### **Building the Solution**
```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build --configuration Release

# Run tests
dotnet test
```

### **Running Locally**

#### **Option 1: Docker Compose (Recommended)**
```bash
# Development environment
docker-compose -f docker-compose.dev.yml up --build

# Test environment
docker-compose -f docker-compose.test.yml up --build
```

#### **Option 2: Manual Setup**
```bash
# 1. Start database services
docker run -d -p 5432:5432 --name postgres-dev -e POSTGRES_PASSWORD=password postgres:15
docker run -d -p 6379:6379 --name redis-dev redis:7-alpine

# 2. Run API server
cd src/Laundromat.API
dotnet run

# 3. Run POS application
cd src/Laundromat.POS
dotnet run

# 4. Run cloud portal
cd src/Laundromat.CloudPortal
dotnet run
```

### **Testing**
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/UnitTests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Integration tests
docker-compose -f docker-compose.test.yml up -d
dotnet test tests/IntegrationTests/
```

---

## **📁 Project Structure Details**

### **Laundromat.POS (WPF Application)**
```
Laundromat.POS/
├── Views/                    # XAML views
│   ├── MainWindow.xaml      # Main POS screen
│   ├── PaymentView.xaml     # Payment processing
│   └── SettingsView.xaml    # System settings
├── ViewModels/              # MVVM view models
├── Models/                  # Data models
├── Services/                # Business services
│   ├── TransactionService.cs
│   ├── PaymentService.cs
│   └── LocalizationService.cs
├── Controls/                # Custom controls
│   ├── VirtualKeyboard.xaml
│   └── ReceiptPreview.xaml
└── Resources/              # Localization resources
```

### **Laundromat.API (Local API Server)**
```
Laundromat.API/
├── Controllers/            # API endpoints
│   ├── TransactionsController.cs
│   ├── PaymentsController.cs
│   └── EBMsController.cs
├── Services/              # Business logic
│   ├── EBMIntegrationService.cs
│   ├── IoTService.cs
│   └── SyncService.cs
├── Data/                  # Data access
│   ├── ApplicationDbContext.cs
│   └── Migrations/
├── Models/                # Request/Response models
├── Middleware/           # Custom middleware
└── BackgroundServices/   # Hosted services
```

### **Database Schema Highlights**
```sql
-- Core Tables
- branches           # Branch information
- transactions       # Transaction records
- transaction_items  # Line items
- ebm_queue_items    # Pending EBM submissions
- iot_devices        # Connected devices
- customers          # Customer information
- services           # Available services
- sync_logs          # Synchronization logs
```

---

## **🔧 Configuration**

### **Environment Variables**
```bash
# Database
DB_CONNECTION_STRING=Host=localhost;Database=laundromat;Username=postgres;Password=password

# Redis
REDIS_CONNECTION_STRING=localhost:6379

# RRA EBM
RRA_EBM_BASE_URL=https://ebm.rra.gov.rw/api/v1
RRA_EBM_API_KEY=your-api-key
RRA_EBM_SANDBOX_URL=https://sandbox.ebm.rra.gov.rw/api/v1

# IoT Configuration
SERIAL_PORT=COM3
BAUD_RATE=9600

# Security
JWT_SECRET_KEY=your-secret-key
ENCRYPTION_KEY=your-encryption-key
```

### **Application Settings**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=laundromat;Username=postgres;Password=password",
    "Redis": "localhost:6379"
  },
  "EBM": {
    "BaseUrl": "https://ebm.rra.gov.rw/api/v1",
    "SandboxUrl": "https://sandbox.ebm.rra.gov.rw/api/v1",
    "ApiKey": "your-api-key",
    "TimeoutSeconds": 30
  },
  "IoT": {
    "SerialPort": "COM3",
    "BaudRate": 9600,
    "DeviceAddresses": {
      "CoinMachine": 1,
      "CardReader": 2
    }
  }
}
```

---

## **🚀 Deployment**

### **Local Branch Deployment**
```bash
# Build and deploy to a single branch
./scripts/deploy-branch.sh --branch-id=001 --environment=production

# Or using Docker Compose
docker-compose -f docker-compose.production.yml up -d
```

### **Cloud Portal Deployment**
```bash
# Deploy to Azure
az webapp up --name laundromat-portal --resource-group Laundromat-RG

# Deploy to AWS
./scripts/deploy-aws.sh
```

### **Update Mechanism**
The system supports remote updates without on-site visits:
1. Cloud portal pushes update package
2. Local system downloads and validates
3. Update applied during low-activity periods
4. Automatic rollback on failure

---

## **📊 Monitoring & Maintenance**

### **Health Checks**
```bash
# API health
curl https://localhost:5001/health

# Database health
curl https://localhost:5001/health/database

# RRA EBM connectivity
curl https://localhost:5001/health/ebm
```

### **Logging**
- **Application Logs**: Structured logging with Serilog
- **Audit Logs**: All transactions and system changes
- **Error Tracking**: Integration with Application Insights
- **Performance Metrics**: Prometheus + Grafana dashboard

### **Backup & Recovery**
```bash
# Daily backup
./scripts/backup-database.sh

# Restore from backup
./scripts/restore-database.sh --backup-file=backup-2026-01-15.sql
```

---

## **🔒 Security Features**

### **Data Protection**
- **Encryption at Rest**: AES-256 for sensitive data
- **Encryption in Transit**: TLS 1.3 for all communications
- **Secure Authentication**: JWT with refresh tokens
- **Role-Based Access Control**: 4-tier permission system
- **PCI DSS Compliance**: For card payment processing

### **Security Best Practices**
1. Regular security audits
2. Dependency vulnerability scanning
3. Penetration testing
4. Security headers implementation
5. Input validation and sanitization

---

## **🤝 Contributing**

### **Development Workflow**
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### **Code Standards**
- Follow C# coding conventions
- Write unit tests for new features
- Update documentation accordingly
- Use meaningful commit messages
- Ensure backward compatibility

### **Branch Naming Convention**
- `feature/` - New features
- `bugfix/` - Bug fixes
- `hotfix/` - Critical production fixes
- `release/` - Release preparation
- `docs/` - Documentation updates

---

## **📞 Support & Contact**

### **Team Contacts**
- **Project Manager**: [Name] - [email@example.com]
- **Technical Lead**: [Name] - [email@example.com]
- **Support Team**: support@laundromat-system.com

### **Emergency Contacts**
- **Critical System Failure**: +250 788 111 111
- **Security Incident**: security@laundromat-system.com
- **RRA Compliance Issues**: compliance@laundromat-system.com

### **Documentation**
- [API Documentation](docs/api.md)
- [User Manual](docs/user-manual.md)
- [Administration Guide](docs/admin-guide.md)
- [Troubleshooting](docs/troubleshooting.md)

---

## **📄 License**

This project is proprietary and confidential. All rights reserved.

---

## **📈 Status**

| Component | Status | Version | LMSt Updated |
|-----------|--------|---------|--------------|
| POS Application | 🟢 Production Ready | 1.0.0 | 2026-01-15 |
| Local API Server | 🟢 Production Ready | 1.0.0 | 2026-01-15 |
| Cloud Portal | 🟡 Beta Testing | 0.9.0 | 2026-01-15 |
| EBM Integration | 🟢 Production Ready | 1.0.0 | 2026-01-15 |
| IoT Integration | 🟢 Production Ready | 1.0.0 | 2026-01-15 |

---

## **🔄 Changelog**

### **v1.0.0 (2026-01-15)**
- Initial production release
- Complete POS functionality
- RRA EBM integration
- Multi-language support
- Remote management capabilities

### **v0.9.0 (2023-12-01)**
- Beta release with core features
- Basic payment processing
- Local database implementation
- Initial IoT device integration

---

## **🎯 Roadmap**

### **Q1 2026**
- [ ] Mobile app for order tracking
- [ ] Advanced analytics dashboard
- [ ] Integration with delivery services

### **Q2 2026**
- [ ] Loyalty program implementation
- [ ] Predictive maintenance features
- [ ] Additional payment methods

### **Q3 2026**
- [ ] AI-powered demand forecasting
- [ ] Smart energy management
- [ ] Enhanced security features

---

*LMSt Updated: January 05, 2026*
*System Version: 1.0.0*