using System.Reflection;

namespace FinansoApp.Helpers
{
    public class ErrorInfo
    {
        public bool IsError()
        {
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);


            // Here we want to have simplicity,
            // so we use crude and simple foreach loop, instance of linq
            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType.Name != "Boolean")
                {
                    continue;
                }

                if ((bool)property.GetValue(this) == true)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
