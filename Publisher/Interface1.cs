﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Publisher
{
    public interface ISender
    {
        Task Send();
    }
}
