using System.Runtime.CompilerServices;

namespace FinansoData.Helpers
{
    public static class MethodName
    {
        /// <summary>
        /// Get the name of the method that calls this method
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static string GetMethodName([CallerMemberName] string methodName = "")
        {
            return methodName;
        }
    }
}
