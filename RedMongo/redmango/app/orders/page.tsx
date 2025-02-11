"use client";

import withAuth from "../../HOC/withAuth";
import { useSelector } from "react-redux";
import { useGetAllOrdersQuery } from "@/Apis/orderApi";
import { RootState } from "@/Storage/Redux/store";
import userModel from "@/Interfaces/userModel";

export default function MyOrders() {
    const { id, role }: userModel = useSelector((state: RootState) => state.userAuthStore);
    console.log(id);
    const { data, isLoading } = useGetAllOrdersQuery(id);
    console.log(data);
    if (role == "Admin") {

    }
    return (
        <>   {isLoading ? 
            <div>Loading...</div>
            : (   
            <div className="p-5">
                <h1 className="text-2xl font-bold mb-4">Orders List</h1>
                <div className="p-2">
                    <div className="grid grid-cols-8 border-b border-gray-300 font-semibold">
                        <div className="col-span-1 p-2">ID</div>
                        <div className="col-span-1 p-2">Name</div>
                        <div className="col-span-1 p-2">Phone</div>
                        <div className="col-span-1 p-2">Total</div>
                        <div className="col-span-1 p-2">Items</div>
                        <div className="col-span-1 p-2">Date</div>
                        <div className="col-span-1 p-2">Details</div>
                    </div>
                    {data.result?.map((order: any) => {
                        return ( 
                        <div className="break-words grid grid-cols-8 border-b border-gray-200" key={order.orderHeaderId}>
                            <div className="col-span-1 p-2">{ order.orderHeaderId} </div>
                            <div className="col-span-1 p-2">{ order.pickupName }</div>
                            <div className="col-span-1 p-2">{  order.pickupPhoneNumber }</div>
                            <div className="col-span-1 p-2">{ order.orderTotal.toFixed(2) }</div>
                            <div className="col-span-1 p-2">{ order.totalItems }</div>
                            <div className="col-span-1 p-2">{ new Date(order.orderDate).toLocaleDateString() }</div>
                            <div className="col-span-1 p-2">
                                <button onClick={() => 
                                    {
                                        window.location.href = "/orders/orderDetails/" + order.orderHeaderId;
                                    }
                                } className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 transition">
                                    Details
                                </button>
                            </div>
                        </div>
                        )
                    })}                      
                </div>
            </div>
              )} 
        </>        
    );
}
