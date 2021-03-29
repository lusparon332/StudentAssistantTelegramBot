﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Telegram.Bot;

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

            /* =================================== РАБОТАЮТ ВЕЗДЕ =================================== */
            if (message == "/start")
            {
                answer = $"Привет, студент! Я бот-помощник в подготовке к сессии, моя задача - помочь тебе " +
                    $"правильно распределить время в подготовке к сессии, составив расписание, и " +
                    $"стимулировать начать готовиться к ней периодическими напоминаниями.\n\n" +
                    $"Сейчас ты в главном меню.\n" +
                    $"Отсюда ты можешь перейти:\n" +
                    $"• в раздел учёбы командой /study\n" +
                    $"• в раздел развлечений командой /fan\n\n" +
                    $"Ну что, будем учиться или отдохнём?";
                stud.users_loc = LevelOfCode.MAIN_MENU;
            }
            else if (message == "/menu")
            {
                answer = $"Ты в главном меню.\n" +
                    $"Отсюда ты можешь перейти:\n" +
                    $"• в раздел учёбы командой /study\n" +
                    $"• в раздел развлечений командой /fan\n\n" +
                    $"Ну что, будем учиться или отдохнём?";
                stud.users_loc = LevelOfCode.MAIN_MENU;
            }
            /* =================================== MAIN_MENU =================================== */
            else if (message == "/fan" && stud.users_loc == LevelOfCode.MAIN_MENU)
            {
                answer = $"Ты в меню развлечений.\n" +
                    $"Здесь я могу:\n" +
                    $"• рассказать тебе случайный анекдот (команда /joke)\n" +
                    $"• порекомендовать музыку (команда /music)\n\n" +
                    $"Вернуться в главное меню - команда /menu";
                stud.users_loc = LevelOfCode.FAN_MENU;
            }
            else if (message == "/study" && stud.users_loc == LevelOfCode.MAIN_MENU)
            {
                answer = $"Ты в меню подготовки к сессии. ✏\n" +
                    $"Здесь я могу:\n" +
                    $"• составить тебе расписание для сессии (команда /makeschedule)\n" +
                    $"• дать совет по подготовке к сессии (команда /advice)\n\n" +
                    $"Вернуться в главное меню - команда /menu";
                stud.users_loc = LevelOfCode.STUDY_MENU;
            }
            /* =================================== FAN_MENU =================================== */
            else if (message == "/joke" && stud.users_loc == LevelOfCode.FAN_MENU)
            {
                answer = "шутка";
            }
            else if (message == "/music" && stud.users_loc == LevelOfCode.FAN_MENU)
            {
                answer = "музыка";
            }
            /* =================================== STUDY_MENU =================================== */
            else if (message == "/advice" && stud.users_loc == LevelOfCode.STUDY_MENU)
            {
                answer = "советов нет, но вы держитесь";

            }
            /* ============================= СОСТАВЛЕНИЕ РАСПИСАНИЯ ============================= */
            else if (message == "/makeschedule" && stud.users_loc == LevelOfCode.STUDY_MENU)
            {
                answer = "Отлично. Перед тем, как начать, введи название предмета, по которому будет ближайший экзамен.";
                stud.users_loc = LevelOfCode.MAKE_EXAM_NAME;
            }
            else if (stud.users_loc == LevelOfCode.MAKE_EXAM_NAME)
            {
                stud.current_exam.name = message;
                answer = "Теперь введи количество вопросов в экзамене.";
                stud.users_loc = LevelOfCode.MAKE_EXAM_CNT;
            }
            else if (stud.users_loc == LevelOfCode.MAKE_EXAM_CNT)
            {
                int cnt_of_q = 1;
                if (int.TryParse(message, out cnt_of_q))
                {
                    answer = "Принято. Теперь введи дату экзамена в формате ДД.ММ.ГГГГ, например 12.06.2021";
                    stud.current_exam.question_cnt = cnt_of_q;
                    stud.users_loc = LevelOfCode.MAKE_EXAM_DATE;
                }
                else
                {
                    answer = "Количество вопросов - число. Попробуй ввести ещё раз.";
                }
            }
            else if (stud.users_loc == LevelOfCode.MAKE_EXAM_DATE)
            {
                int e_day = 1;
                int e_month = 1;
                int e_year = 2000;
                if (int.TryParse(message.Substring(0, 2), out e_day) && 
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
                            DateTime date = new DateTime(now.Year, now.Month, now.Day, 13, 01, 7).AddDays(i + 1);
                            dates[i] = date;
                        };
                        answer = " Твоё расписание готово. Каждый день в обед я буду напоминать тебе о том, что пора " +
                            "начать подготовку. В случае чего ты всегда можешь её перенести на несколько минут или часов.";
                        stud.Shedule.Add(stud.current_exam.name, dates);
                    }
                    stud.users_loc = LevelOfCode.MAIN_MENU;
                }
                else
                {
                    answer = "Похоже, ты ввёл некорректную дату. Попробуй ввести ещё раз.";
                }
            }
            
            
            /* ================================================================================== */

                Console.WriteLine($" | level after: {stud.users_loc}");

            if (answer == "no_answer")
                return;

            Program.Bot.SendTextMessageAsync(stud.student_id, answer); // отправка сообщения
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