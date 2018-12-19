using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using System.IO;
using System.Net;

namespace ConsoleTelegram.Views
{
    class ViewLightBot
    {
        public static ReplyKeyboardMarkup MainMenu()
        {
            ReplyKeyboardMarkup ReplyKeyboard = new[]
{
                        new[] { "🌓 Освещение", "⛈ Климат" },
                        new[] { "🛡 Безопасность", "🏠 Состояние дома" },

                    };
            return ReplyKeyboard;
        }

        public static InlineKeyboardMarkup KeyLightMode(PresenceEff presenceeffect, LightinNight lightintight)
        {
            var inlineLight = new InlineKeyboardMarkup(new[]
{
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{presenceeffect.IconCurrent} Эффект присутствия","presence"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"        Автовыкл. света","autoshut"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lightintight.IconCurrent}                  Свет в ночи","lightinnihgt"),
                        },

                    });
            return inlineLight;
        }

        public static InlineKeyboardMarkup KeyLightInit(List<Lamp> lamp)
        {
            var inlineLight = new InlineKeyboardMarkup(new[]
            {
                        lamp.Where(item=>item.NumberLineKeyboard==1).Select(item=>InlineKeyboardButton.WithCallbackData(item.IconCurrent+item.Name,item.Command)),
                        lamp.Where(item=>item.NumberLineKeyboard==2).Select(item=>InlineKeyboardButton.WithCallbackData(item.IconCurrent+item.Name,item.Command)),
                        lamp.Where(item=>item.NumberLineKeyboard==3).Select(item=>InlineKeyboardButton.WithCallbackData(item.IconCurrent+item.Name,item.Command)),
                        lamp.Where(item=>item.NumberLineKeyboard==4).Select(item=>InlineKeyboardButton.WithCallbackData(item.IconCurrent+item.Name,item.Command)),

                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"💡 Режимы освещения","lightmode"),
                        },

            });
            return inlineLight;

        }

        public static InlineKeyboardMarkup KeyboardLightModeAutoOff(List<Lamp> lamps, List<AutoOffMode> autooff)
        {
            var inlineLight = new InlineKeyboardMarkup(new[]
                 {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(autooff[0].IconCurrent+lamps[0].Name,autooff[0].Command),
                            InlineKeyboardButton.WithCallbackData(autooff[0].TimeBegin.ToString(),"changetime"+autooff[0].AutoOffModeID),
                        },

                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(autooff[1].IconCurrent+lamps[1].Name,autooff[1].Command),
                            InlineKeyboardButton.WithCallbackData(autooff[1].TimeBegin.ToString(),"changetime"+autooff[0].AutoOffModeID),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(autooff[2].IconCurrent+lamps[2].Name,autooff[2].Command),
                            InlineKeyboardButton.WithCallbackData(autooff[2].TimeBegin.ToString(),"changetime"+autooff[0].AutoOffModeID),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(autooff[4].IconCurrent+lamps[3].Name,autooff[3].Command),
                            InlineKeyboardButton.WithCallbackData(autooff[4].TimeBegin.ToString(),"changetime"+autooff[0].AutoOffModeID),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(autooff[5].IconCurrent+lamps[5].Name,autooff[5].Command),
                            InlineKeyboardButton.WithCallbackData(autooff[5].TimeBegin.ToString(),"changetime"+autooff[0].AutoOffModeID),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(autooff[6].IconCurrent+lamps[6].Name,autooff[6].Command),
                            InlineKeyboardButton.WithCallbackData(autooff[6].TimeBegin.ToString(),"changetime"+autooff[0].AutoOffModeID),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(autooff[7].IconCurrent+lamps[7].Name,autooff[7].Command),
                            InlineKeyboardButton.WithCallbackData(autooff[7].TimeBegin.ToString(),"changetime"+autooff[0].AutoOffModeID),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(autooff[8].IconCurrent+lamps[8].Name,autooff[8].Command),
                            InlineKeyboardButton.WithCallbackData(autooff[8].TimeBegin.ToString(),"changetime"+autooff[0].AutoOffModeID),
                        },




                    });
            return inlineLight;
        }

        public static async void SendOnlyMessageToChat(TelegramBotClient Bot, string msg)
        {
            await Bot.SendTextMessageAsync(129973487, msg);


        }

        public static async void SendMessageToChat(TelegramBotClient Bot, string stringmsg,Telegram.Bot.Types.Message message,InlineKeyboardMarkup inlinekeyboard)
        {
            await Bot.SendTextMessageAsync(
            message.Chat.Id,
            stringmsg,
            replyMarkup: inlinekeyboard);
        }

        public static async Task EditMessageToChat(TelegramBotClient Bot, string stringmsg, Telegram.Bot.Types.Message message, InlineKeyboardMarkup inlinekeyboard)
        {
            await Bot.EditMessageTextAsync(
            message.Chat.Id,
            message.MessageId,
            stringmsg,
            replyMarkup: inlinekeyboard);
        }

        public static async void SendKeyboartToChat(TelegramBotClient Bot, string stringmsg, Telegram.Bot.Types.Message message, ReplyKeyboardMarkup replykeyboard)
        {
            await Bot.SendTextMessageAsync(
            message.Chat.Id,
            stringmsg,
            replyMarkup:replykeyboard);
        }

        public static async void SendPhotoToChat(TelegramBotClient Bot, Telegram.Bot.Types.Message message)
        {
            NetworkCredential myCred = new NetworkCredential("admin", "");
            CredentialCache credsCache = new CredentialCache();

            credsCache.Add(new Uri("http://192.168.88.234/dms.jpg"), "Basic", myCred);

            WebRequest wr = WebRequest.Create("http://192.168.88.234/dms.jpg");
            wr.Credentials = credsCache;
            var resp = wr.GetResponse();
            var stream = resp.GetResponseStream();

            await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

            

            //WebClient webClient = new WebClient();
            //webClient.DownloadFile("http://192.168.88.234/dms.jpg", @"123.png");

            const string file = @"123.png";

            var fileName = file.Split(Path.DirectorySeparatorChar).Last();

            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
             await Bot.SendPhotoAsync(
                    message.Chat.Id,
                    stream,
                    "Прихожая");
            }
            
        }

    }
}
