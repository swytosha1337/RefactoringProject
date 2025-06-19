using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Windows;
namespace WpfAppWithoutRefactoring
{
    internal class OrderCreater
    {
        public OrderCreater(string customerName, List<string> items, string paymentMethod)
        {
            MethodValidation(customerName, items);
            decimal total = ClassOrderProcessingLogic.MethodCalculationOfTheTotalAmount(items);
            ClassDiscounts.MethodApplyingDiscounts(paymentMethod, total);
            ClassPaymentProcessing.MethodRouting(customerName);
            ClassSendingNotification.MethodLetter(customerName, total);
        }
        private static void MethodValidation(string customerName, List<string> items)
        {
            if (string.IsNullOrEmpty(customerName))
                throw new ArgumentException("Данные заказчика указаны не корректно!");
            if (items == null || items.Count == 0)
                throw new ArgumentException("Не выбраны позиции для заказа!");
        }
        private static class ClassOrderProcessingLogic
        {
            private static Dictionary<string, decimal> ItemPrices = new Dictionary<string, decimal>
            {
                {"Ноутбук", 1200m},
                {"Мышь", 25m},
                {"Клавиатура", 50m},
                {"Камера", 500m},
                {"Колонки", 150m}
            };
            public static decimal MethodCalculationOfTheTotalAmount(List<string> items)
            {
                decimal total = 0;
                foreach (string item in items)
                {
                    if (ItemPrices.ContainsKey(item))
                    {
                        total += ItemPrices[item];
                    }
                }
                if (items.Count > 2)
                {
                    total *= 0.9m;
                }
                return total;
            }
        }
        private static class ClassDiscounts
        {
            public static void MethodApplyingDiscounts(string method, decimal amount)
            {
                switch (method)
                {
                    case "По карте":
                        ProcessCardPayment(amount);
                        break;
                    case "PayPal":
                        ProcessPayPalPayment(amount);
                        break;
                    default:
                        throw new ArgumentException("Неподдерживаемый способ оплаты.");
                }
            }
            private static void ProcessCardPayment(decimal amount)
            {
                MessageBox.Show($"Обработка платежа по кредитной карте: {amount}");
            }
            private static void ProcessPayPalPayment(decimal amount)
            {
                Console.WriteLine($"Обработка PayPal платежа: {amount}");
            }
        }
        private static class ClassPaymentProcessing
        {
            private const string LogPath = @".\logs\order.txt";
            public static void MethodRouting(string customerName)
            {
                EnsureLogDirectoryExists();
                File.WriteAllText(LogPath, $"Order processed for {customerName} at {DateTime.Now}");
            }
            private static void EnsureLogDirectoryExists()
            {
                string logDir = Path.GetDirectoryName(LogPath);
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
            }
        }
        private static class ClassSendingNotification
        {
            public static void MethodLetter(string customerName, decimal total)
            {
                try
                {
                    using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                    using (MailMessage message = CreateEmailMessage(customerName, total))
                    {
                        smtpClient.Send(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка отправки email: {ex.Message}");
                }
            }
            private static MailMessage CreateEmailMessage(string customerName, decimal total)
            {
                MailAddress from = new MailAddress("lomovala@gmail.com", "LomovaLA");
                MailAddress to = new MailAddress("lomovala@gmail.com");
                MailMessage message = new MailMessage(from, to);
                message.Subject = "Новый заказ";
                message.Body = $"<h2>Новый заказ для {customerName} общей стоимостью {total:C}.</h2>";
                message.IsBodyHtml = true;
                return message;
            }
        }
    }
}