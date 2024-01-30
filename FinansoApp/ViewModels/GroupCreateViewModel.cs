namespace FinansoApp.ViewModels
{
    public class GroupCreateViewModel
    {
        public string Name { get; set; }

        public GroupCreateViewModelErrorInfo Error { get;  } = new GroupCreateViewModelErrorInfo();

        public class GroupCreateViewModelErrorInfo : FinansoData.Helpers.ErrorInfo
        {
            public bool MaxGroupsLimitReached { get; set; }
            public bool InternalError { get; set; }
        }
    }
}
