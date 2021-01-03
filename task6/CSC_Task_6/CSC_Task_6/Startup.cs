using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(CSC_Task_6.Startup))]

namespace CSC_Task_6
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            Stripe.StripeConfiguration.ApiKey = Config.StripeSecretKey;
        }
    }
}
