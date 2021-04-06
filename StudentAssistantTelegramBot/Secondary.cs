using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace StudentAssistantTelegramBot
{
    class Secondary
    {
        /// <summary>
        /// Вернёт рандомный анекдот
        /// </summary>
        /// <returns></returns>
        public static string RandMilJoke()
        {
            
            string Joke = "_Joke";
            try
            {
                string path = "MilJoke.txt";
                Console.WriteLine();
                Console.WriteLine($"Идёт чтение из файла: {path}");
                string[] joke = File.ReadAllLines(path).ToArray();
                if (joke.Length == 0)
                    Console.WriteLine($"Файл {path} пуст.");
                else
                {
                    Random rnd = new Random();
                    Joke = joke[rnd.Next(0, joke.Length - 1)];
                    Console.WriteLine($"Чтение из {path} прошло успешно. Считано {joke.Length} строк");
                }
            }
            catch (FileNotFoundException exc)
            {
                Console.WriteLine(exc.Message);
            }


            return Joke;
        }

        /// <summary>
        /// Считает музыку из БД
        /// </summary>
        /// <param name="path"></param>
        /// <param name="janr"></param>
        /// <returns></returns>
        protected static string[] GetMusic(string path, string janr)
        {
            string[] mres = new string[0];
            try
            {
                Console.WriteLine();
                Console.WriteLine($"Идёт чтение из файла: {path}");
                string[] mus = File.ReadAllLines(path).ToArray();
                if (mus.Length == 0)
                    Console.WriteLine($"Файл {path} пуст.");
                else
                {
                    Console.WriteLine($"Чтение из {path} прошло успешно. Считано {mus.Length} строк");
                    if (janr.Length > 0)
                    {
                        int counter = 0;
                        string[] res = new string[mus.Length];

                        int ind_ptr = 0;
                        while (ind_ptr < mus.Length && mus[ind_ptr].ToLower() != janr)
                        {
                            ind_ptr++;
                        }

                        if (ind_ptr >= mus.Length)
                        {
                            res = new string[1] {"Я не знаю такого жанра" };
                        }
                        else
                        {
                            for (int i = ind_ptr + 1; i < mus.Length; i++)
                            {
                                if (mus[i][0] == '*')
                                {
                                    break;
                                }

                                res[counter] = mus[i];
                                counter++;

                        }
                        Array.Resize(ref res, counter);
                        }

                        mres = res;
                    }
                    else
                    {
                        int counter = 0;
                        string[] res = new string[mus.Length];
                        for (int i = 0; i < mus.Length; i++)
                        {
                            if (mus[i][0] != '*')
                            {
                                res[counter] = mus[i];
                                counter++;
                            }

                        }
                        Array.Resize(ref res, counter);
                        mres = res;
                    }
                }
                
            }
            catch (FileNotFoundException exc)
            {
                Console.WriteLine(exc.Message);
            }

            return mres;
        }

        /// <summary>
        /// Вернёт рандомную музыку рандомного жанра
        /// </summary>
        public static string RandMusic()
        {
            string path = "Music.txt";
            string[] mus = GetMusic(path, "");
            Random rnd = new Random();

            return  mus[rnd.Next(0, mus.Length - 1)];


        }

        /// <summary>
        /// Вернёт рандомную музыку определённого жанра: Альтернатива, поп, рок, классическая
        /// </summary>
        /// <param name="janr"></param>
        public static string JanrRandMusic(string janr)
        {
            string path = "Music.txt";
            string j1 = "*";
            j1 += janr.ToLower();
            string[] mus = GetMusic(path, j1);
            Random rnd = new Random();

            return mus[rnd.Next(0, mus.Length - 1)];
        }
    }
}
