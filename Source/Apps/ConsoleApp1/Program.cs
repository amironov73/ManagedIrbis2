using System;
using System.Threading.Tasks;

using ManagedIrbis;

using static System.Console;

// ReSharper disable StringLiteralTypo
// ReSharper disable LocalizableElement

namespace ConsoleApp1
{
    class Program
    {
        static void HandleBusyChanged(object sender, EventArgs args)
        {
            IrbisConnection connection = (IrbisConnection) sender;
            WriteLine($"BUSY: {connection.Busy}");
        }

        static async Task AsyncMain()
        {
            using (var connection = new IrbisConnection())
            {
                connection.BusyChanged += HandleBusyChanged;

                connection.Username = "librarian";
                connection.Password = "secret";
                connection.Workstation = "A";

                if (!await connection.Connect())
                {
                    WriteLine("Не удалось подключиться!");
                    return;
                }

                var maxMfn = await connection.GetMaxMfn();
                WriteLine($"Max MFN: {maxMfn}");

                var formatted = await connection.FormatRecord("@brief", 123);
                WriteLine($"FORMATTED: {formatted}");

                var version = await connection.GetServerVersion();
                WriteLine($"{version.Organization} : {version.MaxClients}");

                var record = await connection.ReadRecord(123);
                WriteLine($"{record.Fields.Count}");

                var text = await connection.ReadTextFile("3.IBIS.WS.OPT");
                WriteLine(text);

                var files = await connection.ListFiles("3.IBIS.*.pft");
                WriteLine(string.Join(", ", files));

                var processes = await connection.ListProcesses();
                foreach (var process in processes)
                {
                    WriteLine(process);
                }

                var found = await connection.Search("\"A=ПУШКИН$\"");
                var count = await connection.SearchCount("\"A=ПУШКИН$\"");
                WriteLine($"COUNT {count}: " + string.Join(", ", found));

                object[] terms = await connection.ReadTerms("J=");
                WriteLine(string.Join("\n", terms));

                terms = await connection.ReadAllTerms("J=");
                WriteLine();
                WriteLine(string.Join("\n", terms));
                WriteLine();

                var records = await connection.SearchRead("\"A=ПУШКИН$\"", 10);
                WriteLine($"{records.Length}");

                record = await connection.SearchReadOneRecord("\"A=ПУШКИН$\"");
                WriteLine($"{record}");

                await connection.Disconnect();
            }
        }

        static void Main()
        {
            AsyncMain().Wait();
        }
    }
}
