using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppWithoutRefactoring
{
    internal class OrderCreater
    {
        public OrderCreater(string customerName, List<string> items, string paymentMethod) 
        {
            // Валидация
            if (string.IsNullOrEmpty(customerName))
            {
                throw new ArgumentException("Данные заказчика указаны не корректно!");
            }
            if (items == null || items.Count == 0)
            {
                throw new ArgumentException("Не выбраны позиции для заказа!");
            }
            
            // Логика обработки заказа
            decimal total = 0;
            foreach (var item in items)
            {
                if (item == "Ноутбук") 
                    total = total + 1200;
                else if (item == "Мышь") 
                    total = total +25;
                else if (item == "Клавиатура") 
                    total = total + 50;
                else if (item == "Камера")
                    total = total + 500;
                else if (item == "Колонки")
                    total = total + 150;
            }
           
            // Скидки
            if (items.Count > 2)
            {
                total *= (decimal)0.9; // 10% скидка
            }
            
            // Обработка платежа
            if (paymentMethod == "По карте")
            {
                MessageBox.Show($"Обработка платежа по кредитной карте: {total}");
            }
            else if (paymentMethod == "PayPal")
            {
                Console.WriteLine($"Обработка PayPal платежа  {total}");
            }
            else
            {
                throw new ArgumentException("Неподдерживаемый способ оплаты.");
            }

            // Логирование
            string logEntry = $"Order processed for {customerName} at {DateTime.Now}";
            File.WriteAllText(@".\logs\order.txt", logEntry);

            // Отправка уведомления
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            MailAddress from = new MailAddress("lomovala@gmail.com", "LomovaLA");
            MailAddress to = new MailAddress("lomovala@gmail.com");
            var message = new MailMessage(from, to)
            {
                Subject = "Новый заказ",
                Body = $"<h2>Новый заказ для {customerName} общей стоимостью {total:C}.</h2>"
            };
            smtpClient.Send(message);
        }
    }
}
