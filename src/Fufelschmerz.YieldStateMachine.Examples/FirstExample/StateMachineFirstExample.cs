using System.Collections;
using System.Runtime.CompilerServices;

namespace Fufelschmerz.YieldStateMachine.Examples.FirstExample;

/// <summary>
/// Пример 1. Преобразование в конечный автомат метода возвращающего IEnumerable
/// </summary>
public class StateMachineFirstExample
{
    #region Кусочек кода

    // public IEnumerable<int> SomeMethod(int maxValue)
    // {
    //     var prev = 0;
    //     var current = 1;
    //
    //     while (current <= maxValue)
    //     {
    //         yield return current;
    //
    //         var newCurrent = prev + current;
    //         prev = current;
    //         current = newCurrent;
    //     }
    // }

    #endregion

    #region Декомпиляция написанного кода, перевод его на "понятный язык"

    /// <summary>
    /// Вызов исходного yield метода. Преобразование в конечный автомат внутри которого создаётся генератор последовательности
    /// </summary>
    /// <param name="maxValue">Максимально значение элементов последовательности</param>
    /// <returns></returns>
    [IteratorStateMachine(typeof(SomeMethod_generator))]
    public IEnumerable<int> SomeMethod(int maxValue)
    {
        SomeMethod_generator generator = new(-2);

        // Инициализация переменных для параметров метода
        generator.param_local_max_value = maxValue;

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
        private int m_local_max_value;
        private int m_local_prev;
        private int m_local_current;
        private int m_initial_thread_id;

        #endregion


        # region Поля для аргументов yield метода

        public int param_local_max_value;

        # endregion

        public SomeMethod_generator(int startState)
        {
            m_state = startState;
            m_initial_thread_id = Environment.CurrentManagedThreadId;
        }

        public int Current => m_current;
        
        object? IEnumerator.Current => Current;

        /// <summary>
        /// Метод получения итератора
        /// </summary>
        /// <returns>Итератор</returns>
        public IEnumerator<int> GetEnumerator()
        {
            // Данный метод может быть вызван несколько раз
            // При первом вызове метода GetEnumerator() будет возвращён не новый объект итератора, а тот же самый
            // При следующих вызовах уже будет возвращаться новый итератор
            // Это обусловлено тем, что для каждой коллекции требуется свой итератор

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
            generator.m_local_max_value = param_local_max_value;

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
                    // Первый вызов MoveNext до первого выполнения yield return
                    m_state = -1;
                    m_local_prev = 0;
                    m_local_current = 1;
                    break;
                case 1:
                    // Последующие вызовы в цикле while
                    m_state = -1;
                    var local_new_current = m_local_current + m_local_prev;
                    m_local_prev = m_local_current;
                    m_local_current = local_new_current;
                    break;
                default:
                    return false;
            }

            // Условие выхода из цикла и тем самым прекращение генерирования последовательности
            if (m_local_current > m_local_max_value)
            {
                return false;
            }

            m_current = m_local_current;
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