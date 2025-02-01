# Зачем два поля для одного параметра?

При разворачивании кода в конечный автомат для аргументов метода под капотом создаются две переменные.

Рассматриваемый пример кода:

```csharp
    public IEnumerable<int> SomeMethod(int i)
    {
        while (true)
        {
            yield return i++;
        }
    }
```

Значение параметра `i` будет изменяться каждый раз при вызове метода `MoveNext()`. Всё это у нас примерно развернётся в примерно в такой конечный автомат.

```csharp
public class StateMachineSecondExample
{
    #region Декомпиляция написанного кода, перевод его на "понятный язык"

    /// <summary>
    /// Вызов исходного yield метода. Преобразование в конечный автомат внутри которого создаётся генератор последовательности
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    [IteratorStateMachine(typeof(SomeMethod_generator))]
    public IEnumerable<int> SomeMethod(int i)
    {
        SomeMethod_generator generator = new(-2);

        // Инициализация переменных для параметров метода
        generator.param_local_i = i;

        return generator;
    }

    private class SomeMethod_generator : IEnumerable<int>,
        IEnumerable,
        IEnumerator<int>,
        IEnumerator
    {
        # region Поля для построения конечного автомата

        private int m_state;
        private int m_current;
        private int m_local_i;
        private int m_initial_thread_id;

        #endregion

        # region Поля для аргументов yield метода

        public int param_local_i;

        # endregion

        public SomeMethod_generator(int startState)
        {
            m_state = startState;
        }

        public int Current => m_current;

        object? IEnumerator.Current => Current;

        public IEnumerator<int> GetEnumerator()
        {
            SomeMethod_generator generator;

            if (m_state == -2 && m_initial_thread_id == Environment.CurrentManagedThreadId)
            {
                m_state = 0;
                generator = this;
            }
            else
            {
                generator = new SomeMethod_generator(0);
            }

            // Присваиваем значения переданные в аргументе закрытой переменной конечного автомата, которая будет изменяться
            // Очень важная строчка в этом примере :)
            generator.m_local_i = param_local_i;

            return generator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool MoveNext()
        {
            switch (m_state)
            {
                case 0:
                    m_state = -1;
                    break;
                case 1:
                    m_state = -1;
                    break;
                default:
                    return false;
            }

            m_current = m_local_i++;
            m_state = 1;
            return true;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            // TODO: очистка не управляемых ресурсов
        }
    }

    #endregion
}
```

Основной момент, на который стоит обратить внимание, — это две переменные: `m_local_i` и `param_local_i`. Значение `param_local_i` задаётся при вызове исходного yield-метода `SomeMethod()` и после этого не изменяется.

Дело в том, что у одного класса-генератора метод `GetEnumerator()` может быть вызван несколько раз. При первом вызове `GetEnumerator()` возвращается не новый объект итератора, а тот же самый. А при последующих вызовах будет возвращаться новый итератор.

```csharp
        var firstEnumerator = enumerable.GetEnumerator();
        //enumerable.local_i = enuemrable.param_i;

        firstEnumerator.MoveNext();
        // firstEnumerator.local_i++
        firstEnumerator.MoveNext();
        // firstEnumerator.local_i++ 
        firstEnumerator.MoveNext();
        // firstEnumerator.local_i++
```

Затем есть у `firstEnumerator` будет вызван несколько раз метод `MoveNext()`, и значение `m_local_i` будет изменено.

При получении второго итератора мы ожидаем, что значение `local_i` будет проинициализировано тем же значением, которое было передано в метод изначально.

```csharp
        var secondEnumerator = enumerable.GetEnumerator();
        // secondEnumerator.local_i = 0
        // Значение переменной local_i будет присвоено из переменной enumerable.param_i
```

**Резюме**. Объекты, возвращаемые при вызове `GetEnumerator()`, в определённой степени независимы друг от друга. Каждый из них начинает генерировать последовательность, используя значения параметров, переданных при вызове yield-метода. Это достигается за счёт хранения исходного значения параметра в дополнительном поле.