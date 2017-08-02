namespace Newegg.OZZO.GodEye.Statistics.Winform
{
    public class StatisticsInfo
    {
        public From From {get;set;}

        public string User{get;set;}

        public string ComputerName{get;set;}

        public DateTime BeginTime{get;set;}

        public DateTime EndTime{get;set;}

        public bool StatisticsFinished{get;set;}

        public string ToJsonString()
        {

        }
    }

    public class StatisticsInfoTransporter
    {
        private bool m_StopFlag = false;

        public List<StatisticsInfo> m_StatisticInfoQueue = new List<StatisticsInfo>()

        public void StartStatistics(Form form)
        {
            this.m_StatisticInfoQueue.Add(new StatisticsInfo
            {
                Form = form,
                User = user，
                ComputerName = "";
                BeginTime = DateTime.Now,
                EndTime = DateTime.Now，
                IsStatisticsFinished = false
            });
        }

        public void StopStatistics(Form form)
        {
            var info = this.m_StatisticInfoQueue.FirstOrDefault(each => each.Form = form && !each.IsStatisticsFinished);
            if(info != null)
            {
                info.EndTime = DateTime.Now;
                info.IsStatisticsFinished = true;
            }
        }

        public void Start()
        {
            Thread thread = new Thread();
            thread.IsBackGround = true;
            thread.Start(this.Transport);
        }

        public void Stop()
        {
            this.Transport
        }

        private void Transport()
        {
            while(true)
            {
                if(!this.m_StopFlag)
                {
                    CallApi();
                    break;
                }
                else
                {
                    var statisticsInfos = this.m_StatisticInfoQueue.Where(each => each.IsStatisticsFinished).ToList();
                    if(statisticsInfos.Count >= 300)
                    {
                        CallApi();
                    }
                    else
                    {
                        Thread.Sleep(2000);
                    }
                }
            }
        }
    }

    public class GodEye
    {
        private Form m_MDIParentForm;
        private List<Form> childForms = new List<Form>();
        private StatisticsInfoTransporter m_StatisticsInfoTransporter = new StatisticsInfoTransporter();

        public void Watch(Form mdiForm)
        {
            if(mdiForm == null)
            {
                throw new ArgumentNullException(nameof(mdiForm));
            }

            if(!midForm.IsMDIParent)
            {
                throw new NotSupportedException("Only support multiple document interface WinFrom application.");
            }

            this.m_MDIParentForm = mdiForm;
        }

        public void Start()
        {
            this.m_StatisticsInfoTransporter.StartTransport();
            this.m_MDIParentForm.ActivedChildForm += (sender, args) =>
            {
                var activedForm = ((Form)sender).ActivedChildForm;
                if(this.m_ChildForms.Contains(from))
                {
                    return;
                }

                activedForm.OnActived += (s, a) =>
                {

                };

                activedForm.AfterShown += (s, a) => 
                {
                    try
                    {
                        var form = (Form)s;
                        if(!this.m_ChildForms.Contains(form))
                        {
                            this.m_ChildForms.Add(form);
                            this.m_StatisticsInfoTransporter.StartStatistics(form);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Logger.WriterException("Active shown failed", ex);
                    }
                }; 

                activedForm.Closed += (s, a) =>
                {
                    try
                    {
                        var form = (Form)s;
                        if(this.m_ChildForms.Contains(from))
                        {
                            this.m_ChildForms.Remove(from);
                            this.m_StatisticsInfoTransporter.StopStatistics(form);
                        }
                    }
                    catch(System.Exception ex)
                    {
                        Logger.WriteException("Close failed", ex);
                    }
                };
            };

            this.m_MDIParentForm.Closed += (s, a) =>
            {
                this.StopWatch();
            };
        }

        public void StopWatch()
        {
            this.m_StatisticsInfoTransporter.Stop();
        }
    }

    public class Client()
    {
        public void Main()
        {
            var mainForm = new MainForm();
            var godEye = new GodEye();
            godEye.Watch(mainForm);
            godEye.Start();
            Run(mainForm);
        }
    }
}
