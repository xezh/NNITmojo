﻿// Author:		        
// Created:             2012-09-23
// Last Modified:       2012-10-04
// 
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software.

using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Web.UI;
using mojoPortal.Business;

namespace mojoPortal.Web.Commerce
{
    public abstract class WorldPayResponseHandlerProvider : ProviderBase
    {
        public abstract bool HandleRequest(
            WorldPayPaymentResponse wpResponse,
            PayPalLog worldPayLog,
            Page page); // we are piggy backing on the already existing paypal log rather than create a new table
    }
}