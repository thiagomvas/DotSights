using DotSights.CLI.Commands;
using System.CommandLine;

var root = new RootCommand("A CLI for interacting with DotSights and it's tool box. Visualize data in a table, start or stop the tracker or change configuration. ");

var displayCommand = new DisplayCommand();
displayCommand.Setup(root);

var configCommand = new ConfigCommand();
configCommand.Setup(root);

var trackerCommand = new TrackerCommand();
trackerCommand.Setup(root);

return await root.InvokeAsync(args);
