﻿using System.Web.Http;
using System.Web.Routing;
using System.Web.Mvc;

namespace mojoPortal.Web.Routing
{
    public interface IRegisterRoutes
    {
       void Register(HttpConfiguration config);
       void RegisterRoutes(RouteCollection routes);
       void RegisterGlobalFilters(GlobalFilterCollection filters);
    }

    
}