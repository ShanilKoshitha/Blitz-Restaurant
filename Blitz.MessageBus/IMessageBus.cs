﻿using System;
using System.Threading.Tasks;

namespace Blitz.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessage(BaseMessage message, string topicName);
    }
}
