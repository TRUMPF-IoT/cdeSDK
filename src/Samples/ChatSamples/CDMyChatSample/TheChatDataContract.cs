// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CDMyChat
{
    // Messages for sending through the mesh
    public class MsgChatHello
    {
        public string SenderName { get; set; }
        public string Message { get; set; }
    }

    public class MsgChatHelloResponse
    {
        public bool Acknowledged { get; set; }
        public string SenderName { get; set; }
    }

    // Message to show in the UI
    public class ChatMessage
    {
        public Guid MessageID;
        public string Message;
        public string SenderName;
        public int SeenBy;
        public DateTimeOffset Sent;
        public DateTimeOffset Received;

        public override string ToString()
        {
            return $"[{Sent}] {SenderName}: {Message} (Seen By: {SeenBy}, Latency (ms): {(Received - Sent).TotalMilliseconds})";
        }
    }
}
