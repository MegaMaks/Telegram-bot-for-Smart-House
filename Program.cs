using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO.Ports;
using System.Net.Sockets;
using System.Net;

namespace ConsoleTelegram
{
    class Program
    {

        
        private static readonly TelegramBotClient Bot = new TelegramBotClient("550808830:AAERRZ0qIsXIdgMTgksg2tcA0DQeWqL3r5g");

        static byte[] lightStatus = new byte[2] { 0x02, 0x00 };
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
        static IPAddress  Ipadress {get;}=IPAddress.Parse("192.168.88.10");
        static int Port { get; } = 80;

        static void Main(string[] args)
        {

            //lamps[0].Statuslamp = 0;
            //lamps[0].Iconlamp = "⚪️";
            
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
            Console.ReadLine();
            Bot.StopReceiving();
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
            
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.Text) return;

            switch (message.Text)
            {
                case "/inline":
                    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                    await Task.Delay(500); // simulate longer running task

                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new [] // first row
                        {
                            InlineKeyboardButton.WithCallbackData("1.1"),
                            InlineKeyboardButton.WithCallbackData("1.2"),
                        },
                        new [] // second row
                        {
                            InlineKeyboardButton.WithCallbackData("2.1"),
                            InlineKeyboardButton.WithCallbackData("2.2"),
                        }
                    });

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose",
                        replyMarkup: inlineKeyboard);
                    break;

                // send custom keyboard
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
                            lamps[i].Statuslamp = 1;
                            lamps[i].Iconlamp ="🎾";
                        }
                        else
                        {
                            lamps[i].Statuslamp = 0;
                            lamps[i].Iconlamp = "⚪️";
                        }
                    }

                  var inlineLight = new InlineKeyboardMarkup(new[]
                  {
                        new []
                        {
                            
                            InlineKeyboardButton.WithCallbackData($"{lamps[0].Iconlamp} Гостинная","living"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[1].Iconlamp} Спальня","sleeping"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lamps[2].Iconlamp} Кухня","kitchen"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[3].Iconlamp} Детская","child"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lamps[4].Iconlamp} Коридор","hall"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[5].Iconlamp} Ванная","bath"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lamps[6].Iconlamp} Подвал","ground"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[7].Iconlamp} Уличный","street"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"💡 Режимы освещения","lightmode"),
                        },

                    });


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
                    const string usage = @"
Usage:
/start - send custom keyboard";


                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        usage,
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
            }
        }


        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;


            if (callbackQuery == null || callbackQuery.Message.Type != MessageType.Text) return;
            try
            {
                //was message.Text.Split(' ').First()
                switch (callbackQuery.Data)
                {
                    //Освещение
                    case "living":
                        await LightSelect(0, callbackQuery);
                        break;


                    case "sleeping":
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
                    case "ground":
                        await LightSelect(6, callbackQuery);
                        break;

                    case "street":
                        await LightSelect(7, callbackQuery);
                        break;

                }
            }
            catch
            {
                
            }
        }


        private static async Task DimmerLamp(int idlamp, Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            string sqlstr = ConfigurationManager.ConnectionStrings["cnStrJarvis"].ConnectionString;
            DataTable t = new DataTable();
            JarvisDAL JDAL = new JarvisDAL();

            JDAL.OpenConnection();
            t = JDAL.Sellamp(idlamp);
            int lampdimmer = (int)t.Rows[0]["dimmer"];
            JDAL.CloseConnection();

            string DimmerSetting = $@"
Текущая яркость {lampdimmer} %";

            var inlineDimmerSetting = new InlineKeyboardMarkup(new[]
           {
                        new [] // first row
                        {
                            InlineKeyboardButton.WithCallbackData("⬇️","lightdown"),
                            InlineKeyboardButton.WithCallbackData("⬆️","lightup"),
                        },
                        new [] // first row
                        {
                            InlineKeyboardButton.WithCallbackData("☀️ Максимальная яркость","lightfull"),
                        },

                    });

            await Bot.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                DimmerSetting,
                replyMarkup: inlineDimmerSetting);
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
                if (lamps[idlamp].Statuslamp == 0)
                {
                        lamps[idlamp].Statuslamp = 1;
                        lamps[idlamp].Iconlamp = "🎾";
                }
                else
                {
                    lamps[idlamp].Statuslamp = 0;
                    lamps[idlamp].Iconlamp = "⚪️";
                }
            }
            string changesleepinglamp = $@"Управление освещением";


            var inlineLight = new InlineKeyboardMarkup(new[]
            {
                                                new []
                        {

                            InlineKeyboardButton.WithCallbackData($"{lamps[0].Iconlamp} Гостинная","living"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[1].Iconlamp} Спальня","sleeping"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lamps[2].Iconlamp} Кухня","kitchen"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[3].Iconlamp} Детская","child"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lamps[4].Iconlamp} Коридор","hall"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[5].Iconlamp} Ванная","bath"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lamps[6].Iconlamp} Подвал","ground"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[7].Iconlamp} Уличный","street"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"💡 Режимы освещения","lightmode"),
                        },

                    });

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
    }
}

