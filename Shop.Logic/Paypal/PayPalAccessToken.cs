using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Logic.Paypal
{
    public class PayPalAccessToken
    {
        public string Scope { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string app_id { get; set; }
        public string expire_in { get; set; }
        public string nonce { get; set; }
    }
}
