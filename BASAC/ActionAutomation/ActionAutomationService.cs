using System;
using System.Collections.Concurrent;
using System.Threading;
using BASAC.Util;

namespace BASAC.ActionAutomation
{
    public class ActionAutomationService
    {
        private static ActionAutomationService _instance;
        private static readonly object Lock = new object();
        Thread tANU;
        ConcurrentQueue<int> ActionAutomationQ;
        public static ActionAutomationService Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                        _instance = new ActionAutomationService();
                }
                return _instance;
            }
        }

        public ActionAutomationService()
        {
            tANU = new Thread(
                () =>
                {
                    while (true)
                    {
                        Thread.Sleep(100);
                        foreach (var item in ActionAutomationQ)
                        {

                        }
                        //UpdateAAstate();

                    }
                });
            tANU.Start();
        }

        public void InitAutomanionService()
        {
                Log.WriteLine("Init Action Automation Service");
        }
    }
}
