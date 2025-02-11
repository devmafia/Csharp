"use client";

import { useParams } from "next/navigation";
import styles from "./page.module.css";
import Link from "next/link";
import { useState, useEffect } from "react";
import { menuItemModel } from "../../../Interfaces/menuItemModel";
import { setMenuItem } from "../../../Storage/Redux/menuItemSlice";
import { useGetMenuItemByIdQuery } from "../../../Apis/menuItemApi";
import { useDispatch, useSelector } from 'react-redux';
import { useUpdateShoppingCartMutation } from "../../../Apis/shoppingCartApi";
import userModel from "../../../Interfaces/userModel";
import { RootState } from "../../../Storage/Redux/store";
import withAuth from "@/HOC/withAuth";

export default withAuth(function ItemDetails() {
  const params = useParams();
  const menuItemId = params.menuItemId;
  const dispatch = useDispatch();
  const { data, isLoading } = useGetMenuItemByIdQuery(menuItemId);
  const [quantity, setQuantity] = useState<number>(0)
  const [isAddingToCart, setIsAddingToCart] = useState<boolean>(false);
  const [updateShoppingCart] = useUpdateShoppingCartMutation();
  const userData: userModel = useSelector((state: RootState) => state.userAuthStore);

  useEffect(() => {
    if (data && data.result) {
      dispatch(setMenuItem(data.result));
    }
  }, [data, dispatch]);

  if (isLoading) {
    return <div>Loading...</div>;
  }

  function handleQuantity(num: number) {
    if (num < 0) {
      setQuantity((prev: number) => {
        return (prev == 0) ?
        prev : (prev - (-num));
      })
    } else if (num >= 0) {
      setQuantity((prev: number) => {
        return prev + num;
      })
    }
  }

  const handleAddToCart = async (menuItemId: number, userId: string) => {
    setIsAddingToCart(true);

    const res = await updateShoppingCart({menuItemId: menuItemId, 
      updateQuantityBy: quantity, 
      userId: userId
    })

    console.log(res.data);
    setIsAddingToCart(false);
  }

  const menuItem = data.result;

  return (
    <div className={styles.page}>
      <div className="container mx-auto pt-6 px-4">
        <div className="flex flex-wrap md:flex-nowrap gap-6 items-start">
          <div className="flex-1 bg-white p-6 rounded-lg shadow-md">
            <h2 className="text-2xl font-bold text-gray-800 mb-4">{menuItem.name}</h2>
            <p className="text-lg text-gray-600 mb-2">{menuItem.category}</p>
            {menuItem.specialTag && (
              <span className="inline-block bg-lime-100 text-lime-600 text-sm font-semibold px-3 py-1 rounded-full mb-4">
                {menuItem.specialTag}
              </span>
            )}
            <p className="text-gray-700 text-base mb-6 leading-relaxed">
              {menuItem.description}
            </p>
            <div className="flex items-center gap-4 mb-6">
              <span className="text-xl font-semibold text-gray-800">Price (UAH): {menuItem.price}</span>
              <div className="flex items-center gap-2 border rounded-lg px-4 py-2">
                <button onClick={() => handleQuantity(-1)} className="text-lg font-bold text-gray-800">-</button>
                <span className="text-lg font-semibold text-gray-700">{quantity}</span>
                <button onClick={() => handleQuantity(1)} className="text-lg font-bold text-gray-800">+</button>
              </div>
            </div>
            <div className="flex gap-4">
              <button onClick={() => handleAddToCart(menuItem.id, userData.id)} className="flex-1 bg-blue-600 text-white py-3 rounded-lg font-semibold hover:bg-blue-700 transition">
                Add to Cart
              </button>
              <Link href="/" className="flex-1">
                <button className="w-full bg-gray-200 text-gray-700 py-3 rounded-lg font-semibold hover:bg-gray-300 transition">
                  Home
                </button>
              </Link>
            </div>
          </div>

          <div className="flex-none w-full md:w-1/3">
            <div className="bg-gray-100 p-6 rounded-lg shadow-md flex justify-center items-center">
              <img
                className="w-full max-w-sm h-auto rounded-lg object-cover"
                src={menuItem.image || "https://via.placeholder.com/300"}
                alt={menuItem.name}
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
})
