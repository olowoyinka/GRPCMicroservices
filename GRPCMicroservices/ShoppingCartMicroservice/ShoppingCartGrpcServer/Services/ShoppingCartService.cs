﻿using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ShoppingCartGrpcServer.Data;
using ShoppingCartGrpcServer.Models;
using ShoppingCartGrpcServer.Protos;

namespace ShoppingCartGrpcServer.Services;


[Authorize]
public class ShoppingCartService : ShoppingCartProtoService.ShoppingCartProtoServiceBase
{
    private readonly ShoppingCartContext _shoppingCartDbContext;
    private readonly DiscountService _discountService;
    private readonly IMapper _mapper;
    private readonly ILogger<ShoppingCartService> _logger;

    public ShoppingCartService(ShoppingCartContext shoppingCartDbContext, 
                                 DiscountService discountService, 
                                 IMapper mapper, 
                                 ILogger<ShoppingCartService> logger)
    {
        _shoppingCartDbContext = shoppingCartDbContext;
        _discountService = discountService;
        _mapper = mapper;
        _logger = logger;
    }


    public override async Task<ShoppingCartModel> GetShoppingCart(GetShoppingCartRequest request, ServerCallContext context)
    {
        var shoppingCart = await _shoppingCartDbContext.ShoppingCart.FirstOrDefaultAsync(s => s.UserName == request.Username);

        if (shoppingCart == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"ShoppingCart with UserName={request.Username} is not found."));
        }

        var shoppingCartModel = _mapper.Map<ShoppingCartModel>(shoppingCart);

        return shoppingCartModel;
    }


    public override async Task<ShoppingCartModel> CreateShoppingCart(ShoppingCartModel request, ServerCallContext context)
    {
        var shoppingCart = _mapper.Map<ShoppingCart>(request);

        var isExist = await _shoppingCartDbContext.ShoppingCart.AnyAsync(s => s.UserName == shoppingCart.UserName);

        if (isExist)
        {
            _logger.LogError("Invalid UserName for ShoppingCart creation. UserName : {userName}", shoppingCart.UserName);
            throw new RpcException(new Status(StatusCode.NotFound, $"ShoppingCart with UserName={request.Username} is already exist."));
        }

        _shoppingCartDbContext.ShoppingCart.Add(shoppingCart);
        await _shoppingCartDbContext.SaveChangesAsync();

        _logger.LogInformation("ShoppingCart is successfully created.UserName : {userName}", shoppingCart.UserName);

        var shoppingCartModel = _mapper.Map<ShoppingCartModel>(shoppingCart);

        return shoppingCartModel;
    }


    [AllowAnonymous]
    public override async Task<AddItemIntoShoppingCartResponse> AddItemIntoShoppingCart(IAsyncStreamReader<AddItemIntoShoppingCartRequest> requestStream, ServerCallContext context)
    {
        while (await requestStream.MoveNext())
        {
            var shoppingCart = await _shoppingCartDbContext.ShoppingCart.FirstOrDefaultAsync(s => s.UserName == requestStream.Current.Username);
            
            if (shoppingCart == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"ShoppingCart with UserName={requestStream.Current.Username} is not found."));
            }

            var newAddedCartItem = _mapper.Map<ShoppingCartItem>(requestStream.Current.NewCartItem);

            var cartItem = shoppingCart.Items.FirstOrDefault(i => i.ProductId == newAddedCartItem.ProductId);

            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                // GRPC CALL DISCOUNT SERVICE -- check discount and set the item price
                var discount = await _discountService.GetDiscount(requestStream.Current.DiscountCode);

                newAddedCartItem.Price -= discount.Amount;

                shoppingCart.Items.Add(newAddedCartItem);
            }
        }

        var insertCount = await _shoppingCartDbContext.SaveChangesAsync();

        var response = new AddItemIntoShoppingCartResponse
        {
            Success = insertCount > 0,
            InsertCount = insertCount
        };

        return response;
    }


    [AllowAnonymous]
    public override async Task<RemoveItemIntoShoppingCartResponse> RemoveItemIntoShoppingCart(RemoveItemIntoShoppingCartRequest request, ServerCallContext context)
    {
        var shoppingCart = await _shoppingCartDbContext.ShoppingCart.FirstOrDefaultAsync(s => s.UserName == request.Username);
        
        if (shoppingCart == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"ShoppingCart with UserName={request.Username} is not found."));
        }

        var removeCartItem = shoppingCart.Items.FirstOrDefault(i => i.ProductId == request.RemoveCartItem.ProductId);
        
        if (null == removeCartItem)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"CartItem with ProductId={request.RemoveCartItem.ProductId} is not found in the ShoppingCart."));
        }

        shoppingCart.Items.Remove(removeCartItem);

        var removeCount = await _shoppingCartDbContext.SaveChangesAsync();

        var response = new RemoveItemIntoShoppingCartResponse
        {
            Success = removeCount > 0
        };

        return response;
    }
}