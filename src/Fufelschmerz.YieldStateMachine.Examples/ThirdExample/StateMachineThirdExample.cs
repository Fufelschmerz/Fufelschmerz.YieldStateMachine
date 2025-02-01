using System.Collections;
using System.Runtime.CompilerServices;

namespace Fufelschmerz.YieldStateMachine.Examples.ThirdExample;

/// <summary>
/// Пример 3. Преобразование в конечный автомат метода возвращающего IEnumerator
/// </summary>
public class StateMachineThirdExample
{
    // public IEnumerator<int> SomeMethod(int i)
    // {
    //     while (i < 3)
    //     {
    //         yield return i++;
    //     }
    // }

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