You can run the system in two different ways:
1. Project Tye

    Install tye in console/powershell with
    ```powershell
    dotnet tool install --global Microsoft.Tye --version 0.4.0-alpha.20371.1
    ```
    see [Microsoft.Tye@Nuget](https://www.nuget.org/packages/Microsoft.Tye/0.4.0-alpha.20371.1)

    In the `src` folder run `tye run` to start the whole system. After that, you can watch the status at `http://127.0.0.1:8000`, and attach to each process with VisualStudio etc.

2. Standalone

    You can start each service by itself to get a better debugging experience (esp. at startup), but you also have to run the appropriate containers. Have a look at `tye.yaml` and `launchsettings.json` of each service to see the details.