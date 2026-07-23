using System;

namespace Pixyz
{
    public class PixyzException : Exception
    {
        public PixyzException(string message) : base(message) { }
    }

    public class NoValidLicenseException : PixyzException
    {
        public NoValidLicenseException() : base("The Pixyz Plugin for Unity requires a valid license.\nPlease install yours via the License Manager or visit www.pixyz-software.com to get one") { }
    }

    public class OutOfTermsException : PixyzException
    {
        public OutOfTermsException() : base("Using Pixyz Plugin for Unity to import 3D model is limited to manual imports via the GUI of the Unity 3D editor (General Terms & Conditions for Pixyz Software). \nIn order to import .pxz via CLI or at Runtime, use our Pixyz Loader for Unity instead (free).\nhttps://www.pixyz-software.com/documentations/html/2021.1/plugin4unity/LicensingPolicy.html") { }
    }

    public class CoreLoadingException : PixyzException
    {
        public CoreLoadingException() : base("Impossible to load Pixyz Core. Please close your project, delete the Plugins/Pixyz folder and reinstall. Please contact the support if the issue persists.") { }
    }
}