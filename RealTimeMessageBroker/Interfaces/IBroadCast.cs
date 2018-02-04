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
using System.Text;
using System.Threading.Tasks;
using Common;

namespace RealTimeServiceBroker.Interfaces
{
    /// <summary>
    /// IBroadCast interface
    /// </summary>
    public interface IBroadCast
    {
        /// <summary>
        /// BroadCast messsage
        /// </summary>
        /// <param name="messageRequest">MessageRequest value</param>
        void BroadCast(MessageRequest messageRequest);

        /// <summary>
        /// Message Listener event handler
        /// </summary>
        event EventHandler<BroadCastEventArgs> MessageListened;
    }
}
