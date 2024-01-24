using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Faktory.Core.Exceptions;
using Faktory.Core.Extensions;
using Faktory.Core.Logging;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Faktory.Core
{
    public static class FaktoryProgram
    {
        public const string AppName = "Faktory";
        public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static void Run(string[] argsArray)
        {
            PrintHeader();

            var args= string.Join(" ", argsArray);
            AnsiConsole.Status()
                .Start("Loading...", ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots2);
                Action<string> updateStatus = (s) => UpdateStatus(s, ctx);
                if (FaktoryRunner.BootUp(args, updateStatus) == false) return;

                var faktory = BuildCustomFaktory(updateStatus);
                if (FaktoryRunner.Run(faktory))
                {
                    PrintSummary(faktory);
                }
            });
        }

        static void PrintHeader() => AnsiConsole.MarkupLine($"{AppName.Colorify(LogColor.Yellow)} {Version.Colorify(LogColor.Green)}");

        static Faktory BuildCustomFaktory(Action<string> updateStatus)
        {
            try
            {
                Faktory instance = null;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var type = assembly.GetTypes().FirstOrDefault(t => t.IsClass && !t.IsAbstract && typeof(Faktory).IsAssignableFrom(t));
                    if (type == null) continue;

                    updateStatus($"Starting {type.Name} as Faktory...");
                    Boot.Logger.Verbose($"Starting {type.Name} as Faktory...");

                    instance = Activator.CreateInstance(type) as Faktory;
                    break;
                }

                if (instance == null)
                {
                    throw new MissingFaktoryException();
                }

                instance.SetStatusUpdater(updateStatus);
                return instance;
            }
            catch (MissingFaktoryException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new NotImplementedException("Unable to find implementation of Faktory.", e);
            }
        }

        static void PrintSummary(Faktory faktory)
        {
            var table = new Table();
            table.AddColumns("Task", "Result", "Duration");
            var duration = new TimeSpan();
            const string timeFormat = @"hh\:mm\:ss\:fff";

            foreach (var result in faktory.ActionResults)
            {
                var r = result.Success ? ":check_mark_button:" : ":cross_mark:";
                table.AddRow(RowParameters(result.Name, r, result.Duration.ToString(timeFormat)));
                duration += result.Duration;
            }

            table.AddRow(RowParameters(string.Empty, string.Empty, duration.ToString(timeFormat).Colorify(LogColor.Green)));

            AnsiConsole.WriteLine();
            AnsiConsole.Write(
                new Panel(table)
                    .Header("Build Summary")
                    .Collapse()
                    .RoundedBorder()
                    .BorderColor(Color.Green));
            return;

            static IEnumerable<IRenderable> RowParameters(params string[] values) => values.Select(v => new Markup(v));
        }

        static void UpdateStatus(string status, StatusContext ctx)
        {
            ctx.Status(status.Colorify(LogColor.Blue));
        }
    }
}