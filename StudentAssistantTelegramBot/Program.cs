using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using System.Threading;

namespace StudentAssistantTelegramBot
{
    class Program
    {
        // бот
        public static readonly TelegramBotClient Bot = new TelegramBotClient("1751839958:AAF9dLZAu3u56Wx66mMLenx_IOGJ3CqxabA");

        // пользователи
        public static Students students = new Students();

        static void Main()
        {
            Thread BotMain = new Thread(ThreadMain);
            Thread WarnThread = new Thread(Shedule_Sender.Bot_SendWarn);

            BotMain.Start();
            WarnThread.Start();
        }


        static void ThreadMain()
        {
            string path = "students.txt";
            students.ReadStudentsList(path);
            Shedule_Sender.SetTestShed(ref students, 790754149);
            Console.WriteLine("\nБот включён. Для выключения нажмите любую кнопку.\n");

            Bot.OnMessage += OnMessage.Bot_OnMessage;


            Bot.StartReceiving();
            Console.ReadKey();
            Bot.StopReceiving();

            Console.WriteLine("\nБот выключен.\n");
            students.WriteStudentsList(path);
        }
    }
}
