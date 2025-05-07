![Frame 39263](https://github.com/user-attachments/assets/d69fae32-4339-4239-8950-133a89763a82)

Gml (GamerVII Minecraft Launcher)
=======
The Minecraft Launcher is a user-friendly application designed to streamline the process of launching Minecraft and managing game settings. It provides an intuitive interface and essential features to enhance the Minecraft gaming experience.

## Installation

### Prerequisites

Before installing the GamerVII Launcher, ensure you have the following prerequisites:

- **.NET 8.0 SDK:** You need to have .NET 8.0 SDK installed on your system. You can download it
  from [Microsoft's official website](https://dotnet.microsoft.com/download/dotnet/8.0) or use a package manager
  suitable for your operating system.

- **Git:** Ensure Git is installed on your system. You can download it from
  the [Git website](https://git-scm.com/downloads) or use a package manager.

### Steps to Install GamerVII Launcher

1. **Clone the Repository:**
   Open a terminal and clone the repository using Git with the `--recursive` option:
   ```bash
   git clone --recursive https://github.com/GamerVII-NET/minecraft-launcher.git
   cd minecraft-launcher
   ```

2. **Build the Project:**
   Restore the dependencies and build the project using the .NET CLI:
   ```bash
   dotnet restore
   dotnet build
   ```

3. **Run the Launcher:**
   Once the project is built, you can run the launcher using the following command:
   ```bash
   dotnet run --project path/to/your/project
   ```

### Troubleshooting

If you encounter any issues during the installation, ensure the following:

- Ensure the .NET SDK is correctly installed and the `DOTNET_HOME` environment variable is set.
- Ensure Git is correctly installed and available in the terminal.
- Make sure you have the necessary permissions to run the build and execute the application.

For additional help, check the project's issue tracker or community forums.

### Updating

To update the GamerVII Launcher to the latest version, navigate to the project directory and pull the latest changes
from the repository:

```bash
git pull origin main
git submodule update --recursive --remote
dotnet build
```

Then, run the launcher again using the command mentioned earlier.

### Uninstallation

To uninstall the GamerVII Launcher, simply delete the project directory:

```bash
rm -rf minecraft-launcher
```
