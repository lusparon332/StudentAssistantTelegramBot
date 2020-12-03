using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;

namespace StudentAssistantTelegramBot
{
    class Program
    {
        // бот
        public static readonly TelegramBotClient Bot = new TelegramBotClient("1373599681:AAFoHfRgljVyPQvADGvcc2_-rrPIeo6CQV4");

        // пользователи
        public static Students students = new Students();

        static void Main()
        {
            string path = "students.txt";
            students.ReadStudentsList(path);
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

/*DateTime a = System.DateTime.Now;
            var dow = a.DayOfWeek;
            Console.WriteLine(dow.ToString());
            Console.WriteLine(dow.ToString().ToLower() == "monday");
            Dictionary<DayOfWeek, (int a, int end)> freeTime = new Dictionary<DayOfWeek, (int a, int end)>();
            freeTime.Add(DayOfWeek.ПН, (1, 2));
            foreach (var aa in freeTime.Values)
                Console.WriteLine(aa.a + aa.end);*/