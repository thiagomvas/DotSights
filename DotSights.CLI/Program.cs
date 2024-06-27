using DotSights.CLI.Commands;
using System.CommandLine;

var root = new RootCommand("Foo! Bar!");

var displayCommand = new DisplayCommand();
displayCommand.Setup(root);

var configCommand = new ConfigCommand();
configCommand.Setup(root);

var trackerCommand = new TrackerCommand();
trackerCommand.Setup(root);

return await root.InvokeAsync(args);
