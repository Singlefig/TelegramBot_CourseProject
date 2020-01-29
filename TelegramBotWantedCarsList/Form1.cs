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

namespace TelegramBotWantedCarsList
{
    public partial class Form1 : Form
    {
        BackgroundWorker bw;
        List<CarInfo> cars = new List<CarInfo>();
        //string carsList;
        //StringBuilder sb;
        public Form1()
        {
            InitializeComponent();

            this.bw = new BackgroundWorker();
            bw.DoWork += this.bw_DoWork;

        }

        void getCarsInfo()
        {
            string fileName = @"C:\Users\singlefig-ap\Downloads\mvswantedtransport_1.json";
            //using (StreamReader file = File.OpenText(@"C:\Users\singlefig-ap\Downloads\mvswantedtransport_1.json"))
                //var carsInfos = JsonConvert.DeserializeObject<List<CarInfoRoot>>(File.ReadAllText(fileName));
            cars = JsonConvert.DeserializeObject<List<CarInfo>>(File.ReadAllText(fileName));
            //sb = new StringBuilder();
            //foreach (var car in cars)
            //{
            //    string currentCar = $"ID:{car.ID}\n" +
            //         $"OVD:{ car.OVD}\n" +
            //         $"BRAND:{ car.BRAND}\n" +
            //         $"COLOR:{ car.COLOR}\n" +
            //         $"VEHICLENUMBER:{ car.VEHICLENUMBER}\n" +
            //         $"BODYNUMBER:{ car.BODYNUMBER}\n" +
            //         $"CHASSISNUMBER:{ car.CHASSISNUMBER}\n" +
            //         $"ENGINENUMBER:{ car.ENGINENUMBER}\n" +
            //         $"THEFT_DATA:{ car.THEFT_DATA}\n" +
            //         $"INSERT_DATE:{car.INSERT_DATE}\n";
            //    sb.Append(currentCar);
            //}
        }

        async void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            var key = e.Argument as String; // получаем ключ из аргументов

            try
            {
                var Bot = new Telegram.Bot.TelegramBotClient(key); // инициализируем API
                await Bot.SetWebhookAsync("");
                //Bot.SetWebhook(""); // Обязательно! убираем старую привязку к вебхуку для бота
                int offset = 0; // отступ по сообщениям
                while (true)
                {
                    var updates = await Bot.GetUpdatesAsync(offset); // получаем массив обновлений

                    foreach (var update in updates) // Перебираем все обновления
                    {
                        var message = update.Message;
                        if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                        {
                            if (message.Text == "/test")
                            {
                                // в ответ на команду /saysomething выводим сообщение
                                await Bot.SendTextMessageAsync(message.Chat.Id, "test",
                                       replyToMessageId: message.MessageId);
                            }
                            if (message.Text == "/stop")
                            {
                                await Bot.StopPollAsync(message.Chat.Id,message.MessageId);
                                // в ответ на команду /saysomething выводим сообщение
                                await Bot.SendTextMessageAsync(message.Chat.Id, "Stopped!",
                                       replyToMessageId: message.MessageId);
                            }
                            if (message.Text == "/list")
                            {
                                await Bot.SendTextMessageAsync(message.Chat.Id,$"List of wanted cars:");
                                foreach (var car in cars)
                                {
                                    await Bot.SendTextMessageAsync(message.Chat.Id, "ID:" + car.ID + "\n" +
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
                        }
                        offset = update.Id + 1;
                    }

                }
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
    }
}
