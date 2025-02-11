"use client";

import styles from "./page.module.css";
import { useSelector, useDispatch } from "react-redux";
import cartItemModel from "../../Interfaces/cartItemModel"
import { RootState } from "../../Storage/Redux/store";
import { removeFromCart, setShoppingCart, updateQuantity } from "../../Storage/Redux/shoppingCartSlice";
import CartPickUpDetails from '../../Components/CartPickUpDetails';
import withAuth from '../../HOC/withAuth';
import userModel from "@/Interfaces/userModel";
import { useGetShoppingCartQuery, useUpdateShoppingCartMutation } from "@/Apis/shoppingCartApi";

function ShoppingCart() {
    const shoppingCartFromStore: cartItemModel[] = useSelector(
      (state: RootState) => state.shoppingCartStore.cartItems ?? []
    );
    const [updateShoppingCart] = useUpdateShoppingCartMutation()
    const dispatch = useDispatch();

    const userData: userModel = useSelector((state: RootState) => state.userAuthStore);

    const { refetch } = useGetShoppingCartQuery(userData.id, {skip:! userData.id });

    const handleQuantity = async (updateQuantityBy: number, cartItem: cartItemModel) => {
      const matchingCartItem = shoppingCartFromStore.find(item => item.menuItemId === cartItem.menuItemId);
  
      if (!matchingCartItem) {
          console.error("Cart item not found in the store");
          return;
      }
  
      const newQuantity = matchingCartItem.quantity + updateQuantityBy;
  
      if (updateQuantityBy === 0 || newQuantity <= 0) {
          // Remove the item completely
          dispatch(removeFromCart(matchingCartItem.menuItemId));
          await updateShoppingCart({
              menuItemId: matchingCartItem.menuItemId,
              updateQuantityBy: 0, // Indicates removal
              userId: userData.id,
          });
      } else {
          // Update the item quantity
          dispatch(updateQuantity({ cartItemId: matchingCartItem.menuItemId, quantity: newQuantity }));
          await updateShoppingCart({
              menuItemId: matchingCartItem.menuItemId,
              updateQuantityBy,
              userId: userData.id,
          });
      }
  
      refetch(); // Refresh the cart
  };
  
    return (
      <div className={styles.page}>
        <div className="flex justify-between row w-100">
          <div className="w-4/6 col-lg-6 col-12">
            <div className="container p-4 m-2 bg-white rounded-lg shadow-lg">
              <h4 className="text-center text-success font-bold mb-4">
                Cart Summary
              </h4>
              {shoppingCartFromStore.length > 0 ? (
                shoppingCartFromStore.map((item, index) => (
                  <div
                    key={index}
                    className="flex flex-sm-row flex-column align-items-center border-b py-4"
                  >
                    <div className="p-3">
                      <img
                        style={{ borderRadius: "50%", width: "100px", height: "100px", objectFit: "cover" }}
                        src={item.menuItem.image || "https://via.placeholder.com/100"}
                        alt={item.menuItem.name}
                      />
                    </div>
  
                    <div className="p-2 mx-3 w-full">
                      <div className="flex justify-between items-center mb-2">
                        <h5 className="font-semibold">{item.menuItem.name}</h5>
                        <h5 className="font-semibold text-gray-600">{item.menuItem.price} UAH</h5>
                      </div>
                      <div className="flex items-center justify-between w-100">
                        <div
                          className="flex items-center justify-between p-2 border rounded-lg"
                          style={{ width: "120px" }}
                        >
                          <span onClick={() => handleQuantity(-1, item)}
                            role="button"
                            className="text-xl font-semibold cursor-pointer px-2"
                          >
                            -
                          </span>
                          <span>
                            <b>{item.quantity}</b>
                          </span>
                          <span onClick={() => handleQuantity(1, item)}
                            role="button"
                            className="text-xl font-semibold cursor-pointer px-2"
                          >
                            +
                          </span>
                        </div>
                        <span className="text-red-600 hover:underline font-medium ml-4" onClick={() => handleQuantity(0, item)}>
                          Remove
                        </span>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <p className="text-center text-gray-500">Your cart is empty.</p>
              )}
            </div>
          </div>
  
          <div className="w-4/12 col-lg-6 col-12 p-4">
            <div className="container bg-white p-4 rounded-lg shadow-lg">
              <CartPickUpDetails></CartPickUpDetails>
            </div>
          </div>
        </div>
      </div>
    );
  }
  
  export default withAuth(ShoppingCart);
