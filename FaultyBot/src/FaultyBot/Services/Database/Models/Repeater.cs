﻿using System;

namespace FaultyBot.Services.Database.Models
{
    public class Repeater :DbEntity
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public string Message { get; set; }
        public TimeSpan Interval { get; set; }
    }
}
