# Finlytics

A **C#/.NET personal finance analytics application** built with SQLite for persistent financial data management, transaction tracking, financial reporting, and wishlist affordability forecasting.

## Overview

Finlytics is a console-based application designed to model real-world personal finance management.

The system allows users to create and manage financial profiles, record income and expenses, categorize transactions, monitor monthly spending, and create wishlist goals. It also analyzes financial information to determine whether a user can currently afford a target purchase and estimates the time required to save for it.

The project was developed to apply **Object-Oriented Programming, database integration, software architecture, and analytical problem-solving** to a practical application.

## Features

### Account & Profile Management

* Create multiple independent financial profiles
* List and switch between profiles
* Configure monthly income
* Configure target savings rate
* Maintain profile-specific financial data

### Transaction Management

* Record income and expense transactions
* Categorize transactions
* Store timestamps and optional notes
* View transaction history
* Update existing transactions
* Remove transactions

### Financial Analytics

* Generate monthly financial summaries
* Group transactions by category
* Compare monthly income and expenses
* Detect overspending conditions
* Identify when spending approaches the user's income threshold

### Wishlist & Affordability Forecasting

* Create wishlist items with target prices
* Track future purchase goals
* Compare available financial resources against target prices
* Determine whether an item is currently affordable
* Estimate the number of months required to save for an unaffordable item

### Validation & Exception Handling

* Validate numeric and monetary input
* Prevent invalid or negative values
* Handle missing records
* Use custom application exceptions
* Maintain normal program flow when invalid input is provided

## Architecture

The application follows a modular structure that separates user interaction, business logic, and data access.

```text
Console Interface
       │
       ▼
Service Layer
       │
       ├── AccountService
       ├── TransactionService
       ├── WishlistService
       └── ReportService
       │
       ▼
Repository Layer
       │
       └── IRepository<T>
       │
       ▼
DatabaseContext
       │
       ▼
SQLite Database
```

This separation keeps database operations independent from the application's business logic and makes the system easier to maintain and extend.

## Object-Oriented Design

The project applies the following OOP principles:

### Encapsulation

Important account values are protected within their respective classes, with validation rules controlling how they can be modified.

### Inheritance

`Income` and `Expense` are derived from the common `Transaction` model.

### Polymorphism

Different transaction types provide their own implementation of financial effects through overridden behavior.

### Abstraction

The generic `IRepository<T>` interface separates data-access operations from the application's business logic.

## Database

Finlytics uses a local **SQLite database** for persistent storage.

The database stores:

* Financial profiles
* Transaction records
* Wishlist targets
* Related financial information

The application initializes the database when required, allowing records to remain available after the application is closed and reopened.

## Technology Stack

| Technology         | Purpose                           |
| ------------------ | --------------------------------- |
| C# 12              | Application development           |
| .NET 10            | Application framework             |
| SQLite             | Persistent relational database    |
| ADO.NET            | Database interaction              |
| LINQ               | Financial reporting and analytics |
| Visual Studio 2022 | Development environment           |

## Key Components

| Component            | Responsibility                                              |
| -------------------- | ----------------------------------------------------------- |
| `DatabaseContext.cs` | Database initialization, schema, and database configuration |
| `IRepository<T>`     | Generic repository abstraction                              |
| `AccountService`     | Account and profile operations                              |
| `TransactionService` | Transaction management                                      |
| `WishlistService`    | Wishlist management                                         |
| `ReportService`      | Financial reports and forecasting                           |
| `ConsoleHelpers.cs`  | Input validation utilities                                  |
| `Income.cs`          | Income transaction model                                    |
| `Expense.cs`         | Expense transaction model                                   |
| `WishlistItem.cs`    | Wishlist goal model                                         |

## My Role

**Team Lead & Core Developer**

I was responsible for the core technical implementation of the project, including:

* Configuring the initial C# project structure
* Designing and implementing the SQLite database schema
* Implementing database initialization and triggers
* Designing the generic `IRepository<T>` interface
* Developing `AccountService`, `TransactionService`, and `WishlistService`
* Implementing financial forecasting in `ReportService`
* Developing LINQ-based monthly financial analytics
* Building input validation utilities
* Creating custom application exceptions
* Defining core financial domain models and business rules

## Challenges & Solutions

### Persistent Data

An early implementation relied on in-memory data, which caused financial records to be lost when the application closed.

The system was redesigned to use SQLite for persistent local storage, allowing records to remain available across application sessions.

### Separation of Responsibilities

As the application grew, combining database operations with business logic would have made the system harder to maintain.

A repository and service-based structure was introduced to separate:

```text
Data Access
     ↓
Business Logic
     ↓
User Interaction
```

### Project Configuration

Configuration issues in the project file caused build-time errors during development.

The project configuration was reorganized to correctly define the application structure and dependencies.

## What I Learned

This project strengthened my understanding of:

* Object-oriented programming
* Database integration with C#
* Layered software architecture
* Repository and service patterns
* LINQ-based data processing
* Business-rule implementation
* Input validation and exception handling
* Designing software around real-world requirements

## Future Improvements

* Add a WPF or Windows Forms graphical interface
* Introduce financial charts and visual analytics
* Support recurring transactions
* Add cloud synchronization
* Integrate external financial APIs

## Limitations

Finlytics is currently a **local console-based application** using SQLite.

It does not currently provide:

* Live bank or credit-card API integration
* Cloud synchronization
* Distributed multi-device storage
* A graphical user interface

## Project Context

**Course:** Object-Oriented Programming
**Project Type:** Console-based Desktop Application
**Language:** C#
**Framework:** .NET 10
**Database:** SQLite
**Development Environment:** Visual Studio 2022

## Author

**Arooba Kinza**
BS Computer Science — DHA Suffa University

**Role:** Team Lead & Core Developer
