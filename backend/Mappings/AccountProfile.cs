using AutoMapper;
using Student_management.DTOs.Account;
using Student_management.Enum;

namespace Student_management.Mappings
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountDto>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.AccountID))
                .ForMember(dest => dest.RoleID, otp => otp.MapFrom(src => src.RoleID.ToString()))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.RoleName : null));

            CreateMap<CreateAccount, Account>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email != null ? src.Email.Trim() : null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (byte)AccountStatus.Active))
             
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.AccountID, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());
        }
    }
}
