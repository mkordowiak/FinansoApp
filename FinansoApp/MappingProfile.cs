using AutoMapper;
using FinansoApp.ViewModels;
using FinansoData.DataViewModel.Group;

namespace FinansoApp
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GetGroupMembersViewModel, GroupMembersViewModel>();
            
        }
    }
}
