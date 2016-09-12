using System;
using System.Threading.Tasks;

namespace Mnubo.SmartObjects.Client.Impl
{
    internal class ClientUtils
    {
        internal static void WaitTask(Task task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException aggreEx)
            {
                throw aggreEx.InnerException;
            }
        }

        internal static T WaitTask<T>(Task<T> task)
        {
            try
            {
                task.Wait();

                return task.Result;
            }
            catch (AggregateException aggreEx)
            {
                throw aggreEx.InnerException;
            }
        }
    }
}
