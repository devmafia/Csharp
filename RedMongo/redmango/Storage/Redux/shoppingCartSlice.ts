import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import shoppingCartApi from '../../Apis/shoppingCartApi';
import cartItemModel from "../../Interfaces/cartItemModel";

interface ShoppingCartState {
    cartItems: cartItemModel[];
  }
  
const initialState: ShoppingCartState = {
    cartItems: [],
};

const shoppingCartSlice = createSlice({
    name: "cartItems",
    initialState: initialState,
    reducers: {
    setShoppingCart: (state, action: PayloadAction<cartItemModel[]>) => {
      state.cartItems = action.payload;
    },
    updateQuantity: (
      state,
      action: PayloadAction<{ cartItemId: number; quantity: number }>
    ) => {
      const item = state.cartItems.find(i => i.menuItemId === action.payload.cartItemId);
      if (item) {
        item.quantity = action.payload.quantity;
      }
    },
    removeFromCart: (state, action: PayloadAction<number>) => {
      state.cartItems = state.cartItems.filter(
        i => i.menuItemId !== action.payload
      );
    },
  },
});

export const { setShoppingCart, updateQuantity, removeFromCart } = shoppingCartSlice.actions;

export const shoppingCartReducer = shoppingCartSlice.reducer;
