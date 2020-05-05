using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types.ReplyMarkups;


namespace TelegramBotWantedCarsList
{
    public partial class BotOptions : Form
    {
        BackgroundWorker bw;
        public List<CarInfo> cars = new List<CarInfo>();
        public List<UserSubscribes> users = new List<UserSubscribes>();
        public BotOptions()
        {
            InitializeComponent();

            this.bw = new BackgroundWorker();
            bw.DoWork += this.bw_DoWork;
        }

        public void getCarsInfo()
        {
            string fileName = @"C:\Users\singlefig-ap\source\repos\TelegramBotWantedCarsList\mvswantedtransport_1.json";
            cars = JsonConvert.DeserializeObject<List<CarInfo>>(File.ReadAllText(fileName));
        }

        public void getUsers()
        {
            string fileName = @"C:\Users\singlefig-ap\source\repos\TelegramBotWantedCarsList\users.json";
            users = JsonConvert.DeserializeObject<List<UserSubscribes>>(File.ReadAllText(fileName));
        }

        public List<CarInfo> checkCarForUserSubscribes()
        {
            List<CarInfo> foundCars = new List<CarInfo>();
            for (int i = 0; i < users.Count; i++)
            {
                for (int j = 0; j < cars.Count; j++)
                {
                    if (users[i].Subscribes.Contains(cars[j].VEHICLENUMBER))
                    {
                        foundCars.Add(cars[j]);
                    }
                }
            }
            return foundCars;
        }

        public void setUsers(UserSubscribes user)
        {
            JObject usersToFile = new JObject(
                new JProperty("Id", user.Id),
                new JProperty("Name", user.Name),
                new JProperty("Subscribes", user.Subscribes)
                ); 
            using (StreamWriter file = File.CreateText(@"C:\Users\singlefig-ap\source\repos\TelegramBotWantedCarsList\users.json"))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                usersToFile.WriteTo(writer);
            }
        }

        public void updateUsers(string Id,string subscribe)
        {
            string fileName = @"C:\Users\singlefig-ap\source\repos\TelegramBotWantedCarsList\users.json";
            string json = File.ReadAllText(fileName);
            List<UserSubscribes> usersSubs = JsonConvert.DeserializeObject<List<UserSubscribes>>(json);
            foreach (var user in usersSubs)
            {
                if(user.Id == Id && !user.Subscribes.Contains(subscribe))
                {
                    user.Subscribes.Add(subscribe);
                }
            }
            string output = JsonConvert.SerializeObject(usersSubs, Formatting.Indented);
            File.WriteAllText(fileName, output);
        }

        async void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            bool isSub = false;
            bool isUnSub = false;
            var key = e.Argument as String;
            try
            {
                var Bot = new Telegram.Bot.TelegramBotClient(key);
                await Bot.SetWebhookAsync("");

                Bot.OnCallbackQuery += async (object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev) =>
                {
                    var message = ev.CallbackQuery.Message;
                    switch (ev.CallbackQuery.Data)
                    {
                        case "VEHICLENUMBER":
                            {
                                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Please enter /find;VEHICLENUMBER;Your VEHICLENUMBER:", true);
                            }
                            break;
                        case "BODYNUMBER":
                            {
                                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Please enter /find;BODYNUMBER;Your BODYNUMBER:", true);
                            }
                            break;
                        case "ENGINENUMBER":
                            {
                                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Please enter /find;ENGINENUMBER;Your ENGINENUMBER:", true);
                            }
                            break;
                        default:
                            break;
                    }
                    
                };
                Bot.OnUpdate += async (object su, Telegram.Bot.Args.UpdateEventArgs evu) =>
                {
                    if (evu.Update.CallbackQuery != null || evu.Update.InlineQuery != null) return; 
                    var update = evu.Update;
                    var message = update.Message;
                    var messages = message.Text.Split(';');
                    if (message == null) return;
                    if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                    {
                        if (message.Text == "/test")
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "test",
                                   replyToMessageId: message.MessageId);
                        }
                        else if(message.Text == "/check")
                        {
                            List<CarInfo> result = checkCarForUserSubscribes();
                            if (result.Count > 0)
                            {
                                foreach (var car in result)
                                {
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "This car was found\n" + "ID:" + car.Id + "\n" +
                                    "Місце реєстрації:" + car.OVD + "\n" +
                                    "Бренд:" + car.BRAND + "\n" +
                                    "Колір:" + car.COLOR + "\n" +
                                    "Державний номер:" + car.VEHICLENUMBER + "\n" +
                                    "Номер кузову:" + car.BODYNUMBER + "\n" +
                                    "Номер шасі:" + car.CHASSISNUMBER + "\n" +
                                    "Номер двигуна:" + car.ENGINENUMBER + "\n" +
                                    "Дата викрадення:" + car.THEFT_DATA + "\n" +
                                    "Дата додавання в сховище:" + car.INSERT_DATE + "\n");
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(message.Chat.Id,"Nothing found by your request.");
                            }
                        }
                        else if(message.Text == "/subscribe")
                        {
                            getUsers();
                            isSub = true;
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Please write car number to subscribe on it",
                                   replyToMessageId: message.MessageId);
                        }
                        else if (isSub)
                        {
                            if(CheckIfUserExist(message.Chat.Id.ToString()))
                            {
                                UserSubscribes user = new UserSubscribes
                                {
                                    Id = message.Chat.Id.ToString(),
                                    Name = message.Chat.Username,
                                    Subscribes = new List<string>() { message.Text }
                                };
                                users.Add(user);
                                setUsers(user);
                                await Bot.SendTextMessageAsync(message.Chat.Id, "You have been subscribed on "+message.Text);
                            }
                            else
                            {
                                foreach (var user in users)
                                {
                                    if (user.Id == message.Chat.Id.ToString())
                                    {
                                        user.Subscribes.Add(message.Text);
                                        updateUsers(user.Id, message.Text);
                                    }
                                }
                                await Bot.SendTextMessageAsync(message.Chat.Id, "You have been subscribed on " + message.Text);
                            }
                            isSub = false;
                        }
                        else if (message.Text == "/find")
                        {
                            var keyboard = new InlineKeyboardMarkup(
                                                new InlineKeyboardButton[][]
                                                {
                                                            new [] {
                                                                new InlineKeyboardButton
                                                                {
                                                                   Text = "VEHICLENUMBER",
                                                                   CallbackData = "VEHICLENUMBER"
                                                                },
                                                                new InlineKeyboardButton
                                                                {
                                                                   Text = "BODYNUMBER",
                                                                   CallbackData = "BODYNUMBER"
                                                                },
                                                                new InlineKeyboardButton
                                                                {
                                                                   Text = "ENGINENUMBER",
                                                                   CallbackData = "ENGINENUMBER"
                                                                },
                                                            },
                                                }
                                            );
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Choose parameter which bot will use to find:", Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, keyboard);
                        }
                        else if (messages[0] == "/find" && messages[1] == "VEHICLENUMBER")
                        {
                            List<CarInfo> carInfos = FindCar(messages[1], messages[2]);
                            if (carInfos.Count > 0)
                            {
                                foreach (var car in carInfos)
                                {
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "This car was found\n" + "ID:" + car.Id + "\n" +
                                "Місце реєстрації:" + car.OVD + "\n" +
                                "Бренд:" + car.BRAND + "\n" +
                                "Колір:" + car.COLOR + "\n" +
                                "Державний номер:" + car.VEHICLENUMBER + "\n" +
                                "Номер кузову:" + car.BODYNUMBER + "\n" +
                                "Номер шасі:" + car.CHASSISNUMBER + "\n" +
                                "Номер двигуна:" + car.ENGINENUMBER + "\n" +
                                "Дата викрадення:" + car.THEFT_DATA + "\n" +
                                "Дата додавання в сховище:" + car.INSERT_DATE + "\n");
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(message.Chat.Id, "Didn't found any car by this request.");
                            }
                        }
                        else if (messages[0] == "/find" && messages[1] == "BODYNUMBER")
                        {
                            List<CarInfo> carInfos = FindCar(messages[1], messages[2]);
                            if (carInfos.Count > 0)
                            {
                                foreach (var car in carInfos)
                                {
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "This car was found\n" + "ID:" + car.Id + "\n" +
                                 "Місце реєстрації:" + car.OVD + "\n" +
                                 "Бренд:" + car.BRAND + "\n" +
                                 "Колір:" + car.COLOR + "\n" +
                                 "Державний номер:" + car.VEHICLENUMBER + "\n" +
                                 "Номер кузову:" + car.BODYNUMBER + "\n" +
                                 "Номер шасі:" + car.CHASSISNUMBER + "\n" +
                                 "Номер двигуна:" + car.ENGINENUMBER + "\n" +
                                 "Дата викрадення:" + car.THEFT_DATA + "\n" +
                                 "Дата додавання в сховище:" + car.INSERT_DATE + "\n");
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(message.Chat.Id, "Didn't found any car by this request.");
                            }
                        }
                        else if (messages[0] == "/find" && messages[1] == "ENGINENUMBER")
                        {
                            List<CarInfo> carInfos = FindCar(messages[1], messages[2]);
                            if (carInfos.Count > 0)
                            {
                                foreach (var car in carInfos)
                                {
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "This car was found\n" + "ID:" + car.Id + "\n" +
                                "Місце реєстрації:" + car.OVD + "\n" +
                                "Бренд:" + car.BRAND + "\n" +
                                "Колір:" + car.COLOR + "\n" +
                                "Державний номер:" + car.VEHICLENUMBER + "\n" +
                                "Номер кузову:" + car.BODYNUMBER + "\n" +
                                "Номер шасі:" + car.CHASSISNUMBER + "\n" +
                                "Номер двигуна:" + car.ENGINENUMBER + "\n" +
                                "Дата викрадення:" + car.THEFT_DATA + "\n" +
                                "Дата додавання в сховище:" + car.INSERT_DATE + "\n");
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(message.Chat.Id, "Didn't found any car by this request.");
                            }
                        }
                        else if(message.Text == "/unsubscribe")
                        {
                            getUsers();
                            isUnSub = true;
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Please write car number to unsubscribe from it",
                                   replyToMessageId: message.MessageId);
                        }
                        else if (isUnSub)
                        {
                            if (!CheckIfUserExist(message.Chat.Id.ToString()))
                            {
                                foreach (var user in users)
                                {
                                    if(user.Id == message.Chat.Id.ToString())
                                    {
                                        user.Subscribes.Remove(message.Text);
                                        setUsers(user);
                                    }
                                }
                                await Bot.SendTextMessageAsync(message.Chat.Id, "You have been unsubscribed from " + message.Text);
                            }
                            else
                            {
                                foreach (var user in users)
                                {
                                    if (user.Id == message.Chat.Id.ToString())
                                    {
                                        user.Subscribes.Add(message.Text);
                                        updateUsers(user.Id, message.Text);
                                    }
                                }
                                await Bot.SendTextMessageAsync(message.Chat.Id, "You have been unsubscribed from " + message.Text);
                            }
                            isUnSub = false;
                        }
                        else if (message.Text == "/help")
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Available commands:\n/find - check parameter to find a car\n/subscribe - subscribe on car number\n/unsubscribe - unsubscribe from car number\n/check - check your subscribes in database");
                        }
                        else
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Unknown command. Try /help to see list of commands");
                        }

                    }
                };

                Bot.StartReceiving();
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            var text = tokenTextField.Text;
            if (!string.IsNullOrEmpty(text) && this.bw.IsBusy != true)
            {
                if (text == "1064410371:AAFZgAB2Nly164KTF5F2FpJAWdhTDRSwKzI")
                {
                    this.bw.RunWorkerAsync(text);
                    getCarsInfo();
                    getUsers();
                }
                else
                {
                    MessageBox.Show("Wrong Telegram Token");
                }
            }
        }

        public bool CheckIfUserExist(string userId)
        {
            foreach (var user in users)
            {
                if(user.Id == userId)
                {
                    return false;
                }
            }
            return true;
        }

        public List<CarInfo> FindCar(string parameter, string value)
        {
            List<CarInfo> foundCars = new List<CarInfo>();
            switch (parameter)
            {
                case "VEHICLENUMBER":
                    {
                        foreach (var item in cars)
                        {
                            if (item.VEHICLENUMBER.Contains(value))
                            {
                                foundCars.Add(item);
                            }
                        }
                    }
                    break;
                case "BODYNUMBER":
                    {
                        foreach (var item in cars)
                        {
                            if (item.BODYNUMBER.Contains(value))
                            {
                                foundCars.Add(item);
                            }
                        }
                    }
                    break;
                
                case "ENGINENUMBER":
                    {
                        foreach (var item in cars)
                        {
                            if (item.ENGINENUMBER.Contains(value))
                            {
                                foundCars.Add(item);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
                return foundCars;
        }
    }
}
