using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types.ReplyMarkups;


namespace TelegramBotWantedCarsList
{
    public partial class Form1 : Form
    {
        BackgroundWorker bw;
        List<CarInfo> cars = new List<CarInfo>();
        List<UserSubscribes> users = new List<UserSubscribes>();
        public Form1()
        {
            InitializeComponent();

            this.bw = new BackgroundWorker();
            bw.DoWork += this.bw_DoWork;

        }

        void getCarsInfo()
        {
            string fileName = @"C:\Users\singlefig-ap\Downloads\cars.json";
            //using (StreamReader file = File.OpenText(@"C:\Users\singlefig-ap\Downloads\mvswantedtransport_1.json"))
            cars = JsonConvert.DeserializeObject<List<CarInfo>>(File.ReadAllText(fileName));
        }

        void getUsers()
        {
            string fileName = @"C:\Users\singlefig-ap\Downloads\users.json";
            users = JsonConvert.DeserializeObject<List<UserSubscribes>>(File.ReadAllText(fileName));
        }

        void setUsers(UserSubscribes user)
        {
            JObject usersToFile = new JObject(
                new JProperty("Id", user.Id),
                new JProperty("Name", user.Name),
                new JProperty("Subscribes", user.Subscribes)
                ); 
            using (StreamWriter file = File.CreateText(@"C:\Users\singlefig-ap\Downloads\users.json"))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                usersToFile.WriteTo(writer);
            }
        }

        void updateUsers(string Id,string subscribe)
        {
            string fileName = @"C:\Users\singlefig-ap\Downloads\users.json";
            string json = File.ReadAllText(fileName);
            List<UserSubscribes> usersSubs = JsonConvert.DeserializeObject<List<UserSubscribes>>(json);
            foreach (var user in usersSubs)
            {
                if(user.Id == Id)
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
            var key = e.Argument as String; // получаем ключ из аргументов
            try
            {
                var Bot = new Telegram.Bot.TelegramBotClient(key); // инициализируем API
                await Bot.SetWebhookAsync("");
                //Bot.SetWebhook(""); // Обязательно! убираем старую привязку к вебхуку для бота

                Bot.OnCallbackQuery += async (object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev) =>
                {
                    var message = ev.CallbackQuery.Message;
                    switch (ev.CallbackQuery.Data)
                    {
                        case "OVD":
                            {
                                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Please enter in format /find;OVD;Your OVD:", true);
                                
                            }
                            break;
                        case "BRAND":
                            {
                                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Please enter /find;BRAND;Your BRAND:", true);
                            }
                            break;
                        case "COLOR":
                            {
                                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Please enter /find;COLOR;Your COLOR:", true);
                            }
                            break;
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
                        case "CHASSISNUMBER":
                            {
                                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Please enter /find;CHASSISNUMBER;Your CHASSISNUMBER:", true);
                            }
                            break;
                        case "ENGINENUMBER":
                            {
                                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Please enter /find;ENGINENUMBER;Your ENGINENUMBER:", true);
                            }
                            break;
                        case "THEFT_DATA":
                            {
                                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Please enter /find;THEFT_DATA;Your THEFT_DATA:", true);
                            }break;
                        case "INSERT_DATE":
                            {
                                await Bot.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Please enter /find;INSERT_DATE;Your INSERT_DATE:", true);
                            }
                            break;
                        default:
                            break;
                    }
                    
                };
                Bot.OnUpdate += async (object su, Telegram.Bot.Args.UpdateEventArgs evu) =>
                {
                    if (evu.Update.CallbackQuery != null || evu.Update.InlineQuery != null) return; // в этом блоке нам келлбэки и инлайны не нужны
                    var update = evu.Update;
                    var message = update.Message;
                    var messages = message.Text.Split(';');
                    if (message == null) return;
                    if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                    {
                        if (message.Text == "/test")
                        {
                            // в ответ на команду /saysomething выводим сообщение
                            await Bot.SendTextMessageAsync(message.Chat.Id, "test",
                                   replyToMessageId: message.MessageId);
                        }
                        else if (message.Text == "/stop")
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Stopped!",
                                   replyToMessageId: message.MessageId);
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
                            if(CheckUser(message.Chat.Id.ToString()))
                            {
                                UserSubscribes user = new UserSubscribes
                                {
                                    Id = message.Chat.Id.ToString(),
                                    Name = message.Chat.Username,
                                    Subscribes = new List<string>() { message.Text }
                                };
                                users.Add(user);
                                setUsers(user);
                                await Bot.SendTextMessageAsync(message.Chat.Id, "Yuo have been subscribed on"+message.Text);
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

                            }
                        }
                        else if (message.Text == "/list")
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, $"List of wanted cars:");
                            foreach (var car in cars)
                            {
                                //if (count < 20)
                                //{
                                //    count++;
                                await Bot.SendTextMessageAsync(message.Chat.Id, "ID:" + car.Id + "\n" +
                                    "OVD:" + car.OVD + "\n" +
                                    "BRAND:" + car.BRAND + "\n" +
                                    "COLOR:" + car.COLOR + "\n" +
                                    "VEHICLENUMBER:" + car.VEHICLENUMBER + "\n" +
                                    "BODYNUMBER:" + car.BODYNUMBER + "\n" +
                                    "CHASSISNUMBER:" + car.CHASSISNUMBER + "\n" +
                                    "ENGINENUMBER:" + car.ENGINENUMBER + "\n" +
                                    "THEFT_DATA:" + car.THEFT_DATA + "\n" +
                                    "INSERT_DATE:" + car.INSERT_DATE + "\n");
                                //}
                            }
                        }
                        else if (message.Text == "/find")
                        {
                            var keyboard = new InlineKeyboardMarkup(
                                                new InlineKeyboardButton[][]
                                                {
                                                            // First row
                                                            new [] {
                                                                // First column
                                                                new InlineKeyboardButton
                                                                {
                                                                   Text = "OVD",
                                                                   CallbackData = "OVD"
                                                                },

                                                                // Second column
                                                                new InlineKeyboardButton
                                                                {
                                                                   Text = "BRAND",
                                                                   CallbackData = "BRAND"
                                                                },

                                                                new InlineKeyboardButton
                                                                {
                                                                   Text = "COLOR",
                                                                   CallbackData = "COLOR"
                                                                },
                                                            },
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
                                                                   Text = "CHASSISNUMBER",
                                                                   CallbackData = "CHASSISNUMBER"
                                                                },
                                                            },
                                                            new []
                                                            {
                                                                new InlineKeyboardButton
                                                                {
                                                                   Text = "ENGINENUMBER",
                                                                   CallbackData = "ENGINENUMBER"
                                                                },
                                                                new InlineKeyboardButton
                                                                {
                                                                   Text = "THEFT DATA",
                                                                   CallbackData = "THEFT_DATA"
                                                                },
                                                                new InlineKeyboardButton
                                                                {
                                                                   Text = "INSERT DATE",
                                                                   CallbackData = "INSERT_DATE"
                                                                },
                                                            }
                                                }
                                            );
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Choose parameter which bot will use to find:", Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, keyboard);
                        }
                        else if (messages[0] == "/find" && messages[1] == "OVD")
                        {
                            List<CarInfo> carInfos = FindCar(messages[1], messages[2]);
                            if (carInfos.Count > 0)
                            {
                                foreach (var car in carInfos)
                                {
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "ID:" + car.Id + "\n" +
                                        "OVD:" + car.OVD + "\n" +
                                        "BRAND:" + car.BRAND + "\n" +
                                        "COLOR:" + car.COLOR + "\n" +
                                        "VEHICLENUMBER:" + car.VEHICLENUMBER + "\n" +
                                        "BODYNUMBER:" + car.BODYNUMBER + "\n" +
                                        "CHASSISNUMBER:" + car.CHASSISNUMBER + "\n" +
                                        "ENGINENUMBER:" + car.ENGINENUMBER + "\n" +
                                        "THEFT_DATA:" + car.THEFT_DATA + "\n" +
                                        "INSERT_DATE:" + car.INSERT_DATE + "\n");
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(message.Chat.Id, "Didn't found any car by this request.");
                            }
                        }
                        else if (messages[0] == "/find" && messages[1] == "BRAND")
                        {
                            List<CarInfo> carInfos = FindCar(messages[1], messages[2]);
                            if (carInfos.Count > 0)
                            {
                                foreach (var car in carInfos)
                                {
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "ID:" + car.Id + "\n" +
                                        "OVD:" + car.OVD + "\n" +
                                        "BRAND:" + car.BRAND + "\n" +
                                        "COLOR:" + car.COLOR + "\n" +
                                        "VEHICLENUMBER:" + car.VEHICLENUMBER + "\n" +
                                        "BODYNUMBER:" + car.BODYNUMBER + "\n" +
                                        "CHASSISNUMBER:" + car.CHASSISNUMBER + "\n" +
                                        "ENGINENUMBER:" + car.ENGINENUMBER + "\n" +
                                        "THEFT_DATA:" + car.THEFT_DATA + "\n" +
                                        "INSERT_DATE:" + car.INSERT_DATE + "\n");
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(message.Chat.Id, "Didn't found any car by this request.");
                            }
                        }
                        else if (messages[0] == "/find" && messages[1] == "COLOR")
                        {
                            List<CarInfo> carInfos = FindCar(messages[1], messages[2]);
                            if (carInfos.Count > 0)
                            {
                                foreach (var car in carInfos)
                                {
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "ID:" + car.Id + "\n" +
                                        "OVD:" + car.OVD + "\n" +
                                        "BRAND:" + car.BRAND + "\n" +
                                        "COLOR:" + car.COLOR + "\n" +
                                        "VEHICLENUMBER:" + car.VEHICLENUMBER + "\n" +
                                        "BODYNUMBER:" + car.BODYNUMBER + "\n" +
                                        "CHASSISNUMBER:" + car.CHASSISNUMBER + "\n" +
                                        "ENGINENUMBER:" + car.ENGINENUMBER + "\n" +
                                        "THEFT_DATA:" + car.THEFT_DATA + "\n" +
                                        "INSERT_DATE:" + car.INSERT_DATE + "\n");
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(message.Chat.Id, "Didn't found any car by this request.");
                            }
                        }
                        else if (messages[0] == "/find" && messages[1] == "VEHICLENUMBER")
                        {
                            List<CarInfo> carInfos = FindCar(messages[1], messages[2]);
                            if (carInfos.Count > 0)
                            {
                                foreach (var car in carInfos)
                                {
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "ID:" + car.Id + "\n" +
                                        "OVD:" + car.OVD + "\n" +
                                        "BRAND:" + car.BRAND + "\n" +
                                        "COLOR:" + car.COLOR + "\n" +
                                        "VEHICLENUMBER:" + car.VEHICLENUMBER + "\n" +
                                        "BODYNUMBER:" + car.BODYNUMBER + "\n" +
                                        "CHASSISNUMBER:" + car.CHASSISNUMBER + "\n" +
                                        "ENGINENUMBER:" + car.ENGINENUMBER + "\n" +
                                        "THEFT_DATA:" + car.THEFT_DATA + "\n" +
                                        "INSERT_DATE:" + car.INSERT_DATE + "\n");
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
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "ID:" + car.Id + "\n" +
                                        "OVD:" + car.OVD + "\n" +
                                        "BRAND:" + car.BRAND + "\n" +
                                        "COLOR:" + car.COLOR + "\n" +
                                        "VEHICLENUMBER:" + car.VEHICLENUMBER + "\n" +
                                        "BODYNUMBER:" + car.BODYNUMBER + "\n" +
                                        "CHASSISNUMBER:" + car.CHASSISNUMBER + "\n" +
                                        "ENGINENUMBER:" + car.ENGINENUMBER + "\n" +
                                        "THEFT_DATA:" + car.THEFT_DATA + "\n" +
                                        "INSERT_DATE:" + car.INSERT_DATE + "\n");
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(message.Chat.Id, "Didn't found any car by this request.");
                            }
                        }
                        else if (messages[0] == "/find" && messages[1] == "CHASSISNUMBER")
                        {
                            List<CarInfo> carInfos = FindCar(messages[1], messages[2]);
                            if (carInfos.Count > 0)
                            {
                                foreach (var car in carInfos)
                                {
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "ID:" + car.Id + "\n" +
                                        "OVD:" + car.OVD + "\n" +
                                        "BRAND:" + car.BRAND + "\n" +
                                        "COLOR:" + car.COLOR + "\n" +
                                        "VEHICLENUMBER:" + car.VEHICLENUMBER + "\n" +
                                        "BODYNUMBER:" + car.BODYNUMBER + "\n" +
                                        "CHASSISNUMBER:" + car.CHASSISNUMBER + "\n" +
                                        "ENGINENUMBER:" + car.ENGINENUMBER + "\n" +
                                        "THEFT_DATA:" + car.THEFT_DATA + "\n" +
                                        "INSERT_DATE:" + car.INSERT_DATE + "\n");
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
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "ID:" + car.Id + "\n" +
                                        "OVD:" + car.OVD + "\n" +
                                        "BRAND:" + car.BRAND + "\n" +
                                        "COLOR:" + car.COLOR + "\n" +
                                        "VEHICLENUMBER:" + car.VEHICLENUMBER + "\n" +
                                        "BODYNUMBER:" + car.BODYNUMBER + "\n" +
                                        "CHASSISNUMBER:" + car.CHASSISNUMBER + "\n" +
                                        "ENGINENUMBER:" + car.ENGINENUMBER + "\n" +
                                        "THEFT_DATA:" + car.THEFT_DATA + "\n" +
                                        "INSERT_DATE:" + car.INSERT_DATE + "\n");
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(message.Chat.Id, "Didn't found any car by this request.");
                            }
                        }
                        else if (messages[0] == "/find" && messages[1] == "THEFT_DATA")
                        {
                            List<CarInfo> carInfos = FindCar(messages[1], messages[2]);
                            if (carInfos.Count > 0)
                            {
                                foreach (var car in carInfos)
                                {
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "ID:" + car.Id + "\n" +
                                        "OVD:" + car.OVD + "\n" +
                                        "BRAND:" + car.BRAND + "\n" +
                                        "COLOR:" + car.COLOR + "\n" +
                                        "VEHICLENUMBER:" + car.VEHICLENUMBER + "\n" +
                                        "BODYNUMBER:" + car.BODYNUMBER + "\n" +
                                        "CHASSISNUMBER:" + car.CHASSISNUMBER + "\n" +
                                        "ENGINENUMBER:" + car.ENGINENUMBER + "\n" +
                                        "THEFT_DATA:" + car.THEFT_DATA + "\n" +
                                        "INSERT_DATE:" + car.INSERT_DATE + "\n");
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(message.Chat.Id, "Didn't found any car by this request.");
                            }
                        }
                        else if (messages[0] == "/find" && messages[1] == "INSERT_DATE")
                        {
                            List<CarInfo> carInfos = FindCar(messages[1], messages[2]);
                            if (carInfos.Count > 0)
                            {
                                foreach (var car in carInfos)
                                {
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "ID:" + car.Id + "\n" +
                                        "OVD:" + car.OVD + "\n" +
                                        "BRAND:" + car.BRAND + "\n" +
                                        "COLOR:" + car.COLOR + "\n" +
                                        "VEHICLENUMBER:" + car.VEHICLENUMBER + "\n" +
                                        "BODYNUMBER:" + car.BODYNUMBER + "\n" +
                                        "CHASSISNUMBER:" + car.CHASSISNUMBER + "\n" +
                                        "ENGINENUMBER:" + car.ENGINENUMBER + "\n" +
                                        "THEFT_DATA:" + car.THEFT_DATA + "\n" +
                                        "INSERT_DATE:" + car.INSERT_DATE + "\n");
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(message.Chat.Id, "Didn't found any car by this request.");
                            }
                        }
                        else if (message.Text == "/help")
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Available commands:\n/list - get list of wanted cars\n/find - check parameter to find a car\n/stop - stop the bot\n/subscribe - subscribe on car number\n/test - test command");
                        }

                    }
                };

                // запускаем прием обновлений
                Bot.StartReceiving();
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                Console.WriteLine(ex.Message); // если ключ не подошел - пишем об этом в консоль отладки
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var text = textBox1.Text; // получаем содержимое текстового поля txtKey в переменную text
            if (!string.IsNullOrEmpty(text) && this.bw.IsBusy != true) // если не запущен
            {
                this.bw.RunWorkerAsync(text); // запускаем
                getCarsInfo();
            }
        }

        public bool CheckUser(string userId)
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
                case "OVD":
                    {
                        foreach (var item in cars)
                        {
                            if(item.OVD.Contains(value))
                            {
                                foundCars.Add(item);
                            }
                        }
                    }
                    break;
                case "BRAND":
                    {
                        foreach (var item in cars)
                        {
                            if (item.BRAND.Contains(value))
                            {
                                foundCars.Add(item);
                            }
                        }
                    }
                    break;
                case "COLOR":
                    {
                        foreach (var item in cars)
                        {
                            if (item.COLOR.Contains(value))
                            {
                                foundCars.Add(item);
                            }
                        }
                    }
                    break;
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
                case "CHASSISNUMBER":
                    {
                        foreach (var item in cars)
                        {
                            if (item.CHASSISNUMBER.Contains(value))
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
                case "THEFT_DATA":
                    {
                        foreach (var item in cars)
                        {
                            if (item.THEFT_DATA.Contains(value))
                            {
                                foundCars.Add(item);
                            }
                        }
                    }
                    break;
                case "INSERT_DATE":
                    {
                        foreach (var item in cars)
                        {
                            if (item.INSERT_DATE.Contains(value))
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
