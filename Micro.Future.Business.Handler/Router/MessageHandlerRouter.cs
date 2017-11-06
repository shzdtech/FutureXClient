using Micro.Future.LocalStorage;
using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Business.Handler.Router
{
    public class MessageHandlerRouter<TMessageHandler> where TMessageHandler : AbstractMessageHandler
    {
        private IDictionary<ProductType, TMessageHandler> HandlerMap
        {
            get;
        } = new Dictionary<ProductType, TMessageHandler>();

        public virtual TMessageHandler GetMessageHandler(ProductType productType)
        {
            TMessageHandler msgHdl;
            HandlerMap.TryGetValue(productType, out msgHdl);
            return msgHdl;
        }

        public virtual TMessageHandler GetMessageHandlerByContract(string contract)
        {
            TMessageHandler msgHdl = null;
            var contractInfo = ClientDbContext.FindContract(contract);
            if (contractInfo != null)
            {
                HandlerMap.TryGetValue((ProductType)contractInfo.ProductType, out msgHdl);
            }
            return msgHdl;
        }

        public virtual void RegisterHandler(ProductType productType, TMessageHandler msgHandler)
        {
            HandlerMap[productType] = msgHandler;
        }
    }
}
