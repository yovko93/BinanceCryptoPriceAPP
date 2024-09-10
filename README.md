## Overview
This project is a C# application that utilizes the Binance WebSocket API to collect price data for BTCUSDT, ADAUSDT, and ETHUSDT symbols. The application stores the price data in a relational database and exposes an HTTP API for querying average prices and calculating simple moving averages. It also includes a Console application for direct interaction. The application structure consists class libraries for managing the data logic and help services. This project contains a Docker setup for a PostgreSQL database and a Binance crypto price API.

## Tech Stack
- ASP.NET
- PostgreSQL
- EntityFramework
- Docker

## Installation and Setup
1. Clone the Repository:

  ```bash
  git clone https://github.com/yovko93/BinanceCryptoPriceAPP.git
  ```
2. Navigate to the `BinanceCryptoPriceAPI` project:

Open `appsettings.json` and place the connection string for your PostgreSQL database.

3. Navigate to the `CryptoPriceConsoleApp` project:

Open `appsettings.json` and place the connection string for your PostgreSQL database.

4. Open Package Manager Console and set as default project `Data`:
* Enter the following commands:
  
  ```powershell
  Add-Migration InitialCreate
  ```

  ```powershell
  Update-Database
  ```

5. Run `BinanceCryptoPriceAPI` or `CryptoPriceConsoleApp`

## Run the project with Docker
Follow the instructions below to set up and run the services in Docker containers.

#### Ensure you have the following installed on your system:
  * WSL
  * Docker Desktop for Windows

#### 1. Clone the Repository:
   
  ```bash
  git clone https://github.com/yovko93/BinanceCryptoPriceAPP.git
  ```

#### 2. Build and start the Docker containers:

The `docker-compose.yml` file contains the configuration to build and run the PostgreSQL database and the Binance Crypto Price API.

  * Open `PowerShell` in the root directory where `docker-compose.yml` is located

  * Use the following command to start the containers:

  ```bash
  docker-compose up --build -d
  ```

This will do the following:

  * Pull the latest PostgreSQL image and create a container (`postgresdb`).
  * Build the API container from the provided Dockerfile and create a service (`binancecryptopriceapi`).
  * Create a shared network (`cryptonetwork`) between the two containers.
  * Set up a persistent storage volume for PostgreSQL data (`postgres_data`).

#### 3. Verify that the services are running:

You can check if the containers are running with the following command:

  ```bash
  docker ps
  ```

  * The `postgresdb` container should be running on port `5432`.
  * The `binancecryptopriceapi` container should be accessible on port `8080` (or `8081` as configured).

#### 4. Access the API:

Once the containers are running, you can access the API by visiting:

  ```bash
  http://localhost:8080/http://localhost:8080/api/{symbol}/24hAvgPrice
  ```

#### 5. Stop the containers:

  ```bash
  docker-compose down
  ```
### Environment Variables
The environment variables for database connection are specified in the `docker-compose.yml` file under the `binancecryptopriceapi` service:

  ```yaml
  ConnectionStrings__DefaultConnection=Host=postgresdb;Port=5432;Database=crypto_price_data_db;Username=postgres;Password=123456789;
  ```
You can modify these values as needed.

### Persistent Data
The PostgreSQL container uses a Docker volume to store its data persistently, even if the container is stopped or removed. This volume is defined as:

  ```yaml
  volumes:
    postgres_data:
  ```

### Healthcheck
A health check is configured for the PostgreSQL service to ensure it is ready before the API container starts. The service will retry for a maximum of 5 times with a 10-second interval between attempts.

  ```yaml
  healthcheck:
    test: ["CMD-SHELL", "pg_isready -U postgres"]
    interval: 10s
    timeout: 5s
    retries: 5
  ```

## Project Structure

### 1. BinanceCryptoPriceAPI

The `BinanceCryptoPriceAPI` project provides a web-based interface for accessing cryptocurrency price data. This project is built with ASP.NET Core and offers several endpoints to fetch price information.

- **PriceController.cs**
  - `GET /api/{symbol}/24hAvgPrice`: Returns the average price for the last 24h of data in the database ( or the oldest available, if 24h of data is not available ).
  - `GET /api/{symbol}/SimpleMovingAverage`: Return the current Simple Moving Average (SMA) of the symbol's price.
 
- **WebSocketController.cs** - There is 3 endpoinds to control proccess for the websocket service
  - `POST /api/WebSocket/start`
  - `POST /api/WebSocket/stop`
  - `POST /api/WebSocket/restart`

### 2. Application

The `Application` project encapsulates business logic and background services for processing cryptocurrency data.

### 3. Data

The `Data` project contains the database context and entity models. It is responsible for initializing the database.

### 4. Models

The `Models` project include several response objects

### 5. CryptoPriceConsoleApp

The `CryptoPriceConsoleApp` is a Console application for direct interaction with the cryptocurrency data.

## License
This project is licensed under the MIT License.
