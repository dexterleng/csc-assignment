using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCTask7
{
    public class Config
    {
        public static String ClarifaiKey = "";
        // this is the general model id used a placeholder.
        // you are recommended to replace it with a custom trained receipt model
        public static String ClarifaiReceiptModelId = "aaa03c23b3724a16a56b629203edc62c";
        public static String AzureKey = "";
        public static String AzureEndpoint = "";
    }
}