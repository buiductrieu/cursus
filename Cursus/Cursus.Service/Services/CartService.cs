using AutoMapper;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> DeleteCart(int id)
        {
           var ExCart = await _unitOfWork.CartRepository.GetCartByID(id);
            if (ExCart == null)
            {
                return false;
            }
            return await _unitOfWork.CartRepository.DeleteCart(ExCart);
        }

        public Task<IEnumerable<Cart>> GetAllCart()
        {
            return _unitOfWork.CartRepository.GetCart();
        }

        public Task<Cart> GetCartByID(int cartId)
        {
            return _unitOfWork.CartRepository.GetCartByID(cartId);
        }

    }
}
