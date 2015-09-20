using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Common;
using Topuc22Top.Data.Infrastructure;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Repositories
{
    public class MessageRepo : RepositoryBase<Message>, IMessageRepo
    {
        public MessageRepo(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }

        public Message GetMessage(int fromId, int toId, int toRole, int msgType, string msgContent)
        {
            var query = (from f in DataContext.Message
                         where f.FromId == fromId && f.ToId == toId && f.ToRole == toRole && f.MessageType == msgType && f.MessageContent == msgContent
                         select f).FirstOrDefault();
            return query;
        }

    }
    public interface IMessageRepo : IRepository<Notification>
    {
        Message GetMessage(int fromId, int toId, int toRole, int msgType, string msgContent);
    }
}
