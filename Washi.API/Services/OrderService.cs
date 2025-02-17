﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Washi.API.Domain.Models;
using Washi.API.Domain.Repositories;
using Washi.API.Domain.Services;
using Washi.API.Domain.Services.Communications;

namespace Washi.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OrderResponse> DeleteAsync(int id)
        {
            var existingOrder = await _orderRepository.FindById(id);
            if (existingOrder == null)
                return new OrderResponse("Order not found");
            try
            {
                _orderRepository.Remove(existingOrder);
                await _unitOfWork.CompleteAsync();

                return new OrderResponse(existingOrder);
            }
            catch (Exception ex)
            {
                return new OrderResponse($"An error ocurred while deleting order: {ex.Message}");
            }
        }

        public async Task<OrderResponse> FindByOrderId(int orderId)
        {
            var existingOrder = await _orderRepository.FindById(orderId);

            if (existingOrder == null)
                return new OrderResponse("Order not found");
            return new OrderResponse(existingOrder);
        }

        public async Task<IEnumerable<Order>> ListAsync()
        {
            return await _orderRepository.ListAsync();
        }
        public async Task<IEnumerable<Order>> ListByUserId(int userId)
        {
            return await _orderRepository.ListByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Order>> ListByUserIdAndOrderStatusId(int userId, int orderStatusId)
        {
            return await _orderRepository.ListByUserIdAndOrderStatusIdAsync(userId, orderStatusId);
        }

        public async Task<OrderResponse> SaveAsync(Order order)
        {
            try
            {
                await _orderRepository.AddAsync(order);
                await _unitOfWork.CompleteAsync();
                return new OrderResponse(order);
            }
            catch (Exception ex)
            {
                return new OrderResponse($"An error ocurred while saving the order: {ex.Message}");
            }
        }

        public async Task<OrderResponse> UpdateAsync(int id, Order order)
        {
            var existingOrder = await _orderRepository.FindById(id);

            if (existingOrder == null)
                return new OrderResponse("Order not found");

            existingOrder.DeliveryAddress = order.DeliveryAddress;
            existingOrder.OrderAmount = order.OrderAmount;
            existingOrder.DeliveryDate = order.DeliveryDate;

            try
            {
                _orderRepository.Update(existingOrder);
                await _unitOfWork.CompleteAsync();

                return new OrderResponse(existingOrder);
            }
            catch (Exception ex)
            {
                return new OrderResponse($"An error ocurred while updating Order: {ex.Message}");
            }
        }

    }
}