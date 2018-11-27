using System;
using System.Collections.Generic;
using System.Text;

namespace SCUScanner.Helpers
{
   public static class GlobalConstants
    {
        public static string MAIN_TAB_PAGE = "MainTabPage";
        public static string UUID_MLDP_PRIVATE_SERVICE = "00035b03-58e6-07dd-021a-08123a000300";
        public static string UUID_MLDP_DATA_PRIVATE_CHAR = "00035b03-58e6-07dd-021a-08123a000301";
        
        public static string UUID_TANSPARENT_PRIVATE_SERVICE ="49535343-fe7d-4ae5-8fa9-9fafd205e455";
        public static string UUID_TRANSPARENT_TX_PRIVATE_CHAR ="49535343-1e4d-4bd9-ba61-23c647249616"; //Characteristic for Transparent Data from BM module, properties - notify, write, write no response
        public  static string UUID_TRANSPARENT_RX_PRIVATE_CHAR = "49535343-8841-43f4-a8d4-ecbe34729bb3"; //Characteristic for Transparent Data to BM module, properties - write, write no response

        public static int WaitingForReconnecting { get; internal set; } = 10000;
        public const string FtpHost = "ftp://35.227.40.251";
        public const int FtpPort = 21;
        //public const string FtpHost = "ftp://ftp.chester.ru";

        public const bool FactoryMode = false;
        //if this property is empty, then the language is set from location of the phone location
        public const string HardDefaultLang = "en";
        public const string PrivacyPolicyLink="http://chester-sw.com/privacy-policy-centriclean.html";
    }

}
