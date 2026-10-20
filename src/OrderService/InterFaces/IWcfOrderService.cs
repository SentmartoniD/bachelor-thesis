using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.WcfModels;

namespace OrderService.InterFaces
{
    [ServiceContract]
    public interface IWcfOrderService
    {
        [OperationContract]
        OrderDto AddOrder(string email, OrderDto orderDto);
        [OperationContract]
        List<OrderDto> GetAllOrders();
        [OperationContract]
        List<OrderDto> GetAllCurrentOrdersForSeller(string email);
        [OperationContract]
        List<OrderDto> GetAllPastOrdersForSeller(string email);
        [OperationContract]
        Tuple<List<OrderDto>, List<OrderDto>> GetCurrentAndPastOrdersForBuyer(string email);
        [OperationContract]
        void CanceleOrder(int id);
        [OperationContract]
        OrderDetailsDto GetOrderDetails(int id, string email);
    }
}
