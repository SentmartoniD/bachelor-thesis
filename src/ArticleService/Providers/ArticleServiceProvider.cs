using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticleService.Interfaces;
using ClassLibrary.WcfModels;
using ArticleService.DBContext;
using System.ServiceModel;
using UserService.Interfaces;
using ClassLibrary.Models;
using AutoMapper;
using ArticleService.Mapping;

namespace ArticleService.Providers
{
    public class ArticleServiceProvider : IWcfArticeService
    {
        private readonly IMapper _mapper;
        private readonly ArticleDBContext _dbContext;
        private readonly ChannelFactory<IWcfInternalUserService> factory;
        private readonly IWcfInternalUserService kanal;

        public ArticleServiceProvider()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();
            _dbContext = new ArticleDBContext();
            factory = new ChannelFactory<IWcfInternalUserService>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:4000/IWcfInternalUserService"));
            kanal = factory.CreateChannel();
        }

        public ArticleDto AddArticle(string email, ArticleDto articleDto)
        {
            WcfUser user = kanal.GetUserByEmail(email);

            Article article = _mapper.Map<Article>(articleDto);
            article.UserSellerId = user.UserId;

            _dbContext.Articles.Add(article);
            _dbContext.SaveChanges();

            return _mapper.Map<ArticleDto>(article);
        }

        public void DeleteArticleById(int id)
        {
            Article article = _dbContext.Articles.Find(id);

            _dbContext.Articles.Remove(article);

            _dbContext.SaveChanges();
        }

        public List<ArticleDto> GetArticlesByEmailForSeller(string email)
        {
            WcfUser user = kanal.GetUserByEmail(email);
            List<ArticleDto> articleDtos = new List<ArticleDto>();
            List<Article> articles = new List<Article>();

            foreach (Article arc in articles)
            {
                if (arc.UserSellerId == user.UserId)
                    articleDtos.Add(_mapper.Map<ArticleDto>(arc));
            }

            return articleDtos;
        }

        public List<ArticleDto> GettArticles()
        {
            return _mapper.Map<List<ArticleDto>>(_dbContext.Articles.ToList());
        }

        public List<ArticleDto> GettArticlesForCart(List<int> articleIds)
        {
            List<ArticleDto> articleDtos = new List<ArticleDto>();

            foreach (int id in articleIds)
            {
                articleDtos.Add(_mapper.Map<ArticleDto>(_dbContext.Articles.Find(id)));
            }

            return articleDtos;
        }

        public ArticleDto UpdateArticle(ArticleDto articleDto)
        {
            Article article = _dbContext.Articles.Find(articleDto.ArticleId);

            if (articleDto.Name != null && articleDto.Name != string.Empty)
                article.Name = articleDto.Name;
            if (articleDto.Price != 0)
                article.Price = articleDto.Price;
            if (articleDto.Description != null && articleDto.Description != string.Empty)
                article.Description = articleDto.Description;
            if (articleDto.Quantity != 0)
                article.Quantity = articleDto.Quantity;
            if (articleDto.ImageFile != null)
                article.ImageFile = articleDto.ImageFile;

            _dbContext.SaveChanges();

            return _mapper.Map<ArticleDto>(article);
        }

        public bool UploadImage(MemoryStream ms, int id)
        {
            bool rez = true;
            if (ms != null)
            {
                Article article = _dbContext.Articles.FirstOrDefault(u => u.ArticleId == id);

                using (ms)
                {
                    article.ImageFile = ms.ToArray();
                }

                _dbContext.SaveChanges();
            }

            return rez;
        }
    }
}
