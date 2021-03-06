using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using System.IO;
using System.Threading;

namespace StudentAssistantTelegramBot
{
    class Program
    {
        static string GetToken()
        {
            return File.ReadAllText("token.txt");
        }

        // бот
        public static readonly TelegramBotClient Bot = new TelegramBotClient(GetToken());

        // пользователи
        public static Students students = new Students();

        //БД армейских анекдотов
        public static string[] MilJokes = new string[0];

        //БД всей музыки без сортировки по жанрам
        public static string[] Allmus = new string[0];

        //БД музыки жанра Rock
        public static string[] Rockmus = new string[0];

        //БД музыки жанра Alternative
        public static string[] Altmus = new string[0];

        //БД музыки жанра Pop
        public static string[] Popmus = new string[0];

        //БД музыки жанра Classic
        public static string[] Classicmus = new string[0];

        //Путь к файлу students
        public static string path = "students.txt";

        //Флаг завершения
        public static bool exit = false;

        static void Main()
        {
            Thread BotMain = new Thread(ThreadMain);
            Thread WarnThread = new Thread(Shedule_Sender.Bot_SendWarn);

            BotMain.Start();
            WarnThread.Start();

            string consoleInput = "";
            while (consoleInput != "stop")
            {
                consoleInput = Console.ReadLine();
            }
            exit = true;
            //Bot.StopReceiving();
            //BotMain.Abort();
            //WarnThread.Abort();
            Console.WriteLine("\nБот выключен окончательно.\n");
            //students.WriteStudentsList(path);
        }


        static void ThreadMain()
        {
            /*Область считывания БД из файлов (Поменять к защите или лучше предзащите)*/
            //string path = "students.txt";
            students.ReadStudentsList(path);


            //БД Secondary
            MilJokes = Secondary.PrmitiveFileReader("MilJoke.txt");
            Allmus = Secondary.GetMusic("Music.txt", "");
            Rockmus = Secondary.GetMusic("Music.txt", "*рок");
            Altmus = Secondary.GetMusic("Music.txt", "*альтернатива");
            Popmus = Secondary.GetMusic("Music.txt", "*поп");
            Classicmus = Secondary.GetMusic("Music.txt", "*классическая");

            /*Конец области заполнения БД*/
            //Shedule_Sender.SetTestShed(ref students, test_uid);
            Console.WriteLine("\nБот включён. Для выключения ввести stop.\n");

            Bot.OnMessage += OnMessage.Bot_OnMessage;


            Bot.StartReceiving();
            while (!exit)
            {
                if (exit)
                    Bot.StopReceiving();
            }
            Bot.StopReceiving();
            //Bot.OnMessage

            Console.WriteLine("\nБот выключен. Поток 1\n");
            students.WriteStudentsList(path);

        }
        /*// бот
        public static readonly TelegramBotClient Bot = new TelegramBotClient(GetToken());
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
        }*/
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
