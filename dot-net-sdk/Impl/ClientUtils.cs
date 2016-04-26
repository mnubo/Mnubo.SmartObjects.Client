using Mnubo.SmartObjects.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        internal static IEnumerable<Result> WaitTask(Task<IEnumerable<Result>> task)
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
