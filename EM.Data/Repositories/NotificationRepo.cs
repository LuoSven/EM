using System;
using System.Linq;
using System.Threading.Tasks;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class NotificationRepo : RepositoryBase<Notification>, INotificationRepo
    {
        public NotificationRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        /// <summary>获取特定内容的站内信
        /// 
        /// </summary>
        /// <param name="fromId"></param>
        /// <param name="fromType"></param>
        /// <param name="toId"></param>
        /// <param name="toType"></param>
        /// <param name="notificationType"></param>
        /// <param name="note"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Notification GetNotification(int fromId, byte? fromType, int toId, byte? toType, int notificationType, string note, string url)
        {
            var query = (from f in DataContext.Notification
                         where f.FromID == fromId && f.ToID == toId && f.ToType == toType && f.FromType == fromType && f.Type == notificationType && f.Note == note
                         select f).FirstOrDefault();
            return query;
        }

        /// <summary>获取站内信数目
        /// 
        /// </summary>
        /// <param name="toID"></param>
        /// <param name="toType"></param>
        /// <param name="notificationType"></param>
        /// <returns></returns>
        public int GetNotificationCount(int toID, byte? toType, byte? notificationType)
        {
            var query = from f in DataContext.Notification
                        where f.ToID == toID && f.ToType == toType && (notificationType == null || f.Type == notificationType)
                        select f;
            return query.Count();
        }

        public void SendApplyPositionNotification(TB_S_Position m, string companyName, string positionName)
        {
            if (!string.IsNullOrEmpty(companyName))
            {
                Notification msg = new Notification()
                {
                    FromID = m.EnterpriseId,
                    ToID = m.UserId,
                    ToType = (byte)RoleType.Student,
                    CreateTime = DateTime.Now,
                    CreateBy = "system",
                    IsNew = true
                    
                };
                if (m.DeleteDate.HasValue)
                {
                    msg.Note = string.Format("{0} HR认为您的简历和职位有些不匹配，很抱歉地通知您无法进入面试，继续加油！", companyName);
                    msg.Type = (byte)MessageType.Refused;
                    msg.URL = "mystep/applylist";
                    msg.PositionName = positionName;
                    msg.PositionID = m.PositionId;
                    Add(msg);
                }
                else if (new int[] { (int)ApplyStatus.Read, (int)ApplyStatus.Interested, (int)ApplyStatus.Approved }.Contains(m.ApplyStatus))
                {
                    switch (m.ApplyStatus)
                    {
                        case (int)ApplyStatus.Read:
                            msg.Note = companyName + "的HR已经查阅了您的简历";
                            msg.Type = (byte)MessageType.Read;
                            msg.URL = "mystep/applylist";   //XX(公司)的HR查阅了你投递的简历。 个人中心--求职管理页面
                            msg.PositionID = m.PositionId;
                            msg.PositionName = positionName;
                            break;
                        case (int)ApplyStatus.Interested:
                            msg.Note = companyName + "的HR将你投递的简历标记为感兴趣。";
                            msg.Type = (byte)MessageType.Interest;
                            msg.URL = "mystep/applylist";     //XX(公司)的HR将你投递的简历标记为感兴趣。 个人中心--求职管理页面
                            msg.PositionName = positionName;
                            msg.PositionID = m.PositionId;
                            break;
                        case (int)ApplyStatus.Approved:
                            msg.Note = companyName + "的HR给你发送了面试通知。";
                            msg.Type = (byte)MessageType.Interview;
                            msg.URL = "mystep/applylist";       //XX(公司)的HR给你发送了面试通知。 个人中心--求职管理页面
                            msg.PositionID = m.PositionId;
                            msg.PositionName = positionName;
                            break;
                    }
                    Add(msg);
                }
              
            }
        }

        public async Task CreateAsync(Notification notification) 
        {
            DataContext.Notification.Add(notification);
            DataContext.SaveChanges();
        }

    }


    public interface INotificationRepo : IRepository<Notification>
    {
        Notification GetNotification(int fromId, byte? fromType, int toId, byte? toType, int notificationType, string note, string url);

        int GetNotificationCount(int toID, byte? toType, byte? notificationType);

        void SendApplyPositionNotification(TB_S_Position m, string companyName, string positionName);

        Task CreateAsync(Notification notification);

    }


}
