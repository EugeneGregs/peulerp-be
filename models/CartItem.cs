//---------------------------------------------------------------------
// <copyright file="CartItem.cs" company="PEUL Africa">
//   Copyright (c) PEUL Africa. All rights reserved.
// </copyright>
//--------------------------------------------------------------------
namespace PeyulErp.Models
{
    public record CartItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}