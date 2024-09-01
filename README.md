## Overview
This project is a C# application that utilizes the Binance WebSocket API to collect price data for BTCUSDT, ADAUSDT, and ETHUSDT symbols. The application stores the price data in a relational database and exposes an HTTP API for querying average prices and calculating simple moving averages. It also includes a Console application for direct interaction. The application structure consists class libraries for managing the data logic and help services.

## Tech Stack
- ASP.NET
- PostgreSQL
- EntityFramework

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
