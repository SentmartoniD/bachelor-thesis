using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticleService.Interfaces;
using ClassLibrary.WcfModels;
using ArticleService.DBContext;
using ArticleService.Mapping;
using AutoMapper;
using ClassLibrary.Models;

namespace ArticleService.Providers
{
    public class InternalArticleServiceProvider : IWcfInternalArticleService
    {
        private readonly ArticleDBContext _dbContext;
        private readonly IMapper _mapper;

        public InternalArticleServiceProvider()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();
            _dbContext = new ArticleDBContext();
        }

        public List<WcfArticle> GetArticleForUserSeller(int id)
        {
            return _mapper.Map<List<WcfArticle>>(_dbContext.Articles.Where(x => x.UserSellerId == id).ToList());
        }

        public List<WcfArticle> GetArticlesByIds(List<int> articleIds)
        {
            List<Article> articles = _dbContext.Articles.ToList();
            List<Article> temp = new List<Article>();
            int counter = 0;
            for (int i = 0; i < articles.Count; i++)
            {
                if (articles[i].ArticleId == articleIds[counter])
                {
                    temp.Add(articles[i]);
                    counter++;
                    continue;
                }
            }

            return _mapper.Map<List<WcfArticle>>(temp);
        }

        public void UpdateArticleQuantities(List<int> articleIds, List<int> articleAmounts, bool increase)
        {
            List<Article> articles = _dbContext.Articles.ToList();
            int counter = 0;
            for (int i = 0; i < articles.Count; i++)
            {
                if (articles[i].ArticleId == articleIds[counter])
                {
                    articles[i].Quantity = increase ? articles[i].Quantity + articleAmounts[counter] : articles[i].Quantity - articleAmounts[counter];
                    counter++;
                    continue;
                }
            }
            _dbContext.SaveChanges();
        }
    }
}
