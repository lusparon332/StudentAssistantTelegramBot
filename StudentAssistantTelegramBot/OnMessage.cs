using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Telegram.Bot;
using System.IO;

namespace StudentAssistantTelegramBot
{
    public class OnMessage
    {
        public static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            Student stud;
            Program.students.ContainsStudByID(e.Message.Chat.Id, out stud);

            string message = "no_message";
            string answer = "no_answer";

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                message = e.Message.Text.ToLower();
            else
                return;

            Console.Write($"{DateTime.Now} | from: {e.Message.From.Username} | text: {message} | level before: {stud.users_loc}");

            var rkm = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup();
            rkm.Keyboard = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[][]
            {
                new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                {
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("📌 МЕНЮ 📌"),
                },
                new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                {
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("ℹ ИНФОРМАЦИЯ О БОТЕ ℹ"),
                }
            };

            /* =================================== РАБОТАЮТ ВЕЗДЕ =================================== */
            if (message == "/start")
            {
                answer = $"Привет, студент! ✌\nЯ бот-помощник в подготовке к сессии, моя задача - помочь тебе " +
                    $"правильно распределить время в подготовке к сессии, составив расписание, и " +
                    $"стимулировать начать готовиться к ней периодическими напоминаниями.\n\n" +
                    $"Сейчас ты в главном меню.\n" +
                    $"Отсюда ты можешь перейти:\n" +
                    $"• в раздел учёбы ✏\n" +
                    $"• в раздел развлечений ☕\n\n" +
                    $"Ну что, будем учиться или отдохнём?";
                stud.users_loc = LevelOfCode.MAIN_MENU;

                

            }
            else if (message.ToLower().Contains("меню"))
            {
                answer = $"Ты в главном меню. 📌\n" +
                    $"Отсюда ты можешь перейти:\n" +
                    $"• в раздел учёбы\n" +
                    $"• в раздел развлечений\n" +
                    $"• получить информацию о боте\n\n" +
                    $"Ну что, будем учиться или отдохнём?";

                stud.users_loc = LevelOfCode.MAIN_MENU;
            }
            if (message.ToLower().Contains("информация о боте"))
            {
                answer = $"Я бот-помощник в подготовке к сессии, моя главная задача - помочь тебе " +
                    $"правильно распределить время в подготовке к сессии, составив расписание, и " +
                    $"стимулировать начать готовиться к ней периодическими напоминаниями. Также я могу помочь" +
                    $" тебе расслабиться во время подготовки рекомендацией музыки или забавным анекдотом.";
            }
            /* =================================== MAIN_MENU =================================== */
            else if (message.ToLower().Contains("развлечения") && (stud.users_loc == LevelOfCode.MAIN_MENU || stud.users_loc == LevelOfCode.FAN_JANR))
            {
                answer = $"Ты в меню развлечений. ☕\n\n" +
                    $"Здесь я могу:\n" +
                    $"• рассказать тебе случайный анекдот 😂\n" +
                    $"• порекомендовать музыку 🎵\n" +
                    $"• порекомендовать музыку по жанру 🎵\n" +
                    $"• вернуться в главное меню 📌";
                stud.users_loc = LevelOfCode.FAN_MENU;

            }
            else if (message.ToLower().Contains("учёба") && stud.users_loc == LevelOfCode.MAIN_MENU)
            {
                answer = $"Ты в меню подготовки к сессии. ✏\n\n" +
                    $"Здесь я могу:\n" +
                    $"• составить тебе расписание для сессии ⏰\n" +
                    $"• дать совет по подготовке к сессии 📒\n" +
                    $"• дать информацию о прогрессе в подготовке, полученную на основе твоего следования расписанию 🔍\n" +
                    $"• удалить расписание 🚫\n" +
                    $"• вернуться в главное меню 📌";
                stud.users_loc = LevelOfCode.STUDY_MENU;
            }
            /* =================================== FAN_MENU =================================== */
            else if (message.ToLower().Contains("анекдот") && stud.users_loc == LevelOfCode.FAN_MENU)
            {
                answer = Secondary.RandMilJoke();
            }
            else if (message.ToLower().Contains("музыка по жанру") && stud.users_loc == LevelOfCode.FAN_MENU)
            {
                answer = $"Хорошо, назови жанр или вернись в меню развлечений. 🎵";
                stud.users_loc = LevelOfCode.FAN_JANR;

            }
            else if (message.ToLower().Contains("музыка") && stud.users_loc == LevelOfCode.FAN_MENU)
            {
                answer = Secondary.RandMusic();
            }
            else if (stud.users_loc == LevelOfCode.FAN_JANR)
            {
                string j = "";
                if (message.ToLower().Contains("классика"))
                    j = "классическая";
                else if (message.ToLower().Contains("рок"))
                    j = "рок";
                else if (message.ToLower().Contains("поп"))
                    j = "поп";
                else if (message.ToLower().Contains("альтернатива"))
                    j = "альтернатива";
                answer = Secondary.JanrRandMusic(j);
            }

            /* =================================== STUDY_MENU =================================== */
            else if (message.ToLower().Contains("совет") && stud.users_loc == LevelOfCode.STUDY_MENU)
            {
                answer = "Задавай свой вопрос, студент!";
                stud.users_loc = LevelOfCode.QUESTIONS_MENU;
            }
            /* =================================== QUESTIONS_MENU =================================== */
            else if (stud.users_loc == LevelOfCode.QUESTIONS_MENU)
            {          
                answer = "Держи список похожих вопросов с ответами. \nЕсли твоего вопроса здесь нет, нажми на соответсвующую кнопку.\nТогда вопрос будет отправлен" +
                    " нашим добровольцам, которые ответят на него, после чего вопрос с ответом пополнят базу. \n\nСписок:\n" + questionnaire.FindMatch(message);
                stud.users_loc = LevelOfCode.QUESTIONS_FIND;
                questionnaire.Question = message;
            }
            else if (stud.users_loc == LevelOfCode.QUESTIONS_FIND)
            {
                if (message.ToLower().Contains(" нет моего вопроса "))
                {
                    answer = "Спасибо, ваш вопрос записан и скоро будет обработан! Как только на него ответят, тебе придёт уведомление. \nВозвращаем тебя в меню УЧЁБЫ ✏.";
                    questionnaire.AddNewQuestion();
                    questionnaire.qt = new QuestionType(stud.student_id, questionnaire.Question, false);
                    questionnaire.SendQuestToModer();
                    stud.users_loc = LevelOfCode.STUDY_MENU;
                }
                if (message.ToLower().Contains(" я получил ответ "))
                {
                    answer = "Вот и хорошо. Возвращаем тебя в меню УЧЁБЫ ✏.";
                    stud.users_loc = LevelOfCode.STUDY_MENU;
                }
                /*else if (int.TryParse(message, out int num) && num > 0 && num <= questionnaire.qss.Length)
                {
                    var lines = File.ReadAllLines("BD_Voprosov_1.txt");
                    foreach (var line in lines)
                    {
                        var QA = line.Split("&&&", StringSplitOptions.RemoveEmptyEntries);
                        if (QA[0] == questionnaire.qss[num - 1])
                        {
                            answer = QA[1];
                            break;
                        }
                    }
                    stud.users_loc = LevelOfCode.STUDY_MENU;
                }
                else
                {
                    answer = "Некорректный ввод! Попробуй ввести номер вопроса ещё раз.";
                }*/
            }
            else if (stud.users_loc == LevelOfCode.TO_ANSWER)
            {
                if (questionnaire.qt.is_answered)
                {
                    answer = "На этот вопрос уже ответили.";
                    stud.users_loc = stud.prev_loc;
                }
                else
                {
                    if (message == "badq")
                        Program.Bot.SendTextMessageAsync(questionnaire.qt.from, "Ваш последний вопрос был помечен как \"спам\". Ответа на него вы не получите.");
                    else
                    {
                        questionnaire.qt.is_answered = true;
                        Program.Bot.SendTextMessageAsync(questionnaire.qt.from, $"На ваш вопрос ответили!\n\nВопрос: {questionnaire.Question}\nОтвет: {message}");
                        stud.users_loc = stud.prev_loc;
                        using (FileStream fs = new FileStream("BD_Voprosov_1.txt", FileMode.Append))
                        using (StreamWriter sw = new StreamWriter(fs))
                            sw.WriteLine($"Вопрос: {questionnaire.Question} | Ответ: {message}");
                    }
                }
            }
            else if (message.ToLower().Contains("удалить расписание") && stud.users_loc == LevelOfCode.STUDY_MENU)
            {
                foreach (var a in stud.Shedule.Keys)
                    stud.Shedule[a] = new DateTime[] { };
                answer = "Все расписания удалены. 🚫";
            }
            else if (message.ToLower().Contains("прогресс") && stud.users_loc == LevelOfCode.STUDY_MENU)
            {
                int dayUntil = (stud.current_exam.date.Date - DateTime.Now.Date).Days;
                if (stud.success < 0)
                    answer = $"Подготовка идёт не особо хорошо. Советую поднапрячься, дней до экзамена: {dayUntil}.";
                if (stud.success == 0)
                    answer = $"Ты пока не начал готовиться.";
                else
                    answer = $"Подготовка идёт стабильно. Дней до экзамена: {dayUntil}.";
            }

            /* ============================= СОСТАВЛЕНИЕ РАСПИСАНИЯ ============================= */
            else if (message.ToLower().Contains("расписание") && stud.users_loc == LevelOfCode.STUDY_MENU)
            {
                if (stud.Shedule.Count > 0)
                {
                    answer = "У тебя уже есть расписание. Если ты продолжишь создание нового, то предыдущее будет удалено. Если ты не хочешь этого, но нажми на кнопку отмены.";
                    stud.users_loc = LevelOfCode.MAKE_EXAM_NAME;
                }
                else
                {
                    foreach (var k in stud.Shedule.Keys)
                        stud.Shedule[k] = new DateTime[] { };
                    answer = "Отлично. Перед тем, как начать, введи название предмета, по которому будет ближайший экзамен.";
                    stud.users_loc = LevelOfCode.MAKE_EXAM_NAME;
                }
            }
            else if (stud.users_loc == LevelOfCode.MAKE_EXAM_NAME)
            {
                if (!stud.Shedule.ContainsKey(message))
                {
                    stud.current_exam.name = message;
                    answer = "Теперь введи время, в которое тебе будет удобно готовиться в формате ЧЧ:ММ.";
                    stud.users_loc = LevelOfCode.MAKE_EXAM_CNT;
                }
                else
                {
                    answer = "Такоt название экзамена ты уже вписывал. Попробуй ввести другое название.";
                }
            }
            else if (stud.users_loc == LevelOfCode.MAKE_EXAM_CNT)
            {
                (int, int) time = (0, 0);
                if (int.TryParse(message.Substring(0, 2), out int hour) && int.TryParse(message.Substring(3, 2), out int min) &&
                    hour < 24 && hour >= 0 && min < 60 && min >= 0 && message.Length == 5)
                {
                    answer = "Принято. Теперь введи дату экзамена в формате ДД.ММ.ГГГГ, например 12.06.2021";
                    time = (hour, min);
                    stud.current_exam.ex_time = time;
                    stud.users_loc = LevelOfCode.MAKE_EXAM_DATE;
                }
                else
                {
                    answer = "Некорректныый ввод времени. Попробуй ввести ещё раз.";
                }
            }
            else if (stud.users_loc == LevelOfCode.MAKE_EXAM_DATE)
            {
                int e_day = 1;
                int e_month = 1;
                int e_year = 2000;
                if (message.Length == 10 && int.TryParse(message.Substring(0, 2), out e_day) &&
                    int.TryParse(message.Substring(3, 2), out e_month) &&
                    int.TryParse(message.Substring(6, 4), out e_year) &&
                    e_month > 0 && e_month < 13 &&
                    e_year > 2020 && e_year < 2900 &&
                    IsRealDay(e_day, e_month, e_year))
                {
                    stud.current_exam.date = new DateTime(e_year, e_month, e_day, 8, 30, 0);
                    DateTime now = DateTime.Now;

                    int dayUntil = (stud.current_exam.date.Date - now.Date).Days;
                    if (dayUntil <= 1)
                        answer = "До экзамена слишком мало времени. Тут уж никакое расписание не поможет, так что прямо сейчас садить и учи всё подряд!";
                    else
                    {
                        DateTime[] dates = new DateTime[dayUntil];
                        for (int i = 0; i < dayUntil; i++)
                        {
                            DateTime date = new DateTime(now.Year, now.Month, now.Day, stud.current_exam.ex_time.Item1, stud.current_exam.ex_time.Item2, 0).AddDays(i).AddMilliseconds(stud.student_id % 10000);
                            dates[i] = date;
                        };
                        answer = "Твоё расписание готово. Каждый день в удобное для тебя время я буду напоминать о том, что пора " +
                            "начать подготовку. В случае чего ты всегда можешь её перенести на несколько минут или часов. Возвращаю тебя главное меню.";
                        stud.Shedule.Add(stud.current_exam.name, dates);
                    }
                    stud.users_loc = LevelOfCode.MAIN_MENU;
                }
                else
                {
                    answer = "Похоже, ты ввёл некорректную дату. Попробуй ввести ещё раз.";
                }
            }
            /* =============================== ПЕРЕНОС ПОДГОТОВКИ =============================== */
            else if (message.ToLower().Contains("да ") && stud.users_loc == LevelOfCode.PREPARE_TIME)
            {
                answer = "Отлично, удачи!";
                stud.success++;
                stud.users_loc = stud.prev_loc;
            }
            else if (message.ToLower().Contains("перенести на 15 минут") && stud.users_loc == LevelOfCode.PREPARE_TIME)
            {
                answer = "Хорошо, подготовка перенесена на 15 минут. Главное - долго не затягивай!";
                DateTime now = DateTime.Now;
                DateTime new_date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddMinutes(15);
                int dayUntil = (stud.current_exam.date.Date - now.Date).Days;
                int l = stud.Shedule[stud.current_exam.name].Length;
                stud.Shedule[stud.current_exam.name][l - dayUntil - 1] = new_date;
                stud.users_loc = stud.prev_loc;
            }
            else if (message.ToLower().Contains("перенести на 30 минут") && stud.users_loc == LevelOfCode.PREPARE_TIME)
            {
                answer = "Хорошо, подготовка перенесена на 30 минут. Главное - долго не затягивай!";
                DateTime now = DateTime.Now;
                DateTime new_date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddMinutes(30);
                int dayUntil = (stud.current_exam.date.Date - now.Date).Days;
                int l = stud.Shedule[stud.current_exam.name].Length;
                stud.Shedule[stud.current_exam.name][l - dayUntil - 1] = new_date;
                stud.users_loc = stud.prev_loc;
            }
            else if (message.ToLower().Contains("перенести на 1 час") && stud.users_loc == LevelOfCode.PREPARE_TIME)
            {
                answer = "Хорошо, подготовка перенесена на 1 час. Главное - долго не затягивай!";
                stud.success--;
                DateTime now = DateTime.Now;
                DateTime new_date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddHours(1);
                int dayUntil = (stud.current_exam.date.Date - now.Date).Days;
                int l = stud.Shedule[stud.current_exam.name].Length;
                stud.Shedule[stud.current_exam.name][l - dayUntil - 1] = new_date;
                stud.users_loc = stud.prev_loc;
            }
            else if (message.ToLower().Contains("перенести на 2 часа") && stud.users_loc == LevelOfCode.PREPARE_TIME)
            {
                answer = "Хорошо, подготовка перенесена на 2 часа. Главное - долго не затягивай!";
                stud.success--;
                DateTime now = DateTime.Now;
                DateTime new_date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddHours(2);
                int dayUntil = (stud.current_exam.date.Date - now.Date).Days;
                int l = stud.Shedule[stud.current_exam.name].Length;
                stud.Shedule[stud.current_exam.name][l - dayUntil - 1] = new_date;
                stud.users_loc = stud.prev_loc;
            }
            else if (message.ToLower().Contains("нет, сегодня я не буду готовиться") && stud.users_loc == LevelOfCode.PREPARE_TIME)
            {
                answer = "Захотелось в армейку? Ну ладно, не буду мешать.";
                stud.success--;
                stud.users_loc = stud.prev_loc;
            }

            /* ================================================================================== */

            Console.WriteLine($" | level after: {stud.users_loc}");

            if (answer == "no_answer")
                return;

            if (stud.users_loc == LevelOfCode.MAIN_MENU)
                rkm.Keyboard = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[][]
                {
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("✏ УЧЁБА ✏"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("☕ РАЗВЛЕЧЕНИЯ ☕"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("ℹ ИНФОРМАЦИЯ О БОТЕ ℹ"),
                    }
                };
            else if (stud.users_loc == LevelOfCode.STUDY_MENU)
                rkm.Keyboard = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[][]
                {
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("⏰ РАСПИСАНИЕ ⏰"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("📒 СОВЕТ 📒"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("🔍 ПРОГРЕСС 🔍"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("🚫 УДАЛИТЬ РАСПИСАНИЕ 🚫"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("📌 МЕНЮ 📌"),
                    }
                };
            else if (stud.users_loc == LevelOfCode.FAN_MENU)
                rkm.Keyboard = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[][]
                {
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("😂 АНЕКДОТ 😂"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("🎵 МУЗЫКА 🎵"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("🎵 МУЗЫКА ПО ЖАНРУ 🎵"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("📌 МЕНЮ 📌"),
                    }
                };
            else if (stud.users_loc == LevelOfCode.QUESTIONS_FIND)
                rkm.Keyboard = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[][]
                {
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("✏ Я ПОЛУЧИЛ ОТВЕТ ✏"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("❓ НЕТ МОЕГО ВОПРОСА ❓"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("📌 МЕНЮ 📌"),
                    }
                };
            else if (stud.users_loc == LevelOfCode.FAN_JANR)
                rkm.Keyboard = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[][]
                {
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("🎼 КЛАССИКА 🎼"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("🎸 РОК 🎸"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("🎶 ПОП 🎶"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("🎺 АЛЬТЕРНАТИВА 🎺"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("☕ РАЗВЛЕЧЕНИЯ ☕"),
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("📌 МЕНЮ 📌"),
                    }
                };
            else if (stud.users_loc == LevelOfCode.MAKE_EXAM_NAME || stud.users_loc == LevelOfCode.MAKE_EXAM_DATE || stud.users_loc == LevelOfCode.MAKE_EXAM_CNT)
                rkm.Keyboard = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[][]
                {
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("📌 ОТМЕНА (меню) 📌"),
                    }
                };


            Program.Bot.SendTextMessageAsync(stud.student_id, answer, replyMarkup: rkm); // отправка сообщения
        }

        public static bool IsRealDay(int day, int month, int year)
        {
            if (new int[] { 1, 3, 5, 7, 8, 10, 12 }.Contains(month))
                return (day > 0 && day < 32);
            else if (new int[] { 4, 6, 9, 11 }.Contains(month))
                return (day > 0 && day < 31);
            else if (IsLeapYear(year))
                return (day > 0 && day < 30);
            else
                return (day > 0 && day < 29);
        }

        public static bool IsLeapYear(int year)
        {
            if (year % 4 != 0)
                return false;
            else if (year % 100 == 0)
                if (year % 400 == 0)
                    return true;
                else
                    return false;
            else
                return true;
        }
    }
}

/*
 rkm.Keyboard = new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[][]
                {
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("Кнопка1"),
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("Кнопка2")
                    },
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                    {
                        new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("Кнопка3")
                    }
                };
 */