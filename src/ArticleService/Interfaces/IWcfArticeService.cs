using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.WcfModels;

namespace ArticleService.Interfaces
{
    [ServiceContract]
    public interface IWcfArticeService
    {
        [OperationContract]
        ArticleDto AddArticle(string email, ArticleDto articleDto);
        [OperationContract]
        List<ArticleDto> GettArticles();
        [OperationContract]
        List<ArticleDto> GetArticlesByEmailForSeller(string email);
        [OperationContract]
        void DeleteArticleById(int id);
        [OperationContract]
        ArticleDto UpdateArticle(ArticleDto articleDto);
        [OperationContract]
        List<ArticleDto> GettArticlesForCart(List<int> articleIds);
        [OperationContract]
        bool UploadImage(MemoryStream ms, int id);
    }
}
