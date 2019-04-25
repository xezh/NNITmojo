﻿//  Author:                     
//  Created:                    2008-06-27
//	Last Modified:              2008-06-27
// 
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software.

using System;
using System.Configuration.Provider;
using System.Web.Configuration;
using log4net;

namespace mojoPortal.Business.WebHelpers.PageEventHandlers
{
    /// <summary>
    ///  
    /// </summary>
    public sealed class PageCreatedEventHandlerPoviderManager
    {
        private static object initializationLock = new object();
        private static readonly ILog log
            = LogManager.GetLogger(typeof(PageCreatedEventHandlerPoviderManager));

        static PageCreatedEventHandlerPoviderManager()
        {
            Initialize();
        }

        private static void Initialize()
        {
            providerCollection = new PageCreatedEventHandlerPoviderCollection();

            try
            {
                PageCreatedEventHandlerPoviderConfig config
                    = PageCreatedEventHandlerPoviderConfig.GetConfig();

                if (config != null)
                {

                    if (
                        (config.Providers == null)
                        || (config.Providers.Count < 1)
                        )
                    {
                        throw new ProviderException("No PageCreatedEventHandlerPoviderCollection found.");
                    }

                    ProvidersHelper.InstantiateProviders(
                        config.Providers,
                        providerCollection,
                        typeof(PageCreatedEventHandlerPovider));

                }
                else
                {
                    // config was null, not a good thing
                    log.Error("PayPalReturnHandlerConfig could not be loaded so empty provider collection was returned");

                }
            }
            catch (NullReferenceException ex)
            {
                log.Error(ex);
            }
            catch (TypeInitializationException ex)
            {
                log.Error(ex);
            }
            catch (ProviderException ex)
            {
                log.Error(ex);
            }
            
            providerCollection.SetReadOnly();
                
            
        }


        private static PageCreatedEventHandlerPoviderCollection providerCollection;

        public static PageCreatedEventHandlerPoviderCollection Providers
        {
            get
            {
                if (providerCollection == null) Initialize();
                return providerCollection;
                
            }
        }


    }
}
