using System.Collections;
using System.Runtime.CompilerServices;

namespace Fufelschmerz.YieldStateMachine.Examples.SecondExample;

/// <summary>
/// Пример 2. Ответ на вопрос: "Зачем два поля для одной и той же переменной передаваемой в аргументе"
/// </summary>
public class StateMachineSecondExample
{
    #region Кусочек кода

    // public IEnumerable<int> SomeMethod(int i)
    // {
    //     while (true)
    //     {
    //         yield return i++;
    //     }
    // }

    #endregion

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
            // TODO: очистка неуправляемых ресурсов
        }
    }

    #endregion
}