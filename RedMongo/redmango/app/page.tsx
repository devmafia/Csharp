"use client";

import styles from "./page.module.css";
import { useState, useEffect } from "react";
import { menuItemModel } from "./../Interfaces/menuItemModel";
import Link from "next/link";
import { setMenuItem } from "./../Storage/Redux/menuItemSlice";
import { useDispatch, useSelector } from "react-redux";
import { useGetMenuItemsQuery } from "./../Apis/menuItemApi";
import { useGetShoppingCartQuery, useUpdateShoppingCartMutation } from "./../Apis/shoppingCartApi";
import { setShoppingCart, updateQuantity } from "./../Storage/Redux/shoppingCartSlice";
import userModel from "./../Interfaces/userModel";
import { RootState } from "./../Storage/Redux/store";
import toastNotify from "./../Helper/toastNotify";

export default function Home() {
  const { data, isLoading } = useGetMenuItemsQuery(null);
  const dispatch = useDispatch();
  const [isAddingToCart, setIsAddingToCart] = useState<boolean>(false);
  const [updateShoppingCart] = useUpdateShoppingCartMutation()
  const userData: userModel = useSelector((state: RootState) => state.userAuthStore);
  const { data: shop, isLoading: loader, error } = useGetShoppingCartQuery(userData.id, {skip: !userData.id});

  useEffect(() => {
    if (error) {
      console.warn("Error loading shopping cart or it is empty");
      dispatch(setShoppingCart([]));
    } else if (shop && shop.result) {
      dispatch(setShoppingCart(data.result));
    }
  }, [dispatch]);

  useEffect(() => {
    if (data && data.result) {
      dispatch(setMenuItem(data.result));
    }
  }, [dispatch]);

  if (loader) {
    return (
      <div className="flex justify-center items-center h-screen">
        <p className="text-lg text-gray-600">Loading shopping cart ...</p>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-screen">
        <p className="text-lg text-gray-600">Loading menu items...</p>
      </div>
    );
  }

  const handleAddToCart = async (menuItemId: number, userId: string) => {
    setIsAddingToCart(true);
    dispatch(updateQuantity({ cartItemId: menuItemId, quantity: 1 }));
    const res = await updateShoppingCart({menuItemId: menuItemId, updateQuantityBy: 1, userId: userId});
    if (res.data && res.data.isSuccess) {
      toastNotify("One dish has been successfully added to the cart", "success")
    }
    setIsAddingToCart(false);
  }

  return (
    <div className={styles.page}>
      <div className="flex flex-wrap justify-center gap-6">
        {data.result.length > 0 ? (
          data.result.map((menuItem: menuItemModel) => (
            <div
              className="sm:w-full md:w-4/12 lg:w-3/12 p-4"
              key={menuItem.id}
            >
              <div className="grid place-items-center bg-white rounded-lg shadow-lg border overflow-hidden">
                <div className="flex justify-between items-center w-full px-4 py-2 bg-gray-100">
                  {menuItem.specialTag && (
                    <span className="text-xs font-semibold text-red-500 bg-red-100 px-2 py-1 rounded">
                      {menuItem.specialTag}
                    </span>
                  )}
                  <button onClick={() => handleAddToCart(menuItem.id, userData.id)} className="ml-auto text-sm text-blue-600 hover:underline">
                    Add to cart
                  </button>
                </div>

                <img
                  src={menuItem.image}
                  alt={`Image of ${menuItem.name}`}
                  className="w-32 h-32 rounded-full mt-4"
                  style={{ objectFit: "cover" }}
                />

                <div className="p-4 text-center">
                  <p className="text-lg font-bold">{menuItem.name}</p>
                  <p className="text-gray-500 mt-1">{menuItem.description}</p>
                  <p className="text-green-600 font-semibold text-lg mt-2">
                    ${menuItem.price.toFixed(2)}
                  </p>
                </div>

                <div className="flex w-full justify-center border-t bg-gray-50">
                  <Link href={`/menuItemDetail/${menuItem.id}`}>
                    <button className="w-full py-2 text-sm font-semibold text-gray-600 hover:bg-gray-100">
                      Details
                    </button>
                  </Link>
                </div>
              </div>
            </div>
          ))
        ) : (
          <p className="text-gray-600 text-lg">No items found</p>
        )}
      </div>
    </div>
  );
}
