using System;
using System.Threading;
using BASAC.Util;

namespace BASAC.ActionAutomation
{
    public class ActionAutomationService
    {
        private static ActionAutomationService _instance;
        private static readonly object Lock = new object();
        Thread tANU;

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
                        /*if (_AutomationNodesUpdateQueue.Count > 0)
                        {
                            ANqueueLock = true;
                            _UpdateQueue.Clear();
                            _UpdateQueueAL.Clear();
                            foreach (var item in _AutomationNodesUpdateQueue)
                            {
                                _UpdateQueue.Add(item);
                            }
                            foreach (var item in _AutomationNodesUpdateQueueAL)
                            {
                                _UpdateQueueAL.Add(item);
                            }
                            _AutomationNodesUpdateQueue.Clear();
                            _AutomationNodesUpdateQueueAL.Clear();
                            ANqueueLock = false;
                            _UpdateQueue = _UpdateQueue.Distinct().ToList();
                            foreach (var item in _UpdateQueue)
                            {
                                UpdateANstate2(item, "", "", "", _UpdateQueueAL);
                            }
                        }*/
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
