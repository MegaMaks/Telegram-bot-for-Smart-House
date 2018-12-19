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
using APIXULib;
using Timer = System.Timers.Timer;

namespace ConsoleTelegram
{
    class Program
    {


        private static readonly TelegramBotClient Bot = new TelegramBotClient("1111111111111111111111111111111111");

        private static string keyWeather = "1111111111111111111111111111";
        static IRepository repo = new Repository();
        static string country = "11111111";

        static byte[] lightStatus = new byte[2] { 0x02, 0x00 };
        static Timer timer;


        static LightinNight lightintight = new LightinNight();
        static PresenceEff presenceeffect = new PresenceEff();

        static List<int> accesslist = new List<int>() { TelegramUsersID };

        static List<Climate> climates = new List<Climate>
        {
            new Climate { NumSensor=0},
            new Climate { NumSensor=1},
        };


        static IPAddress  Ipadress {get;}=IPAddress.Parse("IP Adress");

        

        static string PortCOM { get; } = "COM3";
        static int Port { get; } = 80;

        

        static void Main(string[] args)
        {
            //Comment the line below afte DB init and filling
            //Model.SmartHouseDbFill.DbInit();
            while (true)
            {
                try
                {
                    var me = Bot.GetMeAsync().Result;
                    Console.Title = me.Username;

                    Bot.OnMessage += BotOnMessageReceived;
                    Bot.OnMessageEdited += BotOnMessageReceived;
                    Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
                    //Bot.OnInlineQuery += BotOnInlineQueryReceived;
                    Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
                    Bot.OnReceiveError += BotOnReceiveError;

                    Bot.StartReceiving();
                    Console.WriteLine($"Start SmartHome for @{me.Username}");

                    timer = new Timer();
                    timer.AutoReset = true;
                    timer.Interval = 60000;
                    timer.Elapsed += new ElapsedEventHandler(LightCheck);
                    timer.Enabled = true;

                    Console.ReadLine();
                    Bot.StopReceiving();
                }
                catch (Exception e)
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
                            Model.SmartHouseDbFill.lamps[i].Status = 1;
                        }
                        else
                        {
                            Model.SmartHouseDbFill.lamps[i].Status = 0;
                        }
                    }

                    var inlineLight =Views.ViewLightBot.KeyLightInit(Model.SmartHouseDbFill.lamps);
                    Views.ViewLightBot.SendMessageToChat(Bot, InfoMesage.Lighcontol, message, inlineLight);
                    break;


                case "⛈ Климат":

                    foreach (var item in climates)
                    {
                        string timeandhum = InterfaceMK.ViaCOM.SendtoCOM(PortCOM, item.NumSensor);
                        item.SetClimate(timeandhum);

                    }
                    var GetCurrentWeather= repo.GetWeatherData(keyWeather, GetBy.CityName, country);

                    InfoMesage.SetInfoClimate(climates,GetCurrentWeather.current.temp_c, GetCurrentWeather.current.humidity,Convert.ToInt32(GetCurrentWeather.current.wind_kph/3.6));
                    
                    Views.ViewLightBot.SendMessageToChat(Bot, InfoMesage.StatusClimate, message, null);
                    break;

                default:
                    Views.ViewLightBot.SendMessageToChat(Bot, InfoMesage.Starttext, message, null);
                    break;

                case "🛡 Безопасность":
                    Views.ViewLightBot.SendPhotoToChat(Bot, message);
                    break;
            }
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;
            bool accessuser = false;

            foreach (var item in accesslist)
            {
                if (callbackQuery.From.Id == item) accessuser = true;
            }
            
            if (callbackQuery == null || callbackQuery.Message.Type != MessageType.Text|| accessuser == false) return;
            try
            {
                var lampcmd = Model.SmartHouseDbFill.lamps.Find(x => x.Command.Contains(callbackQuery.Data));

                if (lampcmd != null)
                {
                    await LightSelect(lampcmd.Number, callbackQuery);
                }
                else
                {
                    switch (callbackQuery.Data)
                    {
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

            }
            catch
            {
                
            }
        }

        private static async Task EditAutoShut(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {

            var inlineLightSetting = Views.ViewLightBot.KeyboardLightModeAutoOff(Model.SmartHouseDbFill.lamps, Model.SmartHouseDbFill.autooff);
            Views.ViewLightBot.SendMessageToChat(Bot, InfoMesage.Lightmode, callbackQuery.Message, inlineLightSetting);

            //if (autooff.Status == false)
            //    {
            //    autooff.Status = true;
            //    timer.Elapsed +=new ElapsedEventHandler(LightAutoOff);
            //    }
            //else autooff.Status = false;
            //var inlineLightSetting = Views.ViewLightBot.KeyLightMode(presenceeffect,autooff,lightintight);
            //await Views.ViewLightBot.EditMessageToChat(Bot, InfoMesage.Lightmode, callbackQuery.Message, inlineLightSetting);
        }

        private static async Task EditLightinNight(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            if (lightintight.Status == false) lightintight.Status = true;
            else lightintight.Status = false;

            //var inlineLightSetting = Views.ViewLightBot.KeyLightMode(presenceeffect, autooff, lightintight);
            //await Views.ViewLightBot.EditMessageToChat(Bot, InfoMesage.Lightmode, callbackQuery.Message, inlineLightSetting);
        }
        private static void LightSettings(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            var inlineLightSetting = Views.ViewLightBot.KeyLightMode(presenceeffect, lightintight);
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
                if (Model.SmartHouseDbFill.lamps[idlamp].Status == 0)
                {
                    Model.SmartHouseDbFill.lamps[idlamp].Status = 1;
                }
                else
                {
                    Model.SmartHouseDbFill.lamps[idlamp].Status = 0;
                }
            }
            var inlineLight = Views.ViewLightBot.KeyLightInit(Model.SmartHouseDbFill.lamps);
            await Views.ViewLightBot.EditMessageToChat(Bot, InfoMesage.Lighcontol, callbackQuery.Message, inlineLight);
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
            
            //if ((DateTime.Now.TimeOfDay > autooff.TimeBegin)&&(DateTime.Now.TimeOfDay < autooff.TimeBegin.Add(new TimeSpan(0,0,1,0,0))))
            //{
                //string msgautooff;
                //byte[] lightChange = new byte[2] { 0x01, 0x00 };
                //TcpClient newClient = new TcpClient();
                //newClient.Connect(Ipadress, Port);
                //byte bt = InterfaceMK.ViaTCP.SendCMD(newClient, lightStatus);

                //if (((bt >> 7) & 1) != 0)
                //{
                //    bt ^= (byte)(1 << 7);
                //    lightChange[1] = bt;
                //    byte btget = InterfaceMK.ViaTCP.SendCMD(newClient, lightChange);
                //    if (bt == btget)
                //    {
                //        lamps[7].Status = 0;
                //        msgautooff = InfoMesage.AutoOffSuccess;
                //    }
                //    else
                //    {
                //        msgautooff = InfoMesage.AutoOffFail;
                //    }
                //    Views.ViewLightBot.SendOnlyMessageToChat(Bot,msgautooff);
                //}                 
                //newClient.Close();                
            //}
        }
    }
}

