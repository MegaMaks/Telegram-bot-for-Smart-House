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

        public static InlineKeyboardMarkup KeyLightMode(PresenceEff presenceeffect, AutoOff autooff, LightinNight lightintight)
        {
            var inlineLight = new InlineKeyboardMarkup(new[]
{
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{presenceeffect.IconCurrent} Эффект присутствия","presence"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{autooff.IconCurrent}        Автовыкл. света","autoshut"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lightintight.IconCurrent}                  Свет в ночи","lightinnihgt"),
                        },

                    });
            return inlineLight;
        }

        public static InlineKeyboardMarkup KeyLightInit(List<Lamp> lamps)
        {
            var inlineLight = new InlineKeyboardMarkup(new[]
{
                        new []
                        {

                            InlineKeyboardButton.WithCallbackData($"{lamps[0].IconCurrent} Гостинная","living"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[1].IconCurrent}   Кабинет","study"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lamps[2].IconCurrent}         Кухня","kitchen"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[3].IconCurrent}   Детская","child"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lamps[4].IconCurrent}   Коридор","hall"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[5].IconCurrent}    Ванная","bath"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"{lamps[6].IconCurrent} Прихожая","nobody"),
                            InlineKeyboardButton.WithCallbackData($"{lamps[7].IconCurrent} Уличный","street"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData($"💡 Режимы освещения","lightmode"),
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

    }
}
