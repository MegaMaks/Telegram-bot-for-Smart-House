using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ConsoleTelegram
{
    class Program
    {
        const int living = 0;
        const int study = 1;
        const int kitchen = 2;
        const int child = 3;
        const int hall = 4;
        const int bath = 5;
        const int street = 7;

        private static readonly TelegramBotClient Bot = new TelegramBotClient("550808830:AAERRZ0qIsXIdgMTgksg2tcA0DQeWqL3r5g");

        static byte[] lightStatus = new byte[2] { 0x02, 0x00 };
        static Timer timer;

        static List<Lamp> lamps = new List<Lamp>
        {
            new Lamp(),
            new Lamp(),
            new Lamp(),
            new Lamp(),
            new Lamp(),
            new Lamp(),
            new Lamp(),
            new Lamp(),
        };
        static LightinNight lightintight = new LightinNight();
        static PresenceEff presenceeffect = new PresenceEff();
        static AutoOff autooff = new AutoOff();

        static List<int> accesslist = new List<int>() { 352840946 , 129973487 };


        static IPAddress  Ipadress {get;}=IPAddress.Parse("192.168.88.10");
        static int Port { get; } = 80;

        

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    var me = Bot.GetMeAsync().Result;
                    Console.Title = me.Username;

                    Bot.OnMessage += BotOnMessageReceived;
                    Bot.OnMessageEdited += BotOnMessageReceived;
                    Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
                    Bot.OnInlineQuery += BotOnInlineQueryReceived;
                    Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
                    Bot.OnReceiveError += BotOnReceiveError;

                    Bot.StartReceiving();
                    Console.WriteLine($"Start listening for @{me.Username}");

                    timer = new Timer();
                    timer.AutoReset = true;
                    timer.Interval = 60000;
                    timer.Elapsed += new ElapsedEventHandler(LightCheck);
                    timer.Enabled = true;                   
                
                    Console.ReadLine();
                    Bot.StopReceiving();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Thread.Sleep(10000);
                    
                }
            }
        }

        private static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            bool accessuser = false;
            var message = messageEventArgs.Message;

            foreach (var item in accesslist)
            {
                if (message.From.Id == item) accessuser = true;
            }

            if (message == null || message.Type != MessageType.Text||accessuser==false) return;

            switch (message.Text)
            {
                case "/start":

                    var ReplyKeyboard = Views.ViewLightBot.MainMenu();
                    ReplyKeyboard.ResizeKeyboard = true;
                    Views.ViewLightBot.SendKeyboartToChat(Bot, InfoMesage.FirstMsg, message, ReplyKeyboard);
                    break;

                case "🌓 Освещение":
                    TcpClient newClient = new TcpClient();
                    newClient.Connect(Ipadress, Port);
                    byte bt =InterfaceMK.ViaTCP.SendCMD(newClient,lightStatus);
                    newClient.Close();

                    for (int i = 0; i < 8; i++)
                    {
                        if (((bt >> i) & 1) != 0)
                        {
                            lamps[i].Status = 1;
                        }
                        else
                        {
                            lamps[i].Status = 0;
                        }
                    }

                    var inlineLight =Views.ViewLightBot.KeyLightInit(lamps);
                    Views.ViewLightBot.SendMessageToChat(Bot, InfoMesage.Lighcontol, message, inlineLight);
                    break;

                    default:
                    Views.ViewLightBot.SendMessageToChat(Bot, InfoMesage.Starttext, message, null);
                    break;
            }
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            if (callbackQuery == null || callbackQuery.Message.Type != MessageType.Text) return;
            try
            {
                switch (callbackQuery.Data)
                {
                    case "living":
                        await LightSelect(living, callbackQuery);
                        break;

                    case "study":
                        await LightSelect(study, callbackQuery);
                        break;

                    case "kitchen":
                        await LightSelect(kitchen, callbackQuery);
                        break;

                    case "child":
                        await LightSelect(child, callbackQuery);
                        break;

                    case "hall":
                        await LightSelect(hall, callbackQuery);
                        break;

                    case "bath":
                        await LightSelect(bath, callbackQuery);
                        break;

                    case "street":
                        await LightSelect(street, callbackQuery);
                        break;

                    case "lightmode":
                        LightSettings(callbackQuery);
                       break;
                    case "autoshut":
                        await EditAutoShut(callbackQuery);
                        break;
                    case "lightinnihgt":
                        await EditLightinNight(callbackQuery);
                        break;
                }
            }
            catch
            {
                
            }
        }

        private static async Task EditAutoShut(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            if (autooff.Status == 0)
                {
                autooff.Status = 1;
                timer.Elapsed +=new ElapsedEventHandler(LightAutoOff);
                }
            else autooff.Status = 0;
            var inlineLightSetting = Views.ViewLightBot.KeyLightMode(presenceeffect,autooff,lightintight);
            await Views.ViewLightBot.EditMessageToChat(Bot, InfoMesage.Lightmode, callbackQuery.Message, inlineLightSetting);
        }

        private static async Task EditLightinNight(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            if (lightintight.Status == 0) lightintight.Status = 1;
            else lightintight.Status = 0;

            var inlineLightSetting = Views.ViewLightBot.KeyLightMode(presenceeffect, autooff, lightintight);
            await Views.ViewLightBot.EditMessageToChat(Bot, InfoMesage.Lightmode, callbackQuery.Message, inlineLightSetting);
        }
        private static void LightSettings(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            var inlineLightSetting = Views.ViewLightBot.KeyLightMode(presenceeffect, autooff, lightintight);
            Views.ViewLightBot.SendMessageToChat(Bot, InfoMesage.Lightmode, callbackQuery.Message, inlineLightSetting);
        }

        private static async Task LightSelect(int idlamp, Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            TcpClient newClient = new TcpClient();
            byte[] lightChange = new byte[2] { 0x01, 0x00 };

            newClient.Connect(Ipadress, Port);
            byte bt = InterfaceMK.ViaTCP.SendCMD(newClient,lightStatus);
            bt ^= (byte)(1 << idlamp);
            lightChange[1] = bt;
            byte btget = InterfaceMK.ViaTCP.SendCMD(newClient,lightChange);
            newClient.Close();
            if (bt == btget)
            {
                if (lamps[idlamp].Status == 0)
                {
                        lamps[idlamp].Status = 1;
                }
                else
                {
                    lamps[idlamp].Status = 0;
                }
            }
            var inlineLight = Views.ViewLightBot.KeyLightInit(lamps);
            await Views.ViewLightBot.EditMessageToChat(Bot, InfoMesage.Lighcontol, callbackQuery.Message, inlineLight);
        }

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                new InlineQueryResultLocation(
                    id: "1",
                    latitude: 40.7058316f,
                    longitude: -74.2581888f,
                    title: "New York")   // displayed result
                    {
                        InputMessageContent = new InputLocationMessageContent(
                            latitude: 40.7058316f,
                            longitude: -74.2581888f)    // message if result is selected
                    },

                new InlineQueryResultLocation(
                    id: "2",
                    latitude: 13.1449577f,
                    longitude: 52.507629f,
                    title: "Berlin") // displayed result
                    {

                        InputMessageContent = new InputLocationMessageContent(
                            latitude: 13.1449577f,
                            longitude: 52.507629f)   // message if result is selected
                    }
            };

            await Bot.AnswerInlineQueryAsync(
                inlineQueryEventArgs.InlineQuery.Id,
                results,
                isPersonal: true,
                cacheTime: 0);
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }

        private static void LightCheck(object sender, ElapsedEventArgs eventArgs)
        {
            Console.WriteLine("exxxyyy");
        }

        private static void LightAutoOff(object sender, ElapsedEventArgs eventArgs)
        {
            
            if ((DateTime.Now.TimeOfDay > autooff.TimeBegin)&&(DateTime.Now.TimeOfDay < autooff.TimeBegin.Add(new TimeSpan(0,0,1,0,0))))
            {
                string msgautooff;
                byte[] lightChange = new byte[2] { 0x01, 0x00 };
                TcpClient newClient = new TcpClient();
                newClient.Connect(Ipadress, Port);
                byte bt = InterfaceMK.ViaTCP.SendCMD(newClient, lightStatus);

                if (((bt >> 7) & 1) != 0)
                {
                    bt ^= (byte)(1 << 7);
                    lightChange[1] = bt;
                    byte btget = InterfaceMK.ViaTCP.SendCMD(newClient, lightChange);
                    if (bt == btget)
                    {
                        lamps[7].Status = 0;
                        msgautooff = InfoMesage.AutoOffSuccess;
                    }
                    else
                    {
                        msgautooff = InfoMesage.AutoOffFail;
                    }
                    Views.ViewLightBot.SendOnlyMessageToChat(Bot,msgautooff);
                }                 
                newClient.Close();                
            }
        }
    }
}

