using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.WcfModels;

namespace ArticleService.Interfaces
{
    [ServiceContract]
    public interface IWcfInternalArticleService
    {
        [OperationContract]
        List<WcfArticle> GetArticleForUserSeller(int id);

        [OperationContract]
        List<WcfArticle> GetArticlesByIds(List<int> articleIds);

        [OperationContract]
        void UpdateArticleQuantities(List<int> articleIds, List<int> articleAmounts, bool increase);
    }
}
