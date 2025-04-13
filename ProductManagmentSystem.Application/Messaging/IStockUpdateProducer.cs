using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagmentSystem.Application.Messaging
{
    public interface IStockUpdateProducer
    {
        Task PublishStockUpdateAsync(int id, int stockQuantity);
    }
}
