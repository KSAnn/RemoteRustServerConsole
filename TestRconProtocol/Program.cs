using System.Net.WebSockets;
using RustRcon;
using RustRcon.Types.Commands.Server;
using RustRcon.Types.Server.Messages;

class Programm
{
    public static string ip = null;
    public static string port = null;
    public static string pass = null;
    public static string str = null;
    public static RconClient rc;
    public static void Main(string[] args)
    {
        if (args.Count() > 0)
        {
            ip = args[0].ToString();
            port = args[1].ToString();
            pass = args[2].ToString();
        }
        else
        {
            Init();
        }
        Connect();
        Thread.Sleep(1000);
        while (rc.State.Value == WebSocketState.Open)
        {
            rc.OnConsoleMessage += onConsoleMassage_InCome;
            rc.OnChatMessage += onChatMessage_InCome;
            Thread.Sleep(100);
            Console.WriteLine("Enter command ==========================================================");
            str = Console.ReadLine();
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write("\r" + new string(' ', Console.BufferWidth) + "\r");
            Console.SetCursorPosition(0, Console.CursorTop - 2);
            Console.Write("\r" + new string(' ', Console.BufferWidth) + "\r");
            Console.WriteLine($"Command: {str}");
            Console.WriteLine(RespSrv(str).Result);
        }
    }
    static async void Connect()
    {
        rc = new RconClient(ip, int.Parse(port), pass);
        await rc.ConnectAsync();
    }
    static void Init()
    {
        Console.WriteLine("Enter adres or IP:");
        ip = Console.ReadLine();
        Console.WriteLine("Enter rcon port:");
        port = Console.ReadLine();
        Console.WriteLine("Enter rcon password:");
        pass = Console.ReadLine();
        Console.WriteLine("Initialaze...");
    }
    static void onChatMessage_InCome(ChatMsg e)
    {
        if (e.Message != "")
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write("\r" + new string(' ', Console.BufferWidth) + "\r");
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine($"Chat: {e.Message}");
            Console.WriteLine("Enter command ==========================================================");
        }
        e.Dispose();
    }
    static void onConsoleMassage_InCome(ConsoleMsg e)
    {
        if (e.Message != "")
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write("\r" + new string(' ', Console.BufferWidth) + "\r");
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine($"{e.Message}");
            Console.WriteLine("Enter command ==========================================================");

        }
        e.Dispose();
    }
    static async Task<string> RespSrv(string comm)
    {
        var command = ConsoleCommand.Create(comm);
        try
        {
            command = ConsoleCommand.Create(comm);
            await rc.SendCommandAsync(command).WaitAsync(new TimeSpan(0, 0, 2));
            string result = command.ServerResponse.Content.ToString();
            command.Dispose();
            return result;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        finally
        {
            command.Dispose();
        }
    }
}