# Конечный автомат yield методов возвращающий интерфейс IEnumerator

Рассматриваемый пример кода:

```csharp
    public IEnumerator<int> SomeMethod(int i)
    {
        while (i < 3)
        {
            yield return i++;
        }
    }
```

Конечный автомат

```csharp
public class StateMachineThirdExample
{
    [IteratorStateMachine(typeof(SomeMethod_generator))]
    public IEnumerator<int> SomeMethod(int i)
    {
        SomeMethod_generator generator = new(i);

        return generator;
    }

    private class SomeMethod_generator : IEnumerator<int>,
        IEnumerator
    {
        #region Поля для построения конечного автомата

        private int m_state;
        private int m_current;
        private int m_local_i;

        #endregion

        public SomeMethod_generator(int param_local_i)
        {
            m_state = 0;
            m_current = param_local_i;
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

            if (m_current > 3)
            {
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

        public int Current => m_current;

        object? IEnumerator.Current => Current;

        public void Dispose()
        {
            // TODO: очистка неуправляемых ресурсов
        }
    }
}
```
В предыдущих примерах я рассмотрел генераторы, возвращающие `IEnumerable`. Обычно такие генераторы реализуют интерфейсы `IEnumerable` и `IEnumerator`. Теперь хотелось бы остановиться на yield-методах, возвращающих `IEnumerator`. Такие методы разворачиваются в несколько иной конечный автомат. Основные отличия заключаются в следующем:

1. Отсутствие метода `GetEnumerator()`.
2. Отсутствие поля `_initialThreadId`.
3. Использование одного поля для хранения значения параметра вместо двух.

Кроме того, есть небольшое отличие при создании объекта генератора. Для yield-метода, возвращающего `IEnumerable`, в поле `_state` при создании экземпляра записывается значение -2, которое изменяется только при вызове `GetEnumerator()`. При таком значении `_state` вызов `MoveNext()` просто возвращает false без выполнения каких-либо действий.

Если же генератор создаётся для метода, возвращающего `IEnumerator`, то метод `GetEnumerator()` у него отсутствует. Поэтому значение 0 записывается в поле `_state` сразу при создании экземпляра.