using System;
using System.Collections.Generic;
using System.Threading;

// Интерфейс Subject
public interface ISubject
{
    string Request(string request);
}

// Реальный субъект, содержащий бизнес-логику
public class RealSubject : ISubject
{
    public string Request(string request)
    {
        return "RealSubject обработал запрос: " + request;
    }
}

// Проксирующий класс, контролирующий доступ к RealSubject
public class Proxy : ISubject
{
    private RealSubject _realSubject;
    private Dictionary<string, string> _cache = new Dictionary<string, string>();
    private Dictionary<string, DateTime> _cacheExpiration = new Dictionary<string, DateTime>();
    private TimeSpan _cacheDuration = TimeSpan.FromSeconds(30); // Время жизни кэша

    public string Request(string request)
    {
        // Проверка прав доступа
        if (!CheckAccess())
        {
            return "Доступ запрещен";
        }

        // Проверяем кэшированный ответ
        if (_cache.ContainsKey(request) &&
            _cacheExpiration[request] > DateTime.Now)
        {
            return _cache[request];
        }

        // Если нет в кэше, вызываем реальный субъект
        if (_realSubject == null)
            _realSubject = new RealSubject();
        string result = _realSubject.Request(request);

        // Сохраняем результат в кэш
        _cache[request] = result;
        _cacheExpiration[request] = DateTime.Now.Add(_cacheDuration);

        return result;
    }

    private bool CheckAccess()
    {
        // Здесь можно добавить логику для проверки прав доступа
        return true; // Для примера всегда возвращаем true
    }
}

// Пример использования
public class Program
{
    public static void Main(string[] args)
    {
        ISubject proxy = new Proxy();

        Console.WriteLine(proxy.Request("Запрос 1")); // Обработка запроса
        Console.WriteLine(proxy.Request("Запрос 1")); // Из кэша

        Thread.Sleep(35000); // Ждем 35 секунд

        Console.WriteLine(proxy.Request("Запрос 1")); // Снова обработка, так как кэш истек
    }
}