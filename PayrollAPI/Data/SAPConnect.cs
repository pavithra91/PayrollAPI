using SAP.Middleware.Connector;

namespace PayrollAPI.Data
{
    public class SAPConnect : IDestinationConfiguration
    {
        public RfcConfigParameters GetParameters(String destinationName)
        {
            if ("ZRFC_TEST".Equals(destinationName))
            {
                RfcConfigParameters parms = new RfcConfigParameters();
                parms.Add(RfcConfigParameters.AppServerHost, "10.10.50.51");
                parms.Add(RfcConfigParameters.SystemNumber, "00");
                parms.Add(RfcConfigParameters.User, "TESTRFC");
                parms.Add(RfcConfigParameters.Password, "707602");
                parms.Add(RfcConfigParameters.Client, "120");
                parms.Add(RfcConfigParameters.Language, "EN");
                parms.Add(RfcConfigParameters.PoolSize, "5");
                return parms;
            }
            else
            {
                return null;
            }
        }
        // The following two are not used in this example:
        public bool ChangeEventsSupported()
        {
            return false;
        }
        public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;
    }
    }
