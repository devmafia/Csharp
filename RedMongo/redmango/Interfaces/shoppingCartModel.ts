import cartItemModel from "./cartItemModel";

export interface shoppingCartModel {
    id: number;
    userId: number;
    cartItems: cartItemModel[];
    cartTotal: number;
    wayForPayPaymentIntentId?: any;
    clientSecret: any;
}
