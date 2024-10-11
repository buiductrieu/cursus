using AutoMapper;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class CartItemsService : ICartItemsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartItemsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> DeleteCart(int id)
        {
            var OldCartItem= await _unitOfWork.CartItemsRepository.GetItemByID(id);
            if (OldCartItem == null)
            {
                return false;
            }
            return await _unitOfWork.CartItemsRepository.DeleteCartItems(OldCartItem);
        }

        public async Task<IEnumerable<CartItems>> GetAllCartItems()
        {
            return await _unitOfWork.CartItemsRepository.GetAllItems();
        }

        public async Task<CartItems> UpdateCartItems(CartItems cartItems, int id)
        {
            var OldCartItem = await _unitOfWork.CartItemsRepository.GetItemByID(id);
            if (OldCartItem == null)
            {
                throw new Exception("Cart Item not found");
            }
            OldCartItem.Course= cartItems.Course;
            OldCartItem.Price= cartItems.Price;
            return await _unitOfWork.CartItemsRepository.UpdateCartItems(OldCartItem);
        }
    }
}
