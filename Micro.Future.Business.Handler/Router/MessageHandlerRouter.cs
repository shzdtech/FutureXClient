using Micro.Future.LocalStorage;
using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Business.Handler.Router
{
    public class MessageHandlerRouter<TMessageHandler> where TMessageHandler : BaseTraderHandler
    {
        private IDictionary<ProductType, TMessageHandler> _handlerMap = new Dictionary<ProductType, TMessageHandler>();

        public virtual TMessageHandler GetMessageHandler(ProductType productType)
        {
            TMessageHandler msgHdl;
            _handlerMap.TryGetValue(productType, out msgHdl);
            return msgHdl;
        }

        public virtual TMessageHandler GetMessageHandlerByContract(string contract)
        {
            TMessageHandler msgHdl = null;
            var contractInfo = ClientDbContext.FindContract(contract);
            if (contractInfo != null)
            {
                _handlerMap.TryGetValue((ProductType)contractInfo.ProductType, out msgHdl);
            }

            return msgHdl;
        }

        public virtual void RegisterHandler(ProductType productType, TMessageHandler msgHandler)
        {
            _handlerMap[productType] = msgHandler;
        }
    }
}
