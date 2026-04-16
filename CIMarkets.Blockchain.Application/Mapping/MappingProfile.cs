/*
 * File: MappingProfile.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: AutoMapper profile for entity to DTO mapping
 * 
 * Usage: Defines mappings between domain entities and DTOs.
 *        Automatically discovered by AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()).
 *        Used in command handlers and query handlers for response transformation.
 *        Converts PaymentTransaction entities to PaymentTransactionDto responses.
 * 
 * Dependencies: IMapper, AutoMapper.Profile, PaymentTransaction, PaymentTransactionDto
 */

using AutoMapper;
using CIMarkets.Blockchain.Application.DTOs;
using CIMarkets.Blockchain.Domain.Entities;

namespace CIMarkets.Blockchain.Application.Mapping
{
    /// <summary>
    /// AutoMapper profile for entity to DTO mappings.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes AutoMapper mappings.
        /// </summary>
        public MappingProfile()
        {
            // PaymentTransaction to PaymentTransactionDto
            CreateMap<PaymentTransaction, PaymentTransactionDto>()
                .ForMember(dest => dest.StatusName, opt => opt.Ignore()); // StatusName is set manually in handler
        }
    }
}
