﻿using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using ProductGrpc.Protos;

namespace ProductGrpc.Mapper
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Models.Product, ProductModel>()
                .ForMember(dest => dest.CreatedTime,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(DateTime.SpecifyKind(src.CreatedTime, DateTimeKind.Utc))));

            CreateMap<ProductModel, Models.Product>()
                .ForMember(dest => dest.CreatedTime,
                opt => opt.MapFrom(src => src.CreatedTime.ToDateTime()));
        }

    }
}
