# Asset Price API - ReadMe

## Overview

# Asset Price API - ReadMe

## Overview

This application provides an API to manage asset information and their price data. The system is designed to:
- Store information about assets (e.g., stock data, commodities).
- Track the price history of assets from various sources on a daily basis.
- Allow CRUD operations on assets and prices.
- Provide price data for one or more assets, optionally from a specific source, for a specific date.

This API is designed using **ASP.NET Core** with **Entity Framework Core** for data persistence and **SQLite** as the database for demonstration purposes. In a real-world scenario, a more robust relational database management system (RDBMS) like **PostgreSQL** would be used for scalability and production deployment.

## Functionality

### 1. **AssetController**
The `AssetController` manages operations related to assets. It provides endpoints to:
- **Get all assets**: Retrieve a list of all assets in the system.
- **Get a specific asset**: Retrieve details of an asset by its ID.
- **Create a new asset**: Add a new asset to the system, including its name, symbol, and ISIN.
- **Update an existing asset**: Modify the details of an existing asset (e.g., update name, symbol).

### 2. **SourceController**
The `SourceController` is responsible for managing sources of price data. It provides endpoints to:
- **Get all sources**: Retrieve a list of all data sources (e.g., "Yahoo Finance", "Reuters").
- **Get a specific source**: Retrieve details of a specific source by its ID.
- **Create a new source**: Add a new source to the system.
- **Delete a source**: Remove a source from the system by its ID.

### 3. **PriceController**
The `PriceController` handles price data for assets. It allows for:
- **Get prices**: Retrieve prices for one or more assets for a specific date, optionally filtered by source.
- **Create or update prices**: Add or update the price for an asset from a specific source for a given date.

### 4. **Price History**
Each time a new price is added or updated, a **PriceHistory** record is created for auditing purposes. This allows for tracking changes in asset prices over time, providing an essential auditing mechanism in trading and financial applications.

---

## Database Design Choices

The database is designed to handle assets, sources, prices, and price histories efficiently. Below are the key design choices:

### 1. **Entities**
- **Asset**: Represents the asset being tracked (e.g., stocks, commodities). Each asset has a unique `Id`, `Name`, `Symbol`, and `ISIN`. The `Symbol` and `ISIN` are indexed to enforce uniqueness, ensuring that the system prevents duplicate assets from being created.
- **Source**: Represents the data sources from which asset prices are gathered (e.g., "Yahoo Finance"). The `Name` field is indexed to ensure the uniqueness of each source.
- **Price**: Represents the price of an asset on a specific date from a particular source. Each price is associated with an `AssetId`, `SourceId`, and `PriceDate`. The combination of these three fields is indexed to enforce the rule that there can only be one price per asset per source per date.
- **PriceHistory**: This table stores a historical record of asset prices over time. Each entry has a `PriceDate`, `AssetId`, `SourceId`, `PriceValue`, and an optional `ValidTo` date to mark when the price is no longer valid (for price updates). Price histories are stored in a separate table from the Prices table to optimize query performance, ensuring fast lookups in the Prices table.

### 2. **Price History Table for Auditing**
A key decision in the design was to include the `PriceHistory` table for auditing price changes. This allows for tracking historical prices of assets, making it easier to debug issues, analyze price fluctuations, and provide full transparency of price data. In trading software, price discrepancies or errors are often difficult to debug without knowing what prices were used at various points in time. The `PriceHistory` table enables this by keeping a record of all price changes along with timestamps.

### 3. **Foreign Keys and Indexing**
Foreign key relationships are used between:
- **Prices** and **Assets**: Ensures that each price is associated with a valid asset.
- **Prices** and **Sources**: Ensures that each price is linked to a valid source.
- **PriceHistory** and **Assets**, **PriceHistory** and **Sources**: Ensures that historical price data is properly linked to assets and sources.

Indexes are applied to the fields most frequently used in queries:
- **Assets**: Unique indexes on `Symbol` and `ISIN` ensure no duplicate assets exist.
- **Prices**: A unique composite index on `AssetId`, `SourceId`, and `PriceDate` to enforce a single price per asset per source per date.
- **PriceHistory**: An index on `AssetId`, `SourceId`, and `PriceDate` for faster retrieval of historical prices.

### 4. **Database Choice: SQLite**
SQLite is used for demonstration purposes in this project due to its simplicity and ease of setup. It is an embedded database, which makes it perfect for small-scale or demo applications. However, in a production environment, a more scalable RDBMS such as **PostgreSQL** would be more appropriate due to its support for larger datasets, better concurrency handling, and advanced features like replication and horizontal scaling.

---

## Program Design Choices

### 1. **Repository-Service-Controller Pattern**
This application follows the **Repository-Service-Controller** design pattern, which ensures clear separation of concerns and better testability:

- **Repository Layer**: Provides methods for direct interaction with the database (e.g., querying, adding, updating). This layer abstracts the database logic, making the application more maintainable and testable.

- **Service Layer**: Contains the business logic and uses the repository to interact with the database. The service layer is responsible for validating data, enforcing rules, and orchestrating database operations.

- **Controller Layer**: Exposes the API to the outside world. The controller receives HTTP requests, interacts with the service layer, and returns appropriate responses.

### 2. **Testability**
The separation of the repository, service, and controller layers enhances testability:
- **Repository tests** ensure that the database interaction works as expected.
- **Service tests** verify that business logic is correctly implemented.
- **Controller tests** check that the API behaves as expected under different conditions (e.g., valid input, invalid input, exceptions).

Each layer is tested independently, making the application more modular and easier to maintain.

---

## Running the Application Locally

To run the application locally, follow these steps:

1. **Clone the repository**:
   ```bash
   git clone https://github.com/your-repository/asset-price-api.git
   cd asset-price-api
2. **Restore dependencies**:

    Ensure you have the latest version of .NET SDK installed, then run:
    ```bash
    dotnet restore
3.  **Run the application**:

    Start the application with:
    ```bash
    dotnet run
    
4.  **Access Swagger UI:**

    Once the application is running, navigate to http://localhost:5000/swagger in your web browser. The Swagger UI will allow you to interact with the API endpoints and see the available functionality.


5. **Data Seeding**:

   By default, the application will seed the following data into the SQLite database on startup:
   - Example assets (e.g., "Microsoft Corporation", "Apple Inc.").
   - Example sources (e.g., "Reuters Market Data", "Yahoo Finance").
   - Example prices for the assets from various sources.
   
   The seeding process ensures that the application has some initial data to work with when you start it for the first time.