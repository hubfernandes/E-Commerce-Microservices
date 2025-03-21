using AutoMapper;
using Payment.Application.Commands;
using Payment.Domain.Dtos;

namespace Payment.Application.Mappings
{
    internal class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Domain.Entities.Payment, PaymentDto>();
            CreateMap<CreatePaymentCommand, Domain.Entities.Payment>();
            CreateMap<UpdatePaymentStatusCommand, Domain.Entities.Payment>()
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.Amount, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentMethod, opt => opt.Ignore());
        }
    }
}
