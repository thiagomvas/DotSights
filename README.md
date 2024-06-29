# DotSights
A screen usage tracking tool for Windows.


> [!IMPORTANT]  
> DotSights saves the data in a local file on the user's computer. The data is not sent to any server or shared with anyone. The data is only used for the user's own reference. **The developer is not responsible for any privacy issues** that may arise from the use of this tool or any sharing, intentional or not, of the data file.
>
> The user is responsible for the data file and its contents. Do not share the data file with anyone unless you are sure that you want to share the data with them.

## How is my data handled
As said previously, **all tracked data is saved locally**. Every second, the tracker checks which window is currently open using built in windows functions through ``user32.dll`` and saves it to the data dictionary.

After a specific interval set by the config file, the tracker [saves the data locally to a file](https://github.com/thiagomvas/DotSights/blob/master/DotSights.Tracker/Worker.cs#L74) after serializing it to json.

When running the tracker again, it'll [look for the data file in AppData/DotSights](https://github.com/thiagomvas/DotSights/blob/master/DotSights.Tracker/Worker.cs#L28), hold it in cache and keep running, with the cycle repeating.

All data handling is made through the main [DotSights](https://github.com/thiagomvas/DotSights/blob/master/DotSights.Core/DotSights.cs) class. The only project that changes data is ``DotSights.Tracker``, all the other ones only change settings or read said data.

## Instalation
### Tracker
To get the tracker, head to the [Releases](https://github.com/thiagomvas/DotSights/releases) page, find the latest release and download the ``DotSights.Tracker.Exe``.

For accurate tracking, it should run on start up. To do this, open your start up folder and add either the exe itself, or a shortcut to it. Make sure the exe is somewhere that won't be deleted.

## CLI
To install the dashboard, you need the .NET SDK

After it is installed, run ``dotnet tool install --global DotSights.CLI`` to install the tool globally. Use ``DotSights -h`` on the terminal for more help.

### Dashboard
**TO RUN THE DASHBOARD, YOU NEED .NET 8 INSTALLED**

Run the following commands:

```
git clone https://github.com/thiagomvas/DotSights.git
cd DotSights/DotSights.Dashboard
dotnet build
```
Right-click ``DotSights.Dashboard.exe``, create a shortcut and move it wherever you'd like.

For some reason, Avalonia does not like to be published, so that's the temporary way of building the Dashboard.
