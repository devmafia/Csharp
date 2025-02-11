"use client";

import { RootState } from "../Storage/Redux/store";
import cartItemModel from "../Interfaces/cartItemModel";
import { useDispatch, useSelector } from "react-redux";
import { useState } from "react";
import { useInitialPaymentMutation } from "@/Apis/paymentApi";
import OrderStatusTracker from "./OrderStatusTracker";
import userModel from "@/Interfaces/userModel";

export default function CartPickUpDetails() {
    const userData: userModel = useSelector((state: RootState) => state.userAuthStore);
    const shoppingCartFromStore: cartItemModel[] = useSelector(
        (state: RootState) => state.shoppingCartStore.cartItems ?? []
    );
    const [formData, setFormData] = useState({
        name: "",
        email: "",
        phone: ""
    })
    let grandTotal = 0;
    let totalItems = 0;

    shoppingCartFromStore?.map((cartItem: cartItemModel) => {
        totalItems += cartItem.quantity ?? 0;
        grandTotal += (cartItem.menuItem?.price ?? 0) * (cartItem.quantity ?? 0)
        return null;
    })

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { id, value } = e.target;
        setFormData((prev) => ({
            ...prev,
            [id]: value
        }))
    }


    const [initialPayment] = useInitialPaymentMutation();
    const [paymentStarted, setPaymentStarted] = useState(false); 
    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
    
        const { data } = await initialPayment({userId: userData.id, name: formData.name, email: formData.email, phone: formData.phone });
        if (data?.clientSecret) {
            setPaymentStarted(true);
            window.open(data.clientSecret, "_blank");
        } else {
            alert("Error generating payment link. Please try again.");
        }
    };
    

    return (
        <div className="bg-white p-6 rounded-lg shadow-lg max-w-md mx-auto">
            <h2 className="text-2xl font-bold text-gray-800 mb-4 text-center">Pickup Details</h2>
            <form onSubmit={handleSubmit}  className="space-y-4">
                <div>
                <label htmlFor="name" className="block text-sm font-medium text-gray-700">
                    Pickup Name
                </label>
                <input
                    id="name"
                    name="name"
                    type="text"
                    className="mt-1 p-2 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                    onChange={handleInputChange}
                    placeholder="Enter your name"
                />
                </div>

                <div>
                <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                    Pickup Email
                </label>
                <input
                    id="email"
                    name="email"
                    type="email"
                    className="mt-1 p-2 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                    onChange={handleInputChange}
                    placeholder="Enter your email"
                />
                </div>

                <div>
                <label htmlFor="phone" className="block text-sm font-medium text-gray-700">
                    Pickup Phone Number
                </label>
                <input
                    id="phone"
                    name="phone"
                    type="tel"
                    className="mt-1 p-2 block w-full border border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                    onChange={handleInputChange}
                    placeholder="Enter your phone number"
                />
                </div>

                <div className="bg-gray-100 p-4 rounded-lg shadow-inner">
                <p className="text-lg font-semibold text-gray-700">
                    Grand Total: <span className="text-green-600">{grandTotal.toFixed(0)} UAH</span>
                </p>
                <p className="text-lg font-semibold text-gray-700">
                    No of Items: <span className="text-blue-600">{totalItems}</span>
                </p>
                </div>

                <button
                type="submit"
                className="w-full bg-blue-600 text-white font-medium py-2 px-4 rounded-md shadow hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition"
                >
                Look Good? Place Order!
                </button>
            </form>
            {paymentStarted && <OrderStatusTracker userId={userData.id} />}
        </div>
    )
}
