using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.Models;
using ClassLibrary.WcfModels;

namespace ArticleService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ArticleDto, Article>();
            CreateMap<Article, WcfArticle>();

            CreateMap<Article, ArticleDto>();
            CreateMap<WcfArticle, Article>();
        }
    }
}
