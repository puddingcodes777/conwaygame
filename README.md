# Conway's Game of Full Stack Project

A complete implementation of Conway's Game of Life featuring an ASP.NET 8.0 backend API and a React Material-UI frontend.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [API Endpoints](#api-endpoints)
- [Game Implementation](#game-implementation)
- [Data Flow](#data-flow)
- [Dependencies](#dependencies)
- [Getting Started Quick Guide](#getting-started-quick-guide)
- [Demo Video](#demo-video)
- [License](#license)

## Overview

This full-stack application simulates Conway's Game of Life, a cellular automaton devised by mathematician John Conway. The system consists of a robust backend API for game logic and persistence, coupled with a modern React frontend for interactive visualization.

Reference: Conway's Game of Life on Wikipedia

## Architecture

Full Stack Architecture

```
ConwayGameOfLife/
├── backend/                          # ASP.NET 8.0 API
│   ├── ConwayGameOfLife.API/         # Web API Layer
│   ├── ConwayGameOfLife.Core/        # Business Logic Layer
│   └── ConwayGameOfLife.Infrastructure/ # Data Access Layer
└── frontend/                         # React TypeScript Application
    ├── src/
    │   ├── components/               # React components
    │   ├── configs/                  # Configuration files
    │   ├── constants/                # Application constants
    │   ├── lib/                      # Library utilities
    │   ├── services/                 # API services
    │   ├── theme/                    # Material-UI theme
    │   └── types/                    # TypeScript type definitions
    ├── public/                       # Static assets
    └── configuration files           # Build and linting configs
```

Backend Architecture

```
ConwayGameOfLife/
├── ConwayGameOfLife.API/ (Web API Layer)
│ ├── Controllers/ (API Controllers)
│ ├── DTOs/ (Data Transfer Objects)
│ ├── Properties/ (Assembly properties)
│ ├── appsettings.json (Application configuration)
│ ├── ConwayGameOfLife.API.http (API test requests)
│ └── Program.cs (Application entry point)
├── ConwayGameOfLife.Core/ (Business Logic Layer)
│ ├── DTOs/ (Core data transfer objects)
│ ├── Exceptions/ (Custom exceptions)
│ ├── Models/ (Domain models)
│ └── Services/ (Core services)
└── ConwayGameOfLife.Infrastructure/ (Data Access Layer)
├── Data/ (Data context and configurations)
├── Models/ (Infrastructure models)
├── Repositories/ (Data repositories)
└── Services/ (Infrastructure services)
```

Frontend Architecture

```
frontend/
├── build/                              # Build output directory
├── node_modules/                       # Dependencies
├── public/                             # Static files
│   └── index.html                      # Main HTML file
├── src/                               # Source code
│   ├── components/                     # React components
│   │   └── ConwayGame.tsx              # Main game component
│   ├── configs/                        # Configuration files
│   │   ├── endpoints/                  # API endpoint configurations
│   │   │   └── index.ts                # Endpoint definitions
│   │   └── index.ts                    # Main config exports
│   ├── constants/                      # Application constants
│   │   └── index.ts                    # Constant definitions
│   ├── lib/                           # Library utilities
│   │   └── axios.ts                    # Axios HTTP client configuration
│   ├── services/                      # API services
│   │   └── index.ts                    # Service layer exports
│   ├── theme/                         # Material-UI theme
│   │   └── index.ts                    # Theme configuration
│   ├── types/                         # TypeScript definitions
│   │   └── index.ts                    # Type definitions
│   ├── App.tsx                        # Main App component
│   ├── index.css                      # Global styles
│   └── index.tsx                      # Application entry point
├── .env                               # Environment variables
├── .gitignore                         # Git ignore rules
├── .prettierrc.json                   # Prettier configuration
├── eslint.config.js                   # ESLint configuration
├── package.json                       # Dependencies and scripts
├── package-lock.json                  # Lock file
├── tsconfig.json                      # TypeScript configuration
└── webpack.config.js                  # Webpack build configuration
```

## Features

### Backend

- **Board Management:** Upload and store initial board states
- **Generation Simulation:** Calculate next states and multiple generations ahead using `Hashlife` and `QuadTree` Algorithm
- **Stability Detection:** Identify final stable or cyclical states
- **Persistence:** SQL Server database storage for board states
- **RESTful API:** Clean HTTP endpoints with proper status codes
- **Error Handling:** Comprehensive exception handling and validation
- **Modular Architecture:** Clean separation of concerns with dependency injection

### Frontend

- **Type Safety:** Full TypeScript implementation for better development experience
- **Interactive Board:** Visual grid with click-to-toggle cells
- **Real-time Simulation:** Animated generation transitions
- **Responsive Design:** Material-UI components for consistent UI
- **Modern Build System:** Webpack with hot reload for development
- **Code Quality:** ESLint and Prettier for consistent code style
- **Toast Notifications:** React Toastify for user feedback

## Prerequisites

### Backend Requirements

- .NET 8.0 SDK
- SQL Server or SQL Server Express
- Visual Studio 2022 or VS Code

### Frontend Requirements

- Node.js (version 16 or higher)
- npm (comes with Node.js)

## Installation

1. **Clone the Repository**
   - `git clone <repository-url>`
   - `cd ConwayGameOfLife`

2. **Backend Setup**
   - Update the connection string in `ConwayGameOfLife.API/appsettings`
   - `cd ConwayGameOfLife.API`
   - `dotnet restore`
   - `dotnet build`
   - `dotnet run` / `dotnet watch run`

3. **Frontend Setup**
   - `cd frontend`
   - `npm install`
   - `BASE_URL=https://localhost:7145/api`
   - `npm run dev`

## API Endpoints

### Board Management

- **Upload Board State**
  - **POST** `/api/Board/upload`
  - **Description:** Upload a new 2D grid board state
  - **Request Body:**
    ```json
    {
        "liveCells": [
            {
                "x": 0,
                "y": 0
            }
        ]
    }
    ```
  - **Response Body:**
    ```json
    {
        "boardId": 0
    }
    ```

- **Get All Boards**
  - **GET** `/api/Board/all`
  - **Description:** Retrieve all stored boards
  - **Response:** List of board summaries

- **Get Current Board State**
  - **GET** `/api/Board/{id}/current`
  - **Description:** Get the current state of a specific board
  - **Response:** Board state with current generation

### Simulation Endpoints

- **Simulate Board**
  - **POST** `/api/Board/simulate`
  - **Description:** Perform simulation operations on a board and get final state
  - **Request Body:**
    ```json
    {
        "liveCells": [
            {
                "x": 0,
                "y": 0
            }
        ],
        "maxStepNum": 2147483647
    }
    ```
  - **Response Body:**
    ```json
    {
        "liveCells": [
            {
                "x": 0,
                "y": 0
            }
        ],
        "periodicNum": 0,
        "generateStepNum": 0
    }
    ```

- **Get Next Generation**
  - **GET** `/api/Board/{id}/next`
  - **Description:** Calculate and return the next generation of the board
  - **Response:** Updated board state after one generation

- **Get N Generations Ahead**
  - **GET** `/api/Board/{id}/next/{generationNum}`
  - **Description:** Calculate and return the board state after N generations
  - **Response:** Board state after specified number of generations

## Game Implementation

- The core simulation in `HashtifeGameOfLifeService.cs` follows standard Conway's Game of Life rules:
  - **Underpopulation:** Live cell with fewer than 2 live neighbors dies
  - **Survival:** Live cell with 2 or 3 live neighbors lives on
  - **Overpopulation:** Live cell with more than 3 live neighbors dies
  - **Reproduction:** Dead cell with exactly 3 live neighbors becomes alive

- **Algorithm Details:**
  
  `Hashlife Algorithm`: Bill Gosper's revolutionary approach (1984)

  `Quadtree Data Structures`: Spatial partitioning for efficient  computation

  `Memoization Patterns`: Dynamic programming optimization

  `Cellular Automata Theory`: Mathematical foundations of Game of Life

## Data Flow

```
Request → Controller: HTTP requests received by BoardController
Controller → Service: Controllers call GameService for business logic
Service → Repository: Services use BoardRepository for data persistence
Repository → Database: EF Core handles database operations
Algorithm Service: HashtifeGameOfLifeService performs cell state calculations
```

## Dependencies

### Main NuGet Packages

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.17" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
```

### NPM Packages

```json
{
    "@emotion/react": "^11.14.0",
    "@emotion/styled": "^11.14.0",
    "@mui/icons-material": "^7.1.1",
    "@mui/material": "^7.1.1",
    "axios": "^1.10.0",
    "react": "^19.1.0",
    "react-dom": "^19.1.0",
    "react-router-dom": "^7.6.2",
    "react-toastify": "^11.0.5"
}
```


**Key Dependency Purposes:**

- **React 19:** Latest React with concurrent features
- **Material-UI:** Component library for consistent UI
- **Axios:** HTTP client for API communication
- **React Router DOM:** Client-side routing
- **React Toastify:** Toast notifications
- **Emotion:** CSS-in-JS styling solution

## Getting Started Quick Guide

- **Start Backend:** `cd backend && dotnet run`
- **Start Frontend:** `cd frontend && npm run dev`
- **Open Browser:** Navigate to `http://localhost:3000`
- **Create Pattern:** Click cells on the game board
- **Start Simulation:** Use the control buttons to begin simulation

## Demo Video

Watch the application in action:

<p align="center">
  <img src="assets/demo-video.gif" alt="Demo Video" width="100%" />
  <br>
  <i>ConwayGameOfLife Implementation</i>
</p>

### What the demo shows:

- ✅ Board creation and cell toggling
- ✅ Get Next Step
- ✅ Get Next N Ahead
- ✅ Simulate and Get Final Stable

## License

This project is created as part of a coding assessment exercise.

### Production Ready Features:

- ✅ Clean Architecture with separation of concerns
- ✅ Comprehensive error handling and validation
- ✅ Database persistence with Entity Framework
- ✅ RESTful API design with proper HTTP status codes
- ✅ Modular and testable code structure
- ✅ Configuration management
- ✅ Logging and monitoring readiness