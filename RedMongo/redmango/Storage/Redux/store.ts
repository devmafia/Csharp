import { configureStore } from "@reduxjs/toolkit";
import { menuItemReducer } from "./menuItemSlice";
import menuItemApi from "../../Apis/menuItemApi";
import shoppingCartApi from "../../Apis/shoppingCartApi";
import { shoppingCartReducer } from "./shoppingCartSlice";
import authApi from "../../Apis/authApi";
import { userAuthReducer } from "./userAuthSlice";
import paymentApi from "@/Apis/paymentApi";
import orderApi from "@/Apis/orderApi";

const store = configureStore({
    reducer: {
        shoppingCartStore: shoppingCartReducer,
        menuItemStore: menuItemReducer,
        userAuthStore: userAuthReducer,
        [orderApi.reducerPath]: orderApi.reducer,
        [authApi.reducerPath]: authApi.reducer,
        [menuItemApi.reducerPath]: menuItemApi.reducer,
        [shoppingCartApi.reducerPath]: shoppingCartApi.reducer,
        [paymentApi.reducerPath]: paymentApi.reducer
    },
    middleware: (getDefaultMiddleware) => 
        getDefaultMiddleware().concat(menuItemApi.middleware).concat(shoppingCartApi.middleware).concat(authApi.middleware)
    .concat(paymentApi.middleware).concat(orderApi.middleware)
})

export type RootState = ReturnType<typeof store.getState>;

export default store;
