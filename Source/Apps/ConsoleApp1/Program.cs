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
        static async Task AsyncMain()
        {
            using (var connection = new IrbisConnection())
            {
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
                WriteLine(string.Join(", ", found));

                await connection.Disconnect();
            }
        }

        static void Main()
        {
            AsyncMain().Wait();
        }
    }
}
