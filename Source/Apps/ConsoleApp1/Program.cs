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
            using var connection = new IrbisConnection();

            connection.BusyChanged += HandleBusyChanged;
            connection.Host = "10.211.55.3";
            connection.Username = "librarian";
            connection.Password = "secret";
            connection.Workstation = "A";

            if (await connection.ConnectAsync() is null)
            {
                WriteLine("Не удалось подключиться!");
                return;
            }

            var maxMfn = await connection.GetMaxMfnAsync();
            WriteLine($"Max MFN: {maxMfn}");

            var formatted = await connection.FormatRecordAsync("@brief", 123);
            WriteLine($"FORMATTED: {formatted}");

            var version = await connection.GetServerVersionAsync();
            WriteLine($"{version.Organization} : {version.MaxClients}");

            var record = await connection.ReadRecordAsync(123);
            WriteLine($"{record.Fields.Count}");

            var text = await connection.ReadTextFileAsync("3.IBIS.WS.OPT");
            WriteLine(text);

            var files = await connection.ListFilesAsync("3.IBIS.*.pft");
            WriteLine(string.Join(", ", files));

            var processes = await connection.ListProcessesAsync();
            foreach (var process in processes)
            {
                WriteLine(process);
            }

            var found = await connection.SearchAsync("\"A=ПУШКИН$\"");
            var count = await connection.SearchCountAsync("\"A=ПУШКИН$\"");
            WriteLine($"COUNT {count}: " + string.Join(", ", found));

            object[] terms = await connection.ReadTermsAsync("J=");
            WriteLine(string.Join("\n", terms));

            terms = await connection.ReadAllTermsAsync("J=");
            WriteLine();
            WriteLine(string.Join("\n", terms));
            WriteLine();

            var records = await connection.SearchReadAsync("\"A=ПУШКИН$\"", 10);
            WriteLine($"{records.Length}");

            record = await connection.SearchReadOneRecordAsync("\"A=ПУШКИН$\"");
            WriteLine($"{record}");

            await connection.DisconnectAsync();

            WriteLine("THATS ALL, FOLKS!");
        } // method AsyncMain

        static void Main()
        {
            AsyncMain().Wait();
        }
    }
}
