using System;
using Autofac;

// Интерфейс для отправки уведомлений
public interface INotificationSender
{
    void Send(string message);
}

// Реализация для отправки уведомлений по Email
public class EmailNotificationSender : INotificationSender
{
    public void Send(string message)
    {
        Console.WriteLine($"Отправлено по Email: {message}");
    }
}

// Реализация для отправки уведомлений через SMS
public class SmsNotificationSender : INotificationSender
{
    public void Send(string message)
    {
        Console.WriteLine($"Отправлено по SMS: {message}");
    }
}

// Реализация для отправки уведомлений через Telegram
public class TelegramNotificationSender : INotificationSender
{
    public void Send(string message)
    {
        Console.WriteLine($"Отправлено через Telegram: {message}");
    }
}

// Сервис для отправки уведомлений
public class NotificationService
{
    private readonly INotificationSender _notificationSender;

    public NotificationService(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public void Notify(string message)
    {
        _notificationSender.Send(message);
    }
}

// Консольное приложение
class Program
{
    static void Main(string[] args)
    {
        // Настраиваем DI-контейнер Autofac
        var builder = new ContainerBuilder();

        // Регистрируем зависимости
        builder.RegisterType<EmailNotificationSender>().Keyed<INotificationSender>("email");
        builder.RegisterType<SmsNotificationSender>().Keyed<INotificationSender>("sms");
        builder.RegisterType<TelegramNotificationSender>().Keyed<INotificationSender>("telegram");

        var container = builder.Build();

        // Выбор способа уведомления
        Console.WriteLine("Выберите способ уведомления: 1. Email, 2. SMS, 3. Telegram");
        int choice = int.Parse(Console.ReadLine() ?? "1");

        string key;
        switch (choice)
        {
            case 1:
                key = "email";
                break;
            case 2:
                key = "sms";
                break;
            case 3:
                key = "telegram";
                break;
            default:
                throw new ArgumentException("Неверный выбор");
        }

        using (var scope = container.BeginLifetimeScope())
        {
            // Получаем объект отправщика уведомлений из контейнера
            var sender = scope.ResolveKeyed<INotificationSender>(key);
            var service = new NotificationService(sender);

            // Ввод сообщения от пользователя
            Console.WriteLine("Введите ваше сообщение:");
            string message = Console.ReadLine();

            // Отправка уведомления
            service.Notify(message);
        }
    }
}
