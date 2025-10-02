using System;
using System.Collections.Generic;
using DesignPatternsDemo.Behavioral;
using DesignPatternsDemo.Creational;
using DesignPatternsDemo.Structural;

namespace DesignPatternsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Behavioral Patterns ===");
            ObserverPatternDemo.Run();
            StrategyPatternDemo.Run();

            Console.WriteLine("\n=== Creational Patterns ===");
            SingletonPatternDemo.Run();
            FactoryPatternDemo.Run();

            Console.WriteLine("\n=== Structural Patterns ===");
            AdapterPatternDemo.Run();
            DecoratorPatternDemo.Run();
        }
    }
}

// ====================== BEHAVIORAL PATTERNS ======================

// 1. Observer Pattern
namespace DesignPatternsDemo.Behavioral
{
    public interface IObserver { void Update(float temperature); }

    public class WeatherStation
    {
        private List<IObserver> observers = new List<IObserver>();
        private float temperature;

        public void AddObserver(IObserver obs) => observers.Add(obs);
        public void RemoveObserver(IObserver obs) => observers.Remove(obs);

        public void SetTemperature(float temp)
        {
            temperature = temp;
            NotifyObservers();
        }

        private void NotifyObservers()
        {
            foreach (var obs in observers) obs.Update(temperature);
        }
    }

    public class Display : IObserver
    {
        private string name;
        public Display(string name) => this.name = name;
        public void Update(float temperature) =>
            Console.WriteLine($"{name} Display: Temp updated to {temperature}°C");
    }

    public static class ObserverPatternDemo
    {
        public static void Run()
        {
            var station = new WeatherStation();
            var phoneDisplay = new Display("Phone");
            var tvDisplay = new Display("TV");

            station.AddObserver(phoneDisplay);
            station.AddObserver(tvDisplay);

            station.SetTemperature(25.5f);
            station.SetTemperature(30.2f);
        }
    }
}

// 2. Strategy Pattern
namespace DesignPatternsDemo.Behavioral
{
    public interface IPaymentStrategy { void Pay(decimal amount); }

    public class CreditCardPayment : IPaymentStrategy
    {
        public void Pay(decimal amount) => Console.WriteLine($"Paid {amount} using Credit Card.");
    }

    public class PayPalPayment : IPaymentStrategy
    {
        public void Pay(decimal amount) => Console.WriteLine($"Paid {amount} using PayPal.");
    }

    public class PaymentContext
    {
        private IPaymentStrategy strategy;
        public void SetStrategy(IPaymentStrategy s) => strategy = s;
        public void Pay(decimal amount) => strategy.Pay(amount);
    }

    public static class StrategyPatternDemo
    {
        public static void Run()
        {
            var context = new PaymentContext();

            context.SetStrategy(new CreditCardPayment());
            context.Pay(100);

            context.SetStrategy(new PayPalPayment());
            context.Pay(200);
        }
    }
}

// ====================== CREATIONAL PATTERNS ======================

// 3. Singleton Pattern
namespace DesignPatternsDemo.Creational
{
    public sealed class Logger
    {
        private static readonly Logger instance = new Logger();
        private Logger() { }
        public static Logger Instance => instance;

        public void Log(string message) =>
            Console.WriteLine($"[LOG]: {message}");
    }

    public static class SingletonPatternDemo
    {
        public static void Run()
        {
            Logger.Instance.Log("This is a singleton logger example.");
            Logger.Instance.Log("Only one logger instance exists.");
        }
    }
}

// 4. Factory Pattern
namespace DesignPatternsDemo.Creational
{
    public interface IShape { void Draw(); }

    public class Circle : IShape { public void Draw() => Console.WriteLine("Drawing Circle."); }
    public class Square : IShape { public void Draw() => Console.WriteLine("Drawing Square."); }

    public class ShapeFactory
    {
        public static IShape GetShape(string type)
        {
            return type.ToLower() switch
            {
                "circle" => new Circle(),
                "square" => new Square(),
                _ => throw new ArgumentException("Invalid shape type")
            };
        }
    }

    public static class FactoryPatternDemo
    {
        public static void Run()
        {
            IShape shape1 = ShapeFactory.GetShape("circle");
            shape1.Draw();

            IShape shape2 = ShapeFactory.GetShape("square");
            shape2.Draw();
        }
    }
}

// ====================== STRUCTURAL PATTERNS ======================

// 5. Adapter Pattern
namespace DesignPatternsDemo.Structural
{
    public interface INewMediaPlayer { void Play(string filename); }

    public class LegacyAudioPlayer
    {
        public void PlayMp3(string file) => Console.WriteLine($"Playing MP3: {file}");
    }

    public class MediaAdapter : INewMediaPlayer
    {
        private LegacyAudioPlayer legacyPlayer = new LegacyAudioPlayer();
        public void Play(string filename) => legacyPlayer.PlayMp3(filename);
    }

    public static class AdapterPatternDemo
    {
        public static void Run()
        {
            INewMediaPlayer player = new MediaAdapter();
            player.Play("song.mp3");
        }
    }
}

// 6. Decorator Pattern
namespace DesignPatternsDemo.Structural
{
    public interface ICoffee { string GetDescription(); double GetCost(); }

    public class SimpleCoffee : ICoffee
    {
        public string GetDescription() => "Simple Coffee";
        public double GetCost() => 5.0;
    }

    public abstract class CoffeeDecorator : ICoffee
    {
        protected ICoffee coffee;
        protected CoffeeDecorator(ICoffee c) { coffee = c; }
        public virtual string GetDescription() => coffee.GetDescription();
        public virtual double GetCost() => coffee.GetCost();
    }

    public class MilkDecorator : CoffeeDecorator
    {
        public MilkDecorator(ICoffee c) : base(c) { }
        public override string GetDescription() => base.GetDescription() + ", Milk";
        public override double GetCost() => base.GetCost() + 1.5;
    }

    public class SugarDecorator : CoffeeDecorator
    {
        public SugarDecorator(ICoffee c) : base(c) { }
        public override string GetDescription() => base.GetDescription() + ", Sugar";
        public override double GetCost() => base.GetCost() + 0.5;
    }

    public static class DecoratorPatternDemo
    {
        public static void Run()
        {
            ICoffee coffee = new SimpleCoffee();
            Console.WriteLine($"{coffee.GetDescription()} -> ${coffee.GetCost()}");

            coffee = new MilkDecorator(coffee);
            coffee = new SugarDecorator(coffee);
            Console.WriteLine($"{coffee.GetDescription()} -> ${coffee.GetCost()}");
        }
    }
}

