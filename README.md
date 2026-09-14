# IcaTask
Work Sample Assignment - Payment Approval Portal
# Payment Approval Portal

## Architecture

Controller
  ↓
Service
  ↓
Repository (ConcurrentDictionary)

## Business Rules

- Payments ≤ 10,000 SEK execute immediately.
- Payments > 10,000 SEK require approval.
- Every state transition creates an audit entry.

## Technical Choices
- ASP.NET Core MVC (.NET 8)
- Repository Pattern
- Dependency Injection
- ConcurrentDictionary for thread-safe in-memory storage
- Bootstrap UI
