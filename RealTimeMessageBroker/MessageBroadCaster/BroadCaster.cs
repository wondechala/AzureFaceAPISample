//|---------------------------------------------------------------|
//|                     REAL TIME MESSAGE BROKER                  |
//|---------------------------------------------------------------|
//|                     Developed by Wonde Tadesse                |
//|                        Copyright ©2018 - Present              |
//|---------------------------------------------------------------|
//|                     REAL TIME MESSAGE BROKER                  |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Common;
using RealTimeServiceBroker.Interfaces;

namespace RealTimeServiceBroker.MessageBroadCaster
{
    /// <summary>
    /// BroadCaster class
    /// </summary>
    public class BroadCaster : IBroadCast
    {
        #region Public Variables 

        private EventHandler<BroadCastEventArgs> messageListenedHandler;
        private readonly object eventLocker = new object();

        #endregion

        #region Public Methods 

        /// <summary>
        /// BroadCaster class
        /// </summary>
        /// <param name="messageRequest">MessageRequest value</param>
        public void BroadCast(MessageRequest messageRequest)
        {
            EventHandler<BroadCastEventArgs> handler;
            lock (eventLocker)
            {
                handler = messageListenedHandler;
                if (handler != null)
                {
                    handler(this, new BroadCastEventArgs(messageRequest));
                }
            }

        }
        
        #endregion

        #region Public Event 

        /// <summary>
        /// Message Listened event handler
        /// </summary>
        public event EventHandler<BroadCastEventArgs> MessageListened
        {
            add
            {
                lock (eventLocker)
                {
                    messageListenedHandler += value;
                }
            }
            remove
            {
                lock (eventLocker)
                {
                    messageListenedHandler -= value;
                }
            }
        }

        #endregion
    }
}