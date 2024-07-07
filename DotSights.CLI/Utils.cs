using SharpTables;

namespace DotSights.CLI
{
    internal static class Utils
    {
        public const int RowsPerPage = 10;
        public static void EnterInteractableMode(Table original)
        {
            var pages = original.ToPaginatedTable(RowsPerPage); // Creates a paginated table from the original table
            while (true)
            {
                Console.Clear();
                pages.PrintCurrentPage();
                Console.WriteLine($"Page {pages.CurrentPageIndex + 1} of {pages.TotalPages}");
                Console.WriteLine("[<-] Previous Page \n[->] Next Page \n[S] Write as plain text \n[H] Write as HTML \n[M] Write as Markdown \n[F] Print full table \n[ESC] Exit");


                var key = Console.ReadKey().Key;
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        pages.PreviousPage(); // Prints the previous page and moves the page index back
                        break;
                    case ConsoleKey.RightArrow:
                        pages.NextPage(); // Prints the next page and moves the page index forward
                        break;
                    case ConsoleKey.Escape:
                        return;
                    case ConsoleKey.S:
                        Console.Clear();
                        Console.WriteLine(pages.Current.ToString()); // Writes the table as a string
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                    case ConsoleKey.H:
                        Console.Clear();
                        Console.WriteLine(pages.Current.ToHtml()); // Writes the table as HTML
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                    case ConsoleKey.M:
                        Console.Clear();
                        Console.WriteLine(pages.Current.ToMarkdown()); // Writes the table as Markdown
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                    case ConsoleKey.F:
                        Console.Clear();
                        original.Print(); // Prints the original table, before pagination
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
