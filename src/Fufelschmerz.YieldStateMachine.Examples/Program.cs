using Fufelschmerz.YieldStateMachine.Examples.FirstExample;
using Fufelschmerz.YieldStateMachine.Examples.SecondExample;
using Fufelschmerz.YieldStateMachine.Examples.ThirdExample;

namespace Fufelschmerz.YieldStateMachine.Examples;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Введите номер действия, которое вы желаете запустить\n" +
                          "№ 1. Запустить первый пример. Первый пример показывает во что разворачиваются yield методы\n" +
                          "№ 2. Запускает второй пример. Второй пример -- это ответ на вопрос: \"Зачем два поля для одной и той же переменной передаваемой в аргументе\"\n" +
                          "№ 3. Запускает третий пример. Третий пример показывает во что разворачиваются yield методы возвращающие IEnumerator");

        var numberAction = Console.ReadLine();

        switch (numberAction)
        {
            case "1":
                FirstExample();
                break;
            case "2":
                SecondExample();
                break;
            case "3":
                ThirdExample();
                break;
            default:
                Console.WriteLine("Введён неверный номер примера");
                break;
        }

        Console.ReadKey();
    }

    private static void FirstExample()
    {
        var firstExample = new StateMachineFirstExample();

        var items = firstExample.SomeMethod(5);

        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }

    private static void SecondExample()
    {
        var secondExample = new StateMachineSecondExample();

        var enumerable = secondExample.SomeMethod(0);
        //enumerable.param_i = 0;

        var firstEnumerator = enumerable.GetEnumerator();
        //enumerable.local_i = enuemrable.param_i;

        firstEnumerator.MoveNext();
        // firstEnumerator.local_i++
        firstEnumerator.MoveNext();
        // firstEnumerator.local_i++ 
        firstEnumerator.MoveNext();
        // firstEnumerator.local_i++

        var secondEnumerator = enumerable.GetEnumerator();
        // secondEnumerator.local_i = 0
        // Значение переменной local_i будет присвоено из переменной enumerable.param_i
    }

    private static void ThirdExample()
    {
        var thirdExample = new StateMachineThirdExample();

        var enumerator = thirdExample.SomeMethod(0);

        enumerator.MoveNext();
        Console.WriteLine(enumerator.Current);
        enumerator.MoveNext();
        Console.WriteLine(enumerator.Current);
        enumerator.MoveNext();
        Console.WriteLine(enumerator.Current);
    }
}