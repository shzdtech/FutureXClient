﻿using Micro.Future.LocalStorage;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class CompositeTradeExHandler : BaseTraderHandler
    {
        public static CompositeTradeExHandler DefaultInstance { get; } = new CompositeTradeExHandler();

        private ISet<BaseTraderHandler> HandlerMap
        {
            get;
        } = new HashSet<BaseTraderHandler>();


        public void RegisterHandler(BaseTraderHandler handler)
        {
            handler.OnTraded += OnReturnTraded;
            handler.OnPositionUpdated += OnSubPositionUpdated;
        }

        private void OnSubPositionUpdated(PositionVM position)
        {
            lock (PositionVMCollection)
            {
                PositionVM positionVM = PositionVMCollection.FirstOrDefault(p =>
                    p.Contract == position.Contract && p.Direction == position.Direction && p.Portfolio == position.Portfolio);

                if (position.TodayPosition + position.YdPosition == 0)
                {
                    if (positionVM != null)
                    {
                        PositionVMCollection.Remove(positionVM);
                    }

                    if (!PositionVMCollection.Any(p => p.Contract == position.Contract))
                    {
                        PositionContractSet.Remove(position.Contract);
                    }
                }
                else
                {
                    if (positionVM == null)
                    {
                        PositionVMCollection.Add(position);
                        PositionContractSet.Add(position.Contract);
                    }
                    else
                    {
                        positionVM.OpenVolume = position.OpenVolume;
                        positionVM.CloseVolume = position.CloseVolume;
                        positionVM.OpenAmount = position.OpenAmount;
                        positionVM.CloseAmount = position.CloseAmount;
                        positionVM.OpenCost = position.OpenCost;

                        if (positionVM.YdPosition != position.YdPosition || positionVM.TodayPosition != position.TodayPosition)
                        {
                            positionVM.TodayPosition = position.TodayPosition;
                            positionVM.YdPosition = position.YdPosition;
                            positionVM.Position = position.YdPosition + position.TodayPosition;
                        }
                    }
                }
            }
        }

        private void OnReturnTraded(TradeVM tradeVM)
        {
            lock (TradeVMCollection)
            {
                if (!TradeVMCollection.Any(t => t.TradeID == tradeVM.TradeID))
                {
                    TradeVMCollection.Add(tradeVM);
                }
            }
        }

        public override void QueryTrade()
        {
            foreach (var hdl in HandlerMap)
            {
                if (hdl.MessageWrapper != null && hdl.MessageWrapper.HasSignIn)
                    hdl.QueryTrade();
            }
        }

        public override void QueryPosition()
        {
            foreach (var hdl in HandlerMap)
            {
                if (hdl.MessageWrapper != null && hdl.MessageWrapper.HasSignIn)
                    hdl.QueryPosition();
            }
        }
    }
}