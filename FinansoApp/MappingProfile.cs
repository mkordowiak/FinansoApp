using AutoMapper;
using FinansoApp.ViewModels;
using FinansoApp.ViewModels.Transaction;
using FinansoData;
using FinansoData.DataViewModel.Group;

namespace FinansoApp
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GetGroupMembersViewModel, GroupMembersViewModel>()
                .ForMember(dest => dest.GroupUserId, opt => opt.MapFrom(src => src.Id));

            CreateMap<DeleteGroupUserViewModel, ConfirmGroupUserDeleteViewModel>();
            


            CreateMap<FinansoData.DataViewModel.Transaction.GetTransactionsForUser, TransactionViewModel>();
            CreateMap<FinansoData.RepositoryResult<IEnumerable<FinansoData.DataViewModel.Transaction.GetTransactionsForUser>>, TransactionListViewModel>()
                .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.CurrentPage, opt => opt.Ignore());

        }
    }
}
