"use client";

import { useGetOrderDetailsQuery } from "@/Apis/orderApi";

import { useParams } from "next/navigation";
import cartItemModel from '../../../../Interfaces/cartItemModel';

function OrderDetails() {
    const params = useParams();
    let userInput, orderDetails;
    const { data, isLoading } = useGetOrderDetailsQuery(params.id);
    if (!isLoading && data.result) {
        userInput = {
            name: data.result[0].pickupName,
            email: data.result[0].pickupEmail,
            phoneNumber: data.result[0].pickupPhoneNumber
        };
        orderDetails = {
            id: data.result[0].orderHeaderId,
            cartItems: data.result[0].orderDetails,
            cartTotal: data.result[0].orderTotal,
            status:data.result[0].status
        }
    }

    return (
        <div className="flex justify-center items-center min-h-screen bg-gray-100">
        {isLoading ? (
            <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-t-4 border-blue-500 mx-auto"></div>
            <p className="mt-4 text-gray-600 font-medium">Loading...</p>
            </div>
        ) : (
            <div className="max-w-3xl w-full bg-white shadow-lg rounded-lg p-8">
            <h3 className="text-2xl font-semibold text-gray-800 mb-6">Order Summary</h3>
            <div className="space-y-4">
                <div className="bg-gray-50 border rounded-md py-3 px-4">
                <strong>Name:</strong> {userInput?.name}
                </div>
                <div className="bg-gray-50 border rounded-md py-3 px-4">
                <strong>Email:</strong> {userInput?.email}
                </div>
                <div className="bg-gray-50 border rounded-md py-3 px-4">
                <strong>Phone:</strong> {userInput?.phoneNumber}
                </div>
            </div>
            <div className="mt-8">
                <h4 className="text-lg font-medium text-gray-700 mb-4">Menu Items</h4>
                {orderDetails && orderDetails.cartItems ? (
                orderDetails.cartItems.map((cartItem: cartItemModel, index: number) => (
                    <div
                    key={index}
                    className="flex flex-col md:flex-row justify-between items-start md:items-center mb-4 p-4 bg-gray-50 rounded-lg shadow-sm"
                    >
                    <div className="space-y-1">
                        <p className="text-gray-800 font-medium">{cartItem.menuItem.name}</p>
                        <p className="text-gray-600 text-sm">{cartItem.menuItem.description}</p>
                    </div>
                    <div className="text-gray-800 font-semibold">
                        SubTotal: ${cartItem.menuItem.price * cartItem.quantity}
                    </div>
                    </div>
                ))
                ) : (
                <p className="text-gray-600">No items found.</p>
                )}
            </div>

            <div className="mt-6 border-t pt-4">
                <h4 className="text-xl font-bold text-gray-800">
                Total: ${orderDetails?.cartTotal.toFixed(2)}
                </h4>
            </div>
            </div>
        )}
        </div>
    )  
}

export default OrderDetails;
