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


        static List<int> accesslist = new List<int>() { 352840946 , 129973487 };


        static IPAddress  Ipadress {get;}=IPAddress.Parse("192.168.88.10");
        static int Port { get; } = 80;

        private static readonly ManualResetEvent _ResetEvent =   new ManualResetEvent(false);

        private static bool statusAutoOff = false;
        private static bool statusLightinNight = false;
        private static bool statusPrecence = false;

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
                    timer.Interval = 30000;
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



        private static byte SendCMD(TcpClient newClient,byte[] sendBytes)
        {
            NetworkStream tcpStream = newClient.GetStream();
            tcpStream.Write(sendBytes, 0, sendBytes.Length);

            byte[] bytes = new byte[newClient.ReceiveBufferSize];
            int bytesRead = tcpStream.Read(bytes, 0, newClient.ReceiveBufferSize);
            return bytes[1];
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
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
                    ReplyKeyboardMarkup ReplyKeyboard = new[]
                    {
                        new[] { "🌓 Освещение", "⛈ Климат" },
                        new[] { "🛡 Безопасность", "🏠 Состояние дома" },

                    };
                    ReplyKeyboard.ResizeKeyboard = true;

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Выберите",
                        replyMarkup: ReplyKeyboard);
                    break;


                case "🌓 Освещение":
                    string LightHome = $@"Управление освещением";
                    TcpClient newClient = new TcpClient();
                    newClient.Connect(Ipadress, Port);
                    byte bt =SendCMD(newClient,lightStatus);
                    newClient.Close();


                    for (int i = 0; i < 8; i++)
                    {
                        if (((bt >> i) & 1) != 0)
                        {
                            lamps[i].Status = 1;
                            //lamps[i].Iconlamp ="🎾";
                        }
                        else
                        {
                            lamps[i].Status = 0;
                            //lamps[i].Iconlamp = "⚪️";
                        }
                    }


                    var inlineLight = KeyLightInit();

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        LightHome,
                        replyMarkup: inlineLight);
                    break;


                // send a photo
                case "/photo":
                    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                    const string file = @"123.jpg";

                    var fileName = file.Split(Path.DirectorySeparatorChar).Last();

                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await Bot.SendPhotoAsync(
                            message.Chat.Id,
                            fileStream,
                            "Nice Picture");
                    }
                    break;

                // request location or contact
                case "/request":
                    var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        KeyboardButton.WithRequestLocation("Location"),
                        KeyboardButton.WithRequestContact("Contact"),
                    });

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Who or Where are you?",
                        replyMarkup: RequestReplyKeyboard);
                    break;

                default:
                    const string usage = @"Usage:/start";


                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        usage,
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
            }
        }

        private static InlineKeyboardMarkup KeyLightInit()
        {
            var inlineLight = new InlineKeyboardMarkup(new[]
{
                        new []
                        {

                            InlineKeyboardButton.WithCallbackData($"{lamps[0].IconCurrent} Гостинная","living"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[1].IconCurrent} Кабинет","study"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lamps[2].IconCurrent} Кухня","kitchen"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[3].IconCurrent} Детская","child"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lamps[4].IconCurrent} Коридор","hall"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[5].IconCurrent} Ванная","bath"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lamps[6].IconCurrent} Не работает","nobody"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[7].IconCurrent} Уличный","street"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"💡 Режимы освещения","lightmode"),
                        },

                    });
            return inlineLight;
        }


        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;


            if (callbackQuery == null || callbackQuery.Message.Type != MessageType.Text) return;
            try
            {
                switch (callbackQuery.Data)
                {
                    //Освещение
                    case "living":
                        await LightSelect(0, callbackQuery);
                        break;


                    case "study":
                        await LightSelect(1, callbackQuery);
                        break;

                    case "kitchen":
                        await LightSelect(2, callbackQuery);
                        break;

                    case "child":
                        await LightSelect(3, callbackQuery);
                        break;

                    case "hall":
                        await LightSelect(4, callbackQuery);
                        break;

                    case "bath":
                        await LightSelect(5, callbackQuery);
                        break;

                    case "street":
                        await LightSelect(7, callbackQuery);
                        break;

                    case "lightmode":
                        await LightSettings(callbackQuery);
                       break;

                }
            }
            catch
            {
                
            }
        }
        private static async Task LightSettings(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            string ClimateSetting = $@"Режимы освещения";

            var inlineLightSetting = new InlineKeyboardMarkup(new[]
           {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("Эффект присутствия","presence"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("Автовыключение света","autoshut"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("Свет в ночи","lightinnihgt"),
                        },

                    });

            await Bot.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                ClimateSetting,
                replyMarkup: inlineLightSetting);
        }

        private static async Task LightSelect(int idlamp, Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            TcpClient newClient = new TcpClient();
            byte[] lightChange = new byte[2] { 0x01, 0x00 };

            newClient.Connect(Ipadress, Port);
            byte bt = SendCMD(newClient,lightStatus);
            
            bt ^= (byte)(1 << idlamp);

            lightChange[1] = bt;
            byte btget = SendCMD(newClient,lightChange);
            newClient.Close();
            if (bt == btget)
            {
                if (lamps[idlamp].Status == 0)
                {
                        lamps[idlamp].Status = 1;
                        //lamps[idlamp].Iconlamp = "🎾";
                }
                else
                {
                    lamps[idlamp].Status = 0;
                    //lamps[idlamp].Iconlamp = "⚪️";
                }
            }
            string changesleepinglamp = $@"Управление освещением";


            var inlineLight = KeyLightInit();

            await Bot.EditMessageTextAsync(callbackQuery.Message.Chat.Id,
                callbackQuery.Message.MessageId,
                changesleepinglamp,
                replyMarkup: inlineLight);
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
            //TcpClient newClient = new TcpClient();
            //newClient.Connect(Ipadress, Port);
            //byte bt = SendCMD(newClient, lightStatus);
            //newClient.Close();
            string statusHome = $@"

🌡 Система отопления:
Давление в системе: 1.9 атм.
Температура теплоносителя:  °C

🚰 Система водоснабжения:
Давление в системе: 2.3 атм.
Температура горячей воды  °C

🚽 Септик заполнен на 45%

🛎 Тревожные сообщения: нет";
            Console.WriteLine("exxxyyy");
            SendMessageToChat(statusHome);
        }

        private static async void SendMessageToChat(string msg)
        {
            await Bot.SendTextMessageAsync(129973487, msg);
            

        }
    }
}

