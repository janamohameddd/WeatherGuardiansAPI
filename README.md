# WeatherGuardiansAPI

**WeatherGuardiansAPI** is a backend API for a next-generation weather forecasting & recommendation system. The application offers:

- Current weather and short-term forecast  
- Long-term prediction (e.g. 10 years into the future)  
- Multiple “Weather Guardians” (Blaze, Gale, Drizzle, Haze) each predicting a dimension of weather  
- A “Terra” aggregator that compiles all guardian outputs and gives a final forecast + advice  

---

## 🧱 Architecture & Components

- **GuardianResult.cs** — Model to represent the output from a single guardian (name, predicted condition, confidence, recommendation).  
- **TerraResult.cs** — Model to aggregate results from all guardians and provide the final output.  
- **DrizzleService.cs / DrizzleController.cs** — Logic + API endpoints for the “Drizzle” guardian (rain).  
- **HazeService.cs / HazeController.cs** — Logic + API endpoints for the “Haze” guardian (air quality).  
- (You likely have similar services/controllers for **Blaze** and **Gale**.)  
- The **Terra** component (service/controller) combines guardians’ results into a unified output.

Each guardian is responsible for predicting its aspect of the weather. Terra orchestrates calls to the guardians, aggregates their outputs, and produces a final recommendation (e.g. “safe to go outside” or “stay indoors”).

---

## 📂 Project Structure 

/src
/Controllers
DrizzleController.cs
HazeController.cs
TerraController.cs
BlazeController.cs
GaleController.cs
/Services
DrizzleService.cs
HazeService.cs
BlazeService.cs
GaleService.cs
TerraAggregatorService.cs
/Models
GuardianResult.cs
TerraResult.cs
/Utils / Helpers
(optional utility classes, e.g. for date handling, external API wrappers, etc.)
Program.cs
Startup.cs (or equivalent configuration)
appsettings.json


---

## 🚀 Getting Started

### Prerequisites

- .NET SDK (version X.Y or higher)  
- (Optional) An external weather data API key if you are integrating with real weather data  
- IDE or code editor (Visual Studio, Visual Studio Code, Rider, etc.)

## Installation & Setup

### 1. Clone the repository
git clone https://github.com/your-username/WeatherGuardiansAPI.git
cd WeatherGuardiansAPI

### 2. Configure settings
-Open appsettings.json (or appsettings.Development.json)
-Add your weather API key
-Set base URLs for external APIs if needed
-Adjust logging, ports, or other configuration

### 3. Build the project
dotnet build

### 4. Run the API
dotnet run

### 5. The API should now be running at:
http://localhost:5000
