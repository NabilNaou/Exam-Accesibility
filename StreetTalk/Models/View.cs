﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreetTalk.Models
{
    public class View
    {
        public virtual string UserId { get; set; }
        public virtual StreetTalkUser User { get; set; }
        public virtual int PostId { get; set; }
        public virtual PublicPost Post { get; set; }
        
    }
}
