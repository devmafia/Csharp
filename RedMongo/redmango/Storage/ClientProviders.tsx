"use client";

import { useSelector } from "react-redux";
import { RootState } from "./Redux/store";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import Header from "../Components/Header";
import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { useGetShoppingCartQuery } from "../Apis/shoppingCartApi";
import userModel from "../Interfaces/userModel";
import { setShoppingCart } from "./Redux/shoppingCartSlice";

export default function ClientProviders({ children }: { children: React.ReactNode }) {
  const dispatch = useDispatch();
  const userData: userModel = useSelector((state: RootState) => state.userAuthStore);
  const {data, isLoading} = useGetShoppingCartQuery(userData.id,  {
    skip: !userData.id
  });
  useEffect(() => {
    if (data && !isLoading) {
        dispatch(setShoppingCart(data.result?.cartItems))
    }
  }, [data, isLoading])

  return (
    <>
      <Header></Header>
      <ToastContainer />
      {children}
    </>
  );
}
