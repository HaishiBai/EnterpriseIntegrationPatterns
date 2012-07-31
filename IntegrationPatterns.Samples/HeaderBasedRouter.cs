using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationPatterns.Routers;

namespace IntegrationPatterns.Samples.Routers
{
    public class HeaderBasedRouter: DynamicRouter
    {
        protected override int PickOutputChannel(Infrastructure.ChannelMessageEventArgs e)
        {
            if (e.Message.Headers.ContainsKey("ProductType"))
                return (string)e.Message.Headers["ProductType"] == "Cars" ? 0 : 1;
            else
                return base.PickOutputChannel(e);
        }
    }
}
