using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;


namespace EM.ServiceRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));
            HostFactory.Run(x =>
            {
                x.UseLog4Net();

                x.Service<ServiceRunner>();

                x.SetDescription("浙江财务系统系统服务");
                x.SetDisplayName("EM.Service");
                x.SetServiceName("EM.Service");

                x.EnablePauseAndContinue();
              
            });
        }
    }
}
