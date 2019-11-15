﻿using QueueReciverService.Models;
using System.Threading.Tasks;

namespace QueueReciverService.Services
{
    public interface IAccessService
    {
        ValueTask<bool> HandleRequest(AccessInfo accessInfo);
    }
}
