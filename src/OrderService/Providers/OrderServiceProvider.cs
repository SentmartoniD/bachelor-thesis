using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClassLibrary.WcfModels;
using OrderService.InterFaces;
using OrderService.DBContext;
using System.ServiceModel;
using UserService.Interfaces;
using OrderService.Mapper;
using ArticleService.Interfaces;
using ClassLibrary.Models;
using System.Text.Json;

namespace OrderService.Providers
{
    public class OrderServiceProvider : IWcfOrderService
    {
        private readonly IMapper _mapper;
        private readonly OrderDBContext _dbContext;
        private readonly ChannelFactory<IWcfInternalUserService> factoryUser;
        private readonly IWcfInternalUserService kanalUser;
        private readonly ChannelFactory<IWcfInternalArticleService> factoryArticle;
        private readonly IWcfInternalArticleService kanalArticle;

        public OrderServiceProvider()
        {
            MapperConfiguration config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();
            _dbContext = new OrderDBContext();
            factoryUser = new ChannelFactory<IWcfInternalUserService>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:4000/IWcfInternalUserService"));
            kanalUser = factoryUser.CreateChannel();
            factoryArticle = new ChannelFactory<IWcfInternalArticleService>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:4011/IWcfInternalArticleService"));
            kanalArticle = factoryArticle.CreateChannel();
        }

        public OrderDto AddOrder(string email, OrderDto orderDto)
        {
            WcfUser user = kanalUser.GetUserByEmail(email);

            Order order = _mapper.Map<Order>(orderDto);
            order.UserBuyerId = user.UserId;
            order.DateOfOrder = DateTime.Now.ToString();
            // order.ArticleIdsString = string.Join(",", order.ArticleIds);
            // order.AmountOfArticlesString = string.Join(",", order.ArticleIds);

            _dbContext.Orders.Add(order);

            kanalArticle.UpdateArticleQuantities(order.ArticleIds.ToList(), order.AmountOfArticles.ToList(), false);

            _dbContext.SaveChanges();

            return _mapper.Map<OrderDto>(order);
        }

        public void CanceleOrder(int id)
        {
            Order order = _dbContext.Orders.FirstOrDefault(X => X.OrderId == id);
            order.DateOfOrder = "canceled";
            _dbContext.SaveChanges();

            kanalArticle.UpdateArticleQuantities(order.ArticleIds.ToList(), order.AmountOfArticles.ToList(), true);
        }

        public List<OrderDto> GetAllCurrentOrdersForSeller(string email)
        {
            WcfUser user = kanalUser.GetUserByEmail(email);

            List<Order> orders = _dbContext.Orders.ToList();

            List<WcfArticle> articles = kanalArticle.GetArticleForUserSeller(user.UserId);

            List<Order> ordersForSeller = new List<Order>();

            Order tempOrder = new Order();

            foreach (Order o in orders)
            {

                if (o.DateOfOrder == "canceled")
                    continue;
                TimeSpan timeDifference = DateTime.Now - DateTime.Parse(o.DateOfOrder);
                if (timeDifference.TotalMinutes >= 2)
                    continue;

                string jsonString = JsonSerializer.Serialize(o);
                tempOrder = JsonSerializer.Deserialize<Order>(jsonString);

                tempOrder.ArticleIds = new List<int>();
                for (int i = 0; i < o.ArticleIds.Count; i++)
                {
                    foreach (WcfArticle a in articles)
                    {
                        if (o.ArticleIds.ToList()[i] == a.ArticleId)
                        {
                            tempOrder.ArticleIds.Add(a.ArticleId);
                            break;
                        }
                    }
                }
                if (tempOrder.ArticleIds.Count != 0)
                    ordersForSeller.Add(tempOrder);
            }


            return _mapper.Map<List<OrderDto>>(ordersForSeller);
        }

        public List<OrderDto> GetAllOrders()
        {
            return _mapper.Map<List<OrderDto>>(_dbContext.Orders.ToList());
        }

        public List<OrderDto> GetAllPastOrdersForSeller(string email)
        {
            WcfUser user = kanalUser.GetUserByEmail(email);

            List<Order> orders = _dbContext.Orders.ToList();

            List<WcfArticle> articles = kanalArticle.GetArticleForUserSeller(user.UserId);

            List<Order> ordersForSeller = new List<Order>();

            Order tempOrder = new Order();

            foreach (Order o in orders)
            {
                if (o.DateOfOrder == "canceled")
                    continue;
                else
                {
                    TimeSpan timeDifference = DateTime.Now - DateTime.Parse(o.DateOfOrder);
                    if (timeDifference.TotalMinutes < 2)
                        continue;
                }

                string jsonString = JsonSerializer.Serialize(o);
                tempOrder = JsonSerializer.Deserialize<Order>(jsonString);

                tempOrder.ArticleIds = new List<int>();
                for (int i = 0; i < o.ArticleIds.Count; i++)
                {
                    foreach (WcfArticle a in articles)
                    {
                        if (o.ArticleIds.ToList()[i] == a.ArticleId)
                        {
                            tempOrder.ArticleIds.Add(a.ArticleId);
                            break;
                        }
                    }
                }
                if (tempOrder.ArticleIds.Count != 0)
                    ordersForSeller.Add(tempOrder);
            }


            return _mapper.Map<List<OrderDto>>(ordersForSeller);
        }

        public Tuple<List<OrderDto>, List<OrderDto>> GetCurrentAndPastOrdersForBuyer(string email)
        {
            WcfUser user = kanalUser.GetUserByEmail(email);

            List<Order> orders = _dbContext.Orders.Where(x => x.UserBuyerId == user.UserId).ToList();
            List<Order> currentOrders = new List<Order>();
            List<Order> pastOrders = new List<Order>();

            foreach (Order o in orders)
            {
                if (o.DateOfOrder != "canceled")
                {
                    TimeSpan timeDifference = DateTime.Now - DateTime.Parse(o.DateOfOrder);
                    if (timeDifference.TotalMinutes < 2)
                        currentOrders.Add(o);
                    else
                        pastOrders.Add(o);
                }
            }

            List<OrderDto> currentOrdersDto = _mapper.Map<List<OrderDto>>(currentOrders);
            List<OrderDto> pasttOrdersDto = _mapper.Map<List<OrderDto>>(pastOrders);

            return new Tuple<List<OrderDto>, List<OrderDto>>(currentOrdersDto, pasttOrdersDto);
        }

        public OrderDetailsDto GetOrderDetails(int id, string email)
        {
            WcfUser user = kanalUser.GetUserByEmail(email);

            Order order = _dbContext.Orders.FirstOrDefault(u => u.OrderId == id);

            List<WcfArticle> tempArticles = kanalArticle.GetArticlesByIds(order.ArticleIds.ToList());

            OrderDetailsDto orderDetailsDto = new OrderDetailsDto();
            orderDetailsDto.Address = order.Address; orderDetailsDto.Comment = order.Comment; orderDetailsDto.Price = order.Price; orderDetailsDto.DateOfOrder = order.DateOfOrder;
            orderDetailsDto.AmountOfArticles = new List<int>(); orderDetailsDto.Articles = new List<WcfArticle>(); orderDetailsDto.OrderId = order.OrderId;

            Article tempArticle = new Article();

            if (user.UserType == "admin" || user.UserType == "buyer")
            {
                for (int i = 0; i < order.ArticleIds.ToList().Count; i++)
                {
                    orderDetailsDto.Articles.Add(tempArticles[i]);
                    orderDetailsDto.AmountOfArticles.Add(order.AmountOfArticles.ToList()[i]);
                }
            }
            else
            {
                List<WcfArticle> articles = kanalArticle.GetArticleForUserSeller(user.UserId);
                for (int i = 0; i < order.ArticleIds.ToList().Count; i++)
                {
                    foreach (WcfArticle a in articles)
                    {
                        if (order.ArticleIds.ToList()[i] == a.ArticleId)
                        {
                            orderDetailsDto.Articles.Add(a);
                            orderDetailsDto.AmountOfArticles.Add(order.AmountOfArticles.ToList()[i]);
                            break;
                        }
                    }
                }
            }

            return orderDetailsDto;
        }
    }
}
